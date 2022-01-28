using GeneticSharp.Domain.Chromosomes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public class Game
    {
        public static int[][] win = new int[][]
        {
            new int[] {0, 1, 2},
            new int[] {3, 4, 5},
            new int[] {6, 7, 8},
            new int[] {0, 3, 6},
            new int[] {1, 4, 7},
            new int[] {2, 5, 8},
            new int[] {0, 4, 8},
            new int[] {2, 4, 6}
        };

        public Game(int boardSize, int winningCondition, Player player1, Player player2)
        {
            Players = new Player[2];
            Players[0] = player1;
            Players[1] = player2;
            CurrentPlayer = Players[0];
            WinPlayer = null;
            WinningCondition = winningCondition;
            Board = new Chess[boardSize, boardSize];
            MoveHistory = new List<Move>();
            States = GameStates.Playing;
        }

        public enum Chess
        {
            Empty,
            O,
            X
        }

        public enum GameStates
        {
            Playing,
            Win,
            Tie
        }

        [JsonProperty("Board", ItemConverterType = typeof(StringEnumConverter))]
        public Chess[,] Board { get; private set; }

        public Player CurrentPlayer { get; private set; }

        public List<Move> MoveHistory { get; private set; }

        public Player[] Players { get; private set; }

        public GameStates States { get; private set; }

        public int WinningCondition { get; private set; }

        public Player WinPlayer { get; private set; }

        public static string ConvertBoardToHash(Chess[,] board)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    sb.Append((int)board[x, y]);
                }
            }
            return sb.ToString();
        }

        public static Chess[,] ConvertHashToBoard(string hash, int boardSize, Player[] players)
        {
            Chess[,] board = new Chess[boardSize, boardSize];
            for (int i = 0; i < hash.Length; i++)
            {
                int x = i % 3;
                int y = i / 3;

                int playerIndex = int.Parse(hash[i].ToString()) - 1;
                if (playerIndex >= 0)
                {
                    board[x, y] = players[playerIndex].Chess;
                }
            }

            return board;
        }

        public static bool IsBoardValid(Chess[,] board, Player[] players)
        {
            int[] chessCount = new int[players.Length];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (board[x, y] == players[i].Chess)
                        {
                            chessCount[i]++;
                        }
                    }
                }
            }

            for (int i = 0; i < chessCount.Length; i++)
            {
                if (Math.Abs(chessCount[i] - chessCount[0]) > 1)
                {
                    return false;
                }
            }

            int playerWinCount = 0;
            foreach (var player in players)
            {
                if (IsPlayerWin(board, player))
                {
                    playerWinCount++;
                }
            }

            if (playerWinCount > 1)
            {
                return false;
            }

            return true;
        }

        public static bool IsPlayerWin(Chess[,] board, Player p)
        {
            for (int i = 0; i < win.Length; i++)
            {
                int winLength = win[i].Length;
                int winCount = 0;
                for (int j = 0; j < winLength; j++)
                {
                    int x = win[i][j] % 3;
                    int y = win[i][j] / 3;
                    if (board[x, y] == p.Chess)
                    {
                        winCount++;
                    }
                }

                if (winCount == winLength)
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckWin()
        {
            foreach (var player in Players)
            {
                if (IsPlayerWin(Board, player))
                {
                    WinPlayer = player;
                    States = GameStates.Win;
                }
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

        public Player GetNextPlayer()
        {
            return CurrentPlayer == Players[0] ? Players[1] : Players[0];
        }

        public bool IsLocationAvailable(Location location)
        {
            if (location == null)
            {
                return false;
            }

            return Board[location.X, location.Y] == 0;
        }

        public bool PlaceMovement(Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (States == GameStates.Playing && IsLocationAvailable(move.Location))
            {
                Board[move.Location.X, move.Location.Y] = CurrentPlayer.Chess;
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
                    sb.Append(Board[x, y]);
                }
                if (y < Board.GetLength(1) - 1)
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }
    }
}