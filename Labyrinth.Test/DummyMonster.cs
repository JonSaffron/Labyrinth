﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Labyrinth.GameObjects;
using Labyrinth.GameObjects.Monsters.Actions;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class DummyMonster : Monster
        {
        public DummyMonster(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.Mobility = MonsterMobility.Placid;
            this.IsActive = true;

            base._actions = new List<BaseAction>();
            var action = new DummyAction();
            action.Init(this);
            base._actions.Add(action);
            
            var action2 = new Flitter();
            action2.Init(this);
            base._actions.Add(action2);
            }

        public override void ResetPosition(Vector2 position)
            {
            // ignore this
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            return new DummyMonsterMovement(this);
            }

        public List<PositionAndTime> Log
            {
            get
                {
                var dummyAction = (DummyAction) base._actions[0];
                return dummyAction._calls;
                }
            }
        }

    internal class DummyMonsterMovement : MonsterMotionBase
        {
        public DummyMonsterMovement(Monster monster) : base(monster)
            {
            }

        public override Direction DetermineDirection()
            {
            if (Game1.Ticks < 100)
                {
                if (this.Monster.TilePosition.X < 5)
                    return Direction.Right;
                return Direction.None;
                }
            return this.Monster.CanMoveInDirection(Direction.Right) ? Direction.Right : Direction.None;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.StandardSpeed);
            return true;
            }
        }

    class DummyAction : BaseAction
        {
        public List<PositionAndTime> _calls = new List<PositionAndTime>();

        public override void Init(Monster monster)
            {
            base.Init(monster);
            PerformAction();
            }

        public override void PerformAction()
            {
            var pat = new PositionAndTime {Position = this.Monster.Position};
            this._calls.Add(pat);
            }
        }

    class PositionAndTime
        {
        public Vector2 Position;
        public readonly int Ticks = Game1.Ticks;
        }
    }
