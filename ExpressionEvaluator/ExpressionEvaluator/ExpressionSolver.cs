using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionEvaluator
{
    class ExpressionSolver
    {
        private string InputExpression;
        private List<char> ValidOperators = new List<char> { '(', ')', '+', '*'};


        public ExpressionSolver()
        {
            InputExpression = "";
        }

        public bool Evaluate(string[] args)
        {
            bool retValue;
            string errorMsg;

            retValue = ProcessInputs(args, out errorMsg);

            if (!retValue)
            {
                Console.WriteLine(errorMsg);
                return false;
            }




            return true;
        }

        private bool ProcessInputs(string[] args, out string errorMsg)
        {
            errorMsg = "";

            if (args.Length == 0)
            {
                errorMsg = "Missing expression input. Correct usage for example expression 4+12*(3+4):\n\tExpressionEvaluator.exe \"4 + 12 * (3 + 4)\" OR\n\tExpressionEvaluator.exe 4 + 12 * (3 + 4) OR\n\tExpressionEvaluator.exe 4+12*(3+4)";
                return false;
            }
            else if (args.Length == 1)
            {
                InputExpression = args[0];
            }
            else
            {
                // Console app input is space separated, join with spaces between to recreate exact input
                InputExpression = String.Join(" ", args);
            }

            #if DEBUG
            Console.WriteLine("Processing input: " + InputExpression);
            #endif

            // validate
            // no spaces between numbers (missing operator)
            // parens are opened and closed as expected
            // all operators are recognized (no letters or invalid symbols)

            #region Parentheses Check
            int parenCount = 0;
            foreach (char c in InputExpression)
            {
                if (c == '(') parenCount++;
                else if (c == ')') parenCount--;

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

            #region Extra Spaces Check
            int temp;
            bool lastCharWasInt = false;
            foreach (string s in String.Join(" ", InputExpression.Split('(', ')')).Split(' '))
            {
                // at this point, there is no whitespace in the input
                // we are looking for numbers in consecutive indices

                /*
                 * Examples
                 * Input            => remove parens    => final split
                 * 5 + 2 1          => 5 + 2 1		    => 5,+,2,1
                 * (5 + 2) 1	    =>  5 + 2  1		=> 5,+,2,1
                 * (5 + 2) * (2 1)	=>  5 + 2  *  2 1	=> 5,+,2,*,2,1
                 */

                Console.WriteLine("NEED TO COME BACK AND ADDRESS THE ISSUE OF AN OPERATOR AND NUMBER NOT BEING SEPARATED BY A SPACE");
                // ex 12 + 3 +5

                if (Int32.TryParse(s, out temp))
                {
                    if (lastCharWasInt)
                    {
                        errorMsg = "Invalid input! An operator is missing. Please double check the input.";
                        return false;
                    }
                    lastCharWasInt = true;
                }
                else
                {
                    lastCharWasInt = false;
                }
            }
            #endregion

            #region Unrecognized Operators Check
            foreach (char c in InputExpression)
            {
                if (!char.IsDigit(c) && !c.Equals(' ') && !ValidOperators.Contains(c))
                {
                    errorMsg = "Invalid input! Input contains an unrecognized operator \'" + c + "\'. Please double check the input.";
                    errorMsg += "\n\tValid operators: " + String.Join(" ", ValidOperators);
                    return false;
                }
            }
            #endregion

            return true;
        }



    }
}
