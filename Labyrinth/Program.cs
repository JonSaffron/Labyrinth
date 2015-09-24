using System;
using System.Windows.Forms;

namespace Labyrinth
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
            {
            int result;
            try
                {
                IoC.Create();

                using (var game = Game1.Create())
                    {
                    game.Run();
                    }
                result = 0;
                }
            catch (ApplicationException e)
                {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = 1;
                }
            return result;
            }
        }
    }
