using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace K1.Dapper.ContribPlus.Filters
{
    public class BetweenFilter : Filter
    {
        const string Between = " Between ";
        public override string SqlOperator => Between;

        private readonly object _minValue;
        private readonly object _maxValue;

        private string MinParamName => $"@{Name}_Min";
        private string MaxParamName => $"@{Name}_Max";


        public BetweenFilter(string name, object minValue, object maxValue)
        {
            this.Name = name;
            this._minValue = minValue;
            this._maxValue = maxValue;
        }

        public override string SqlFilterStatement => $" ( {Name} {SqlOperator} {MinParamName} AND {MaxParamName} ) ";

        public override void AppendToParameters(DynamicParameters parameters)
        {
            parameters?.Add(MinParamName, _minValue);
            parameters?.Add(MaxParamName, _maxValue);
        }
    }
}
