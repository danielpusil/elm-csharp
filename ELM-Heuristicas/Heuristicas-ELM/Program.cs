using System;
using System.Collections.Generic;
using System.Linq;
using Heuristicas_ELM.ANN;
using Heuristicas_ELM.core;
using Heuristicas_ELM.Evaluation;
using Heuristicas_ELM.Metaheuristicas;
using Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC;
using Heuristicas_ELM.Metaheuristicas.EstadoSimple.RS;
using Heuristicas_ELM.Metaheuristicas.EstadoSimple.SA;
using Heuristicas_ELM.Metaheuristicas.Poblacional.PSO;

namespace Heuristicas_ELM
{
    class Program
    {
        static void Main(/*string[] args*/)
        {
            var datadirectory =
                "D:\\Google Drive\\ccobos\\Proyectos\\2017-TG-Daniel-Pusil-MELM\\ELM-Heuristicas\\Datos\\";

            // Following lines in comments: Just to try the ELM model
            /*
            var dataTraining = "diabetes_train";
            var dataTesting = "diabetes_test";

            var insTraining = new Instances();
            insTraining.Load(datadirectory + dataTraining);
            var insTesting = new Instances();
            insTesting.Load(datadirectory + dataTesting);

            var elm1 = new ELM(ElmType.Classification, 20, Functions.Sign, new Random(10));
            var ttbf = new TrainingTestingByFile(elm1, insTraining, insTesting);
            ttbf.Execute();
            Console.WriteLine("Accuracy : " + ttbf.Accuracy + " Seconds : " + ttbf.Time);
            */

            // Following lines: Just to Try several executions of CrossValidation
            /*
            var totalEvaluations = 30;
            var data = "iris";

            var allInstances = new Instances();
            allInstances.Load(datadirectory + data);
            var ac = new List<double>();
            var time = new List<double>();
            for (var evals = 0; evals <= totalEvaluations; evals++)
            {
                var elm = new ELM(ElmType.Classification, 20, Functions.Sign, new Random(evals));
                var cv = new CrossValidation(elm, allInstances);
                cv.Execute(10);
                ac.Add(cv.Accuracy);
                time.Add(cv.Time);
                //Console.WriteLine("Accuracy using CV: " + cv.Accuracy + " Seconds : " + cv.Time);
            }
            var acpro = ac.Average();
            var timepro = time.Average();
            Console.WriteLine("Accuracy on " + data + " using CV over " + totalEvaluations +
                              " trials: " + acpro + "+/-" + 
                              (ac.Sum(d => Math.Pow(d - acpro, 2)/(ac.Count - 1))) +
                              " Seconds : " + timepro + "+/-" +
                              (time.Sum(d => Math.Pow(d - timepro, 2) / (time.Count - 1))));
            */

            // Following lines: Execute the Global-Best Harmony Search over ELM model
            /**/
            var totalEvaluations = 30;

            //var data = "iris";
            //var myInstances = new Instances();
            //myInstances.Load(datadirectory + data);
            //var elm3 = new ELM(ElmType.Classification, 20, Functions.Sign, new Random(1));
            //var eval3 = new CrossValidation(elm3, myInstances, 10);

            var dataTraining = "iris_train";
            var dataTesting = "iris_test";
            var insTraining = new Instances();
            insTraining.Load(datadirectory + dataTraining);
            var insTesting = new Instances();
            insTesting.Load(datadirectory + dataTesting);
            var elm3 = new ELM(ElmType.Classification, 20, Functions.Sign, new Random(1));
            var eval3 = new TrainingTestingByFile(elm3, insTraining, insTesting);

            var listaAlgoritmos = new List<Algoritmo>
            {
                //new HC(){Radio = 0.1,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new SAHC(){Radio = 0.1,
                //    MaximosVecinos = 20,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new SAHCWR(){Radio = 0.1,
                //    MaximosVecinos = 20,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new RS(){MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new HCRR(){Radio = 0.1, MaximosVecinos = 200, MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new SA(){Radio = 0.1, ParametroDeRecocido = 1,
                //    ElTipoRecocido = SA.TipoRecocido.Exponencial,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new CC(){Desviacion = 0.1,
                //    MaximosVecinos = 20,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 1000},

                //new EDiferencial(){   
                //    TamPoblacion = 50, 
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 500000,
                //    MaximoNumeroGeneraciones = 10000,
                //    CR = 0.9,
                //    Mutacion = 0.5},

                //new EDkmeans(){   
                //    TamPoblacion = 50, 
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 500000,
                //    MaximoNumeroGeneraciones = 10000,
                //    CR = 0.9,
                //    Mutacion = 0.5},

                new PSO(){
                    TamPoblacion = 20,
                    MaximoNumeroEvaluacionesFuncionObjetivo = 1000, //500000,
                    MaximoNumeroGeneraciones = 50, //10000,
                    C1F = 2.5,
                    C1I = 2.5, //0.5,
                    C2F = 2.5, //0.5,
                    C2I = 2.5,
                    UseOptimoGlobal = false,
                    MomentoMinimo = 0.7, // 0.4,
                    MomentoMaximo = 0.7, // 0.9,
                    VelocidadMinima = -5,
                    VelocidadMaxima = 5},

                //new FSPContinuo()
                //{
                //    TamPoblacion = 50,
                //    MaximoNumeroEvaluacionesFuncionObjetivo = 500000,
                //    MaximoNumeroGeneraciones = 200, // igual a 500.000 / 50 / 5 / 10  (= t / N / L / M)
                //    L = 5,
                //    M = 10,
                //    C = 2.0
                //}
            };

            foreach (var algoritmo in listaAlgoritmos)
            {
                algoritmo.MiEvaluador = eval3;
                algoritmo.Dimensiones = (eval3.MyInstances.NumberofInputNeurons + 1)*elm3.NumberofHiddenNeurons;
                var optimos = new List<double>();
                var tiempos = new List<double>();
                for (var evals = 0; evals <= totalEvaluations; evals++)
                {
                    var tiempo1 = DateTime.Now;
                    var aleatorio = new Random(evals);
                    algoritmo.Aleatorio = aleatorio; // Cambio el aleatorio en el algoritmo para cada test
                    algoritmo.EFOs = 0;
                    algoritmo.Ejecutar();
                    var total = (new TimeSpan(DateTime.Now.Ticks - tiempo1.Ticks)).TotalSeconds;
                    optimos.Add(algoritmo.MejorSolucion.Fitness);
                    tiempos.Add(total);
                    Console.WriteLine("Accuracy using CV: " + algoritmo.MejorSolucion.Fitness +
                                      " EFOs = " + algoritmo.EFOs + " Segundos = " + total);
                }

                var promedioTiempo = tiempos.Average();
                var desviacionTiempo = optimos.Sum(d => Math.Pow(d - promedioTiempo, 2));
                desviacionTiempo = Math.Sqrt((desviacionTiempo) / (optimos.Count() - 1));

                var mejorOptimo = optimos.Min();
                var peorOptimo = optimos.Max();
                var promedioOptimo = optimos.Average();
                var desviacionOptimos = optimos.Sum(d => Math.Pow(d - promedioOptimo, 2));
                desviacionOptimos = Math.Sqrt((desviacionOptimos)/(optimos.Count() - 1));

                var resultados = "";
                resultados += algoritmo.AlgorithmName + " ";
                resultados += algoritmo.Dimensiones + " ";
                resultados += $" {mejorOptimo,20:#0.0000000000}" + " ";
                resultados += $" {peorOptimo,20:#0.0000000000}" + " ";
                resultados += $" {promedioOptimo,20:#0.0000000000}" + " ";
                resultados += $" {desviacionOptimos,20:#0.0000000000}" + " ";
                resultados += $" {promedioTiempo,20:#0.0000000000}" + " ";
                resultados += $" {desviacionTiempo,20:#0.0000000000}";
                Console.WriteLine(resultados);
            }
            /**/

            Console.WriteLine("");
            Console.WriteLine("Press <RETURN>");
            Console.ReadKey();
        }
    }
}