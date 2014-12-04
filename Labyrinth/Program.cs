using System;

namespace Labyrinth
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
            {
            try
                {
                using (var soundLibrary = new SoundLibrary())
                    {
                    using (var game = new Game1(soundLibrary))
                        {
                        game.Run();
                        }
                    }
                }
            catch (ApplicationException e)
                {
                System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }
        }
    }
