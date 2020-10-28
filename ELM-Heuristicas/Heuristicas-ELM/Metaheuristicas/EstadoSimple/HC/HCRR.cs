using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC
{
    /// <summary>
    /// Hill-Climbing with Random Restarts
    /// </summary>
    public class HCRR: Algoritmo
    {
        public double Radio { get; set; }
        public int MaximosVecinos { get; set; }

        public HCRR()
        {
            AlgorithmName = NombreAlgoritmo.HCRR;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            
            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            MejorSolucion = new Solucion(s);
            while (EFOs < MaximoNumeroEvaluacionesFuncionObjetivo)
            {
                // Hill Climbing
                for (var vecinos = 0; vecinos < MaximosVecinos; vecinos++)
                {
                    var r = new Solucion(s);
                    r.Tweak(Radio);
                    if (r.Fitness > s.Fitness) // Es mejor  --- estamos maximizando
                        s = r;
                    if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                    if (Math.Abs(1.0 - r.Fitness) <= 1e-12) break;

                }

                if (s.Fitness > MejorSolucion.Fitness)  // Es mejor  --- estamos maximizando
                    MejorSolucion = new Solucion(s);

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12)break;

                // Reinicio Aleatorio
                s.InicializarAleatorio();
            }
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}