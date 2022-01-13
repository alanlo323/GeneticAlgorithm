using GAConsoleApp.GameEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GAConsoleApp
{
    internal class CustomFitness : IFitness
    {
        public CustomFitness(in IFitnessable reference)
        {
            this.Reference = reference;
        }

        public IFitnessable Reference { get; }

        public double Evaluate(IChromosome chromosome)
        {
            return Reference.Evaluate(chromosome);
        }
    }
}