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
            (this IDbConnection db, PageListRequest<TEntity> listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
            where TEntity : class
        {
            var pagedList = new PagedList<TEntity>();

            if (listRequest.FetchCount < 1)
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

            if (pagedList.Total < 1 || pagedList.Total < listRequest.OffsetCount)
            {
                return pagedList;
            }

            string orders = string.Join(',', listRequest.Orders);
            orders = SqlSanatizer.Sanatize(orders);

            ApplyFilters(listRequest.GetFilters(), builder, parameters);

            pagedList.TotalFiltered = db.ExecuteScalar<int>(sql: builder.ToString().Replace("*", "Count(1)"), param: parameters, transaction: dbTransaction, commandTimeout: commandTimeout);

            if (pagedList.TotalFiltered < 1 || pagedList.TotalFiltered < listRequest.OffsetCount)
            {
                return pagedList;
            }

            builder.AppendFormat(" Order by {0} {1} Offset (@OFFSET) ROWS Fetch Next (@FETCH) Rows Only", orders, listRequest.OrderDirection.ToString());
            parameters.Add("@OFFSET", listRequest.OffsetCount);
            parameters.Add("@FETCH", listRequest.FetchCount);


            pagedList.Data = db.Query<TEntity>(sql: builder.ToString(), parameters, dbTransaction, commandTimeout: commandTimeout).ToList();

            return pagedList;
        }

        public static long Count<TEntity>
            (this IDbConnection db, PageListRequest<TEntity> listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
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
}
