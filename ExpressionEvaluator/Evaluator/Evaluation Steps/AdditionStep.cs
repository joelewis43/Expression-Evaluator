using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator.Evaluation_Steps
{
    public class AdditionStep : EvaluationStep
    {

        public AdditionStep()
        {
            this.ValidOperators = new List<string> { "+" };
        }

        /*
         * Name:        PerformOperation
         * Inputs:      String array of input numbers and operators, the index of the operator to evaluate
         * Outputs:     The result of the operation as a string
         * Description: Determines the operator at the provided index and evaluates it.
         */
        override protected string PerformOperation(List<string> expression, int operIndex)
        {
            int left =  Int32.Parse(expression[operIndex - 1]);
            int right = Int32.Parse(expression[operIndex + 1]);
            string res = "";

            switch (expression[operIndex])
            {
                case "+":
                    res = (left + right).ToString();
                    break;
            }

            return res;
        }


    }
}
