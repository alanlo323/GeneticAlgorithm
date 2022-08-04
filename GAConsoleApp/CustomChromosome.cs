using System.Linq;
using GeneticAlgorithm.Engine;
using GeneticSharp.Domain.Chromosomes;
using Newtonsoft.Json;

namespace GAConsoleApp
{
    public class CustomChromosome : ChromosomeBase
    {
        public CustomChromosome(Gene[] genes, in IGeneable reference) : this(reference)
        {
            ReplaceGenes(0, genes);
        }

        public CustomChromosome() : base(GenesCount)
        {
        }

        public CustomChromosome(in IGeneable reference)
            : base(GenesCount)
        {
            this.Reference = reference;
            CreateGenes();
        }

        public CustomChromosome(in IGeneable reference, int genesCount)
            : base(genesCount)
        {
            GenesCount = genesCount;
            this.Reference = reference;
            CreateGenes();
        }

        public Gene[] Genes { get => GetGenes(); set => ReplaceGenes(0, value); }

        public static int GenesCount { get; set; } = 4416;

        [JsonIgnore]
        public IGeneable Reference { get; set; }

        public override IChromosome CreateNew()
        {
            var reference = Reference;
            Gene[] genes = GetGenes();
            if (genes == null || genes.Length == 0 || genes.All(x => x.Value == null))
            {
                return new CustomChromosome(in reference, GenesCount);
            }
            return new CustomChromosome(Genes, in reference);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return Reference.GenerateGene(geneIndex);
        }
    }
}