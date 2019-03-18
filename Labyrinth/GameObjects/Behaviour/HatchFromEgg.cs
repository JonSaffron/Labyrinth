using System;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;

namespace Labyrinth.GameObjects.Behaviour
    {
    class HatchFromEgg: BaseBehaviour, IMovementBehaviour
        {
        private static readonly Animation EggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
        private static readonly Animation HatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);
        private readonly GameTimer _hatchingTimer;
        
        public HatchFromEgg(Monster monster, int gameTicks): base(monster)
            {
            var timeSpan = TimeSpan.FromSeconds(gameTicks * Constants.GameClockResolution);
            this._hatchingTimer = GameTimer.AddGameTimer(timeSpan, EggIsHatching, false);
            }

        public override void Perform()
            {
            if (!this.Monster.IsEgg)
                {
                this.RemoveMe();
                return;
                }

            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster);
            this._hatchingTimer.Enabled = inSameRoom;
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            if (this.Monster.IsExtant)
                {
                this.Monster.PlaySoundWithCallback(GameSound.EggHatches, EggHatches);
                }
            }

        private void EggHatches(object sender, EventArgs args)
            {
            // ????
            }
        }
    }
