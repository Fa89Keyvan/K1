namespace Dapper.Contrib.Extensions
{
    public static class SqlSanatizer
    {
        public static string Sanatize(string str)
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
}
