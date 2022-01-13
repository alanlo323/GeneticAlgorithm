using static GameEngine_TicTacToe.Game;

namespace GameEngine_TicTacToe
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
    }
}