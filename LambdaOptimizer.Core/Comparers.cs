using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace LambdaOptimizer.Core
{
    public class ExpressionComparer : IEqualityComparer<Expression>
    {
        public bool Equals(Expression x, Expression y)
        {
            //TODO сделать нормальную проверку
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(Expression obj)
        {
            return obj.ToString().GetHashCode();
        }
    }

    public class ParamsCollectionComparer : IEqualityComparer<IReadOnlyCollection<Expression>>
    {
        public bool Equals(IReadOnlyCollection<Expression> x, IReadOnlyCollection<Expression> y)
        {
            return x.SequenceEqual(y, new ExpressionComparer());
        }

        public int GetHashCode(IReadOnlyCollection<Expression> obj)
        {
            int hc = 0;
            foreach (var expression in obj)
                hc ^= expression.GetHashCode();

            return hc;
        }
    }

    public class ParametersComparer : IEqualityComparer<ParameterExpression>
    {
        public bool Equals(ParameterExpression x, ParameterExpression y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(ParameterExpression obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
