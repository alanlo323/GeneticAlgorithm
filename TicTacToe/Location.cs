using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Location
    {
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Location target)
            {
                return target.X == this.X && target.Y == this.Y;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}