using GeneticAlgorithm.Engine;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using System;
using System.IO;
using Newtonsoft.Json;
using GeneticSharp.Domain.Chromosomes;
using JsonNet.ContractResolvers;
using System.Threading;

namespace GAConsoleApp
{
    public class Program
    {
        /// <summary>
        /// GeneticSharp Console Application template.
        /// <see href="https://github.com/giacomelli/GeneticSharp"/>
        /// </summary>
        public static void Main(string[] args)
        {
            string TicTacToeMoveFirstChromosomeFileName = "TicTacToeMoveFirstChromosomeFileName.json";

            IGameEngine gameEngine = new TicTacToe.GameEngine(true, simulateRound: 100);

            TicTacToe.GameEngine tttGameEngine = (TicTacToe.GameEngine)gameEngine;
            Console.WriteLine("Generating all possible board...");
            var possibleBoards = tttGameEngine.GetAllPossibleBoard();

            IChromosome baseChromosome;
            if (File.Exists(TicTacToeMoveFirstChromosomeFileName))
            {
                Console.Write("Loading model from file...");
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver()
                };
                baseChromosome = JsonConvert.DeserializeObject<CustomChromosome>(File.ReadAllText(TicTacToeMoveFirstChromosomeFileName), settings);
                if (baseChromosome is CustomChromosome customChromosome)
                {
                    customChromosome.Reference = gameEngine;
                }
                Console.CursorLeft = 0;
                Console.WriteLine($"Loaded model from file (Fitness:{baseChromosome.Fitness})");

                gameEngine.ComputerChromosome = baseChromosome.Clone();
            }
            else
            {
                baseChromosome = new CustomChromosome(gameEngine);

                gameEngine.ComputerChromosome = baseChromosome.Clone();
                Gene[] emptyGenes = new Gene[gameEngine.ComputerChromosome.Length];

                for (int i = 0; i < emptyGenes.Length; i++)
                {
                    emptyGenes[i] = new Gene((double)0);
                }
                gameEngine.ComputerChromosome.ReplaceGenes(0, emptyGenes);
            }

            Console.WriteLine("GA running...");
            GeneticSharp.Domain.GeneticAlgorithm ga = null;
            IChromosome chromosome = baseChromosome; ;
            for (int i = 1; i <= 6; i++)
            {
                Console.WriteLine($"Round {i}");
                if (ga?.BestChromosome != null)
                {
                    gameEngine.ComputerChromosome = ga.BestChromosome;
                    chromosome = ga.BestChromosome;
                }

                ga = Train(gameEngine, chromosome);

                Console.WriteLine();
                Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
                Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
            }

            File.WriteAllText(TicTacToeMoveFirstChromosomeFileName, JsonConvert.SerializeObject(ga.BestChromosome));
            Console.WriteLine($"Saved best solution to {TicTacToeMoveFirstChromosomeFileName}");
            Console.ReadKey();
        }

        private static void drawTextProgressBar(string msg, int progress, int total)
        {
            string leftMsg = $"[";
            //draw empty progress bar
            Console.Write(leftMsg); //start
            float onechunk = 30.0f / total;

            //draw filled part
            int position = leftMsg.Length;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 30 + leftMsg.Length; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write($"] {msg}"); //blanks at the end remove any excess
        }

        private static GeneticSharp.Domain.GeneticAlgorithm Train(IGameEngine gameEngine, IChromosome chromosome)
        {
            var lastMsgTime = DateTime.MinValue;

            var selection = new StochasticUniversalSamplingSelection();
            var crossover = new UniformCrossover(0.8f);
            var mutation = new UniformMutation(true);
            var fitness = new CustomFitness(gameEngine);
            var population = new TplPopulation(50, 70, chromosome);
            population.GenerationStrategy = new PerformanceGenerationStrategy();

            var ga = new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.1f;
            ga.Reinsertion = new ElitistReinsertion();
            ga.Termination = new OrTermination(new ITermination[]
            {
                new AndTermination(new  ITermination[]
                {
                    new FitnessThresholdTermination(1f),
                new FitnessStagnationTermination(100),
                }),
                new TimeEvolvingTermination(TimeSpan.FromSeconds(20)),
            });
            ga.TaskExecutor = new TplTaskExecutor
            {
                MinThreads = 1,
                MaxThreads = 100
            };
            ga.OperatorsStrategy = new TplOperatorsStrategy();
            ga.GenerationRan += (s, e) =>
            {
                if (DateTime.Now >= lastMsgTime.AddMilliseconds(100))
                {
                    lastMsgTime = DateTime.Now;
                    Console.CursorLeft = 0;
                    string msg = $"Elapsed time: {ga.TimeEvolving.ToString(@"hh\:mm\:ss")} Generation: {ga.GenerationsNumber} Best fitness: {string.Format("{0:0.0000}", ga.BestChromosome.Fitness)}";
                    drawTextProgressBar(msg, (int)(ga.BestChromosome.Fitness * 100), 100);
                }
            };

            gameEngine.GA = ga;
            ga.Start();

            return ga;
        }
    }
}