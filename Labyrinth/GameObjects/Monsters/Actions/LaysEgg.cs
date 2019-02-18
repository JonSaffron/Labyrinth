using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.GameObjects.Actions
    {
    class LaysEgg : BaseBehaviour, IMovementBehaviour
        {
        public override void Perform()
            {
            if (!ShouldAttemptToLayEgg())
                return;

            TilePos tp = this.Monster.TilePosition;
            if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                {
                this.PlaySound(GameSound.MonsterLaysEgg);
                MonsterDef md = MonsterDef.FromExistingMonster(this.Monster);
                md.IsEgg = true;
                md.TimeBeforeHatching = (this.Random.Next(256) & 0x1f) + 8;
                md.LaysEggs = false;
                GlobalServices.GameState.AddMonster(md);
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
