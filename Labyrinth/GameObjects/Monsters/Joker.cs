using System;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Joker : Monster
        {
        public Joker(AnimationPlayer animationPlayer, Vector2 position, int energy) : base("", animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Joker", 4));

            this.Behaviours.Add<Flitter>();
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Behaviours.Add<LaysEgg>();
            this.Behaviours.Add<SpawnsUponDeath>();
            this.AddShootsAtPlayerBehaviour();
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return GlobalServices.MonsterMovementFactory.Cautious(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
