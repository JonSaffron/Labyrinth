using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Tank : Monster
        {
        private static readonly Dictionary<Direction, decimal> RotationForDirection = BuildRotationForDirection();
        private decimal _hullRotation;
        private decimal _turretRotation;
        private readonly Turret _turret;

        public Motion LeftTrack { get; private set; } = Motion.Still;
        public Motion RightTrack { get; private set; } = Motion.Still;

        public Tank(MonsterDef definition) : base(definition)
            {
            this.AnimationPlayer = new TankAnimation(this);
            this._turret = new Turret(this);
            }

        private static Dictionary<Direction, decimal> BuildRotationForDirection()
            {
            var result = new Dictionary<Direction, decimal>
                {
                    {Direction.Up, 0m},
                    {Direction.Right, 0.5m},
                    {Direction.Down, 1m},
                    {Direction.Left, -0.5m}
                };
            return result;
            }

        public float HullRotation => (float) this._hullRotation * MathHelper.Pi;
        public float TurretRotation => (float) this._turretRotation * MathHelper.Pi + HullRotation;

        public override bool Update(GameTime gameTime)
            {
            var result = base.Update(gameTime);
            this._turret.Update(gameTime);
            return result;
            }

        protected override IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            while (true)
                {
                Direction direction = this.GetDirection();
                if (direction == Direction.None)
                    {
                    this.RightTrack = Motion.Still;
                    this.LeftTrack = Motion.Still;
                    double timeLeftStationary = Constants.TileLength / (double) (Constants.BaseSpeed * this.SpeedAdjustmentFactor);
                    while (true)
                        {
                        bool hasMoveCompleted = timeLeftStationary <= this.RemainingTime;
                        if (hasMoveCompleted)
                            {
                            this.RemainingTime -= timeLeftStationary;
                            break;  // end of stationary move, so do behaviours and get next direction
                            }

                        timeLeftStationary -= this.RemainingTime;
                        this.RemainingTime = 0;
                        this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticMonster);
                        yield return hasMovedSinceLastCall;
                        hasMovedSinceLastCall = false;
                        }
                    }
                else
                    {
                    var changeOfRotation = GetChangeOfRotationRequired(direction);
                    if (changeOfRotation != 0)
                        {
                        this.LeftTrack = changeOfRotation < 0 ? Motion.Backwards : Motion.Forwards;
                        this.RightTrack = changeOfRotation < 0 ? Motion.Forwards : Motion.Backwards;
                        }

                    while (changeOfRotation != 0)
                        {
                        if (this.TryToCompleteRotationToTarget(ref this.RemainingTime, ref changeOfRotation))
                            break;

                        this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticMonster);
                        yield return hasMovedSinceLastCall;
                        hasMovedSinceLastCall = false;
                        }

                    if (!this.CanMoveInDirection(direction))
                        {
                        continue;
                        }

                    Move(direction);
                    this.RightTrack = Motion.Forwards;
                    this.LeftTrack = Motion.Forwards;
                    while (true)
                        {
                        hasMovedSinceLastCall = true;
                        if (this.TryToCompleteMoveToTarget(ref this.RemainingTime))
                            break;

                        this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.MovingMonster);
                        yield return true; // we have moved
                        }
                    }

                this.Behaviours.Perform<IMovementBehaviour>();
                if (this.RemainingTime <= Double.Epsilon * 2)
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private bool TryToCompleteRotationToTarget(ref double remainingTime, ref decimal changeOfRotation)
            {
            decimal speedOfRotation = Math.Sign(changeOfRotation) * 0.5m * Constants.BaseDistancesMovedPerSecond * this.SpeedAdjustmentFactor;
            double timeNeededToFinishRotation = (double) (changeOfRotation / speedOfRotation);
            bool hasCompletedRotation = timeNeededToFinishRotation <= remainingTime;
            if (hasCompletedRotation)
                {
                remainingTime -= timeNeededToFinishRotation;
                this._hullRotation += changeOfRotation;
                if (this._hullRotation >= 2m)
                    this._hullRotation -= 2m;
                else if (this._hullRotation <= -2m)
                    this._hullRotation += 2m;
                changeOfRotation = 0;
                }
            else
                {
                decimal changeInRotation = (decimal) ((double) speedOfRotation * remainingTime);
                this._hullRotation += changeInRotation;
                changeOfRotation -= changeInRotation;
                remainingTime = 0;
                }

            return hasCompletedRotation;
            }

        /// <summary>
        /// Returns the angle of change needed to go from the current position of the hull to the desired position
        /// </summary>
        /// <param name="direction">The direction to change towards</param>
        /// <returns>A value between -1 (indicating -180°) to 1 (indicating 180°).</returns>
        private decimal GetChangeOfRotationRequired(Direction direction)
            {
            var currentRotation = this._hullRotation;
            var requiredRotation = RotationForDirection[direction];
            var difference = requiredRotation - currentRotation;
            if (difference > 1m)
                difference -= 2m;
            else if (difference < -1m)
                difference += 2m;
            return difference;
            }

        private class Turret
            {
            private readonly Tank _tank;
            private decimal _desiredRotation;

            public Turret([NotNull] Tank tank)
                {
                this._tank = tank ?? throw new ArgumentNullException(nameof(tank));
                }

            public void Update(GameTime gameTime)
                {

                }
            }
        }
    }
