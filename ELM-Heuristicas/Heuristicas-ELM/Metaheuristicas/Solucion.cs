using System;
using Heuristicas_ELM.Utilidades;

namespace Heuristicas_ELM.Metaheuristicas
{
    public class Solucion : IComparable<Solucion>, IEquatable<Solucion>
    {
        public double Fitness = double.MaxValue;
        public double[] Cromosoma;
        protected Algoritmo MiContenedor;

        public Solucion(int dimensiones, Algoritmo elContenedor)
        {
            Cromosoma = new double[dimensiones];
            MiContenedor = elContenedor;
        }

        public Solucion(Solucion original)
        {
            Cromosoma = new double[original.Cromosoma.Length];
            original.Cromosoma.CopyTo(Cromosoma, 0);
            Fitness = original.Fitness;
            MiContenedor = original.MiContenedor;
        }

        public virtual void InicializarAleatorio()
        {
            var dimensiones = MiContenedor.Dimensiones;
            var aleatorio = MiContenedor.Aleatorio;
            Cromosoma = new double[dimensiones];
            for (var j = 0; j < dimensiones; j++)
            {
                var limiteInferior = 0;
                var limiteSuperior = 1;
                Cromosoma[j] = limiteInferior + (limiteSuperior - limiteInferior) * aleatorio.NextDouble();
            }
            CalcularFitness(Cromosoma);
        }

        public void Tweak(double radio)
        {
            var dimensiones = MiContenedor.Dimensiones;
            var aleatorio = MiContenedor.Aleatorio;
            for (var j = 0; j < dimensiones; j++)
            {
                var limiteInferior = 0;
                var limiteSuperior = 1;
                var ajuste = -radio + aleatorio.NextDouble() * 2 * radio;
                Cromosoma[j] = Cromosoma[j] + ajuste;
                if (Cromosoma[j] < limiteInferior) Cromosoma[j] = limiteInferior;
                if (Cromosoma[j] > limiteSuperior) Cromosoma[j] = limiteSuperior;
            }
            CalcularFitness(Cromosoma);
        }

        public void Tweak(double media, double desviacion)
        {
            var dimensiones = MiContenedor.Dimensiones;
            var aleatorio = MiContenedor.Aleatorio;
            for (var j = 0; j < dimensiones; j++)
            {
                var limiteInferior = 0;
                var limiteSuperior = 1;
                var ajuste = Calculos.AleatorioNormal(aleatorio, media, desviacion);
                Cromosoma[j] = Cromosoma[j] + ajuste;
                if (Cromosoma[j] < limiteInferior) Cromosoma[j] = limiteInferior;
                if (Cromosoma[j] > limiteSuperior) Cromosoma[j] = limiteSuperior;
            }
            CalcularFitness(Cromosoma);
        }

        public void CalcularFitness(double[] vectorSolucion)
        {
            MiContenedor.MiEvaluador.MyELM.SetWeigthsBias(vectorSolucion, 
                MiContenedor.MiEvaluador.MyInstances.NumberofInputNeurons);
            MiContenedor.MiEvaluador.Execute();
            Fitness = MiContenedor.MiEvaluador.Accuracy;
            MiContenedor.EFOs++;
        }

        public int CompareTo(Solucion other)
        {
            return -1 * Fitness.CompareTo(other.Fitness); //Maximizando
        }

        public override string ToString()
        {
            var cadena = Fitness + ": ";
            for (var i = 0; i < Cromosoma.GetUpperBound(0) + 1; i++)
                cadena += Cromosoma[i] + " ";
            return cadena;
        }

        public bool Equals(Solucion other)
        {
            return Math.Abs(Fitness-other.Fitness) < 1e-12;
        }
    }
}