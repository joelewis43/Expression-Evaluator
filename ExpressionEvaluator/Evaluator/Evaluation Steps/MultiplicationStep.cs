using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator.Evaluation_Steps
{
    public class MultiplicationStep : EvaluationStep
    {
        public MultiplicationStep()
        {
            this.ValidOperators = new List<string> { "*" };
        }

        /*
         * Name:        Evaluate
         * Inputs:      String array of input numbers and operators
         * Outputs:     None
         * Description: Evaluates any valid operator found in the input. Replacing the operator
         *              and the numbers to it's left and right with the resulting value.
         */
        override public void Evaluate(List<string> expression)
        {
            string val;
            int i = 0;

            while (i < expression.Count)
            {
                if (ValidOperators.Contains(expression[i]))
                {
                    val = PerformOperation(expression, i);

                    // replace the operator and its right and left values with the result of the operation
                    expression[i + 1] = val;
                    expression.RemoveAt(i);
                    expression.RemoveAt(i - 1);
                }

                i++;
            }
        }

        /*
         * Name:        PerformOperation
         * Inputs:      String array of input numbers and operators, the index of the operator to evaluate
         * Outputs:     The result of the operation as a string
         * Description: Determines the operator at the provided index and evaluates it.
         */
        override protected string PerformOperation(List<string> expression, int operIndex)
        {
            string res = "";

            int left = Int32.Parse(expression[operIndex - 1]);
            int right = Int32.Parse(expression[operIndex + 1]);

            switch (expression[operIndex])
            {
                case "*":
                    res = (left * right).ToString();
                    break;
            }

            return res;
        }

    }
}
