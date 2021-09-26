using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Evaluator.Evaluation_Steps;

namespace Evaluator
{
    public class ExpressionSolver
    {
        private List<char> ValidOperators = new List<char> { '(', ')', '+', '*' };
        private List<EvaluationStep> EvaluationSteps;
        private List<string> Expression;


        public ExpressionSolver()
        {
            EvaluationSteps = new List<EvaluationStep>();

            // add a step in the order you want it to be processed (i.e. add multiplcation before addition)
            EvaluationSteps.Add(new MultiplicationStep());
            EvaluationSteps.Add(new AdditionStep());
        }

        public bool Evaluate(string[] args, out int result)
        {
            Expression = new List<string>();

            bool retValue;
            string errorMsg;
            result = 0;

            retValue = ProcessInputs(args, out errorMsg);

            if (!retValue)
            {
                Console.WriteLine(errorMsg);
                return false;
            }

            // now that we have valid input, we can evaluate the expression
            result = ProcessExpression();

            return true;
        }

        private bool ProcessInputs(string[] args, out string errorMsg)
        {
            string inputExpression;
            bool retValue;

            errorMsg = "";

            if (args.Length == 0)
            {
                errorMsg = "Missing expression input. Correct usage for example expression 4+12*(3+4):\n\tExpressionEvaluator.exe \"4 + 12 * (3 + 4)\" OR\n\tExpressionEvaluator.exe 4 + 12 * (3 + 4) OR\n\tExpressionEvaluator.exe 4+12*(3+4)";
                return false;
            }
            else if (args.Length == 1)
            {
                inputExpression = args[0];
            }
            else
            {
                // Console app input is space separated, join with spaces between to recreate exact input
                inputExpression = String.Join(" ", args);
            }

#if DEBUG
            Console.WriteLine("Processing input: " + inputExpression);
#endif

            // validate
            // no spaces between numbers (missing operator)
            // parens are opened and closed as expected
            // all operators are recognized (no letters or invalid symbols)

            BuildExpressionList(inputExpression);

            #region Parentheses Check
            int parenCount = 0;
            foreach (string s in Expression)
            {
                if (s == "(") parenCount++;
                else if (s == ")") parenCount--;

                if (parenCount < 0)
                {
                    errorMsg = "Invalid input! Please check all parentheses match.";
                    return false;
                }
            }

            if (parenCount > 0)
            {
                errorMsg = "Invalid input! Please check all parentheses match.";
                return false;
            }
            #endregion

            #region Missing Operator Check
            int temp;
            bool lastCharWasInt = false;
            foreach (string s in Expression)
            {
                // at this point, there is no whitespace in the input
                // we are looking for numbers in consecutive indices (ignoring parentheses)

                /*
                 * Examples
                 * Input            => final split
                 * 5+2 1            => 5,+,2,1
                 * 5 + 2 1          => 5,+,2,1
                 * (5 + 2) 1	    => (,5,+,2,),1
                 * (5 + 2) * (2 1)	=> (,5,+,2,),*,(,2,1,)
                 */

                if (Int32.TryParse(s, out temp))
                {
                    if (lastCharWasInt)
                    {
                        errorMsg = "Invalid input! An operator is missing. Please double check the input.";
                        return false;
                    }
                    lastCharWasInt = true;
                }
                else if (s.Equals("(") || s.Equals(")")) { /* Ignore parentheses */ }
                else
                {
                    lastCharWasInt = false;
                }
            }
            #endregion

            #region Unrecognized Operators Check
            foreach (char c in inputExpression)
            {
                if (!char.IsDigit(c) && !c.Equals(' ') && !ValidOperators.Contains(c))
                {
                    errorMsg = "Invalid input! Input contains an unrecognized operator \'" + c + "\'. Please double check the input.";
                    errorMsg += "\n\tValid operators: " + String.Join(" ", ValidOperators);
                    return false;
                }
            }
            #endregion

#if DEBUG
            Console.WriteLine("Validation complete.");
            Console.WriteLine(String.Format("Final input string: {0}", String.Join("", Expression)));
#endif

            return true;
        }

        private void BuildExpressionList(string input)
        {
            // we know there is some input, we don't know what the input is
            // all we want to do is create a list where each index is one value from the expression
            // { "(", "123", "+", "45", ")", "*", "3" }

            string currentNumber = String.Empty;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else
                {
                    // non digit value

                    if (!currentNumber.Equals(String.Empty))
                    {
                        Expression.Add(currentNumber);
                        currentNumber = String.Empty;
                    }

                    if (c.Equals(' ')) continue;

                    Expression.Add(c.ToString());

                }
            }

            if (!currentNumber.Equals(String.Empty)) Expression.Add(currentNumber);

            Console.WriteLine(String.Join(", ", Expression));
        }

        private int ProcessExpression()
        {




            return 0;
        }



    }
}
