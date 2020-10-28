using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC
{
    /// <summary>
    /// Steepest Ascent Hill-Climbing
    /// </summary>
    public class SAHC: Algoritmo
    {
        public double Radio { get; set; }
        public int MaximosVecinos { get; set; }

        public SAHC()
        {
            AlgorithmName = NombreAlgoritmo.SAHC;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();

            //for (var iteraciones = 0; iteraciones < maxEFOs; iteraciones++)
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

                if (r.Fitness > s.Fitness)  // Es mejor  --- estamos maximizando
                    s = r;
                if (Math.Abs(1.0 - s.Fitness) <= 1e-12)break;
            }
            MejorSolucion = s;
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}