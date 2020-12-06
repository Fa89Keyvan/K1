using System;

namespace K1.Dapper.SimpleCommand.Contracts
{
    public interface ITableNameResolver
    {
        string ResolveTableName(Type type);
    }
}
