using Dapper;

namespace K1.Dapper.ContribPlus.Filters
{
    public abstract class Filter
    {
        public Filter()
        {

        }
        public Filter(Operators @operator)
        {
            this.Operator = @operator;
        }

        public object Value { get; protected set; }
        public abstract string SqlOperator { get; }
        public abstract void AppendToParameters(DynamicParameters parameters);
        public string Name { get; protected set; }
        public abstract string SqlFilterStatement { get; }
        public Operators Operator { get; }
    }
}
