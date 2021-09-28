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

            // add a step in the order you want it to be processed (i.e. PEMDAS; multiplcation before addition)
            EvaluationSteps.Add(new MultiplicationStep());
            EvaluationSteps.Add(new AdditionStep());

            GetValidOperators();
        }

        /*
         * Name:        Evaluate
         * Inputs:      String array of input numbers and operators, out resulting value
         * Outputs:     Bool indicating if the evaluation was successful or not
         * Description: Evaluates the provided input
         */
        public bool Evaluate(string[] input, out int result)
        {
            Expression = new List<string>();

            bool retValue;
            string errorMsg;
            result = 0;

            retValue = ProcessInputs(input, out errorMsg);

            if (!retValue)
            {
                Console.WriteLine(errorMsg);
                return false;
            }

            result = ProcessExpression(Expression);

            return true;
        }

        /*
         * Name:        ProcessInputs
         * Inputs:      String array of input numbers and operators, out error message if any
         * Outputs:     Bool indicating if the provided input is valid or not
         * Description: Validates the input to ensure it is formatted properly and does not
         *              contain unknown values
         */
        private bool ProcessInputs(string[] inputArray, out string errorMsg)
        {
            errorMsg = "";
            string inputString;

            if (inputArray.Length == 0)
            {
                errorMsg = "Missing expression input. Correct usage for example expression 4+12*(3+4):\n\tExpressionEvaluator.exe \"4 + 12 * (3 + 4)\" OR\n\tExpressionEvaluator.exe 4 + 12 * (3 + 4) OR\n\tExpressionEvaluator.exe 4+12*(3+4)";
                return false;
            }
            else if (inputArray.Length == 1)
            {
                inputString = inputArray[0];
            }
            else
            {
                // Join with spaces to prevent two numbers from being improperly joined
                inputString = String.Join(" ", inputArray);
            }

#if DEBUG
            Console.WriteLine("Processing input: " + inputString);
#endif

            BuildExpressionList(inputString);

            #region Validation

            #region Parentheses Check
            // Check that all parentheses match in both count and placement order
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
            // If two numbers occur one after the other, an operator is missing
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
            // Check that all operators in the expression are recognized by the evaluation steps
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

            #endregion

#if DEBUG
            Console.WriteLine("Validation complete.");
            Console.WriteLine("Final input string: " + String.Join("", Expression));
#endif

            return true;
        }

        /*
         * Name:        BuildExpressionList
         * Inputs:      Validated string input consisting of numbers and operators
         * Outputs:     None
         * Description: Converts the input into a List<string> for easy processing. Can't 
         *              only use input.Split(" ") because numbers and parentheses won't be
         *              space separated.
         */
        private void BuildExpressionList(string input)
        {
            // Create a list where each index is one value from the expression
            // { "(", "123", "+", "45", ")", "*", "3" }

            // storage for a multidigit number
            string currentNumber = String.Empty;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else // non digit
                {
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

        /*
         * Name:        ProcessExpression
         * Inputs:      A list of each digit and operator
         * Outputs:     The numeeric solution of the expression
         * Description: A recursive method that iterates through the input in search of
         *              parentheses. When the outer most set of parentheses is found,
         *              the method recursively calls itself with the input as the indices
         *              between the outermost parentheses. Once no parentheses remain, the
         *              remaining expression is evaluated by the EvaluationSteps in order.
         */
        private int ProcessExpression(List<string> expression)
        {
            #region Recursion Visualization
            /*
             * Outer Open                Outer Close
             * |                            |
             * ((2 * (1 + 16) + 12 * 4) * 14) + 2 * 5
             *  |                          |
             * Input start             Input end
             * 
             * Outer Open       Outer Close
             * |                     |
             * (2 * (1 + 16) + 12 * 4) * 14
             *  |                   |
             * Input start       Input end
             * 
             *   Open   Close
             *     |      |
             * 2 * (1 + 16) + 12 * 4
             *      |    |
             *   Start  End
             *   
             * 1 + 16 => 17
             * 
             * 2 * 17 + 12 * 4 => 82
             * 
             * 82 * 14 => 1148
             * 
             * 1148 + 2 * 5 => 1158
             * 
             * 1158
             * 
             */
            #endregion

            int first = 0, parenCount = 0, value = 0;

#if DEBUG
            Console.WriteLine("Processing " + String.Join("", expression));
#endif

            // walk through and identify paren sections
            for (int i = 0; i < expression.Count; i++)
            {
                if (expression[i] == "(")
                {

                    // store the index of the first open paren (outer open)
                    if (parenCount == 0) first = i;

                    parenCount++;
                }
                else if (expression[i] == ")")
                {
                    parenCount--;

                    // reached the index of the outer close paren
                    if (parenCount == 0)
                    {
                        value = ProcessExpression(expression.GetRange(first + 1, i - first - 1));

                        // now the value has to replace everything from first:i
                        
                        // replace the value at i with the new value
                        expression[i] = value.ToString();

                        // remove all values from first : i-1
                        for (int j = i-1; j >= first; j--) 
                        {
                            expression.RemoveAt(j);

                            // decrement i to adjust for removed values
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

        /*
         * Name:        GetValidOperators
         * Inputs:      None
         * Outputs:     None
         * Description: Builds a list of valid operators from all EvaluationSteps
         */
        private void GetValidOperators()
        {
            // There is no evaluation step for parentheses, so we add these manually
            ValidOperators = new List<string>() { "(", ")" };

            foreach (EvaluationStep es in EvaluationSteps)
            {
                ValidOperators.AddRange(es.GetValidOperators());
            }

            
        }

    }
}
