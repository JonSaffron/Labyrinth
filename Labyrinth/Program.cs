using System;
using System.Windows.Forms;
using Castle.Windsor;
using Castle.Windsor.Installer;

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
                using (var container = new WindsorContainer().Install(FromAssembly.This()))
                    {
                    GlobalServices.SetMonsterMovementFactory(container.Resolve<IMonsterMovementFactory>());
                    GlobalServices.SetRandomness(container.Resolve<IRandomess>());

                    var game = container.Resolve<Game1>();
                    game.Run();
                    }
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
