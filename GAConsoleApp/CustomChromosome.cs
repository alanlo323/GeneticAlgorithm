using GeneticAlgorithm.Engine;
using GeneticSharp.Domain.Chromosomes;

namespace GAConsoleApp
{
    internal class CustomChromosome : ChromosomeBase
    {
        public CustomChromosome(in IGeneable reference)
            : base(GenesCount)
        {
            this.Reference = reference;
            CreateGenes();
        }

        public static int GenesCount { get; } = 9;

        public IGeneable Reference { get; }

        public override IChromosome CreateNew()
        {
            var reference = Reference;
            return new CustomChromosome(in reference);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return Reference.GenerateGene(geneIndex);
        }
    }
}