using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Motility;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Tank : Monster
        {
        private static readonly Dictionary<Direction, decimal> RotationForDirection = BuildRotationForDirection();
        
        /// <summary>
        /// The current angle of the hull
        /// </summary>
        /// <remarks>0 indicates facing upwards, -1 would be 180° anticlockwise, 1 would be 180° clockwise</remarks>
        private decimal _hullRotation;
        private readonly Turret _turret;

        public LinearMotion LeftTrack { get; private set; } = LinearMotion.Still;
        public LinearMotion RightTrack { get; private set; } = LinearMotion.Still;

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
        public float TurretRotation => (float) (this._turret.CurrentRotation) * MathHelper.Pi;

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
                    this.RightTrack = LinearMotion.Still;
                    this.LeftTrack = LinearMotion.Still;
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
                    var targetRotation = RotationForDirection[direction];
                    var directionOfTravel = GetDirectionOfTravel(targetRotation);

                    this.LeftTrack = (LinearMotion) directionOfTravel;
                    this.RightTrack = (LinearMotion) (- (int) directionOfTravel);

                    while (this._hullRotation != targetRotation)
                        {
                        if (this.TryToCompleteRotationToTarget(ref this.RemainingTime, targetRotation, directionOfTravel))
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
                    this.RightTrack = LinearMotion.Forwards;
                    this.LeftTrack = LinearMotion.Forwards;
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
                if (this.RemainingTime <= double.Epsilon * 2)
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private bool TryToCompleteRotationToTarget(ref double remainingTime, decimal targetRotation, RotationalMotion directionOfTravel)
            {
            decimal speedOfRotation = (int) directionOfTravel * 0.5m * Constants.BaseDistancesMovedPerSecond * this.SpeedAdjustmentFactor;
            decimal changeOfRotation = GetChangeOfRotationRequired(this._hullRotation, targetRotation);
            double timeNeededToFinishRotation = (double) (changeOfRotation / speedOfRotation);
            bool hasCompletedRotation = timeNeededToFinishRotation <= remainingTime;
            if (hasCompletedRotation)
                {
                remainingTime -= timeNeededToFinishRotation;
                this._hullRotation = targetRotation;
                }
            else
                {
                decimal changeInRotation = (decimal) ((double) speedOfRotation * remainingTime);
                this._hullRotation += changeInRotation;
                remainingTime = 0;
                }
            NormaliseRotation(ref this._hullRotation);

            return hasCompletedRotation;
            }

        private RotationalMotion GetDirectionOfTravel(decimal requiredRotation)
            {
            var difference = GetChangeOfRotationRequired(this._hullRotation, requiredRotation);
            return (RotationalMotion) Math.Sign(difference);
            }

        /// <summary>
        /// Returns the angle of change needed to go from the current position of the hull to the desired position
        /// </summary>
        /// <param name="currentRotation">The current angle</param>
        /// <param name="targetRotation">The angle to change towards</param>
        /// <returns>A value between -1 (indicating -180°) to 1 (indicating 180°).</returns>
        private static decimal GetChangeOfRotationRequired(decimal currentRotation, decimal targetRotation)
            {
            var difference = targetRotation - currentRotation;
            NormaliseRotation(ref difference);
            return difference;
            }

        private static void NormaliseRotation(ref decimal rotation)
            {
            if (rotation > 1m)
                rotation -= 2m;
            else if (rotation < -1m)
                rotation += 2m;
            }

        private class Turret
            {
            private readonly Tank _tank;
            private decimal _currentRotationRelativeToHull;
            private decimal _desiredRotation;

            public Turret([NotNull] Tank tank)
                {
                this._tank = tank ?? throw new ArgumentNullException(nameof(tank));
                }

            public decimal CurrentRotation => this._currentRotationRelativeToHull + this._tank._hullRotation;

            public void Update(GameTime gameTime)
                {
                decimal differenceInRotation = GetChangeOfRotationRequired(this.CurrentRotation, this._desiredRotation);
                RotationalMotion directionOfTravel = (RotationalMotion) Math.Sign(differenceInRotation);
                decimal speedOfRotation = (int) directionOfTravel * 0.75m * Constants.BaseDistancesMovedPerSecond;
                double timeNeededToFinishRotation = (double) (differenceInRotation / speedOfRotation);
                var remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
                bool hasCompletedRotation = timeNeededToFinishRotation <= remainingTime;
                if (hasCompletedRotation)
                    {
                    this._currentRotationRelativeToHull = this._desiredRotation;
                    }
                else
                    {
                    decimal changeInRotation = (decimal) ((double) speedOfRotation * remainingTime);
                    this._currentRotationRelativeToHull += changeInRotation;
                    }
                NormaliseRotation(ref this._currentRotationRelativeToHull);
                }

            public void ReviewDirectionFaced()
                {
                var patrol = this._tank.DetermineDirection as PatrolPerimeter;
                if (patrol == null || patrol.IsDetached)
                    {
                    this._desiredRotation = 0m;
                    return;
                    }
                
                // todo
                }
            }
        }
    }
