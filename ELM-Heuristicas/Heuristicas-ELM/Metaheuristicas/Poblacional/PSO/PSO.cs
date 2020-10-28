using System;
using System.Collections.Generic;
using System.Diagnostics;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.Poblacional.PSO
{
    class PSO : Algoritmo
    {
        public List<Particula> Enjambre; 
        public double C1F = 2.5; // Tendencia a la propia mejor posicion
        public double C1I = 0.5; // Tendencia a la propia mejor posicion
        public double C2F = 0.5; // Tendencia a la propia mejor posicion
        public double C2I = 2.5; // Tendencia a la propia mejor posicion
        public bool UseOptimoGlobal; //Si esta en false (default) se disminuye la probabilidad de caer en optimos locales
        public double MomentoMinimo = 0.4;
        public double MomentoMaximo = 0.9;
        public double VelocidadMinima = -5;
        public double VelocidadMaxima = 5;

        public PSO()
        {
            AlgorithmName = NombreAlgoritmo.PSO;
        }

        protected void Inicializar()
        {
            Enjambre = new List<Particula>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var fila = new Particula(Dimensiones, this);
                fila.InicializarAleatorio();
                Enjambre.Add(fila);
            }
            Enjambre.Sort();
        }

        public override void Ejecutar()
        {
            Inicializar();
            MejorSolucion = new Solucion(Enjambre[0]);

            for (var generacionActual=0; generacionActual < MaximoNumeroGeneraciones; generacionActual++)
            {
                Generacion(generacionActual);

                if (Enjambre[0].Fitness > MejorSolucion.Fitness) // Maximizando
                    MejorSolucion = new Solucion(Enjambre[0]);

                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12)
                    break;

                if (generacionActual % 1000 == 0)
                    Debug.WriteLine(generacionActual + ": " + MejorSolucion.Fitness + " EFOs = " + EFOs);

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
            }
        }

        private void Generacion(int generacionActual)
        {
            foreach (var laParticula in Enjambre)
            {
                laParticula.CalcularFitness(laParticula.Cromosoma);
                laParticula.ActualizarHistoria();
            }
            Enjambre.Sort();

            if (Enjambre[0].Fitness > MejorSolucion.Fitness) // Maximizando
                MejorSolucion = new Solucion(Enjambre[0]);

            var momentum = (MomentoMaximo - MomentoMinimo);
            momentum = momentum * (MaximoNumeroGeneraciones - generacionActual);
            momentum = momentum / MaximoNumeroGeneraciones;
            momentum = momentum + MomentoMinimo;

            var relGen = generacionActual * 1.0 / MaximoNumeroGeneraciones;
            var c1 = (C1F - C1I) * relGen + C1I;
            var c2 = (C2F - C2I) * relGen + C2I;

            foreach (var laParticula in Enjambre)
            {
                if (UseOptimoGlobal)
                    laParticula.ActualizarVelocidadyPosicion(MejorSolucion, momentum, c1, c2);
                else
                    laParticula.ActualizarVelocidadyPosicion(Enjambre[0], momentum, c1, c2);
            }
        }
    }
}