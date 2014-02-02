using System.Collections.Generic;
using System.Linq.Expressions;

namespace LambdaOptimizer.Core
{
    public class ParametersVisitor : ExpressionVisitor
    {
        private readonly HashSet<ParameterExpression> _parameters = new HashSet<ParameterExpression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _parameters.Add(node);
            return base.VisitParameter(node);
        }

        public IEnumerable<ParameterExpression> GetParams()
        {

            return _parameters;
        }
    }
}