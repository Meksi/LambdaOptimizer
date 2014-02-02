using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LambdaOptimizer.Core;

namespace LambdaOptimizer
{
    class Program
    {
        private static int F(int x)
        {
            return x;
        }

        static void Main(string[] args)
        {
            //Лямбда для оптимизации
            Expression<Func<int, int, int>> exp = (x, y) => F(x) > F(y) ? F(x) : (F(x) < F(2 * y) ? F(2 * y) : F(y));
            LambdaExpression lambda = exp;

            //Функция F(x)
            var intParam = Expression.Parameter(typeof (Int32), "x");
            var funcF = Expression.Call(typeof (Program).GetMethod("F", BindingFlags.Static | BindingFlags.NonPublic), intParam);

            //Проводим оптимизацию
            var optimizer = new Optimizer();
            var result = optimizer.Optimize(lambda, funcF, 1, 1);

            Console.WriteLine("Исходная лямбда:{0}{1}{0}", Environment.NewLine, exp);
            Console.WriteLine("Функция:{0}{1}{0}", Environment.NewLine, funcF);

            Console.WriteLine("Предварительно вычисленные параметры оптимизированной лямбды:");
            foreach (var preparedLambda in result.PreparedLambdas)
                Console.WriteLine("{0} = {1} : {2}", preparedLambda.Parameter, preparedLambda.Lambda, preparedLambda.InvocationResult);

            Console.WriteLine("{0}Оптимизированная лямбда:{0}{1}{0}", Environment.NewLine, result.OptimizedExpression);
            Console.WriteLine("Результат: {0}",result.FunctionResult);

            Console.ReadLine();
        }
    }
}
