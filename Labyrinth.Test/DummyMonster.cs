﻿using System.Collections.Generic;
using Labyrinth.GameObjects;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.GameObjects.Motility;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class DummyMonster : Monster
        {
        public DummyMonster(MonsterDef monsterDef, string textureName, int baseMovesDuringAnimation) : base(monsterDef, textureName, baseMovesDuringAnimation)
            {
            this.Mobility = MonsterMobility.Placid;
            this.Activate();

            var action = new DummyAction();
            action.Init(this);
            this.Behaviours.Add(action);
            
            var action2 = new Flitter();
            action2.Init(this);
            this.Behaviours.Add(action2);
            }

        public override void ResetPosition(Vector2 position)
            {
            // ignore this
            }

        public List<PositionAndTime> Log
            {
            get
                {
                var dummyAction = (DummyAction) this.Behaviours[0];
                return dummyAction.Calls;
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

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            return true;
            }
        }

    class DummyAction : BaseBehaviour
        {
        private readonly List<PositionAndTime> _calls = new List<PositionAndTime>();

        public List<PositionAndTime> Calls => this._calls;

        public override void Init(Monster monster)
            {
            base.Init(monster);
            Perform();
            }

        public override void Perform()
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
