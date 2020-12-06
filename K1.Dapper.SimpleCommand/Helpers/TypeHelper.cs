using System.Collections.Generic;
using System.Reflection;

namespace K1.Dapper.SimpleCommand.Helpers
{
    internal static class TypeHelper
    {
        //Get all properties in an entity
        internal static IEnumerable<PropertyInfo> GetAllProperties<T>(T entity) where T : class
        {
            if (entity == null) return new PropertyInfo[0];
            return entity.GetType().GetProperties();
        }
    }
}
