using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Labyrinth.Services;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ComponentRegistration : IWindsorInstaller
        {
        public void Install(IWindsorContainer container, IConfigurationStore store)
            {
            container.Register(Component.For<Game1, ICentrePointProvider>().ImplementedBy<Game1>());
            container.Register(Component.For<GameInput>().ImplementedBy<GameInput>());
            container.Register(Component.For<IPlayerInput>().ImplementedBy<PlayerInput>());
            container.Register(Component.For<IWorldLoader>().ImplementedBy<WorldLoader>());
            container.Register(Component.For<ISoundPlayer>().ImplementedBy<SoundPlayer>());
            container.Register(Component.For<SoundLibrary>());
            container.Register(Component.For<IActiveSoundService>().ImplementedBy<ActiveSoundService>());
            container.Register(Component.For<ISpriteLibrary>().ImplementedBy<SpriteLibrary>());
            container.Register(Component.For<IMonsterMovementFactory>().ImplementedBy<MonsterMovementFactory>());
            container.Register(Component.For<IRandomness>().ImplementedBy<StandardRandom>());
            }
        }
    }
