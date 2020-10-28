using System;
using System.Collections.Generic;
using System.Linq;
using Heuristicas_ELM.Evaluation;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.Poblacional.FSP
{
    public class FSPContinuo: Algoritmo
    {
        // MaximoNumeroGeneraciones es T
        // TamPoblacion es N
        public int L { get; set; }  // numero de veces que se arroja la red de pesca en un punto de captura
        public int M { get; set; }  // cantidad de vectores de posicion de la red
        public double C { get; set; }  // coeficiente de amplitud
        public Solucion Gbest { get; set; } // mejor solucion global

        public List<Pescador> PuntosDeCaptura;

        public FSPContinuo() {
            AlgorithmName = NombreAlgoritmo.FSPContinuo;
        }

        public FSPContinuo(CrossValidation laCV, Random miAletarorio)
        {
            MiEvaluador = laCV;
            Aleatorio = miAletarorio;
        }

        public override string ToString()
        {
            return PuntosDeCaptura.Aggregate("", (current, sol) => current + sol.ToString());
        }

        protected void Inicializar()
        {
            PuntosDeCaptura = new List<Pescador>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var fila = new Pescador(Dimensiones, this);
                fila.InicializarAleatorio(); // Inicializa xi, evalua fitness y actualiza MejorSolucion (gbest)
                PuntosDeCaptura.Add(fila);   // Ingresa el punto pi de captura a la lista (población)
            }
        }

        public override void Ejecutar()
        {
            // Definir el valor de C como una Heuristica aceptable 
            // Al principio debe ser suficientemene grande para explorar el espacio = 2 % rango de busqueda al princio
            // Al final debe ser suficientemete pequeño para explotar el vecinadrio = 0.000001 al final
            var rangoBusqueda = 1;
            C = 0.02*rangoBusqueda;
            var factorDecrecimiento = Math.Pow(0.000001 / C, 1.0/ MaximoNumeroGeneraciones);

            MejorSolucion = new Pescador(Dimensiones, this);
            
            Inicializar();

            var c = C;
            for (var generacionActual = 0; generacionActual < MaximoNumeroGeneraciones; generacionActual++)
            {
                for (var i = 0; i < TamPoblacion; i++)
                {
                    // Cada pescador en su punto de captura tira la red L veces
                    var auxL = L;
                    while ((auxL > 0))
                    {
                        PuntosDeCaptura[i].TirarRed(c); // Tira la red, actualiza pi y gbest si es el caso
                        auxL--;
                        if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                    }

                    if (PuntosDeCaptura[i].Fitness > MejorSolucion.Fitness) // Se esta Maximizando
                        MejorSolucion = new Pescador(PuntosDeCaptura[i]);

                    if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                }

                c = c * factorDecrecimiento;

                if (PuntosDeCaptura[0].Fitness > MejorSolucion.Fitness)  // Maximizando
                    MejorSolucion = new Pescador(PuntosDeCaptura[0]);

                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12)
                    break;

                //if (generacionActual % 20 == 0)
                //    Debug.WriteLine(generacionActual + ": " + MejorSolucion.Fitness + " EFOs = " + EFOs);

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
            }
        }
    }
}