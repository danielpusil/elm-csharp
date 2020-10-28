using System;
using DotNetMatrix;
using Heuristicas_ELM.core;

//Daniel Pusil
namespace Heuristicas_ELM.ANN
{
    public enum ElmType {
        Regression,
        Classification
    }

    public enum Functions {
        Sign,
        Sin
    }

    public class ELM
    {
        private readonly Random _myRandom;
        public float TestingTime;
        public double TestingAccuracy;

        public float TrainingTime;
        public double TrainingAccuracy;

        private readonly ElmType _elmType;
        private readonly Functions _func;
        public readonly int NumberofHiddenNeurons;             // Defined by ANN Designer
        private int _numberofOutputNeurons;                      // The number of classes
        private GeneralMatrix _inputWeight;
        private GeneralMatrix _biasofHiddenNeurons;
        private GeneralMatrix _outputWeight;

        public ELM(ElmType elmType, int numberofHiddenNeurons, Functions activationFunction, Random myRandom)
        {
            _elmType = elmType;
            NumberofHiddenNeurons = numberofHiddenNeurons; //n hidden nodes. 
            // If the activation functiong is infenitely differentiable we can prove 
            // thet the required number of hidden nodes Ñ<=N
            _func = activationFunction;
            TrainingTime = 0;
            TrainingAccuracy = 0;
            _numberofOutputNeurons = 1; // By default for regression problems
            _myRandom = myRandom;
        }

        public ELM(ELM original)
        {
            _myRandom = original._myRandom;
            TrainingTime = original.TrainingTime;
            TrainingAccuracy = original.TrainingAccuracy;

            _elmType = original._elmType;
            _func = original._func;
            NumberofHiddenNeurons = original.NumberofHiddenNeurons;
            _numberofOutputNeurons = original._numberofOutputNeurons;
            _inputWeight = original._inputWeight.Copy();
            _biasofHiddenNeurons = original._biasofHiddenNeurons.Copy();
            _outputWeight = original._outputWeight.Copy();
        }

        public ELM Clone()
        {
            return new ELM(this);
        }

        private GeneralMatrix Tabular(GeneralMatrix t)
        {
            var numTrainData = t.ColumnDimension;
            var label = new int[_numberofOutputNeurons]; //Number of class
            for (var i = 0; i < _numberofOutputNeurons; i++)
                label[i] = i; //Class label starts from 0

            var tempT = new GeneralMatrix(_numberofOutputNeurons, numTrainData); // Initialize with zero

            /* Labels neeed be encode (See BiasOk.pdf)
             * If the classes are categorical and independent, the one target is created for each class. 
             * Targets for the correct classes are set to one, and targets for irrelevant class are set to 0
             * This enconding creates a unit length vector for each class, which is orthogonal to vectors of 
             * all other classes. 
             * Distances between target vector of different class are the same, so the class independence is kept. 
             * The predicted class is assignes acording to the target with the largest ELM output */
            for (var i = 0; i < numTrainData; i++)
            {
                int j;
                for (j = 0; j < _numberofOutputNeurons; j++)
                {
                    if (label[j] == (int)t.GetElement(0, i))
                    {//only for binary Class {0,1}
                        break;
                    }
                }
                tempT.SetElement(j, i, 1);//Set 1 to correct Label on correct position and 0 for incorrect label
            }

            var t2 = new GeneralMatrix(_numberofOutputNeurons, numTrainData);    // T=temp_T*2-1;
            for (var i = 0; i < _numberofOutputNeurons; i++)
            {
                for (var j = 0; j < numTrainData; j++)
                {
                    t2.SetElement(i, j, tempT.GetElement(i, j) * 2 - 1); //set 1 if label is correct and -1 if label is incorrect(value=0)
                }
            }
            return t2;
        }

        public GeneralMatrix CreateH(GeneralMatrix p)
        {
            var numTrainData = p.ColumnDimension;
            var tempH = _inputWeight.Multiply(p); // multiply samples of dataset by weights between layer 0 and layer 1

            var biasMatrix = new GeneralMatrix(NumberofHiddenNeurons, numTrainData);
            for (var j = 0; j < numTrainData; j++)
                for (var i = 0; i < NumberofHiddenNeurons; i++)
                    biasMatrix.SetElement(i, j, _biasofHiddenNeurons.GetElement(i, 0));
            tempH = tempH.Add(biasMatrix); // add the bias of neurons in layer 1 for all values of sample by weights

            var h = new GeneralMatrix(NumberofHiddenNeurons, numTrainData);
            //H is called the hidden layer output matrix of the neural network
            //The ith colum of H is the ith hidden node output with respect to inputs x1,x2....,xn
            //Input weight and hidden layer biases of SLFNs ca be randomly assigned if the activation
            //funtions in the hidden layer are infinitely diffentiable

            switch (_func)
            {
                case Functions.Sign:
                    for (var j = 0; j < NumberofHiddenNeurons; j++)
                    {
                        for (var i = 0; i < numTrainData; i++)
                        {
                            var temp = tempH.GetElement(j, i);//Sigmoid function
                            temp = 1.0f / (1 + Math.Exp(-temp));
                            h.SetElement(j, i, temp);
                        }
                    }
                    break;
                case Functions.Sin:
                    for (var j = 0; j < NumberofHiddenNeurons; j++)
                    {
                        for (var i = 0; i < numTrainData; i++)
                        {
                            var temp = tempH.GetElement(j, i);
                            temp = Math.Sin(temp);
                            h.SetElement(j, i, temp);
                        }
                    }
                    break;
            }
            return h;
        }

        public double Evaluate(GeneralMatrix y, GeneralMatrix yt, GeneralMatrix t, GeneralMatrix transT)
        {
            var numRows = yt.RowDimension;
            var accuracy = 0.0d;
            switch (_elmType)
            {
                case ElmType.Regression:
                    var mse = 0.0d;
                    for (var i = 0; i < numRows; i++)
                        mse += (yt.GetElement(i, 0) - transT.GetElement(i, 0)) * (yt.GetElement(i, 0) - transT.GetElement(i, 0));
                    accuracy = Math.Sqrt(mse / numRows);
                    break;
                case ElmType.Classification:
                    float missClassificationRateTraining = 0;
                    for (var i = 0; i < numRows; i++)
                    {
                        var maxtag1 = y.GetElement(0, i);
                        var tag1 = 0;
                        var maxtag2 = t.GetElement(0, i);
                        var tag2 = 0;
                        for (var j = 1; j < _numberofOutputNeurons; j++)
                        {
                            if (y.GetElement(j, i) > maxtag1)
                            {
                                maxtag1 = y.GetElement(j, i);
                                tag1 = j;
                            }
                            if (t.GetElement(j, i) > maxtag2)
                            {
                                maxtag2 = t.GetElement(j, i);
                                tag2 = j;
                            }
                        }
                        if (tag1 != tag2)
                            missClassificationRateTraining++; //Fitnnes Funttion G(c(x),x)
                    }
                    accuracy = 1 - missClassificationRateTraining * 1.0f / numRows;
                    break;
            }
            return accuracy;
        }

        public void SetWeigthsBias(double[] values, int numberofInputNeurons)
        {
            var pos = 0;
            var dataW = new double[NumberofHiddenNeurons][];
            for (var i = 0; i < NumberofHiddenNeurons; i++)
            {
                dataW[i] = new double[numberofInputNeurons];
                for (var j = 0; j < numberofInputNeurons; j++)
                {
                    dataW[i][j] = values[pos++];
                }
            }
            var dataB = new double[NumberofHiddenNeurons][];
            for (var i = 0; i < NumberofHiddenNeurons; i++)
            {
                dataB[i] = new double[1];
                dataB[i][0] = values[pos++];
            }
            _inputWeight = new GeneralMatrix(dataW);
            _biasofHiddenNeurons = new GeneralMatrix(dataB);
        }

        public void Train(Instances instances)
        {
            var trainSet = instances.Data;
            var numTrainData = trainSet.RowDimension;
            var numberofInputNeurons = trainSet.ColumnDimension - 1;
            _numberofOutputNeurons = instances.NumberofOutputNeurons;

            var transT = trainSet.GetMatrix(0, numTrainData - 1, 0, 0);//Get Class of Samples in DataSet
            var t = transT.Transpose();
            var p = trainSet.GetMatrix(0, numTrainData - 1, 1, numberofInputNeurons).Transpose();//Get Samples in DataSet

            if (_elmType == ElmType.Classification)
            {
                t = Tabular(t);
                transT = t.Transpose();
            }
        
            // Training process really start
            var startTimeTrain = DateTime.Now;
        
            if (_inputWeight == null)
                _inputWeight =  new GeneralMatrix(NumberofHiddenNeurons, numberofInputNeurons, _myRandom);//Here Is Random (Change for a Memetic)

            if (_biasofHiddenNeurons == null)
                _biasofHiddenNeurons = new GeneralMatrix(NumberofHiddenNeurons, 1, _myRandom); //Is Random

            try
            {
                var h = CreateH(p);
                var ht = h.Transpose();
                var pinvHt = h.Transpose().Inverse();
                //fast method Guang-Bin Huang, Hongming Zhou, Xiaojian Ding, and Rui Zhang, "Extreme Learning Machine for Regression and Multi-Class Classification," submitted to IEEE Transactions on Pattern Analysis and Machine Intelligence, October 2010.
                _outputWeight = pinvHt.Multiply(transT);
                var yt = ht.Multiply(_outputWeight);
                var y = yt.Transpose();
                TrainingAccuracy = Evaluate(y, yt, t, transT);
            }
            catch
            {
                TrainingAccuracy = 0.0d;
            }
            var total = new TimeSpan(DateTime.Now.Ticks - startTimeTrain.Ticks);
            TrainingTime = (float)total.TotalSeconds;
        }

        public void Test(Instances instances)
        {
            var testSet = instances.Data;
            var numTestData = testSet.RowDimension;
            var numberofInputNeurons = testSet.ColumnDimension - 1;

            var transT = testSet.GetMatrix(0, numTestData - 1, 0, 0);//Get Class of Samples in DataSet
            var t = transT.Transpose();
            var p = testSet.GetMatrix(0, numTestData - 1, 1, numberofInputNeurons).Transpose(); //Get Samples in DataSet;

            if (_elmType == ElmType.Classification)
            {
                t = Tabular(t);
                transT = t.Transpose();
            }

            // Testing process really start
            var startTimeTrain = DateTime.Now;

            try
            {
                var h = CreateH(p);
                var ht = h.Transpose();
                var yt = ht.Multiply(_outputWeight);
                var y = yt.Transpose();
                TestingAccuracy = Evaluate(y, yt, t, transT);
            }
            catch
            {
                TestingAccuracy = 0.0d;
            }

            var total = new TimeSpan(DateTime.Now.Ticks - startTimeTrain.Ticks);
            TestingTime = (float)total.TotalSeconds;
        }
    }
}