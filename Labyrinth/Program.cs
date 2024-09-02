using System;
using System.Windows.Forms;

namespace Labyrinth
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        // ReSharper disable once UnusedParameter.Local
        static int Main(string[] args)
            {
            int result;
            try
                {
                var game = new LabyrinthGame();
                game.Run();
                result = 0;
                }
            catch (Exception ex)
                {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = 1;
                }
            return result;
            }
        }
    }
