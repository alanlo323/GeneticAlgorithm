using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public class Game
    {
        public Game(int boardSize, int winningCondition, Player player1, Player player2)
        {
            Players = new Player[2];
            Players[0] = player1;
            Players[1] = player2;
            CurrentPlayer = Players[0];
            WinPlayer = null;

            Board = new Player[boardSize, boardSize];
            WinningCondition = winningCondition;
            MoveHistory = new List<Move>();
            States = GameStates.Playing;
        }

        public enum Chess
        {
            Default,
            O,
            X
        }

        public enum GameStates
        {
            Playing,
            Win,
            Tie
        }

        public Player[,] Board { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public List<Move> MoveHistory { get; private set; }
        public Player[] Players { get; private set; }
        public GameStates States { get; private set; }
        public int WinningCondition { get; private set; }
        public Player WinPlayer { get; private set; }

        public void CheckWin()
        {
            Player verticalWinner = GetVerticalWinner();
            if (verticalWinner != null)
            {
                WinPlayer = verticalWinner;
                States = GameStates.Win;
            }
            Player horizontalWinner = GetHorizontalWinner();
            if (horizontalWinner != null)
            {
                WinPlayer = horizontalWinner;
                States = GameStates.Win;
            }
            Player diagonalWinner = GetDiagonalWinner();
            if (diagonalWinner != null)
            {
                WinPlayer = diagonalWinner;
                States = GameStates.Win;
            }

            if (GetAvailableMoves().Count == 0)
            {
                States = GameStates.Tie;
            }
        }

        public List<Move> GetAvailableMoves()
        {
            List<Move> availableMoves = new List<Move>();
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    Location location = new Location(x, y);
                    if (IsLocationAvailable(location))
                    {
                        availableMoves.Add(new Move(CurrentPlayer, new Location(x, y)));
                    }
                }
            }
            return availableMoves;
        }

        public Player GetDiagonalWinner()
        {
            if (Board[0, 0] == Board[1, 1] & Board[1, 1] == Board[2, 2])
            {
                return Board[1, 1];
            }
            if (Board[2, 0] == Board[1, 1] & Board[1, 1] == Board[0, 2])
            {
                return Board[1, 1];
            }

            return null;
        }

        public Player GetHorizontalWinner()
        {
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                Chess lastChess = Chess.Default;
                int count = 0;
                for (int x = 0; x < Board.GetLength(0); x++)
                {
                    Chess currentChess = Chess.Default;
                    if (Board[x, y] != null)
                    {
                        currentChess = Board[x, y].Chess;
                    }

                    if (lastChess == currentChess)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    lastChess = currentChess;

                    if (count >= WinningCondition && lastChess != Chess.Default)
                    {
                        return Board[x, y];
                    }
                }
            }

            return null;
        }

        public Player GetNextPlayer()
        {
            return CurrentPlayer == Players[0] ? Players[1] : Players[0];
        }

        public Player GetVerticalWinner()
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                Chess lastChess = Chess.Default;
                int count = 0;
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    Chess currentChess = Chess.Default;
                    if (Board[x, y] != null)
                    {
                        currentChess = Board[x, y].Chess;
                    }

                    if (lastChess == currentChess)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    lastChess = currentChess;

                    if (count >= WinningCondition && lastChess != Chess.Default)
                    {
                        return Board[x, y];
                    }
                }
            }

            return null;
        }

        public bool IsLocationAvailable(Location location)
        {
            if (location == null)
            {
                return false;
            }

            return Board[location.X, location.Y] == null;
        }

        public bool PlaceMovement(Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (States == GameStates.Playing && IsLocationAvailable(move.Location))
            {
                Board[move.Location.X, move.Location.Y] = CurrentPlayer;
                CurrentPlayer = GetNextPlayer();
                MoveHistory.Add(move);

                CheckWin();

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                for (int x = 0; x < Board.GetLength(0); x++)
                {
                    sb.Append(Board[x, y].Chess);
                }
                if (y < Board.GetLength(1) - 1)
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return base.ToString();
        }
    }
}