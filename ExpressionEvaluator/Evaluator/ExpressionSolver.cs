using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Evaluator.Evaluation_Steps;

namespace Evaluator
{
    public class ExpressionSolver
    {
        private List<string> ValidOperators;
        private List<EvaluationStep> EvaluationSteps;
        private List<string> Expression;


        public ExpressionSolver()
        {
            EvaluationSteps = new List<EvaluationStep>();

            // add a step in the order you want it to be processed (i.e. add multiplcation before addition)
            EvaluationSteps.Add(new MultiplicationStep());
            EvaluationSteps.Add(new AdditionStep());

            GetValidOperators();
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

            result = ProcessExpression(Expression);

            return true;
        }

        private bool ProcessInputs(string[] args, out string errorMsg)
        {
            string input;

            errorMsg = "";

            if (args.Length == 0)
            {
                errorMsg = "Missing expression input. Correct usage for example expression 4+12*(3+4):\n\tExpressionEvaluator.exe \"4 + 12 * (3 + 4)\" OR\n\tExpressionEvaluator.exe 4 + 12 * (3 + 4) OR\n\tExpressionEvaluator.exe 4+12*(3+4)";
                return false;
            }
            else if (args.Length == 1)
            {
                input = args[0];
            }
            else
            {
                // Join with spaces to prevent two numbers from being improperly joined
                input = String.Join(" ", args);
            }

#if DEBUG
            Console.WriteLine("Processing input: " + input);
#endif

            BuildExpressionList(input);

            // validate
            // parens are opened and closed as expected
            // no spaces between numbers (missing operator)
            // all operators are recognized (no letters or invalid symbols)

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

            /*
             * Looking for numbers in consecutive indices (ignoring parentheses)
             * 
             * Examples
             * 5+2 1            => 5,+,2,1
             * 5 + 2 1          => 5,+,2,1
             * (5 + 2) 1	    => (,5,+,2,),1
             * (5 + 2) * (2 1)	=> (,5,+,2,),*,(,2,1,)
             */

            foreach (string s in Expression)
            {
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
            foreach (string s in Expression)
            {
                if (!Int32.TryParse(s, out temp) && !s.Equals(' ') && !ValidOperators.Contains(s))
                {
                    errorMsg = "Invalid input! Input contains an unrecognized operator \'" + s + "\'. Please double check the input.";
                    errorMsg += "\n\tValid operators: " + String.Join(" ", ValidOperators);
                    return false;
                }
            }
            #endregion

#if DEBUG
            Console.WriteLine("Validation complete.");
            Console.WriteLine("Final input string: " + String.Join("", Expression));
#endif

            return true;
        }

        private void BuildExpressionList(string input)
        {
            // Create a list where each index is one value from the expression
            // { "(", "123", "+", "45", ")", "*", "3" }

            // Can't simply input.Split(" ") because numbers and parenteses won't be space separated

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
        }

        private int ProcessExpression(List<string> expression)
        {
            int first = 0, parenCount = 0, value = 0;

#if DEBUG
            Console.WriteLine("Processing " + String.Join("", expression));
#endif

            // walk through and identify paren sections
            for (int i = 0; i < expression.Count; i++)
            {
                if (expression[i] == "(")
                {
                    if (parenCount == 0) first = i;

                    parenCount++;
                }
                else if (expression[i] == ")")
                {
                    parenCount--;

                    // once we have a fully closed paren section, evaluate it
                    if (parenCount == 0)
                    {
                        value = ProcessExpression(expression.GetRange(first + 1, i - first - 1));

                        // now the value has to replace everything from first-i
                        expression[i] = value.ToString();

                        for (int j = i-1; j >= first; j--) 
                        {
                            expression.RemoveAt(j);
                            i--;
                        }
                    }
                }
            }

            foreach (EvaluationStep es in EvaluationSteps)
            {
                es.Evaluate(expression);
            }



            return Int32.Parse(expression[0]);
        }

        private void GetValidOperators()
        {
            ValidOperators = new List<string>() { "(", ")" };

            foreach (EvaluationStep es in EvaluationSteps)
            {
                ValidOperators.AddRange(es.GetValidOperators());
            }
        }

    }
}
