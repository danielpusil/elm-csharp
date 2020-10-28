using System;
using System.Collections.Generic;
using System.IO;
using DotNetMatrix;

namespace Heuristicas_ELM.core
{
    public class Instances
    {
        public GeneralMatrix Data;
        public int NumberofInputNeurons;
        public int NumberofOutputNeurons;

        /// <summary>
        /// the first line of dataset file must be the number of rows and columns, and 
        /// number of classes if neccessary. 
        /// The first column is the norminal class value 0,1,2...n.
        /// If the class value is 1,2...,number of classes should plus 1
        /// </summary>
        /// <param name="filename">filename with data in the appropiate format</param>
        /// <returns></returns>
        public void Load(string filename)
        {
            var registros = File.ReadAllLines(filename);
            var fila1 = registros[0].Split(' ');
            var m = int.Parse(fila1[0]);
            var n = int.Parse(fila1[1]);
            var matrix = new double[m][];
            for (var fila = 1; fila <= m; fila++)
            {
                matrix[fila - 1] = new double[n];
                var filacompleta = registros[fila].Replace(".", ","); // coma como separador decimal
                var datos = filacompleta.Split(' ');
                for (var columna = 0; columna < n; columna++)
                    matrix[fila - 1][columna] = double.Parse(datos[columna]);
            }
            Data = new GeneralMatrix(matrix);
            NumberofInputNeurons = n-1;
            NumberofOutputNeurons = int.Parse(fila1[2]);
        }
    }
}
