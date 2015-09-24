using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Labyrinth.Services.Input;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public class ComponentRegistration : IWindsorInstaller
        {
        public void Install(IWindsorContainer container, IConfigurationStore store)
            {
            container.Register(Component.For<Game1>());
            container.Register(Component.For<IPlayerInput>().ImplementedBy<PlayerInput>());
            container.Register(Component.For<IWorldLoader>().ImplementedBy<WorldLoader>());
            }
        }
    }
