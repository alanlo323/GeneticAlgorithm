using GeneticAlgorithm.Engine;
using GeneticSharp.Domain.Chromosomes;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static TicTacToe.Game;

namespace TicTacToe
{
    public class GameEngine : IGameEngine
    {
        public static int _posibleBoardFound = 0;
        private object _locker = new object();
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

            var possibleBoards = this.GetAllPossibleBoard();
            var possibleBoardsHashes_MoveFirst = possibleBoards.Select((x, index) => new
            {
                Hash = this.GetBoardHash("A", x.Value),
                GeneIndex = index
            }).ToList();
            var count = possibleBoardsHashes_MoveFirst.Count();
            var possibleBoardsHashes_MoveSecond = possibleBoards.Select((x, index) => new
            {
                Hash = this.GetBoardHash("B", x.Value),
                GeneIndex = index + count
            }).ToList();
            BoardHashToGeneDict = possibleBoardsHashes_MoveFirst.Concat(possibleBoardsHashes_MoveSecond).ToDictionary(x => x.Hash, x => x.GeneIndex);
            BoardGeneToHashDict = BoardHashToGeneDict.ToDictionary(x => x.Value, x => x.Key);
        }

        public int BoardSize { get; set; }
        public IChromosome ComputerChromosome { get; set; }
        public string ComputerPlayerName { get; private set; }
        public GeneticSharp.Domain.GeneticAlgorithm GA { get; set; }
        public string GAPlayerName { get; private set; }
        public bool MoveFirst { get; set; }
        public int SimulateRound { get; private set; }
        public int TieScore { get; private set; }
        public int WinningCondition { get; private set; }
        public int WinScore { get; private set; }
        public Dictionary<string, int> BoardHashToGeneDict { get; set; }
        public Dictionary<int, string> BoardGeneToHashDict { get; set; }

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
            Parallel.For(0, SimulateRound, (index, parallelLoopState) =>
            {

                rng = new();
                Game game = new(BoardSize, WinningCondition, player1, player2);
                while (game.States == Game.GameStates.Playing)
                {
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
            });

            //for (int i = 0; i < SimulateRound; i++)
            //{
            //    rng = new();
            //    Game game = new(BoardSize, WinningCondition, player1, player2);
            //    while (game.States == Game.GameStates.Playing)
            //    {
            //        Move move;
            //        if (game.CurrentPlayer.Name == GAPlayerName)
            //        {
            //            move = GetNextAIMove(game, chromosome);
            //        }
            //        else if (game.CurrentPlayer.Name == ComputerPlayerName)
            //        {
            //            //move = availableMoves[rng.Next(0, availableMoves.Count - 1)];
            //            //move = GetNextAIMove(game, ComputerChromosome);
            //            move = GetHardCodedNextMove(game);
            //        }
            //        else
            //        {
            //            throw new NotImplementedException($"Not support player type PlayerName:{game.CurrentPlayer.Name}");
            //        }
            //        game.PlaceMovement(move);
            //    }
            //    switch (game.States)
            //    {
            //        case Game.GameStates.Win:
            //            if (game.WinPlayer.Name == GAPlayerName)
            //            {
            //                scores += WinScore;
            //            }
            //            break;

            //        case Game.GameStates.Tie:
            //            scores += TieScore;
            //            break;
            //    }
            //}

            double fitness = (double)scores / WinScore / SimulateRound;
            if (fitness >= 0.33)
            {
                // debug
                if (true)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        rng = new();
                        Game game = new(BoardSize, WinningCondition, player1, player2);
                        while (game.States == Game.GameStates.Playing)
                        {
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
                                    //scores += WinScore;
                                }
                                break;

                            case Game.GameStates.Tie:
                                //scores += TieScore;
                                break;
                        }
                    }
                }
            }
            return fitness;
        }

        public Gene GenerateGene(int geneIndex)
        {
            return new Gene(new CustomGeneValue()
            {
                Hash = this.GetGenesHashByIndex(geneIndex, BoardGeneToHashDict),
                Weight = rng.NextDouble()
            });
            //if (geneIndex < BoardSize)
            //{
            //    GenerateGene(geneIndex + 1);
            //}
        }

        public List<string> GetAllBoard(int square_num, int[] square, List<string> results)
        {

            if (square_num == Math.Pow(BoardSize, 2))
            {
                string boardHash = string.Empty;
                square.ToList().ForEach(x => boardHash += x);
                results.Add(boardHash);
                return results;
            }

            for (int i = 0; i < BoardSize; i++)
            {
                square[square_num] = i;
                GetAllBoard(square_num + 1, square, results);
            }

            return results;
        }

        public Dictionary<string, Chess[,]> GetAllPossibleBoard()
        {
            var allBoard = GetAllBoard(0, new int[9], new());
            Player[] players = new Player[] { new Player(GAPlayerName, Game.Chess.O), new Player(ComputerPlayerName, Game.Chess.X) };
            Dictionary<string, Chess[,]> allPossibleBoard = new();
            foreach (var hash in allBoard)
            {
                var board = Game.ConvertHashToBoard(hash, BoardSize, players);
                var boardRight90Degree = RotateBaordClockwise(board);
                var boardRight180Degree = RotateBaordClockwise(boardRight90Degree);
                var boardRight270Degree = RotateBaordClockwise(boardRight180Degree);

                string[] allHashs = new string[] {
                        hash,
                        Game.ConvertBoardToHash(boardRight90Degree),
                        Game.ConvertBoardToHash(boardRight180Degree),
                        Game.ConvertBoardToHash(boardRight270Degree),
                    };

                if (!allHashs.Any(h => allPossibleBoard.ContainsKey(h)))
                {
                    if (Game.IsBoardValid(board, players))
                    {
                        allPossibleBoard.Add(hash, board);
                    }
                }
            }

            return allPossibleBoard;
        }

        public string GetBoardHash(string prefix, Chess[,] board)
        {
            string hash = string.Empty;
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    switch (board[x, y])
                    {
                        case Chess.O:
                            hash += "1";
                            break;
                        case Chess.X:
                            hash += "2";
                            break;
                        default:
                            hash += "0";
                            break;
                    }
                }
            }

            return $@"{prefix}{hash}";
        }

        public Move GetHardCodedNextMove(Game game)
        {
            var board = game.Board;
            var me = game.CurrentPlayer.Chess;
            var op = game.GetNextPlayer().Chess;

            #region Check -1

            int score;
            Location targetLocation;

            #region Vertical

            for (int x = 0; x < board.GetLength(0); x++)
            {
                score = 0;
                targetLocation = null;
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] == me)
                    {
                        score += 1;
                    }
                    if (board[x, y] == op)
                    {
                        score -= 1;
                    }
                    if (board[x, y] == Chess.Empty)
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
                    if (board[x, y] == me)
                    {
                        score += 1;
                    }
                    if (board[x, y] == op)
                    {
                        score -= 1;
                    }
                    if (board[x, y] == Chess.Empty)
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
                        if (board[x, y] == me)
                        {
                            score += 1;
                        }
                        if (board[x, y] == op)
                        {
                            score -= 1;
                        }
                        if (board[x, y] == Chess.Empty)
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
                        if (board[x, y] == me)
                        {
                            score += 1;
                        }
                        if (board[x, y] == op)
                        {
                            score -= 1;
                        }
                        if (board[x, y] == Chess.Empty)
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

            if (board[1, 1] == Chess.Empty)
            {
                return new(game.CurrentPlayer, new(1, 1));
            }

            List<Location> coners = new();
            if (board[0, 0] == Chess.Empty) coners.Add(new(0, 0));
            if (board[0, 2] == Chess.Empty) coners.Add(new(0, 2));
            if (board[2, 0] == Chess.Empty) coners.Add(new(2, 0));
            if (board[2, 2] == Chess.Empty) coners.Add(new(2, 2));

            if (coners.Count > 0)
            {
                coners.Shuffle(rng);
                return new(game.CurrentPlayer, coners[0]);
            }

            List<Location> middles = new();
            if (board[1, 0] == Chess.Empty) middles.Add(new(1, 0));
            if (board[0, 1] == Chess.Empty) middles.Add(new(0, 1));
            if (board[2, 1] == Chess.Empty) middles.Add(new(2, 1));
            if (board[1, 2] == Chess.Empty) middles.Add(new(1, 2));

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

            //  Get pobility from availableMovesHashes


            List<Move> availableMoves = game.GetAvailableMoves();
            var previewMovedBoards = game.PreviewMoves(availableMoves);
            var options = previewMovedBoards
                .Select(x => new
                {
                    Move = x.Key,
                    Hash = this.GetBoardHash(MoveFirst ? "A" : "B", x.Value),
                })
                .Select(x => new
                {
                    x.Move,
                    x.Hash,
                    GenesIndex = GetGenesIndexByHash(x.Hash, BoardHashToGeneDict),
                })
                .Select(x => new
                {
                    x.Move,
                    x.Hash,
                    x.GenesIndex,
                    Gene = chromosome.GetGene(x.GenesIndex),
                });
            Move selectedMove = options.RandomElementByWeight(x => ((CustomGeneValue)x.Gene.Value).Weight).Move;

            return selectedMove;
        }

        public string GetGenesHashByIndex(int index, Dictionary<int, string> dict)
        {
            if (!dict.ContainsKey(index))
            {
                throw new ArgumentOutOfRangeException("index", $@"{index} is not in all possible board");
            }

            return dict[index];
        }

        private int GetGenesIndexByHash(string hash, Dictionary<string, int> dict)
        {
            var realHash = hash;
            var hashPrefix = string.Empty;
            if (char.IsLetter(hash[0]))
            {
                realHash = hash.Substring(1);
                hashPrefix = hash[0].ToString();
            }
            Player[] players = new Player[] { new Player(GAPlayerName, Game.Chess.O), new Player(ComputerPlayerName, Game.Chess.X) };
            var board = Game.ConvertHashToBoard(realHash, BoardSize, players);
            var boardRight90Degree = RotateBaordClockwise(board);
            var boardRight180Degree = RotateBaordClockwise(boardRight90Degree);
            var boardRight270Degree = RotateBaordClockwise(boardRight180Degree);
            var hashRight90Degree = $@"{hashPrefix}{Game.ConvertBoardToHash(boardRight90Degree)}";
            var hashRight180Degree = $@"{hashPrefix}{Game.ConvertBoardToHash(boardRight180Degree)}";
            var hashRight270Degree = $@"{hashPrefix}{Game.ConvertBoardToHash(boardRight270Degree)}";
            if (dict.ContainsKey(hash))
            {
                return dict[hash];
            }
            else if (dict.ContainsKey(hashRight90Degree))
            {
                return dict[hashRight90Degree];
            }
            else if (dict.ContainsKey(hashRight180Degree))
            {
                return dict[hashRight180Degree];
            }
            else if (dict.ContainsKey(hashRight270Degree))
            {
                return dict[hashRight270Degree];
            }
            else
            {
                var a = dict.Where(x => x.Key.StartsWith("A")).ToList();
                var b = dict.Where(x => x.Key.StartsWith("B")).ToList();
                throw new ArgumentOutOfRangeException("hash", $@"{hash} is not in all possible board");
            }
        }

        private static Chess[,] RotateBaordClockwise(Chess[,] board)
        {
            int width;
            int height;
            Chess[,] dst;

            width = board.GetUpperBound(0) + 1;
            height = board.GetUpperBound(1) + 1;
            dst = new Chess[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow;
                    int newCol;

                    newRow = col;
                    newCol = height - (row + 1);

                    dst[newCol, newRow] = board[col, row];
                }
            }

            return dst;
        }
    }
}