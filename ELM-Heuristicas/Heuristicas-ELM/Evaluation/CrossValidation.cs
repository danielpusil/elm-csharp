using System.Collections.Generic;
using DotNetMatrix;
using Heuristicas_ELM.ANN;
using Heuristicas_ELM.core;

namespace Heuristicas_ELM.Evaluation
{
    public class CrossValidation:Evaluation
    {
        public readonly int NumberOfFolders;

        public CrossValidation(ELM elm, Instances instances, int folders): base(elm)
        {
            MyInstances = instances;
            NumberOfFolders = folders;
        }

        public override void Execute()
        {
            var mydata = MyInstances.Data.Sort(0); // Sort data based on class

            // Define randomly the folder's content
            var folder = new List<int>();
            var belongsto = 0;
            for (var i = 0; i < mydata.RowDimension; i++)
            {
                folder.Add(belongsto++);
                if (belongsto % NumberOfFolders == 0) belongsto = 0;
            }

            var accuracyAvg = 0.0d;
            var time = 0.0d;

            for (var f = 0; f < NumberOfFolders; f++)
            {
                // Create data for training and testing
                var training = new List<double[]>();
                var testing = new List<double[]>();
                for (var row = 0; row < mydata.RowDimension; row++)
                {
                    var newrow = new double[mydata.ColumnDimension];
                    mydata.Array[row].CopyTo(newrow,0);
                    if (folder[row] == f) testing.Add(newrow);
                    else  training.Add(newrow);
                }
                var insTraining = new Instances
                {
                    Data = new GeneralMatrix(training.ToArray()),
                    NumberofOutputNeurons = MyInstances.NumberofOutputNeurons
                };
                var insTesting = new Instances
                {
                    Data = new GeneralMatrix(testing.ToArray()),
                    NumberofOutputNeurons = MyInstances.NumberofOutputNeurons
                };

                // Training and Testing of the ELM                
                MyELM.Train(insTraining);
                MyELM.Test(insTesting);

                accuracyAvg += MyELM.TestingAccuracy;
                time += MyELM.TestingTime;
            }
            Accuracy = accuracyAvg / NumberOfFolders;
            Time = time / NumberOfFolders;
        }
    }
}