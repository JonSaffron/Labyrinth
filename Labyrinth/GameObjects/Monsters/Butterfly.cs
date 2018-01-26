﻿using System;
using Labyrinth.GameObjects.Actions;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Butterfly : Monster
        {
        public Butterfly(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Butterfly", 3));
            
            this.MovementBehaviours.Add<Flitter>();
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.MovementBehaviours.Add<LaysEgg>();
            this.SetShootsAtPlayer(true);
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
