using static TicTacToe.Game;

namespace TicTacToe
{
    public class Player
    {
        public Player(string name, Chess chess)
        {
            Name = name;
            Chess = chess;
        }

        public Chess Chess { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Chess}";
        }
    }
}