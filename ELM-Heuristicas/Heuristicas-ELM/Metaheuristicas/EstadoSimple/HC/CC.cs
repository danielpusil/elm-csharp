using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.HC
{
    /// <summary>
    /// Hill-Climbing
    /// </summary>
    public class CC: Algoritmo
    {
        public double Desviacion { get; set; }
        public int MaximosVecinos { get; set; }

        public CC()
        {
            AlgorithmName = NombreAlgoritmo.CC;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            var maximoIteracionesEsperadas = MaximoNumeroEvaluacionesFuncionObjetivo / MaximosVecinos;

            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            MejorSolucion = new Solucion(s);
            var iteracion = 0;
            while (EFOs < MaximoNumeroEvaluacionesFuncionObjetivo)
            {
                // Hill Climbing
                var vecinosPorIteracion = maximoIteracionesEsperadas * iteracion - (maximoIteracionesEsperadas - MaximosVecinos * 2);
                vecinosPorIteracion = vecinosPorIteracion / (2 * MaximosVecinos);

                for (var vecinos = 0; vecinos < vecinosPorIteracion; vecinos++)
                {
                    var r = new Solucion(s);
                    r.Tweak(0, Desviacion);
                    if (r.Fitness > s.Fitness) // Es mejor  --- estamos maximizando
                        s = r;
                    if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                    if (Math.Abs(1.0 - r.Fitness) <= 1e-12) break;
                }

                if (s.Fitness > MejorSolucion.Fitness)  // Es mejor  --- estamos maximizando
                    MejorSolucion = new Solucion(s);

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12) break;

                // Reinicio Aleatorio
                s.InicializarAleatorio();
                iteracion++;
            }
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}