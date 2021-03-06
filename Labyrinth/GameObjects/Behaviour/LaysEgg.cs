﻿using Labyrinth.DataStructures;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.GameObjects.Behaviour
    {
    class LaysEgg : BaseBehaviour, IMovementBehaviour
        {
        public LaysEgg(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public LaysEgg()
            {
            // nothing to do
            }

        public override void Perform()
            {
            if (!ShouldAttemptToLayEgg())
                return;

            TilePos tp = this.Monster.TilePosition;
            if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                {
                this.PlaySound(GameSound.MonsterLaysEgg);
                MonsterDef md = this.Monster.Definition;
                md.Position = this.Monster.TilePosition.ToPosition();
                md.InitialDirection = Direction.None;
                md.IsEgg = true;
                md.TimeBeforeHatching = (this.Random.Next(256) & 0x1f) + 8;

                md.LaysEggs = false; // original game says hatched monsters do not lay eggs themselves
                GlobalServices.GameState.AddMonster(md);
                }
            }

        private bool ShouldAttemptToLayEgg()
            {
            var result =
                    !this.Monster.IsStationary
                && this.Monster.Mobility != MonsterMobility.Patrolling
                && this.Player.IsAlive()
                && this.IsInSameRoom()
                && this.Random.Test(0x1f);
            return result;
            }
        }
    }
