using GeneticAlgorithm.Engine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace TicTacToe
{
    public class CustomGeneValue
    {
        public string Hash { get; set; }

        public double Weight { get; set; }
    }
}