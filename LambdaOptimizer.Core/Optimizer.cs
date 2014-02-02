using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LambdaOptimizer.Core
{
    public class Optimizer
    {
        public OptimizationResult Optimize(LambdaExpression le, MethodCallExpression funcF, params object[] args)
        {
            //Маппим параметры выражения на переданные параметры
            var parameters = new ParametersCollection(le, args);

            var result = OptimizeLambda(le, funcF);
            result = InvokeOptimizedLambda(result, parameters);

            return result;
        }

        private OptimizationResult OptimizeLambda(LambdaExpression le, MethodCallExpression funcF)
        {
            //Обходим лямбду и выполняем оптимизацию выражения
            var visitor = new MethodCallVisitor(funcF);
            var result = visitor.Visit(le) as LambdaExpression;
            if (result == null)
                throw new InvalidOperationException("Не получили лямбду при обходе");

            //Подменяем параметры лямбды
            var @params = visitor.GetOptimizedLambdaParameters();
            result = Expression.Lambda(result.Body, @params);

            return new OptimizationResult
            {
                OptimizedExpression = result,
                PreparedLambdas = visitor.GetPreparedLambdas()
            };
        }

        private OptimizationResult InvokeOptimizedLambda(OptimizationResult result, ParametersCollection parameters)
        {
            //Вычисляем предварительные параметры
            foreach (var preparedLambda in result.PreparedLambdas)
            {
                var compiledLambda = preparedLambda.Lambda.Compile();
                var lambdaArgs = parameters.GetParametersForExpression(preparedLambda.Lambda.Parameters);
                var preparedResult = compiledLambda.DynamicInvoke(lambdaArgs);
                preparedLambda.InvocationResult = preparedResult;
            }

            //Вычисляем оптимизированное выражение
            var compiledFunc = result.OptimizedExpression.Compile();
            var optimizedParameters = result.PreparedLambdas.Select(lambda => lambda.InvocationResult).ToArray();
            result.FunctionResult = compiledFunc.DynamicInvoke(optimizedParameters);

            return result;
        }
    }
}
