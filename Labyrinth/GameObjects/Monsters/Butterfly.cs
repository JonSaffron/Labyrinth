using System;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Butterfly : Monster
        {
        public Butterfly(AnimationPlayer animationPlayer, Vector2 position, int energy) : base("", animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Butterfly", 3));
            
            this.Behaviours.Add<Flitter>();
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Behaviours.Add<LaysEgg>();
            this.Behaviours.Add<ShootsAtPlayer>();
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.FullPursuit(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
