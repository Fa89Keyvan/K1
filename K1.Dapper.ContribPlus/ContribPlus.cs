using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {
        
    }

    public class Filter<T>
    {
        private Filter() { }

        public static Filter<T> Create(string name, Operator optr, T value) => new Filter<T>
        {
            Name = name,
            Operator = optr,
            Value = value
        };


        private static readonly Dictionary<Operator, string> SqlOperators = new Dictionary<Operator, string>()
        {
            { Operator.Equal,        " = " },
            { Operator.Grather,      " > " },
            { Operator.Lower,        " < " },
            { Operator.GratherEqual, " >= " },
            { Operator.LowerEqual,   " <= " },
            { Operator.NotEqual,     " <> " },
            { Operator.StartsWith,   " Like " },
            { Operator.EndsWith,     " Like " },
            { Operator.Contains,     " Like " },
            { Operator.IsNull,       " Is Null " },
            { Operator.IsNotNull,    " Is Not Null" }
        };

        public string Name { get; private set; }
        public T Value { get; private set; }
        public Operator Operator { get; private set; }


        public string SqlOperator => SqlOperators[this.Operator];
        public string SqlParameter
        {
            get
            {
                switch (Operator)
                {
                    case Operator.Equal:
                    case Operator.Grather:
                    case Operator.Lower:
                    case Operator.GratherEqual:
                    case Operator.LowerEqual:
                    case Operator.NotEqual:
                        return "@" + Name;
                    case Operator.IsNull:
                    case Operator.IsNotNull:
                        return " ";
                    case Operator.StartsWith:
                        return $"@{Name} + '%'";
                    case Operator.EndsWith:
                        return $"N'%' + @{Name}";
                    case Operator.Contains:
                        return $"N'%' + @{Name} + '%'";
                    default:
                        throw new Exception("invalid operator");
                }
            }
        }
        public string SqlFilterStatement
        {
            get
            {
                switch (Operator)
                {
                    case Operator.Equal:
                    case Operator.Grather:
                    case Operator.Lower:
                    case Operator.GratherEqual:
                    case Operator.LowerEqual:
                    case Operator.NotEqual:
                    case Operator.StartsWith:
                    case Operator.EndsWith:
                    case Operator.Contains:
                        return $" {Name} {SqlFilterStatement} {SqlParameter} ";

                    case Operator.IsNull:
                    case Operator.IsNotNull:
                        return $" {Name} {SqlParameter} ";
                    default:
                        throw new Exception("invalid operator");
                }
            }
        }

        public override string ToString() => SqlFilterStatement;

        public void AddToDapperParameters(DynamicParameters dynamicParameters) => dynamicParameters.Add(Name, Value);
    }

    

    public enum Operator
    {
        Equal = 0,
        Grather = 1,
        Lower = 2,
        GratherEqual = 3,
        LowerEqual = 4,
        NotEqual = 5,
        StartsWith = 6,
        EndsWith = 7,
        Contains = 8,
        IsNull = 9,
        IsNotNull = 10
    }
}
