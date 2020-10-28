using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC
{
    /// <summary>
    /// Steepest Ascent Hill-Climbing With Replacement
    /// </summary>
    public class SAHCWR: Algoritmo
    {
        public double Radio { get; set; }
        public int MaximosVecinos { get; set; }

        public SAHCWR()
        {
            AlgorithmName = NombreAlgoritmo.SAHCWR;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            MejorSolucion = new Solucion(s);

            while (EFOs < MaximoNumeroEvaluacionesFuncionObjetivo)
            {
                var r = new Solucion(s);
                r.Tweak(Radio);

                for (var vecinos = 0; vecinos < MaximosVecinos; vecinos++)
                {
                    var w = new Solucion(s);
                    w.Tweak(Radio);
                    if (w.Fitness > r.Fitness) // Es mejor  --- estamos maximizando
                        r = w;
                    if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                    if (Math.Abs(1.0 - r.Fitness) <= 1e-12) break;
                }
                s = r;

                if (s.Fitness > MejorSolucion.Fitness)  // Es mejor  --- estamos maximizando
                    MejorSolucion = new Solucion(s);
                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12) break;
            }
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}