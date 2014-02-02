using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LambdaOptimizer.Core
{
    public class OptimizationResult
    {
        public LambdaExpression OptimizedExpression { get; set; }
        public List<PreparedLambda> PreparedLambdas { get; set; }
        public Object FunctionResult { get; set; }
    }
}
