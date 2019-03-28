using System;
using Labyrinth.GameObjects.Motility;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class MonsterEgg: MovingItem, IMonster
        {
        private MonsterDef _underlyingMonster;
        private readonly GameTimer _hatchingTimer;
        private LoopedAnimation _animationPlayer;

        public MonsterEgg(MonsterDef underlyingMonster, int timeToHatch) : base(underlyingMonster.Position)
            {
            this._underlyingMonster = underlyingMonster;
            this.Energy = underlyingMonster.Energy;
            this._animationPlayer = new LoopedAnimation(this, "Sprites/Monsters/Egg", 6);
            var timeSpan = TimeSpan.FromSeconds(timeToHatch * Constants.GameClockResolution);
            this._hatchingTimer = GameTimer.AddGameTimer(timeSpan, EggIsHatching, false);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticMonster);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Stationary);
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            if (this.IsExtant)
                {
                this._animationPlayer = new LoopedAnimation(this, "Sprites/Monsters/Egg", 2);
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

            this._underlyingMonster.Energy = this.Energy;
            this._underlyingMonster.Position = this.Position;
            var result = (Monster) GlobalServices.GameState.AddMonster(this._underlyingMonster);

            this.InstantlyExpire(); // must be after copying Energy level
            return result;
            }

        public override bool Update(GameTime gameTime)
            {
            this._animationPlayer.Update(gameTime);
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this);
            this._hatchingTimer.Enabled = inSameRoom;
            return false;
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        }
    }
