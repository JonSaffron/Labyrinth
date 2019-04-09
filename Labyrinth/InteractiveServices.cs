using Labyrinth.Services;
using Labyrinth.Services.Display;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractiveServices : IServiceSetup
        {
        public void Setup(Game game)
            {
            GlobalServices.SetGame(game);

            var soundPlayer = new SoundPlayer(new SoundLibrary(), new ActiveSoundService());
            GlobalServices.SetSoundPlayer(soundPlayer);

            var spriteLibrary = new SpriteLibrary(game);
            GlobalServices.SetSpriteLibrary(spriteLibrary);

            var randomness = new StandardRandom();
            GlobalServices.SetRandomness(randomness);
            }
        }
    }
