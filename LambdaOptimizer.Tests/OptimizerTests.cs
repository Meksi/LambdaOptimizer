using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LambdaOptimizer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaOptimizer.Tests
{
    [TestClass]
    public class OptimizerTests
    {
        private static int F(int x)
        {
            return x;
        }

        [TestMethod]
        public void OptimizeLinearExpression()
        {
            Expression<Func<int, int>> exp = x => F(x);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof(Int32), "x");
            var funcF = Expression.Call(typeof(OptimizerTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);
            var optimizer = new Optimizer();

            var result = optimizer.Optimize(lambda, funcF, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.FunctionResult);
            Assert.AreEqual("p1 => p1", result.OptimizedExpression.ToString());
            Assert.IsNotNull(result.PreparedLambdas);
            Assert.AreEqual(1, result.PreparedLambdas.Count);

            var preparedLambda = result.PreparedLambdas.FirstOrDefault();
            Assert.IsNotNull(preparedLambda);
            Assert.AreEqual("x => F(x)", preparedLambda.Lambda.ToString());
            Assert.AreEqual("p1", preparedLambda.Parameter.ToString());
            Assert.AreEqual(1, preparedLambda.InvocationResult);
        }

        [TestMethod]
        public void OptimizeLinearExpressionWithTwoParameters()
        {
            Expression<Func<int, int, int>> exp = (x, y) => F(x) + F(y);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof(Int32), "x");
            var funcF = Expression.Call(typeof(OptimizerTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);
            var optimizer = new Optimizer();

            var result = optimizer.Optimize(lambda, funcF, 1, 2);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.FunctionResult);
            Assert.AreEqual("(p1, p2) => (p1 + p2)", result.OptimizedExpression.ToString());
            Assert.IsNotNull(result.PreparedLambdas);
            Assert.AreEqual(2, result.PreparedLambdas.Count);

            var firstLambda = result.PreparedLambdas.FirstOrDefault();
            Assert.IsNotNull(firstLambda);
            Assert.AreEqual("x => F(x)", firstLambda.Lambda.ToString());
            Assert.AreEqual("p1", firstLambda.Parameter.ToString());
            Assert.AreEqual(1, firstLambda.InvocationResult);

            var secondLambda = result.PreparedLambdas.LastOrDefault();
            Assert.IsNotNull(secondLambda);
            Assert.AreEqual("y => F(y)", secondLambda.Lambda.ToString());
            Assert.AreEqual("p2", secondLambda.Parameter.ToString());
            Assert.AreEqual(2, secondLambda.InvocationResult);
        }
    }
}
