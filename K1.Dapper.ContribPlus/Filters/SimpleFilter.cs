using Dapper;
using System.Collections.Generic;

namespace K1.Dapper.ContribPlus.Filters
{
    public class SimpleFilter : Filter
    {
        private readonly Operators _operator;

        public SimpleFilter(string name, Operators @operator, object value)
        {
            this.Name = name;
            this._operator = @operator;
            this.Value = value;
        }

        public override string SqlOperator => SqlOperators[_operator];

        public override string SqlFilterStatement => $" {Name} {SqlOperator} {ParamName} ";

        public string ParamName => $"@{Name}";

        public override void AppendToParameters(DynamicParameters parameters) => parameters?.Add(ParamName, Value);


        private static readonly Dictionary<Operators, string> SqlOperators = new Dictionary<Operators, string>()
        {
            { Operators.Equal,        " = " },
            { Operators.Grather,      " > " },
            { Operators.Lower,        " < " },
            { Operators.GratherEqual, " >= " },
            { Operators.LowerEqual,   " <= " },
            { Operators.NotEqual,     " <> " }
        };
    }

    public enum Operators
    {
        Equal = 0,
        Grather = 1,
        Lower = 2,
        GratherEqual = 3,
        LowerEqual = 4,
        NotEqual = 5
    }
}
