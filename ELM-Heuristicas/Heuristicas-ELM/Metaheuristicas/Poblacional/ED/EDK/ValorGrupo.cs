using System;

namespace PruebaFuncion.Metaheuristicas.Poblacional.ED.EDK
{
     public class ValorGrupo: IComparable<ValorGrupo>
     {
         public double FitnessGrupo;
         public int NumeroGrupo;

         public int CompareTo(ValorGrupo other)
         {
             return FitnessGrupo.CompareTo(other.FitnessGrupo);
         }

         public override string ToString()
         {
            // return "F:" + fitnessgrupo.ToString("##0.####") + " G:" + numgrupo + " PG:" + probgrupo.ToString("0.####");
             return "F:" + FitnessGrupo.ToString("##0.####") + " G:" + NumeroGrupo;
         }
     }
}