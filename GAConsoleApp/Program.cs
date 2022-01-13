using GAConsoleApp.Test.GameEngine;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using System;

namespace GAConsoleApp
{
    internal class Program
    {
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

        /// <summary>
        /// GeneticSharp Console Application template.
        /// <see href="https://github.com/giacomelli/GeneticSharp"/>
        /// </summary>
        private static void Main(string[] args)
        {
            var game = new MostClosedNumber();

            var selection = new StochasticUniversalSamplingSelection();
            var crossover = new UniformCrossover(0.8f);
            var mutation = new UniformMutation(true);

            var fitness = new CustomFitness(game);
            var chromosome = new CustomChromosome(game);

            var population = new Population(50, 70, chromosome);
            population.GenerationStrategy = new PerformanceGenerationStrategy();

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Reinsertion = new ElitistReinsertion();
            ga.Termination = new OrTermination(new TerminationBase[]
            {
                new FitnessThresholdTermination(1f),
                new TimeEvolvingTermination(TimeSpan.FromSeconds(3000)),
                //new FitnessStagnationTermination(2500)
            });
            ga.MutationProbability = 0.0001f;
            var lastMsgTick = DateTime.Now;
            ga.GenerationRan += (s, e) =>
            {
                if (DateTime.Now >= lastMsgTick.AddMilliseconds(100))
                {
                    lastMsgTick = DateTime.Now;
                    Console.CursorLeft = 0;
                    string msg = $"Elapsed time: {ga.TimeEvolving.ToString(@"hh\:mm\:ss")} Generation: {ga.GenerationsNumber} Best fitness: {string.Format("{0:0.0000}", ga.BestChromosome.Fitness)}";
                    drawTextProgressBar(msg, (int)(ga.BestChromosome.Fitness * 100), 100);
                }
            };
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 16,
                MaxThreads = 16
            };

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine();
            Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
            Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
            Console.ReadKey();
        }
    }
}