using System.Collections.Generic;

namespace Heuristicas_ELM.Metaheuristicas.Poblacional.ED.EDK
{
    class Grupo
    {
        public bool Viable {
            get { return Soluciones.Count != 0; }
        }

        public SolucionEDK Centro;

        public List<int> Soluciones = new List<int>();

        public int TotalSoluciones {
            get { return Soluciones.Count; }
        }

        public void Limpiar()
        {
            Soluciones.Clear();
        }

        public void RecalcularCentroides(List<SolucionEDK> poblacion)
        {
            var dimensiones = Centro.Cromosoma.GetUpperBound(0) + 1;
                for (var columnas = 0; columnas < dimensiones; columnas++)
                    Centro.Cromosoma[columnas] = 0; // Inicializo los centroides en cero

            foreach (var laSolucion in Soluciones)
            {
                for (var columnas = 0; columnas < dimensiones; columnas++)
                    Centro.Cromosoma[columnas] += poblacion[laSolucion].Cromosoma[columnas];  // Acumulo valores
            }

            for (var columnas = 0; columnas < dimensiones; columnas++)
                Centro.Cromosoma[columnas] = Centro.Cromosoma[columnas]/Soluciones.Count;
        }

        public void ActualizarPoblacion(List<SolucionEDK> poblacion, int idGrupo)
        {
            foreach (var laSolucion in Soluciones)
                poblacion[laSolucion].Grupo = idGrupo;
        }
    }
}
