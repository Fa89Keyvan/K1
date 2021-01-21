using Dapper;

namespace K1.Dapper.ContribPlus.Filters
{
    public abstract class Filter
    {
        public object Value { get; protected set; }
        public abstract string SqlOperator { get; }
        public abstract void AppendToParameters(DynamicParameters parameters);
        public string Name { get; protected set; }
        public abstract string SqlFilterStatement { get; }

        public static Filter Simple(string name, Operators @operator, object value) => new SimpleFilter(name, @operator, value);
        public static Filter IsNull(string name) => new IsNullFilter(name, NullCondition.IsNull);
        public static Filter IsNotNull(string name) => new IsNullFilter(name, NullCondition.IsNotNull);
        public static Filter InClouse(string name, params object[] values) => new InClouseFilter(name, values);
        public static Filter StartsWith(string name, string value) => new ContainFilter(name, ContaintTypes.StartsWith, value);
        public static Filter EndsWith(string name, string value) => new ContainFilter(name, ContaintTypes.EndsWith, value);
        public static Filter Contains(string name, string value) => new ContainFilter(name, ContaintTypes.Contains, value);

    }
}
