using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class KillerCubeRed : KillerCube
        {
        public KillerCubeRed(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/KillerCubeRed", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        
        private static bool ShouldMakeMove()
            {
            return MonsterRandom.Next(8) == 0; 
            }


        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Patrolling:
                    return MonsterMovement.DetermineDirectionStandardPatrolling;
                case MonsterMobility.Placid:
                    return MonsterMovement.DetermineDirectionRolling;
                case MonsterMobility.Aggressive:
                    return DetermineDirectionAggressive;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        private static Direction DetermineDirectionAggressive(Monster m, World w)
            {
            TilePos tp = m.TilePosition;
            bool isCurrentlyMovingTowardsFreeSpace;
            if (m.Direction == Direction.None) 
                isCurrentlyMovingTowardsFreeSpace = false;
            else
                {
                TilePos pp = tp.GetPositionAfterOneMove(m.Direction);
                isCurrentlyMovingTowardsFreeSpace = w.CanTileBeOccupied(pp, true);
                
                if (isCurrentlyMovingTowardsFreeSpace)
                    {
                    Vector2 newPos = pp.ToPosition();
                    if (!MonsterMovement.IsInSameRoom(m.Position, newPos) && MonsterRandom.Next(4) != 0)
                        isCurrentlyMovingTowardsFreeSpace = false;
                    }
                }
                
            Direction newDirection = Direction.None;
            if (w.Player != null)
                {
                TilePos playerPosition = w.Player.TilePosition;
                int yDiff = tp.Y - playerPosition.Y;
                int xDiff = tp.X - playerPosition.X;
                
                if (xDiff > 16 || yDiff > 16)
                    {
                    // player is out of sight
                    }
                else if (m.Direction == Direction.Left || m.Direction == Direction.Right)
                    {
                    // Check if travelling on same row as player
                    if (yDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = MonsterRandom.Next(2) == 0 ? Direction.Up : Direction.Down;
                        }
                    else if ((m.Direction == Direction.Left && xDiff <= -5) || (m.Direction == Direction.Right && xDiff >= 5))
                        {
                        newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                        }
                    }
                else if (m.Direction == Direction.Up || m.Direction == Direction.Down)
                    {
                    if (xDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = MonsterRandom.Next(2) == 0 ? Direction.Left : Direction.Right;
                        }
                    else if ((m.Direction == Direction.Up && yDiff <= -5) || (m.Direction == Direction.Down && yDiff >= 5))
                        {
                        newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                        }
                    }

                if (newDirection != Direction.None)
                    {
                    TilePos pp = tp.GetPositionAfterOneMove(newDirection);
                    if (!w.CanTileBeOccupied(pp, true))
                        newDirection = Direction.None;
                    }
                }

            if (newDirection == Direction.None)
                {                    
                if (m.Direction == Direction.None)
                    {
                    newDirection = MonsterMovement.RandomDirection(m, w);
                    }
                else if (!isCurrentlyMovingTowardsFreeSpace)
                    {
                    // tend to bounce backwards
                    newDirection = MonsterRandom.Next(5) == 0 ? MonsterMovement.GetRandomPerpendicularDirection(m.Direction) : m.Direction.Reversed();
                    }
                else if (MonsterRandom.Next(16) == 0)
                    {
                    newDirection = MonsterMovement.GetRandomPerpendicularDirection(m.Direction);
                    }
                else
                    {
                    newDirection = m.Direction;
                    }
                }
            return newDirection;
            }
        }
    }
