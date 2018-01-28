﻿using System;
using Labyrinth.GameObjects.Actions;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class TigerMoth : Monster
        {
        public TigerMoth(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/TigerMoth", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.MovementBehaviours.Add<LaysMushroom>();
            this.SetShootsOnceProvoked(true);
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.SemiAggressive(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
