using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class CrazyCrawler : Monster
        {
        private bool ShotsBounceOff;

        public CrazyCrawler(AnimationPlayer animationPlayer, Vector2 position, int energy) : base("", animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/CrazyCrawler", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ShotsBounceOff = true;
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.Placid(this);
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.SemiAggressive(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
