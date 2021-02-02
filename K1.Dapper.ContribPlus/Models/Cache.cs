using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dapper.Contrib.Extensions
{
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
