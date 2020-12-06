using System.Reflection;

namespace K1.Dapper.SimpleCommand.Contracts
{
    public interface IColumnNameResolver
    {
        string ResolveColumnName(PropertyInfo propertyInfo);
    }
}
