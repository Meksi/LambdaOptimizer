using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LambdaOptimizer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaOptimizer.Tests
{
    [TestClass]
    public class MethodCallVisitorTests
    {
        private static int F(int x)
        {
            return x;
        }

        [TestMethod]
        public void VisitLinearExpression()
        {
            Expression<Func<int, int>> exp = x => F(x);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof (Int32), "x");
            var funcF = Expression.Call(typeof (MethodCallVisitorTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);

            var methodCallVisitor = new MethodCallVisitor(funcF);
            methodCallVisitor.Visit(lambda);

            var lambdas = methodCallVisitor.GetPreparedLambdas();
            Assert.IsNotNull(lambdas);
            Assert.AreEqual(1, lambdas.Count);

            var preparedLambda = lambdas.FirstOrDefault();
            Assert.IsNotNull(preparedLambda);
            Assert.AreEqual("x => F(x)", preparedLambda.Lambda.ToString());
            Assert.AreEqual("p1", preparedLambda.Parameter.ToString());
            Assert.IsNull(preparedLambda.InvocationResult);
        }

        [TestMethod]
        public void VisitQuadraticExpression()
        {
            Expression<Func<int, int>> exp = x => F(x) + F(x*x);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof(Int32), "x");
            var funcF = Expression.Call(typeof(MethodCallVisitorTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);

            var methodCallVisitor = new MethodCallVisitor(funcF);
            methodCallVisitor.Visit(lambda);

            var lambdas = methodCallVisitor.GetPreparedLambdas();
            Assert.IsNotNull(lambdas);
            Assert.AreEqual(2, lambdas.Count);

            var firstLambda = lambdas.FirstOrDefault();
            Assert.IsNotNull(firstLambda);
            Assert.AreEqual("x => F(x)", firstLambda.Lambda.ToString());
            Assert.AreEqual("p1", firstLambda.Parameter.ToString());
            Assert.IsNull(firstLambda.InvocationResult);

            var secondLambda = lambdas.LastOrDefault();
            Assert.IsNotNull(secondLambda);
            Assert.AreEqual("x => F((x * x))", secondLambda.Lambda.ToString());
            Assert.AreEqual("p2", secondLambda.Parameter.ToString());
            Assert.IsNull(secondLambda.InvocationResult);
        }

        [TestMethod]
        public void VisitLinearExpressionWithTwoParameters()
        {
            Expression<Func<int, int, int>> exp = (x, y) => F(x) + F(y);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof(Int32), "x");
            var funcF = Expression.Call(typeof(MethodCallVisitorTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);

            var methodCallVisitor = new MethodCallVisitor(funcF);
            methodCallVisitor.Visit(lambda);

            var lambdas = methodCallVisitor.GetPreparedLambdas();
            Assert.IsNotNull(lambdas);
            Assert.AreEqual(2, lambdas.Count);

            var firstLambda = lambdas.FirstOrDefault();
            Assert.IsNotNull(firstLambda);
            Assert.AreEqual("x => F(x)", firstLambda.Lambda.ToString());
            Assert.AreEqual("p1", firstLambda.Parameter.ToString());
            Assert.IsNull(firstLambda.InvocationResult);

            var secondLambda = lambdas.LastOrDefault();
            Assert.IsNotNull(secondLambda);
            Assert.AreEqual("y => F(y)", secondLambda.Lambda.ToString());
            Assert.AreEqual("p2", secondLambda.Parameter.ToString());
            Assert.IsNull(secondLambda.InvocationResult);
        }

        //TODO как правильно сравнивать параметры
        [TestMethod]
        public void VisitExpressionWithIdenticalParameters()
        {
            Expression<Func<int, int>> exp = x => F(2*x) + F(x*2);
            LambdaExpression lambda = exp;
            var intParam = Expression.Parameter(typeof(Int32), "x");
            var funcF = Expression.Call(typeof(MethodCallVisitorTests).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);

            var methodCallVisitor = new MethodCallVisitor(funcF);
            methodCallVisitor.Visit(lambda);

            var lambdas = methodCallVisitor.GetPreparedLambdas();
            Assert.IsNotNull(lambdas);
            Assert.AreEqual(1, lambdas.Count);

            var preparedLambda = lambdas.FirstOrDefault();
            Assert.IsNotNull(preparedLambda);
            Assert.AreEqual("x => F((2*x))", preparedLambda.Lambda.ToString());
            Assert.AreEqual("p1", preparedLambda.Parameter.ToString());
            Assert.IsNull(preparedLambda.InvocationResult);
        }
    }
}
