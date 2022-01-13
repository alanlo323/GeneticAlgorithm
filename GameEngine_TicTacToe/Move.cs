namespace GameEngine_TicTacToe
{
    public class Move
    {
        public Move(Player player, Location location)
        {
            Player = player;
            Location = location;
        }

        public Location Location { get; set; }
        public Player Player { get; private set; }
    }
}