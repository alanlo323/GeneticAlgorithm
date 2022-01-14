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

            rng = new();
            ComputerPlayerName = "Computer";
            GAPlayerName = "GeneticAlgorithm";
        }

        public int BoardSize { get; set; }
        public IChromosome ComputerChromosome { get; set; }
        public string ComputerPlayerName { get; private set; }
        public GeneticSharp.Domain.GeneticAlgorithm GA { get; set; }
        public string GAPlayerName { get; private set; }
        public bool MoveFirst { get; private set; }
        public int SimulateRound { get; private set; }
        public int TieScore { get; private set; }
        public int WinningCondition { get; private set; }
        public int WinScore { get; private set; }

        public static Dictionary<Location, Gene> MapGenesToLocation(Gene[] genes)
        {
            if (genes == null)
            {
                throw new ArgumentNullException(nameof(genes));
            }

            Dictionary<Location, Gene> genesDict = new();
            for (int i = 0; i < genes.Length; i++)
            {
                int X = i / 3;
                int Y = i % 3;
                genesDict.Add(new(X, Y), genes[i]);
            }

            return genesDict;
        }

        public double Evaluate(IChromosome chromosome)
        {
            int scores = 0;
            Player player1 = MoveFirst ? new(GAPlayerName, Game.Chess.O) : new(ComputerPlayerName, Game.Chess.O);
            Player player2 = MoveFirst ? new(ComputerPlayerName, Game.Chess.X) : new(GAPlayerName, Game.Chess.X);
            for (int i = 0; i < SimulateRound; i++)
            {
                rng = new();
                Game game = new(BoardSize, WinningCondition, player1, player2);
                List<Move> availableMoves;
                while (game.States == Game.GameStates.Playing)
                {
                    availableMoves = game.GetAvailableMoves();

                    Move move;
                    if (game.CurrentPlayer.Name == GAPlayerName)
                    {
                        move = GetNextAIMove(game, chromosome);
                    }
                    else if (game.CurrentPlayer.Name == ComputerPlayerName)
                    {
                        //move = availableMoves[rng.Next(0, availableMoves.Count - 1)];
                        //move = GetNextAIMove(game, ComputerChromosome);
                        move = GetHardCodedNextMove(game);
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

        public Move GetHardCodedNextMove(Game game)
        {
            var board = game.Board;
            var me = game.CurrentPlayer.Chess;
            var op = game.GetNextPlayer().Chess;

            #region Check -1

            int score = 0;
            Location targetLocation = null;

            #region Vertical

            for (int x = 0; x < board.GetLength(0); x++)
            {
                score = 0;
                targetLocation = null;
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y]?.Chess == me)
                    {
                        score += 1;
                    }
                    if (board[x, y]?.Chess == op)
                    {
                        score -= 1;
                    }
                    if (board[x, y] == null)
                    {
                        targetLocation = new(x, y);
                    }
                }

                if (score == 2 || score == -2)
                {
                    return new(game.CurrentPlayer, targetLocation);
                }
            }

            #endregion Vertical

            #region Horizontal

            for (int y = 0; y < board.GetLength(1); y++)
            {
                score = 0;
                targetLocation = null;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    if (board[x, y]?.Chess == me)
                    {
                        score += 1;
                    }
                    if (board[x, y]?.Chess == op)
                    {
                        score -= 1;
                    }
                    if (board[x, y] == null)
                    {
                        targetLocation = new(x, y);
                    }
                }

                if (score == 2 || score == -2)
                {
                    return new(game.CurrentPlayer, targetLocation);
                }
            }

            #endregion Horizontal

            #region Diagonal1

            score = 0;
            targetLocation = null;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (x == y)
                    {
                        if (board[x, y]?.Chess == me)
                        {
                            score += 1;
                        }
                        if (board[x, y]?.Chess == op)
                        {
                            score -= 1;
                        }
                        if (board[x, y] == null)
                        {
                            targetLocation = new(x, y);
                        }
                    }
                }
            }

            if (score == 2 || score == -2)
            {
                return new(game.CurrentPlayer, targetLocation);
            }

            #endregion Diagonal1

            #region Diagonal2

            score = 0;
            targetLocation = null;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (x == 2 - y)
                    {
                        if (board[x, y]?.Chess == me)
                        {
                            score += 1;
                        }
                        if (board[x, y]?.Chess == op)
                        {
                            score -= 1;
                        }
                        if (board[x, y] == null)
                        {
                            targetLocation = new(x, y);
                        }
                    }
                }
            }

            if (score == 2 || score == -2)
            {
                return new(game.CurrentPlayer, targetLocation);
            }

            #endregion Diagonal2

            #endregion Check -1

            if (board[1, 1] == null)
            {
                return new(game.CurrentPlayer, new(1, 1));
            }

            List<Location> coners = new();
            if (board[0, 0] == null) coners.Add(new(0, 0));
            if (board[0, 2] == null) coners.Add(new(0, 2));
            if (board[2, 0] == null) coners.Add(new(2, 0));
            if (board[2, 2] == null) coners.Add(new(2, 2));

            if (coners.Count > 0)
            {
                coners.Shuffle(rng);
                return new(game.CurrentPlayer, coners[0]);
            }

            List<Location> middles = new();
            if (board[1, 0] == null) middles.Add(new(1, 0));
            if (board[0, 1] == null) middles.Add(new(0, 1));
            if (board[2, 1] == null) middles.Add(new(2, 1));
            if (board[1, 2] == null) middles.Add(new(1, 2));

            if (middles.Count > 0)
            {
                coners.Shuffle(rng);
                return new(game.CurrentPlayer, middles[0]);
            }

            throw new NotSupportedException("No result found!");
        }

        public Move GetNextAIMove(Game game, IChromosome chromosome)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }
            if (chromosome == null)
            {
                throw new ArgumentNullException(nameof(chromosome));
            }

            List<Move> availableMoves = game.GetAvailableMoves();
            Dictionary<Location, Tuple<Move, Gene>> locationMap = new();
            Dictionary<Location, Gene> genesMap = MapGenesToLocation(chromosome.GetGenes());
            foreach (var move in availableMoves)
            {
                locationMap.Add(move.Location, new(move, genesMap.Where(gene => move.Location.Equals(gene.Key)).First().Value));
            }
            Move selectedMove = locationMap.RandomElementByWeight(x => (double)x.Value.Item2.Value).Value.Item1;

            return selectedMove;
        }
    }
}