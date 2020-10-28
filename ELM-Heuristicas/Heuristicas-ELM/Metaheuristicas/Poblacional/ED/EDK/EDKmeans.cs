using System;
using System.Collections.Generic;
using System.Diagnostics;
using Heuristicas_ELM.Utilidades;
using PruebaFuncion.Metaheuristicas;

namespace Heuristicas_ELM.Metaheuristicas.Poblacional.ED.EDK
{
    public class EDkmeans : Algoritmo
    {
        public double CR { get; set; }
        public double Mutacion { get; set; }
        private int _valorK = 10;
        public List<SolucionEDK> Poblacion; 

        /// <summary>
        /// Constructor pode defecto: No olvide fijar la funcion y el aleatorio
        /// </summary>
        public EDkmeans()
        {
            AlgorithmName = NombreAlgoritmo.EDKmeans;
        }

        protected void Inicializar()
        {
            Poblacion = new List<SolucionEDK>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var fila = new SolucionEDK(Dimensiones, this);
                fila.InicializarAleatorio();
                Poblacion.Add(fila);
            }
            Poblacion.Sort();
        }

        public override void Ejecutar()
        {
            Inicializar();

            MejorSolucion = new SolucionEDK(Poblacion[0]);

            var g = 0;
            while (true)
            {
                if (Aleatorio.NextDouble() < 0.95)
                    Generacion();
                else
                    GeneracionConKmeans();

                if (Poblacion[0].Fitness > MejorSolucion.Fitness) // Maximizando
                    MejorSolucion = new SolucionEDK(Poblacion[0]);

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
            var nuevapobl = new List<SolucionEDK>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                var vectorSeleccionados = SeleccionarPadre(i);  // Seleccionar vector solucion para toda la poblacion
                var vectorPerturbado = PerturbacionPadre(vectorSeleccionados);
                var vectorCruzado = CruzarPadre(i, vectorPerturbado);

                if (vectorCruzado.Fitness > Poblacion[i].Fitness) //Maximizando
                    nuevapobl.Add(vectorCruzado);
                else
                {
                    var copia = new SolucionEDK(Poblacion[i]);
                    nuevapobl.Add(copia);
                }
            }
            Poblacion = nuevapobl;
            Poblacion.Sort();
        }

        public void GeneracionConKmeans()
        {
            EjecutarKmeans();

            var nuevapobl = new List<SolucionEDK>();

            for (var i = 0; i < TamPoblacion; i++)
            {
                var vectorSeleccionados = SeleccionarPadreConKmeans(i);  // Seleccionar vector solucion para toda la poblacion
                var vectorPerturbado = PerturbacionPadre(vectorSeleccionados);
                var vectorCruzado = CruzarPadre(i, vectorPerturbado);

                if (vectorCruzado.Fitness < Poblacion[i].Fitness)
                {
                    if (nuevapobl.Exists(x => x.Equals(vectorCruzado)))  // Evitar soluciones repetidas en poblacion
                    {
                        var fila = new SolucionEDK(Dimensiones, this);
                        fila.InicializarAleatorio();
                        nuevapobl.Add(fila);
                    }
                    else
                        nuevapobl.Add(vectorCruzado);
                }
                else
                {
                    if (nuevapobl.Exists(x => x.Equals(Poblacion[i]))) // Evitar soluciones repetidas en poblacion
                    {
                        var fila = new SolucionEDK(Dimensiones, this);
                        fila.InicializarAleatorio();
                        nuevapobl.Add(fila);
                    }
                    else
                    {
                        var copia = new SolucionEDK(Poblacion[i]);
                        nuevapobl.Add(copia);
                    }
                }
            }
            Poblacion = nuevapobl;
            Poblacion.Sort();
        }

        protected int[] SeleccionarPadreConKmeans(int fila)
        {
            var vector = new int[3];
            int gr1, gr2, gr3;
            //seleccionar grupo para el v1
            do
            {
                gr1 = Aleatorio.Next(_valorK);
                vector[0] = elegir_registro(gr1);
            } while (vector[0] == fila);

            //seleccionar grupo para el v2 diferente del grupo de V1
            do
            {
                gr2 = Aleatorio.Next(_valorK);
                vector[1] = elegir_registro(gr2);
            } while ((gr2 == gr1) || (vector[1] == fila));

            //seleccionar grupo para el v3 diferente de los dos anteriores
            do
            {
                gr3 = Aleatorio.Next(_valorK);
                vector[2] = elegir_registro(gr3);

            } while ((gr3 == gr2) || (gr3 == gr1) || (vector[2] == fila));

            return vector;   
        }
        
        int elegir_registro(int grupo)
        {
            var listaRegistros = new List<KeyValuePair<int, double>>();
            for (var i = 0; i < TamPoblacion; i++)
            {
                if (Poblacion[i].Grupo == grupo)
                {
                    var nuevo = new KeyValuePair<int, double>(i, Poblacion[i].Fitness);
                    listaRegistros.Add(nuevo);
                }
            }

            listaRegistros.Sort((x,y)=> -1*x.Value.CompareTo(y.Value));

            return TirarRuletaRank(listaRegistros);
        }

        public int TirarRuletaRank(List<KeyValuePair<int, double>> listaSoluciones)
        {
            const double vNegativo = 0.25;
            const double vPositivo = 1.75;
            const double vRango = vPositivo - vNegativo;
            var n = listaSoluciones.Count;

            var aleatorio = Aleatorio.NextDouble() * n;
            double sum = 0;
            for (var i = 0; i < listaSoluciones.Count; i++)
            {
                sum += vNegativo + vRango * (i * 1.0 / (n - 1));

                if (sum >= aleatorio)
                {
                    return listaSoluciones[i].Key;
                }
            }
            return listaSoluciones[0].Key; // cuando sólo hay un registro en el grupo
        }

        public void EjecutarKmeans()
        {
            const int maximasIteraciones = 20;
            _valorK = 10;
            var losGrupos = new List<Grupo>();            

            // 1.Inicialización de los Centroides iniciales al azar
            var registrosSeleccionados = new List<int>();
            for (var i = 0; i < _valorK; i++)
            {
                var aleatorio = Aleatorio.Next(TamPoblacion);                  // Escoge un registro al azar

                while (registrosSeleccionados.Contains(aleatorio))   // Verifica que el registro no se haya seleccionado
                {
                    aleatorio++;
                    if (aleatorio >= TamPoblacion) aleatorio = 0;
                }
                registrosSeleccionados.Add(aleatorio);               // Adiciona el registro a la lista de Registros Seleccionados

                var nuevoGrupo = new Grupo { Centro = new SolucionEDK(Poblacion[aleatorio]) };
                losGrupos.Add(nuevoGrupo);                            // Adiciono el centroide a la lista de centroides
            }

            for (var i = 0; i < TamPoblacion; i++) Poblacion[i].Grupo = -1;

            int cambios;
            var iteraciones = 0;
            
            do
            {
                foreach (var elGrupo in losGrupos) elGrupo.Limpiar();

                // 2. Calculo membresias de los registros/puntos y contabilizo los registros en grupos
                cambios = 0;
                for (var i = 0; i < TamPoblacion; i++)
                {
                    var distancia = Calculos.DistanciaEuclidiana(Poblacion[i], losGrupos[0].Centro, Dimensiones, true);
                    var migrupo = 0;
                    for (var cActual = 1; cActual < _valorK; cActual++)
                    {
                        var distanciatemporal = Calculos.DistanciaEuclidiana(Poblacion[i], losGrupos[cActual].Centro, Dimensiones, true);
                        if (distanciatemporal < distancia)
                        {
                            migrupo = cActual;
                            distancia = distanciatemporal;
                        }
                    }
                    if (Poblacion[i].Grupo != migrupo) cambios++; //como se determina el tamaño del grupo. 

                    Poblacion[i].Grupo = migrupo;
                    losGrupos[migrupo].Soluciones.Add(i);
                }

                if (cambios != 0)   // Solo se hace si hay cambios, sino se ahorra el tiempo de este proceso
                {
                    // 3. Recalculo centroides de cada grupo
                    var noViables = new List<int>();
                    for (var idGrupo =0; idGrupo < losGrupos.Count; idGrupo++)
                    {
                        if (losGrupos[idGrupo].Viable)
                            losGrupos[idGrupo].RecalcularCentroides(Poblacion);
                        else
                            noViables.Add(idGrupo);
                    }
                    noViables.Sort((x, y) => -1 * x.CompareTo(y));
                    if (noViables.Count != 0)
                    {
                        foreach (var idGrupo in noViables)
                        {
                            losGrupos.RemoveAt(idGrupo);
                            _valorK--;
                        }
                    }

                    for (var idGrupo = 0; idGrupo < losGrupos.Count; idGrupo++)
                    {
                        losGrupos[idGrupo].ActualizarPoblacion(Poblacion, idGrupo);
                    }
                }
                
                iteraciones++;
            } while (cambios != 0 && iteraciones < maximasIteraciones); // Se detiene si no hay cambios o si se han hecho muchas iteraciones           
        }

        protected int[] SeleccionarPadre(int fila)
        {
            var vector = new int[3];

            do
            {
                vector[0] = Aleatorio.Next(TamPoblacion);
            } while (vector[0] == fila);

            do
            {
                vector[1] = Aleatorio.Next(TamPoblacion);
            } while (vector[0] == vector[1] || vector[1] == fila);

            do
            {
                vector[2] = Aleatorio.Next(TamPoblacion);
            } while (vector[1] == vector[2] || vector[0] == vector[2] || vector[2] == fila);

            return vector;
        }

        // pertubacion del vector solucion v1
        protected SolucionEDK PerturbacionPadre(int[] vSeleccionados)
        {
            var wi = new SolucionEDK(Dimensiones, this);
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
        protected SolucionEDK CruzarPadre(int i, SolucionEDK vPerturbado)
        {
            var rndb = Aleatorio.Next(Dimensiones);
            var vCruzado = new SolucionEDK(Dimensiones, this);
            for (var d = 0; d < Dimensiones; d++)
            {
                if ((Aleatorio.NextDouble() < CR) || (d == rndb))
                    vCruzado.Cromosoma[d] = vPerturbado.Cromosoma[d];
                else
                    vCruzado.Cromosoma[d] = Poblacion[i].Cromosoma[d];
            }
            vCruzado.CalcularFitness(vCruzado.Cromosoma);
            return vCruzado;
        }

    }
}