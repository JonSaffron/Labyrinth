using System;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class MonsterEgg: MovingItem, IMonster
        {
        private MonsterDef _underlyingMonster;
        private readonly GameTimer _hatchingTimer;
        private static readonly Animation EggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
        private static readonly Animation HatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);

        public MonsterEgg(AnimationPlayer animationPlayer, MonsterDef underlyingMonster, int timeToHatch) : base(animationPlayer, underlyingMonster.Position)
            {
            this._underlyingMonster = underlyingMonster;
            this.Energy = underlyingMonster.Energy;
            var timeSpan = TimeSpan.FromSeconds(timeToHatch * Constants.GameClockResolution);
            this._hatchingTimer = GameTimer.AddGameTimer(timeSpan, EggIsHatching, false);
            this.Ap.PlayAnimation(EggAnimation);
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            if (this.IsExtant)
                {
                this.Ap.PlayAnimation(HatchingAnimation);
                this.PlaySoundWithCallback(GameSound.EggHatches, HatchingSoundFinished);
                }
            }

        private void HatchingSoundFinished(object sender, EventArgs args)
            {
            HatchEgg();
            }

        public Monster HatchEgg()
            {
            if (!this.IsExtant)
                {
                return null;
                }

            this.InstantlyExpire();
            this._underlyingMonster.Energy = this.Energy;
            this._underlyingMonster.Position = this.Position;
            var result = (Monster) GlobalServices.GameState.AddMonster(this._underlyingMonster);
            return result;
            }

        public override bool Update(GameTime gameTime)
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this);
            this._hatchingTimer.Enabled = inSameRoom;
            return false;
            }

        public override int DrawOrder => (int) SpriteDrawOrder.StaticMonster;

        public bool HasBehaviour<T>() where T : IBehaviour
            {
            return false;
            }

        public override ObjectSolidity Solidity => ObjectSolidity.Stationary;
        }
    }
