using System;
using Heuristicas_ELM.Evaluation;

namespace Heuristicas_ELM.Metaheuristicas
{
    public abstract class Algoritmo
    {
        public NombreAlgoritmo AlgorithmName = NombreAlgoritmo.Base;
        public Solucion MejorSolucion;

        public Random Aleatorio { get; set; }
        public Evaluation.Evaluation MiEvaluador { get; set; }
        public int TamPoblacion { get; set; }
        public int Dimensiones { get; set; }
        public int MaximoNumeroEvaluacionesFuncionObjetivo { get; set; }
        public int MaximoNumeroGeneraciones { get; set; }
        public int EFOs { get; set; }

        protected Algoritmo() {
        }

        protected Algoritmo(CrossValidation elCV, Random miAletarorio)
        {
            MiEvaluador = elCV;
            Aleatorio = miAletarorio;
        }

        public abstract void Ejecutar();
    }
}