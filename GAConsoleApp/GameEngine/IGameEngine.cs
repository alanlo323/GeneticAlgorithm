using GeneticSharp.Domain.Chromosomes;

namespace GAConsoleApp.GameEngine
{
    public interface IFitnessable
    {
        double Evaluate(IChromosome chromosome);
    }

    public interface IGameEngine : IGeneable, IFitnessable
    {
    }

    public interface IGeneable
    {
        Gene GenerateGene(int geneIndex);
    }
}