using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using K1.Dapper.SimpleCommand.Contracts;
using K1.Dapper.SimpleCommand.Attributes;

using static K1.Dapper.SimpleCommand.DialectManager.DialectManager;

namespace K1.Dapper.SimpleCommand.Resolvers
{
    public class ColumnNameResolver : IColumnNameResolver
    {
        public virtual string ResolveColumnName(PropertyInfo propertyInfo)
        {
            var columnName = Encapsulate(propertyInfo.Name);

            var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as dynamic;
            if (columnattr != null)
            {
                columnName = Encapsulate(columnattr.Name);
                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
            }
            return columnName;
        }
    }
}
