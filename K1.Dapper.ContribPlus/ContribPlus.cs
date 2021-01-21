using K1.Dapper.ContribPlus.Filters;
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
            (this IDbConnection db, string orders = "1", OrderDir dir = OrderDir.ASC, int offset = 0, int fetch = 1000, bool withNoLock = true, IDbTransaction dbTransaction = null, Filter[] filters = null, int? commandTimeout = null)
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

                    if (i > 0)
                        builder.Append(" AND ");

                    builder.Append(filter.SqlFilterStatement);
                    filter.AppendToParameters(parameters);

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

            if (TableNames.TryGetValue(typeHandle, out tableName))
            {
                return tableName;
            }

            var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), true)?.FirstOrDefault();

            if (tableAttribute != null && tableAttribute is TableAttribute)
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
