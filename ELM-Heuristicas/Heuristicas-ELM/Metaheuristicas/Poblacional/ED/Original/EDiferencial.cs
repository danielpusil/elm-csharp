using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Heuristicas_ELM.Evaluation;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.Poblacional.ED.Original
{
    public class EDiferencial: Algoritmo
    {
        public double CR { get; set; }
        public double Mutacion { get; set; }
        public List<Solucion> Poblacion; 

        /// <summary>
        /// Constructor pode defecto: No olvide fijar la funcion y el aleatorio
        /// </summary>
        public EDiferencial() {
            AlgorithmName = NombreAlgoritmo.EDiferencial;
        }

        public EDiferencial(CrossValidation elCV, Random miAletarorio)
        {
            MiEvaluador = elCV;
            Aleatorio = miAletarorio;
        }

        public override string ToString()
        {
            return Poblacion.Aggregate("", (current, sol) => current + sol.ToString());
        }

        protected void Inicializar()
        {
            Poblacion = new List<Solucion>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var fila = new Solucion(Dimensiones, this);
                fila.InicializarAleatorio();
                Poblacion.Add(fila);
            }
            Poblacion.Sort();
        }

        public override void Ejecutar()
        {
            Inicializar();
            MejorSolucion = new Solucion(Poblacion[0]);

            var g = 0;
            while (true)
            {
                Generacion();
                if (Poblacion[0].Fitness > MejorSolucion.Fitness) // Maximizando
                    MejorSolucion = new Solucion(Poblacion[0]);

                if (Math.Abs(1.0 - MejorSolucion.Fitness) <= 1e-12)
                    break;

                if (g % 1000 == 0)
                    Debug.WriteLine(g + ": " + MejorSolucion.Fitness + " EFOs = " + EFOs);

                if (EFOs >= MaximoNumeroEvaluacionesFuncionObjetivo) break;
                g++;
            }
        }
        
        protected virtual void Generacion()
        {
            var nuevapobl = new List<Solucion>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var vectorSeleccionados = SeleccionarPadre(i);  // Seleccionar vector solucion para toda la poblacion
                var vectorPerturbado = PerturbacionPadre(vectorSeleccionados);
                var vectorCruzado = CruzarPadre(i, vectorPerturbado);

                if (vectorCruzado.Fitness > Poblacion[i].Fitness) // Maximizando
                    nuevapobl.Add(vectorCruzado);
                else
                {
                    var copia = new Solucion(Poblacion[i]);
                    nuevapobl.Add(copia);
                }
            }
            Poblacion = nuevapobl;
            Poblacion.Sort();
        }

        protected int[] SeleccionarPadre(int fila)
        {
            var vector = new int[3];

            do {
                vector[0] = Aleatorio.Next(TamPoblacion);
            } while (vector[0] == fila);
                
            do {
                vector[1] = Aleatorio.Next(TamPoblacion);
            } while (vector[0] == vector[1] || vector[1]==fila);

            do {
                vector[2] = Aleatorio.Next(TamPoblacion);
            } while (vector[1] == vector[2] || vector[0] == vector[2]|| vector[2]==fila);

            return vector;
        } 

        // pertubacion del vector solucion v1
        protected Solucion PerturbacionPadre(int[] vSeleccionados)
        {
            var wi = new Solucion(Dimensiones, this);
            for (var i = 0; i < Dimensiones; i++)
            {
               wi.Cromosoma[i] = Poblacion[vSeleccionados[0]].Cromosoma[i] + Mutacion * (Poblacion[vSeleccionados[1]].Cromosoma[i] - Poblacion[vSeleccionados[2]].Cromosoma[i]);

               var limiteInferior = 0;
               var limiteSuperior = 1;
               if (wi.Cromosoma[i] < limiteInferior) wi.Cromosoma[i] = limiteInferior;
               if (wi.Cromosoma[i] > limiteSuperior) wi.Cromosoma[i] = limiteSuperior;
            }
            return wi;
        }

        ////cruce con vector padre por selección de cada miembro según tasa de cruce
        protected Solucion CruzarPadre(int i, Solucion vPerturbado)
        {
            var rndb = Aleatorio.Next(Dimensiones);
            var vCruzado = new Solucion(Dimensiones, this);
            for (var d = 0; d < Dimensiones; d++)
            {
                if ((Aleatorio.NextDouble() < CR)|| (d==rndb))
                    vCruzado.Cromosoma[d] = vPerturbado.Cromosoma[d];
                else
                    vCruzado.Cromosoma[d] = Poblacion[i].Cromosoma[d];
            }
            vCruzado.CalcularFitness(vCruzado.Cromosoma);
            return vCruzado;
        }
    }
}