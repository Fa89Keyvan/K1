using K1.Dapper.ContribPlus.Filters;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {


        public static PagedList<TEntity> GetPagedList<TEntity>
            (this IDbConnection db, PageListRequest listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
            where TEntity : class
        {
            var pagedList = new PagedList<TEntity>();

            if (listRequest.Fetch < 1)
            {
                return pagedList;
            }

            var type = typeof(TEntity);
            string tableName = Cache.GetTableName(type);


            var builder = new StringBuilder();
            var parameters = new DynamicParameters();

            builder.AppendFormat("Select * From {0} ", tableName);

            if (listRequest.WithNolock)
                builder.Append(" with(nolock) ");

            pagedList.Total = db.ExecuteScalar<int>(sql: builder.ToString().Replace("*", "Count(1)"), transaction: dbTransaction, commandTimeout: commandTimeout);

            if (pagedList.Total < 1 || pagedList.Total < listRequest.Offset)
            {
                return pagedList;
            }

            string orders = string.Join(',', listRequest.Orders);
            orders = SqlSanatizer.Sanatize(orders);

            ApplyFilters(listRequest.GetFilters(), builder, parameters);

            pagedList.TotalFiltered = db.ExecuteScalar<int>(sql: builder.ToString().Replace("*", "Count(1)"), param: parameters, transaction: dbTransaction, commandTimeout: commandTimeout);

            if (pagedList.TotalFiltered < 1 || pagedList.TotalFiltered < listRequest.Offset)
            {
                return pagedList;
            }

            builder.AppendFormat(" Order by {0} {1} Offset (@OFFSET) ROWS Fetch Next (@FETCH) Rows Only", orders, listRequest.OrderDirection.ToString());
            parameters.Add("@OFFSET", listRequest.Offset);
            parameters.Add("@FETCH", listRequest.Fetch);


            pagedList.Data = db.Query<TEntity>(sql: builder.ToString(), parameters, dbTransaction, commandTimeout: commandTimeout).ToList();

            return pagedList;
        }

        public static long Count<TEntity>
            (this IDbConnection db, PageListRequest listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            long count = 0;

            var type = typeof(TEntity);
            string tableName = Cache.GetTableName(type);

            var builder = new StringBuilder();
            var parameters = new DynamicParameters();

            builder.AppendFormat("Select Count(1) From {0} ", tableName);

            if (listRequest.WithNolock)
                builder.Append(" with(nolock) ");

            ApplyFilters(listRequest.GetFilters(), builder, parameters);

            count = db.ExecuteScalar<long>(sql: builder.ToString(), param: parameters, transaction: dbTransaction, commandTimeout: commandTimeout);

            return count;

        }



        #region ' Private Methods '

        private static void ApplyFilters(List<Filter> filters, StringBuilder builder, DynamicParameters parameters)
        {
            if (filters != null && filters.Count > 0)
            {
                builder.Append(" Where ");

                for (int i = 0; i < filters.Count; i++)
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

    public class PageListRequest
    {
        private List<Filter> _filters;
        public PageListRequest()
        {
            this._filters = new List<Filter>();
            this.Fetch = 1000;
            this.Offset = 0;
            this.OrderDirection = OrderDir.ASC;
            this.WithNolock = false;
        }

        public OrderDir OrderDirection { get; set; }


        private int _offset;
        public int Offset
        {
            get => _offset;
            set => _offset = value < 0 ? 0 : value;
        }

        private int _fetch;
        public int Fetch
        {
            get => _fetch;
            set => _fetch = value < 0 ? 0 : value;
        }

        private string[] _orders;

        public string[] Orders
        {
            get => _orders ?? new string[1] { " 1 " };
            set => _orders = value;
        }

        public bool WithNolock { get; set; }

        public List<Filter> GetFilters() => _filters;

        public PageListRequest AddFilterAddSimple(string name, Operators @operator, object value)
        {
            _filters.Add(new SimpleFilter(name, @operator, value));
            return this;
        }

        public PageListRequest AddFilterIsNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNull));
            return this;
        }

        public PageListRequest AddFilterIsNotNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNotNull));
            return this; 
        }

        public PageListRequest AddFilterInClouse(string name, params object[] values)
        {
            _filters.Add(new InClouseFilter(name, values));
            return this; 
        }

        public PageListRequest AddFilterStartsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.StartsWith, value));
            return this;
        }

        public PageListRequest AddFilterEndsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.EndsWith, value));
            return this;
        }

        public PageListRequest AddFilterContains(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.Contains, value));
            return this;
        }

        public PageListRequest AddFilterBetween(string name, object min, object max)
        {
            _filters.Add(new BetweenFilter(name, min, max));
            return this; 
        }
    }
}
