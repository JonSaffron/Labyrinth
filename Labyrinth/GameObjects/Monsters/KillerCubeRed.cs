﻿using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class KillerCubeRed : KillerCube
        {
        public KillerCubeRed(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/KillerCubeRed", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        
        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Patrolling:
                    return GlobalServices.MonsterMovementFactory.StandardPatrolling(this, this.InitialDirection);
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.StandardRolling(this, this.InitialDirection);
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.KillerCubeRedMovement(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
