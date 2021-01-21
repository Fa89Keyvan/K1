using Dapper;
using System.Text;

namespace K1.Dapper.ContribPlus.Filters
{
    public class InClouseFilter : Filter
    {
        const string In = " In ";
        private readonly object[] _values;
        private string[] _paramNames;
        public InClouseFilter(string name, params object[] values)
        {
            this.Name = name;
            this._values = values;

            if(values != null && values.Length > 0)
            {
                this._paramNames = new string[values.Length];
            }
        }

        public override string SqlOperator => In;

        public override string SqlFilterStatement
        {
            get
            {
                if (_values != null && _values.Length > 0)
                {
                    var inClouse = new StringBuilder();
                    inClouse.AppendFormat(" {0} {1}( ", Name, SqlOperator);

                    for (int index = 0; index < _values.Length; index++)
                    {
                        if (index > 0)
                            inClouse.Append(",");

                        _paramNames[index] = string.Format("@{0}_{1}", Name, index);

                        inClouse.AppendFormat(_paramNames[index]);

                    }

                    inClouse.Append(") ");

                    return inClouse.ToString();
                }

                return "";
            }
        }

        public override void AppendToParameters(DynamicParameters parameters)
        {
            if (_values != null && _values.Length > 0)
            {
                for (int index = 0; index < _values.Length; index++)
                {
                    parameters.Add(_paramNames[index], _values[index]);
                }
            }
        }
    }
}
