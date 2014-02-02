using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace LambdaOptimizer.Core
{
    public class MethodCallVisitor : ExpressionVisitor
    {
        private readonly MethodCallExpression _funcExpression;
        private readonly Dictionary<ReadOnlyCollection<Expression>, ParameterExpression> _oldArgsToNewArgsMap;
        private readonly Dictionary<ParameterExpression, LambdaExpression> _newArgsToPreparedLambdaMap;
        private readonly ParamsCollectionComparer _paramsCollectionComparer;

        public List<PreparedLambda> GetPreparedLambdas()
        {
            return _newArgsToPreparedLambdaMap.Select(pair => new PreparedLambda { Parameter = pair.Key, Lambda = pair.Value }).ToList();
        }

        public IEnumerable<ParameterExpression> GetOptimizedLambdaParameters()
        {
            return _newArgsToPreparedLambdaMap.Keys;
        }

        public MethodCallVisitor(MethodCallExpression funcExpression)
        {
            _funcExpression = funcExpression;
            _oldArgsToNewArgsMap = new Dictionary<ReadOnlyCollection<Expression>, ParameterExpression>();
            _newArgsToPreparedLambdaMap = new Dictionary<ParameterExpression, LambdaExpression>();
            _paramsCollectionComparer = new ParamsCollectionComparer();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //Проверяем что нашли вызов оптимизируемой функции
            if (_funcExpression.Method != node.Method)
                return base.VisitMethodCall(node);
            
            //Аргументы данного вызова функции ставим в соответствие с предварительно вычисляемым параметром
            ParameterExpression modifiedNode;
            var key = _oldArgsToNewArgsMap.Keys.FirstOrDefault(collection => _paramsCollectionComparer.Equals(collection, node.Arguments));
            if (key == null)
            {
                //Заменяем данный вызов функции на параметр вида p1
                modifiedNode = GetPreparedParameter(node);
                //Маппинг параметров функции на параметры p1
                _oldArgsToNewArgsMap.Add(node.Arguments, modifiedNode);
                //Маппинг p1 на предварительно вычисляемое лямбда выражение
                _newArgsToPreparedLambdaMap.Add(modifiedNode, GetModifiedExpression(node));
            }
            else
            {
                modifiedNode = _oldArgsToNewArgsMap[key];
            }

            //Подменяем вызов на предварительное вычисляемый параметр
            return modifiedNode;
        }


        private LambdaExpression GetModifiedExpression(MethodCallExpression methodCall)
        {
            //Получаем параметры из вызова
            var parametersVisitor = new ParametersVisitor();
            parametersVisitor.Visit(methodCall.Arguments);
            var parameters = parametersVisitor.GetParams();

            //Формируем лямбду вида: params => F(params)
            var invokeExpr = Expression.Lambda(methodCall, parameters);
            return invokeExpr;
        }

        private ParameterExpression GetPreparedParameter(MethodCallExpression methodCall)
        {
            return Expression.Parameter(methodCall.Type, String.Format("p{0}", _oldArgsToNewArgsMap.Count + 1));
        }
    }
}