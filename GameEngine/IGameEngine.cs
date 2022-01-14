using GeneticSharp.Domain.Chromosomes;

namespace GeneticAlgorithm.Engine
{
    public interface IFitnessable
    {
        double Evaluate(IChromosome chromosome);
    }

    public interface IGameEngine : IGeneable, IFitnessable
    {
        public IChromosome ComputerChromosome
        {
            get;
            set;
        }

        public GeneticSharp.Domain.GeneticAlgorithm GA { get; set; }
    }

    public interface IGeneable
    {
        Gene GenerateGene(int geneIndex);
    }
}