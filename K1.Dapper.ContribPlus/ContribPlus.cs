using K1.Dapper.ContribPlus.Filters;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {


        public static async Task<PagedList<TEntity>> GetPagedListAsync<TEntity>
            (this IDbConnection db, PageListRequest<TEntity> listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
            where TEntity : class
        {
            var taskResult = await Task<PagedList<TEntity>>.Run(() =>
            {
                return db.GetPagedList<TEntity>(listRequest, dbTransaction, commandTimeout);
            });

            return taskResult;
        }


        public static async Task<long> CountAsync<TEntity>(this IDbConnection db, PageListRequest<TEntity> listRequest, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            long taskResult = await Task<long>.Run(() =>
            {
                return db.Count<TEntity>(listRequest, dbTransaction,commandTimeout);
            });

            return taskResult;
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


            var entities = db.Query<TEntity>(sql: builder.ToString(), parameters, dbTransaction, commandTimeout: commandTimeout);

            pagedList.Data = entities.ToList();

            return pagedList;
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
