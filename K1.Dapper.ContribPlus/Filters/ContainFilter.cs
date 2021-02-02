using Dapper;

namespace K1.Dapper.ContribPlus.Filters
{
    public class ContainFilter : Filter
    {
        const string Like = " Like ";

        private readonly ContaintTypes _containtType;

        public ContainFilter(string name, ContaintTypes containtType, string value)
        {
            this.Name = name;
            this._containtType = containtType;
            this.Value = value;
        }

        public override string SqlOperator => Like;

        private string ParamName => $"@{Name}";

        public override string SqlFilterStatement
        {
            get
            {
                switch (_containtType)
                {
                    case ContaintTypes.StartsWith:
                        return $" {Name} {SqlOperator} {ParamName} + '%' ";
                    case ContaintTypes.EndsWith:
                        return $" {Name} {SqlOperator} N'%' + {ParamName} ";
                    case ContaintTypes.Contains:
                        return $" {Name} {SqlOperator} N'%' + {ParamName} + '%'";
                    default:
                        return " 1 = 2 ";
                }
            }
        }

        public override void AppendToParameters(DynamicParameters parameters) => parameters?.Add(ParamName, Value);
    }
    public enum ContaintTypes
    {
        StartsWith = 6,
        EndsWith = 7,
        Contains = 8,
    }
}
