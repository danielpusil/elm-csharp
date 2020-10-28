namespace Heuristicas_ELM.Metaheuristicas.Poblacional.ED.EDK
{
    public class SolucionEDK:Solucion
    {
        public int Grupo;

        public SolucionEDK(int dimensiones, Algoritmo elContenedor): base(dimensiones, elContenedor){}

        public SolucionEDK(SolucionEDK original): base (original)
        {
            Grupo = original.Grupo;
        }

        public override string ToString()
        {
            return base.ToString() + " -" + Grupo;
        }
    }
}