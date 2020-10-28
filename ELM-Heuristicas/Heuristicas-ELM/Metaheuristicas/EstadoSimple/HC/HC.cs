using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC
{
    /// <summary>
    /// Hill-Climbing
    /// </summary>
    public class HC: Algoritmo
    {
        public double Radio { get; set; }

        public HC()
        {
            AlgorithmName = NombreAlgoritmo.HC;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            var maxEFOs = MaximoNumeroEvaluacionesFuncionObjetivo - 1;

            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            for (var iteraciones = 0; iteraciones < maxEFOs; iteraciones++)
            {
                var r = new Solucion(s);
                r.Tweak(Radio);

                if (r.Fitness > s.Fitness)  // Es mejor  --- estamos maximizando
                    s = r;
                //Debug.WriteLine(s.Fitness);

                if (Math.Abs(1.0 - s.Fitness) <= 1e-12) break;
            }
            MejorSolucion = s;
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}