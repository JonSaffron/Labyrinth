using Labyrinth.Services;
using Labyrinth.Services.ScoreKeeper;
using Labyrinth.Services.Sound;

namespace Labyrinth
    {
    internal class InteractiveServices
        {
        public void Setup(LabyrinthGame game)
            {
            GlobalServices.SetGame(game);

            var soundPlayer = new SoundPlayer(new SoundLibrary(), new ActiveSoundService());
            GlobalServices.SetSoundPlayer(soundPlayer);

            GlobalServices.SetSpriteLibrary(game);

            var randomness = new StandardRandom();
            GlobalServices.SetRandomness(randomness);

            GlobalServices.SetScoreKeeper(new ScoreKeeper());
            }
        }
    }
