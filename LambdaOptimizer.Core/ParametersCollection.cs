using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LambdaOptimizer.Core
{
    public class ParametersCollection
    {
        private Dictionary<ParameterExpression, object> _map;

        public ParametersCollection(LambdaExpression le, params object[] args)
        {
            if (le.Parameters.Count != args.Count())
                throw new InvalidOperationException("Params count mismatch");

            _map = new Dictionary<ParameterExpression, object>();
            for (int i = 0; i < le.Parameters.Count; i++)
                _map.Add(le.Parameters[i], args[i]);
        }

        public object[] GetParametersForExpression(ReadOnlyCollection<ParameterExpression> parameters)
        {
            var paramsComparer = new ParametersComparer();

            return _map
                .Where(pair => parameters.Contains(pair.Key, paramsComparer))
                .Select(pair => pair.Value)
                .ToArray();
        }
    }
}
