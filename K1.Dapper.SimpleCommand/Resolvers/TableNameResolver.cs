using System;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using K1.Dapper.SimpleCommand.Attributes;
using K1.Dapper.SimpleCommand.Contracts;

using static K1.Dapper.SimpleCommand.DialectManager.DialectManager;

namespace K1.Dapper.SimpleCommand.Resolvers
{
    public class TableNameResolver : ITableNameResolver
    {
        public virtual string ResolveTableName(Type type)
        {
            var tableName = Encapsulate(type.Name);

            var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
            if (tableattr != null)
            {
                tableName = Encapsulate(tableattr.Name);
                try
                {
                    if (!String.IsNullOrEmpty(tableattr.Schema))
                    {
                        string schemaName = Encapsulate(tableattr.Schema);
                        tableName = String.Format("{0}.{1}", schemaName, tableName);
                    }
                }
                catch (RuntimeBinderException)
                {
                    //Schema doesn't exist on this attribute.
                }
            }

            return tableName;
        }
    }
}
