using GeneticSharp.Domain.Chromosomes;

namespace GeneticAlgorithm.Engine
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