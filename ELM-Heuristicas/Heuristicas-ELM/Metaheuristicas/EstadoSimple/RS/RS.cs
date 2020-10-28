using System;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.RS
{
    /// <summary>
    /// Random Search
    /// </summary>
    public class RS: Algoritmo
    {
        public RS()
        {
            AlgorithmName = NombreAlgoritmo.RS;
        }

        public override void Ejecutar()
        {
            EFOs = 0;
            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            MejorSolucion = new Solucion(s);

            while (EFOs < MaximoNumeroEvaluacionesFuncionObjetivo)
            {
                s.InicializarAleatorio();
                if (s.Fitness > MejorSolucion.Fitness)  // Es mejor  --- estamos maximizando
                    MejorSolucion = new Solucion(s);
                //Debug.WriteLine(MejorSolucion.Fitness);
                if (Math.Abs(1.0 - s.Fitness) <= 1e-12) break;
            }
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}