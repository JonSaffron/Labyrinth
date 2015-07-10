using System;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Labyrinth.Services.Input;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
            {
            var container = new WindsorContainer();
            container.Register(Component.For<Game1>());
            container.Register(Component.For<IPlayerInput>().ImplementedBy<PlayerInput>());
            container.Register(Component.For<IWorldLoader>().ImplementedBy<WorldLoader>());

            int result;
            try
                {
                using (var game = container.Resolve<Game1>())
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
