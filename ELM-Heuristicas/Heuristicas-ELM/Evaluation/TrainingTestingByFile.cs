using Heuristicas_ELM.ANN;
using Heuristicas_ELM.core;

namespace Heuristicas_ELM.Evaluation
{
    public class TrainingTestingByFile:Evaluation
    {
        public readonly Instances InstacesTraining;
        public readonly Instances InstancesTesting;

        public TrainingTestingByFile(ELM elm, Instances insTraining, Instances insTesting):
            base(elm)
        {
            MyInstances = insTraining;
            InstacesTraining = insTraining;
            InstancesTesting = insTesting;
        }

        public override void Execute()
        {
            MyELM.Train(InstacesTraining);
            MyELM.Test(InstancesTesting);
            Accuracy = MyELM.TestingAccuracy;
            Time = MyELM.TestingTime;
        }
    }
}