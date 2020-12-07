using K1.Dapper.SimpleCommand.Models;

namespace K1.Dapper.SimpleCommand.DialectManager
{
    public static class DialectManager
    {
        private static Dialect _dialect = Dialect.SQLServer;
        private static string _encapsulation;
        private static string _getIdentitySql;
        private static string _getPagedListSql;

        /// <summary>
        /// Sets the database dialect 
        /// </summary>
        /// <param name="dialect"></param>
        public static void SetDialect(Dialect dialect = Dialect.SQLServer)
        {
            switch (dialect)
            {
                default:
                case Dialect.SQLServer:
                    _dialect = Dialect.SQLServer;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _getPagedListSql = "SELECT {SelectColumns} FROM {TableName} {WhereClause} ORDER BY {OrderBy} OFFSET {Offset} ROWS FETCH NEXT {RowsPerPage} ROWS ONLY";
                    break;
                case Dialect.SQLServer2008:
                    _dialect = Dialect.SQLServer2008;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _getPagedListSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNumber BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
                    break;
                case Dialect.PostgreSQL:
                    _dialect = Dialect.PostgreSQL;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    break;
                case Dialect.SQLite:
                    _dialect = Dialect.SQLite;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ROWID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    break;
                case Dialect.MySQL:
                    _dialect = Dialect.MySQL;
                    _encapsulation = "`{0}`";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    break;

            }
        }

        internal static string Encapsulation => _encapsulation;
        internal static string GetIdentitySql => _getIdentitySql;
        internal static string GetPagedListSql => _getPagedListSql;
        internal static Dialect Dialect => _dialect;

        internal static string Encapsulate(string databaseword)
        {
            return string.Format(Encapsulation, databaseword);
        }

        public static Dialect GetDialect() => _dialect;
    }
}
