using System;
using Heuristicas_ELM.Utilidades;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.EstadoSimple.SA
{
    /// <summary>
    /// Simulated Annealing
    /// </summary>
    public class SA: Algoritmo
    {
        public enum TipoRecocido
        {
            Exponencial,
            Lineal
        }
        public double Radio { get; set; }
        public int ParametroDeRecocido { get; set; }
        public TipoRecocido ElTipoRecocido{ get; set; }

        public SA()
        {
            AlgorithmName = NombreAlgoritmo.SA;
        }

        public override void Ejecutar()
        {
            const int temperaturaInicial = 100;
            EFOs = 0;

            var s = new Solucion(Dimensiones, this);
            s.InicializarAleatorio();
            MejorSolucion = new Solucion(s);
            double temperatura = temperaturaInicial;

            while (temperatura > 0)
            {
                var maxVecinos = MaximoNumeroEvaluacionesFuncionObjetivo / temperaturaInicial;
                for (var vecinos = 0; vecinos < maxVecinos; vecinos++)
                {
                    var r = new Solucion(s);
                    r.Tweak(Radio);

                    // Es mejor  --- estamos maximizando o temple simulado le da oportunidad
                    if (r.Fitness > s.Fitness)
                        s = r;
                    else if (Aleatorio.NextDouble() < Calculos.Temple(s.Fitness, r.Fitness, temperatura))
                        s = r;

                    //Debug.WriteLine(temperatura + " " + s.Fitness);
                    if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                    if (Math.Abs(s.Fitness) <= 1e-12) break;
                }

                if (s.Fitness > MejorSolucion.Fitness) // Es mejor --- estamos maximizando
                    MejorSolucion = new Solucion(s);

                switch (ElTipoRecocido)
                {
                    case TipoRecocido.Exponencial:
                        temperatura = temperatura*Math.Pow(0.95, ParametroDeRecocido);
                        break;
                    case TipoRecocido.Lineal:
                        temperatura--;
                        break;
                }

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                if (Math.Abs(MejorSolucion.Fitness) <= 1e-12) break;
            }
        }

        public override string ToString()
        {
            return AlgorithmName.ToString();
        }
    }
}