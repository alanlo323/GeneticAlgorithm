using GeneticAlgorithm.Engine;
using GeneticSharp.Domain.Chromosomes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class GameEngine : IGameEngine
    {
        private Random rng;

        public GameEngine(bool moveFirst, int boardSize = 3, int winningCondition = 3, int simulateRound = 100, int winScore = 15, int tieScore = 5)
        {
            BoardSize = boardSize;
            WinningCondition = winningCondition;
            MoveFirst = moveFirst;
            SimulateRound = simulateRound;
            WinScore = winScore;
            TieScore = tieScore;

            rng = new Random();
            ComputerPlayerName = "Computer";
            GAPlayerName = "GeneticAlgorithm";
        }

        public int BoardSize { get; set; }
        public string ComputerPlayerName { get; private set; }
        public string GAPlayerName { get; private set; }
        public bool MoveFirst { get; private set; }
        public int SimulateRound { get; private set; }
        public int TieScore { get; private set; }
        public int WinningCondition { get; private set; }
        public int WinScore { get; private set; }

        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            Dictionary<int, Location> genesDict = new Dictionary<int, Location>();
            for (int i = 0; i < genes.Length; i++)
            {
                int X = i / 3;
                int Y = i % 3;
                genesDict.Add(i, new Location(X, Y));
            }

            int scores = 0;
            Player player1 = MoveFirst ? new Player(GAPlayerName, Game.Chess.O) : new Player(ComputerPlayerName, Game.Chess.O);
            Player player2 = MoveFirst ? new Player(ComputerPlayerName, Game.Chess.X) : new Player(GAPlayerName, Game.Chess.X);
            for (int i = 0; i < SimulateRound; i++)
            {
                rng = new Random();
                Game game = new Game(BoardSize, WinningCondition, player1, player2);
                List<Move> availableMoves;
                while (game.States == Game.GameStates.Playing)
                {
                    Move move = null;
                    availableMoves = game.GetAvailableMoves();

                    if (game.CurrentPlayer.Name == GAPlayerName)
                    {
                        var availableMoveDict = new Dictionary<int, Move>();
                        foreach (var item in genesDict)
                        {
                            var availableMove = availableMoves.Where(x => x.Location.Equals(item.Value)).FirstOrDefault();
                            if (availableMove != null)
                            {
                                availableMoveDict.Add(item.Key, availableMove);
                            }
                        }
                        move = availableMoveDict.RandomElementByWeight((x => (double)genes[x.Key].Value)).Value;
                    }
                    else if (game.CurrentPlayer.Name == ComputerPlayerName)
                    {
                        move = availableMoves[rng.Next(0, availableMoves.Count - 1)];
                    }
                    else
                    {
                        throw new NotImplementedException($"Not support player type PlayerName:{game.CurrentPlayer.Name}");
                    }
                    game.PlaceMovement(move);
                }
                switch (game.States)
                {
                    case Game.GameStates.Win:
                        if (game.WinPlayer.Name == GAPlayerName)
                        {
                            scores += WinScore;
                        }
                        break;

                    case Game.GameStates.Tie:
                        scores += TieScore;
                        break;
                }
            }

            double fitness = (double)scores / WinScore / SimulateRound;
            return fitness;
        }

        public Gene GenerateGene(int geneIndex)
        {
            return new Gene(rng.NextDouble());
            //if (geneIndex < BoardSize)
            //{
            //    GenerateGene(geneIndex + 1);
            //}
        }
    }
}