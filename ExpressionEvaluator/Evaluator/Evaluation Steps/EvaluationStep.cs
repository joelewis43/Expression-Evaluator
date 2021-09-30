using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    abstract public class EvaluationStep
    {
        protected List<string> ValidOperators;

        public EvaluationStep()
        {
            ValidOperators = new List<string>();
        }

        abstract protected string PerformOperation(List<string> oper, int operIndex);

        /*
         * Name:        Evaluate
         * Inputs:      String array of input numbers and operators
         * Outputs:     None
         * Description: Evaluates any valid operator found in the input. Replacing the operator
         *              and the numbers to it's left and right with the resulting value.
         */
        virtual public void Evaluate(List<string> expression)
        {
            string val;
            int i = 0;

            while (i < expression.Count)
            {
                if (ValidOperators.Contains(expression[i]))
                {
                    val = PerformOperation(expression, i);

                    expression[i + 1] = val;

                    expression.RemoveAt(i);
                    expression.RemoveAt(i - 1);

                    // decrement to account for removing elements
                    i--;
                }

                i++;
            }
        }

        public List<string> GetValidOperators()
        {
            return ValidOperators;
        }
    }
}
