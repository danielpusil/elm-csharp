namespace Heuristicas_ELM.Metaheuristicas.Poblacional.PSO
{
    class Particula:Solucion
    {
        public double[] Velocidad;
        private readonly double[] _mejorSolucionEncontrada;
        private double _fitnessMejorSolucion = double.MaxValue;

        public Particula(int dimensiones, Algoritmo elContenedor): base(dimensiones, elContenedor)
        {
            Velocidad = new double[dimensiones];
            _mejorSolucionEncontrada = new double[dimensiones];
        }

        public Particula(Particula original) : base(original)
        {
            Velocidad = new double[original.Velocidad.Length];
            _mejorSolucionEncontrada = new double[original._mejorSolucionEncontrada.Length];
            original.Velocidad.CopyTo(Velocidad, 0);
            original._mejorSolucionEncontrada.CopyTo(_mejorSolucionEncontrada, 0);
            _fitnessMejorSolucion = original._fitnessMejorSolucion;
        }

        public override void InicializarAleatorio()
        {
            base.InicializarAleatorio();
            Cromosoma.CopyTo(_mejorSolucionEncontrada, 0);
            InicializarVelocidad();
        }

        public void InicializarVelocidad()
        {
            var dimensiones = MiContenedor.Dimensiones;
            Velocidad = new double[dimensiones];
            for (var j = 0; j < dimensiones; j++)
            {
                var limiteInferior = ((PSO) MiContenedor).VelocidadMinima;
                var limiteSuperior = ((PSO)MiContenedor).VelocidadMaxima;
                Velocidad[j] = limiteInferior + (limiteSuperior - limiteInferior) * MiContenedor.Aleatorio.NextDouble();
            }
        }

        public void ActualizarHistoria()
        {
            if (Fitness < _fitnessMejorSolucion)
            {
                Cromosoma.CopyTo(_mejorSolucionEncontrada, 0);
                _fitnessMejorSolucion = Fitness;
            }
        }

        public void ActualizarVelocidadyPosicion(Solucion mejorDeReferencia, double momentum, double c1, double c2)
        {
            var dimensiones = MiContenedor.Dimensiones;

            for (var i = 0; i < dimensiones; i++)
            {
                var r1 = MiContenedor.Aleatorio.NextDouble();
                var r2 = MiContenedor.Aleatorio.NextDouble();

                // New velocity of the particle:
                var newVelocity =
                    momentum * Velocidad[i] +
                    c1 * r1 * (_mejorSolucionEncontrada[i] - Cromosoma[i]) +
                    c2 * r2 * (mejorDeReferencia.Cromosoma[i] - Cromosoma[i]);

                // Limit the velocity to the maximum value:
                if (newVelocity > ((PSO)MiContenedor).VelocidadMaxima)
                    newVelocity = ((PSO)MiContenedor).VelocidadMaxima;

                if (newVelocity < ((PSO)MiContenedor).VelocidadMinima)
                    newVelocity = ((PSO)MiContenedor).VelocidadMinima;

                // Assign new velocity
                Velocidad[i] = newVelocity;
                Cromosoma[i] += newVelocity;

                // Limit the velocity to the maximum value:
                var limiteInferior = 0;
                var limiteSuperior = 1;
                if (Cromosoma[i] > limiteSuperior) Cromosoma[i] = limiteSuperior;
                if (Cromosoma[i] < limiteInferior) Cromosoma[i] = limiteInferior;
            }
        }
    }
}