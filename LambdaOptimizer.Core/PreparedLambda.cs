using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LambdaOptimizer.Core
{
    public class PreparedLambda
    {
        public ParameterExpression Parameter { get; set; }
        public LambdaExpression Lambda { get; set; }
        public object InvocationResult { get; set; }
    }
}
