using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class GameState
    {
        public string Username { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public List<Card> Cards { get; set; }
    }
}