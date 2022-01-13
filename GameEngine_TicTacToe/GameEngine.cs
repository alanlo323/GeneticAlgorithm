using GAConsoleApp.GameEngine;
using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine_TicTacToe
{
    public class GameEngine : IGameEngine
    {
        private bool MoveFirst;
        private Random rng;
        private int WinningCondition;

        public GameEngine(bool moveFirst, int boardSize = 3, int winningCondition = 3)
        {
            BoardSize = boardSize;
            WinningCondition = winningCondition;
            MoveFirst = moveFirst;

            rng = new Random();
        }

        public int BoardSize { get; set; }

        public double Evaluate(IChromosome chromosome)
        {
            int scores = 0;
            Player player1 = MoveFirst ? new Player("GA", Game.Chess.O) : new Player("Computer", Game.Chess.X);
            Player player2 = MoveFirst ? new Player("Computer", Game.Chess.O) : new Player("GA", Game.Chess.X);
            for (int i = 0; i < 5; i++)
            {
                Game game = new Game(BoardSize, WinningCondition, player1, player2);
                List<Move> availableMoves = game.GetAvailableMoves();
                while (true)
                {
                    //TODO
                }
                Move selectedMove = availableMoves[rng.Next(0, availableMoves.Count - 1)];
            }
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