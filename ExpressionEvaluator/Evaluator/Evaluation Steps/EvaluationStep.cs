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

        abstract public void Evaluate(List<string> expression);
        abstract protected string PerformOperation(List<string> oper, int operIndex);
    }
}
