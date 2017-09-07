using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    /// <summary>
    /// struct to store the game board location of a player's piece
    /// </summary>
    public struct GameboardPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }
        
        public GameboardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
