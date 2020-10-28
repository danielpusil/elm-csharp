namespace Heuristicas_ELM.Metaheuristicas.Poblacional.FSP
{
    public class Pescador:Solucion
    {
        public Pescador(int dimensiones, Algoritmo elContenedor): base(dimensiones, elContenedor){}

        public Pescador(Pescador original): base(original){}

        public override void InicializarAleatorio() {
            base.InicializarAleatorio();
            if (Fitness < MiContenedor.MejorSolucion.Fitness) // Se esta MINIMIZANDO
                MiContenedor.MejorSolucion = new Pescador(this);
        }

        public void TirarRed(double c)
        {
            var dimension = MiContenedor.Dimensiones;
            var m = ((FSPContinuo)MiContenedor).M;

            for (var j = 0; j < m; j++)
            {
                var aj = new double[dimension];
                for (var i = 0; i < dimension; i++)
                    aj[i] = -c + 2 * MiContenedor.Aleatorio.NextDouble() * c;

                var y = new double[dimension];

                var limiteInferior = 0;
                var limiteSuperior = 1;

                for (var i = 0; i < dimension; i++)
                {
                    y[i] = Cromosoma[i] + aj[i];

                    if (y[i] < limiteInferior) y[i] = limiteInferior;
                    if (y[i] > limiteSuperior) y[i] = limiteSuperior;
                }

                //TODO: el fitness debe ser arreglado
                //var fitnessEsteNodo = MiContenedor.MiCVdeELM.ValueFunction(y);
                var fitnessEsteNodo = 0;
                MiContenedor.EFOs++;

                if (fitnessEsteNodo < Fitness) // Se esta MINIMIZANDO
                {
                    y.CopyTo(Cromosoma, 0);
                    Fitness = fitnessEsteNodo;
                }

                if (MiContenedor.EFOs >= MiContenedor.MaximoNumeroEvaluacionesFuncionObjetivo) break;
            }
        }
     }
}