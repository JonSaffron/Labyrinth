using Labyrinth.Services.Input;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class UnitTestServices : IServiceSetup
        {
        public PlayerController PlayerController;

        public void Setup(Game game)
            {
            GlobalServices.SetGame(game);

            this.PlayerController = new PlayerController(game);
            game.Components.Add(this.PlayerController);
            GlobalServices.SetPlayerInput(this.PlayerController);

            var soundPlayer = new NullSoundPlayer();
            GlobalServices.SetSoundPlayer(soundPlayer);

            var spriteLibrary = new DummySpriteLibrary();
            GlobalServices.SetSpriteLibrary(spriteLibrary);
            }
        }
    }
