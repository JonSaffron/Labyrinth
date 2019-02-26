using System;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class DiamondDemon : Monster
        {
        public DiamondDemon(AnimationPlayer animationPlayer, Vector2 position, int energy) : base("", animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/DiamondDemon", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.Behaviours.Add<Flitter>();
            this.Behaviours.Add<LaysEgg>();
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.AddShootsAtPlayerBehaviour();
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
