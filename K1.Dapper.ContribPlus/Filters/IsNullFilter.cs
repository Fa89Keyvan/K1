using Dapper;
using System.Collections.Generic;

namespace K1.Dapper.ContribPlus.Filters
{
    public class IsNullFilter : Filter
    {
        private readonly NullCondition _nullCondition;

        public IsNullFilter(string name, NullCondition nullCondition)
        {
            this.Name = name;
            this._nullCondition = nullCondition;
        }

        public override string SqlFilterStatement => $" @{Name} {SqlOperator}";

        public override string SqlOperator => NullConditions[_nullCondition];

        private static readonly Dictionary<NullCondition, string> NullConditions = new Dictionary<NullCondition, string>
        {
            { NullCondition.IsNull, " Is Null " },
            { NullCondition.IsNotNull, " Is Not Null " }
        };

        public override void AppendToParameters(DynamicParameters parameters)
        {
            return;
        }
    }

    public enum NullCondition
    {
        IsNull = 9,
        IsNotNull = 10
    }
}
