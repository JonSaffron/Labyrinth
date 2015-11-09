using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class RotaFloater : Monster
        {
        protected int ChanceOfAggressiveMove;

        protected RotaFloater(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.StandardRolling();
                case MonsterMobility.Patrolling:
                    return GlobalServices.MonsterMovementFactory.StandardPatrolling();
                case MonsterMobility.Aggressive:
                    Func<Monster, Direction> result = m => DetermineDirectionAggressive(this, ShouldMakeAnAggressiveMove);
                    return result;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        private static Direction DetermineDirectionAggressive(Monster m, Func<bool> shouldMakeAnAggressiveMove)
            {
            var p = GlobalServices.GameState.Player;
            if (!p.IsExtant)
                {
                return MonsterMovement.DetermineDirectionRolling(m);
                }
            
            TilePos tp = m.TilePosition;
/*            bool isCurrentlyMovingTowardsFreeSpace;
            if (this.Direction == Direction.None) 
                isCurrentlyMovingTowardsFreeSpace = false;
            else
                {
                TilePos pp = TilePos.GetPositionAfterOneMove(tp, this.Direction);
                isCurrentlyMovingTowardsFreeSpace = this._world.IsTileUnoccupied(pp, true);
                
                if (isCurrentlyMovingTowardsFreeSpace)
                    {
                    Vector2 newPos = World.GetBounds(pp).GetCentre();
                    if (!IsInSameRoom(this.Position, newPos) && MonsterRandom.Next(4) != 0)
                        isCurrentlyMovingTowardsFreeSpace = false;
                    }
                }
*/            
            Direction newDirection = Direction.None;
            TilePos playerPosition = p.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;
            int xDiff = tp.X - playerPosition.X;
            if (m.Direction == Direction.Left || m.Direction == Direction.Right)
                {
                if (Math.Abs(tp.X - playerPosition.X) == 1 && Math.Abs(yDiff) > 3 && Math.Abs(yDiff) <= 16 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                else if ((tp.X - playerPosition.X) == 0 && Math.Abs(yDiff) <= 3 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                }
            else if (m.Direction == Direction.Up || m.Direction == Direction.Down)
                {
                if (Math.Abs(tp.Y - playerPosition.Y) == 1 && Math.Abs(xDiff) > 3 && Math.Abs(xDiff) <= 16 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                else if ((tp.Y - playerPosition.Y) == 0 && Math.Abs(xDiff) <= 3 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                }
            if (newDirection != Direction.None)
                {
                TilePos pp = tp.GetPositionAfterOneMove(newDirection);
                if (GlobalServices.GameState.CanTileBeOccupied(pp, true))
                    {
                    return newDirection;
                    }
                }

            return MonsterMovement.DetermineDirectionRolling(m);
            }

        private bool ShouldMakeAnAggressiveMove()
            {
            return MonsterRandom.Next(this.ChanceOfAggressiveMove) == 0; 
            }
        
        }
    }
