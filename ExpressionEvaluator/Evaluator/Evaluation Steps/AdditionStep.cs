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

        override public void Evaluate(List<string> expression)
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
                }

                i++;
            }
        }

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
