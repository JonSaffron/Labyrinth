using System;

namespace Labyrinth
    {
    public class AddScoreEventArgs : EventArgs
        {
        public AddScoreEventArgs(int score)
            {
            this.Score = score;
            }
        
        public int Score { get; private set; }        
        }
    }