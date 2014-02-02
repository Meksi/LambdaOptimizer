using System;
using System.Linq;
using System.Linq.Expressions;
using LambdaOptimizer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaOptimizer.Tests
{
    [TestClass]
    public class ParametersVisitorTests
    {
        private ParametersVisitor _parametersVisitor;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _parametersVisitor = new ParametersVisitor();
        }

        [TestMethod]
        public void GetSingleParameter()
        {
            var varX = Expression.Parameter(typeof (Int32), "x");

            _parametersVisitor.Visit(varX);
            var parameters = _parametersVisitor.GetParams().ToList();

            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Count());
            Assert.IsTrue(parameters.Contains(varX));
        }

        [TestMethod]
        public void GetSingleParameterFromLinearExpression()
        {
            var varX = Expression.Parameter(typeof(Int32), "x");
            var number = Expression.Constant(2);
            var expr = Expression.Multiply(number, varX);

            _parametersVisitor.Visit(expr);
            var parameters = _parametersVisitor.GetParams().ToList();

            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Count());
            Assert.IsTrue(parameters.Contains(varX));
        }

        [TestMethod]
        public void GetSeveralParameters()
        {
            var varX = Expression.Parameter(typeof(Int32), "x");
            var varY = Expression.Parameter(typeof(Int32), "y");
            var expr = Expression.Multiply(varX, varY);

            _parametersVisitor.Visit(expr);
            var parameters = _parametersVisitor.GetParams().ToList();

            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Count());
            Assert.IsTrue(parameters.Contains(varX));
            Assert.IsTrue(parameters.Contains(varY));
        }

        [TestMethod]
        public void GetSingleParameterWithRepeat()
        {
            var varX = Expression.Parameter(typeof(Int32), "x");
            var expr = Expression.Multiply(varX, varX);

            _parametersVisitor.Visit(expr);
            var parameters = _parametersVisitor.GetParams().ToList();

            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Count());
            Assert.IsTrue(parameters.Contains(varX));
        }
    }
}
