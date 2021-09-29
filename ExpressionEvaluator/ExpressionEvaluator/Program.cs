using System;
using Evaluator;

namespace ExpressionEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            int result;
            ExpressionSolver oEvaluator = new ExpressionSolver();
            
            if (oEvaluator.Evaluate(args, out result))
            {
                Console.WriteLine(String.Join(" ", args) + " = " + result);
            }
        }
    }
}