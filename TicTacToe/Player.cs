using GeneticAlgorithm.Engine;
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

        public static bool operator !=(Player p1, Player p2)
        {
            return !(p1 == p2);
        }

        public static bool operator ==(Player p1, Player p2)
        {
            if (p1 is null && p2 is null)
            {
                return true;
            }
            if (p1 is null || p2 is null)
            {
                return false;
            }
            return p1.Name == p2.Name && p1.Chess == p2.Chess;
        }

        public override string ToString()
        {
            return $"{Chess}";
        }
    }
}