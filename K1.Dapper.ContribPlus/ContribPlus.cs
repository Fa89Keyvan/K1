using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {


        public static PagedList<TEntity> GetPagedList<TEntity>
            (this IDbConnection db, string orders = "1", OrderDir dir = OrderDir.ASC,int offset = 0, int fetch = 1000, bool withNoLock = true ,IDbTransaction dbTransaction = null, Filter[] filters = null, int? commandTimeout = null) 
            where TEntity : class
        {
            var pagedList = new PagedList<TEntity>();

            if (fetch < 1)
            {
                return pagedList;
            }

            var type = typeof(TEntity);
            string tableName = Cache.GetTableName(type);


            var builder = new StringBuilder();
            var parameters = new DynamicParameters();

            builder.AppendFormat("Select * From {0} ", tableName);

            if (withNoLock)
                builder.Append(" with(nolock) ");

            pagedList.Total = db.ExecuteScalar<int>(sql: builder.ToString().Replace("*", "Count(1)"), transaction: dbTransaction, commandTimeout: commandTimeout);

            if (pagedList.Total < 1 || pagedList.Total < offset)
            {
                return pagedList;
            }

            orders = Helpers.SanatizeSql(orders);
            ApplyFilters(filters, builder, parameters);

            pagedList.TotalFiltered = db.ExecuteScalar<int>(sql: builder.ToString().Replace("*", "Count(1)"), param: parameters, transaction: dbTransaction, commandTimeout: commandTimeout);

            if (pagedList.TotalFiltered < 1 || pagedList.TotalFiltered < offset)
            {
                return pagedList;
            }

            builder.AppendFormat(" Order by {0} {1} Offset (@OFFSET) ROWS Fetch Next (@FETCH) Rows Only", orders, dir.ToString());
            parameters.Add("@OFFSET", offset);
            parameters.Add("@FETCH", fetch);


            pagedList.Data = db.Query<TEntity>(sql: builder.ToString(), parameters, dbTransaction, commandTimeout: commandTimeout).ToList();

            return pagedList;
        }

        public static long Count<TEntity>
            (this IDbConnection db, bool withNoLock = true, IDbTransaction dbTransaction = null, Filter[] filters = null, int? commandTimeout = null)
        {
            long count = 0;

            var type = typeof(TEntity);
            string tableName = Cache.GetTableName(type);

            var builder = new StringBuilder();
            var parameters = new DynamicParameters();

            builder.AppendFormat("Select Count(1) From {0} ", tableName);

            if (withNoLock)
                builder.Append(" with(nolock) ");

            ApplyFilters(filters, builder, parameters);

            count = db.ExecuteScalar<long>(sql: builder.ToString(), param: parameters, transaction: dbTransaction, commandTimeout: commandTimeout);

            return count;

        }



        #region ' Private Methods '

        private static void ApplyFilters(Filter[] filters, StringBuilder builder, DynamicParameters parameters)
        {
            if (filters != null && filters.Length > 0)
            {
                builder.Append(" Where ");

                for (int i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];

                    if(i > 0)
                        builder.Append(" AND ");

                    if(filter.Operator == Operator.In)
                    {
                        string inClouse = filter.CreateInClouse(parameters);
                        builder.Append(inClouse);
                    }
                    else
                    {
                        builder.Append(filter.SqlFilterStatement);
                        filter.AddToDapperParameters(parameters);
                    }
                }
            }
        }

        #endregion

    }

    static class Helpers
    {
        internal static string SanatizeSql(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return str
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("-", "")
                .Replace("drop", "")
                .Replace('\b', ' ')
                .Replace('\r', ' ')
                .Replace(';', ' ')
                .Replace('\n', ' ')
                .Replace('\t', ' ');
        }
    }

    public class Filter
    {
        private Filter() { }

        public static Filter Create(string name, Operator optr, object value) => new Filter
        {
            Name = name,
            Operator = optr,
            Values = new object[] { value }
        };

        public static Filter CreateInFilter(string name, params object[] values) => new Filter
        {
            Name = name,
            Operator = Operator.In,
            Values = values
        };

        private static readonly Dictionary<Operator, string> SqlOperators = new Dictionary<Operator, string>()
        {
            { Operator.Equal,        " = " },
            { Operator.Grather,      " > " },
            { Operator.Lower,        " < " },
            { Operator.GratherEqual, " >= " },
            { Operator.LowerEqual,   " <= " },
            { Operator.NotEqual,     " <> " },
            { Operator.StartsWith,   " Like " },
            { Operator.EndsWith,     " Like " },
            { Operator.Contains,     " Like " },
            { Operator.IsNull,       " Is Null " },
            { Operator.IsNotNull,    " Is Not Null" },
            { Operator.In,           " In " }
        };

        public string Name { get; private set; }
        public object[] Values { get; private set; }
        public Operator Operator { get; private set; }


        public string SqlOperator => SqlOperators[this.Operator];
        public string SqlParameter
        {
            get
            {
                switch (Operator)
                {
                    case Operator.Equal:
                    case Operator.Grather:
                    case Operator.Lower:
                    case Operator.GratherEqual:
                    case Operator.LowerEqual:
                    case Operator.NotEqual:
                        return "@" + Name;
                    case Operator.IsNull:
                    case Operator.IsNotNull:
                        return " ";
                    case Operator.StartsWith:
                        return $"@{Name} + '%'";
                    case Operator.EndsWith:
                        return $"N'%' + @{Name}";
                    case Operator.Contains:
                        return $"N'%' + @{Name} + '%'";
                    case Operator.In:
                        return " In ";
                    default:
                        throw new Exception("invalid operator");
                }
            }
        }

        internal string CreateInClouse(DynamicParameters parameters)
        {
            if (Values != null && Values.Length > 0)
            {
                var inClouse = new StringBuilder();
                inClouse.AppendFormat(" {0} {1}( ", Name, SqlOperator);
                for (int index = 0; index < Values.Length; index++)
                {
                    if (index > 0)
                        inClouse.Append(",");

                    string paramName = string.Format("@{0}_{1}", Name, index);

                    inClouse.AppendFormat(paramName);
                    parameters.Add(paramName, Values[index]);

                }
                inClouse.Append(") ");

                return inClouse.ToString();
            }

            return "";
        }

        public string SqlFilterStatement
        {
            get
            {
                switch (Operator)
                {
                    case Operator.Equal:
                    case Operator.Grather:
                    case Operator.Lower:
                    case Operator.GratherEqual:
                    case Operator.LowerEqual:
                    case Operator.NotEqual:
                    case Operator.StartsWith:
                    case Operator.EndsWith:
                    case Operator.Contains:
                        return $" {Name} {SqlOperator} {SqlParameter} ";

                    case Operator.IsNull:
                    case Operator.IsNotNull:
                        return $" {Name} {SqlParameter} ";
                    case Operator.In:
                        return "";
                    default:
                        throw new Exception("invalid operator");
                }
            }
        }

        public override string ToString() => SqlFilterStatement;

        public void AddToDapperParameters(DynamicParameters dynamicParameters) => dynamicParameters.Add(Name, Values[0]);
    }

    public enum Operator
    {
        Equal = 0,
        Grather = 1,
        Lower = 2,
        GratherEqual = 3,
        LowerEqual = 4,
        NotEqual = 5,
        StartsWith = 6,
        EndsWith = 7,
        Contains = 8,
        IsNull = 9,
        IsNotNull = 10,
        In = 11
    }

    public enum OrderDir
    {
        ASC = 0,
        DESC = 1
    }

    public class PagedList<TModel> where TModel : class
    {
        public int Total { get; set; }
        public int TotalFiltered { get; set; }
        public List<TModel> Data { get; set; }
    }

    internal static class Cache
    {
        private static ConcurrentDictionary<RuntimeTypeHandle, string> TableNames = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        internal static string GetTableName(Type type)
        {
            var typeHandle = type.TypeHandle;
            string tableName;

            if(TableNames.TryGetValue(typeHandle, out tableName))
            {
                return tableName;
            }

            var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), true)?.FirstOrDefault();
            
            if(tableAttribute != null && tableAttribute is TableAttribute)
            {
                tableName = (tableAttribute as TableAttribute).Name;
            }
            else
            {
                tableName = type.Name;
            }

            TableNames.TryAdd(typeHandle, tableName);

            return tableName;
            
        }
    }
}
