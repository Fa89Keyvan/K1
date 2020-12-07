using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace K1.Dapper.SimpleCommand.Cache
{
    public static class CacheService
    {
        private static bool StringBuilderCacheEnabled = true;

        private static readonly ConcurrentDictionary<string, string> StringBuilderCacheDict = new ConcurrentDictionary<string, string>();
        internal static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();
        internal static readonly ConcurrentDictionary<string, string> ColumnNames = new ConcurrentDictionary<string, string>();



        /// <summary>
        /// Append a Cached version of a strinbBuilderAction result based on a cacheKey
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="cacheKey"></param>
        /// <param name="stringBuilderAction"></param>
        internal static void StringBuilderCache(StringBuilder sb, string cacheKey, Action<StringBuilder> stringBuilderAction)
        {
            if (StringBuilderCacheEnabled && StringBuilderCacheDict.TryGetValue(cacheKey, out string value))
            {
                sb.Append(value);
                return;
            }

            StringBuilder newSb = new StringBuilder();
            stringBuilderAction(newSb);
            value = newSb.ToString();
            StringBuilderCacheDict.AddOrUpdate(cacheKey, value, (t, v) => value);
            sb.Append(value);
        }


        internal class IdPropsCache
        {
            private const string CacheKeyTemplate = "IdProps.{0}.{1}";

            private static ConcurrentDictionary<string, PropertyInfo[]> _cache = new ConcurrentDictionary<string, PropertyInfo[]>();

            internal static bool TryGet(Type type, out PropertyInfo[] propertyInfos)
                => _cache.TryGetValue(GetKey(type), out propertyInfos);

            internal static void AddCache(Type type, PropertyInfo[] propertyInfos)
                => _cache.TryAdd(GetKey(type), propertyInfos);

            private static string GetKey(Type type) 
                => string.Format(CacheKeyTemplate, type.DeclaringType, type.Name);
        }


    }
}
