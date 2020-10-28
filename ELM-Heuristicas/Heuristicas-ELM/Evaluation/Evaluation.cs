using Heuristicas_ELM.ANN;
using Heuristicas_ELM.core;

namespace Heuristicas_ELM.Evaluation
{
    public abstract class Evaluation
    {
        public double Accuracy;
        public double Time;
        public readonly ELM MyELM;
        public Instances MyInstances;

        protected Evaluation(ELM elm)
        {
            MyELM = elm;
        }

        public abstract void Execute();
    }
}
