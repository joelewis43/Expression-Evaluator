﻿using System;
using System.Collections.Generic;
using Evaluator;
using Evaluator.Evaluation_Steps;

namespace TestFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExpressionEvaluator();
        }

        static void TestExpressionEvaluator()
        {
            ExpressionSolver oEvaluator = new ExpressionSolver();

            int actualResult = 0, expectedResult, testCount = 0, errorCount = 0;
            bool retValue = false;
            bool lineIsExpression = true;
            string[] testLines = System.IO.File.ReadAllLines("TestInputs.txt");

            foreach (string line in testLines)
            {
                if (lineIsExpression)
                {
                    Console.WriteLine("\n\nTesting expression: " + line);
                    retValue = oEvaluator.Evaluate(line.Split(' '), out actualResult);

                    testCount++;

                    lineIsExpression = false;
                }
                else
                {
                    // only two acceptable format for the answer line of input file
                    // a number - representing the solution of the expression
                    // false    - indicating the input was expected to fail

                    if (Int32.TryParse(line, out expectedResult))
                    {
                        Console.WriteLine(String.Format("Comparing expected result: {0} to the actual result {1}", expectedResult, actualResult));
                        // if the line is a number, compare to the output
                        if (actualResult == expectedResult) Console.WriteLine("Passed!");
                        else
                        {
                            errorCount++;
                            Console.WriteLine("Failed!");
                        }
                    }
                    else
                    {
                        // if the line wasn't a number, we expect the input to fail
                        if (!retValue) Console.WriteLine("Passed!");
                        else
                        {
                            errorCount++;
                            Console.WriteLine("Failed!");
                        }
                    }

                    lineIsExpression = true;
                }
            }

            Console.WriteLine("\n\n========================= Results =========================");
            Console.WriteLine(String.Format("{0} test case(s) failed out of {1} that ran", errorCount, testCount));
            Console.WriteLine("===========================================================");
        }

        static void TestEvaluationStep()
        {
            AdditionStep addTest = new AdditionStep();
            MultiplicationStep multTest = new MultiplicationStep();

            List<string> addInputs = new List<string> { "2", "+", "12", "*", "5", "+", "1", "*", "2", "+", "12", "*", "5", "+", "1" };
            List<string> multInputs = new List<string> { "2", "+", "12", "*", "5", "+", "1", "*", "2", "+", "12", "*", "5", "+", "1" };


            addTest.Evaluate(addInputs);
            Console.WriteLine(String.Join(",", addInputs));

            multTest.Evaluate(multInputs);
            Console.WriteLine(String.Join(",", multInputs));
        }


    }
}
