using System;
using Heuristicas_ELM.Metaheuristicas;

namespace Heuristicas_ELM.Utilidades
{
    class Calculos
    {
        /// <summary>
        /// Calcula la distancia euclidiana entre dos puntos con n-dimensiones
        /// </summary>
        /// <param name="p1">Arreglo con la posicion n-dimensional del Primer Punto</param>
        /// <param name="dim"></param>
        /// <param name="p2">Arreglo con la posicion n-dimensional Segundo Punto</param>
        /// <param name="raiz">TRUE si se debe sacar la raiz cuadrada a la distancia</param>
        /// <returns></returns>
        public static double DistanciaEuclidiana(Solucion p1, Solucion p2, int dim, bool raiz)
        {
            double total = 0;
            for (var i = 0; i < dim ; i++)
            {
                total += Math.Pow(Math.Abs(p1.Cromosoma[i] - p2.Cromosoma[i]), 2);
            }
            if (raiz) total = Math.Sqrt(total);
            return total;
        }

        public static double AleatorioNormal(Random aleatorio, double media, double desviacion)
        {
            double x, y, w;
            do
            {
                x = aleatorio.NextDouble();
                y = aleatorio.NextDouble();
                w = x * x + y * y;
            } while (w <= 1);
            if (aleatorio.NextDouble() < 0.5)
                return media + x * desviacion * Math.Sqrt(-2 * Math.Log(w, Math.E) / w);
            return media + y * desviacion * Math.Sqrt(-2 * Math.Log(w, Math.E) / w);
        }

        public static double Temple(double fitnessS, double fitnessR, double t)
        {
            // Se esta minimizando, por eso es al reves Quality(S) - Quality(R)
            var valor = -(fitnessR - fitnessS) / t;
            var valorE = Math.Exp(valor);
            return valorE <= 1 ? valorE : 1;
        }
   }
}