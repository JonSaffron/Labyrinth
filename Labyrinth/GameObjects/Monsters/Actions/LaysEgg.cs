using System;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.GameObjects.Monsters.Actions
    {
    class LaysEgg : BaseAction
        {
        public LaysEgg(Monster monster) : base(monster)
            {
            if (!(monster is ILayEggs))
                throw new InvalidOperationException(string.Format("Cannot set a {0} as laying eggs because the ILayEggs interface is not implemented.", this.GetType().Name));
            }

        public override void PerformAction()
            {
            if (!ShouldAttemptToLayEgg())
                return;

            TilePos tp = this.Monster.TilePosition;
            if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                {
                this.PlaySound(GameSound.MonsterLaysEgg);
                MonsterDef md = ((ILayEggs)this.Monster).LayAnEgg();
                md.IsEgg = true;
                md.TimeBeforeHatching = (this.Random.Next(256) & 0x1f) + 8;
                md.LaysEggs = false;
                GlobalServices.GameState.CreateMonster(md);
                }
            }

        private bool ShouldAttemptToLayEgg()
            {
            var result =
                    !this.Monster.IsEgg
                && this.Player.IsAlive()
                && this.IsInSameRoom()
                && this.Random.Test(0x1f);
            return result;
            }
        }
    }
