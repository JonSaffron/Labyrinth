using System;
using System.Linq;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractionWithMovingItems : IInteraction
        {
        private readonly World _world;
        private readonly MovingItem _movingItem1;
        private readonly MovingItem _movingItem2;

        public InteractionWithMovingItems(World world, MovingItem movingItem1, MovingItem movingItem2)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (movingItem1 == null)
                throw new ArgumentNullException("movingItem1");
            if (movingItem2 == null)
                throw new ArgumentNullException("movingItem2");

            this._world = world;
            this._movingItem1 = movingItem1;
            this._movingItem2 = movingItem2;
            }

        public void Collide()
            {
            if (!this._movingItem1.IsExtant || !this._movingItem2.IsExtant)
                return;

            var items = new[] { this._movingItem1, this._movingItem2 };
            var shot = items.OfType<Shot>().FirstOrDefault();
            if (shot != null)
                {
                var otherItem = items.Single(item => item != shot);
                if (InteractionInvolvingShot(this._world, shot, otherItem))
                    return;
                }

            var mine = items.OfType<Mine>().FirstOrDefault();
            if (mine != null)
                {
                var otherItem = items.Single(item => item != mine);
                mine.SteppedOnBy(otherItem);
                return;
                }

            var player = items.OfType<Player>().SingleOrDefault();
            if (player != null)
                {
                var monster = items.Single(item => item != player) as Monster;
                if (monster != null)
                    {
                    int monsterEnergy = monster.InstantlyExpire();
                    player.ReduceEnergy(monsterEnergy);
                    this._world.AddBang(monster.Position, BangType.Long, GameSound.PlayerCollidesWithMonster);
                    return;
                    }
                }
            
            var moveableObject = items.FirstOrDefault(item => item.Solidity == ObjectSolidity.Moveable);
            var movingObject = items.FirstOrDefault(item => item != moveableObject && (item.Capability == ObjectCapability.CanPushOthers || item.Capability == ObjectCapability.CanPushOrCauseBounceBack));
            if (moveableObject != null && movingObject != null)
                {
                if (PushOrBounceObject(moveableObject, movingObject))
                    return;
                }
            }

        private bool PushOrBounceObject(MovingItem moveableObject, MovingItem movingObject)
            {
            // test whether moveable object can be pushed or bounced
            if (moveableObject.Direction == Direction.None && movingObject.Direction != Direction.None)
                {
                Vector2 currentDifference = movingObject.Position - moveableObject.Position;
                float currentDistanceApart = Math.Abs(currentDifference.Length());
                if (currentDistanceApart < 40)
                    {
                    Vector2 positionWithMovement = movingObject.Position + movingObject.Direction.ToVector();
                    Vector2 potentialDifference = positionWithMovement - moveableObject.Position;
                    float potentialDistanceApart = Math.Abs(potentialDifference.Length());
                    if (potentialDistanceApart < currentDistanceApart)
                        {
                        moveableObject.PushOrBounce(movingObject, movingObject.Direction);
                        var standardShot = movingObject as StandardShot;
                        if (standardShot != null)
                            this._world.ConvertShotToBang(standardShot);
                        return true;
                        }
                    }
                }

            // test whether the moveable object crushing the insubstantial object
            if (moveableObject.Direction != Direction.None && moveableObject.Direction != movingObject.Direction)
                {
                var insubstantialObjectPosition = movingObject.Direction == Direction.None ? movingObject.Position : movingObject.MovingTowards;
                var insubtantialObjectTile = TilePos.TilePosFromPosition(insubstantialObjectPosition);
                var moveableObjectTile = TilePos.TilePosFromPosition(moveableObject.MovingTowards);
                if (insubtantialObjectTile == moveableObjectTile)
                    {
                    var energy = movingObject.InstantlyExpire();
                    var b = this._world.AddBang(movingObject.Position, BangType.Long);
                    if (movingObject is Monster)
                        {
                        b.PlaySound(GameSound.MonsterDies);
                        if (!(movingObject is DeathCube))
                            {
                            var score = ((energy >> 1) + 1) * 20;
                            this._world.IncreaseScore(score);
                            }
                        }
                    return true;
                    }
                }
            return false;
            }

        private static bool InteractionInvolvingShot(World world, Shot shot, MovingItem movingItem)
            {
            if (movingItem is Player)
                {
                var bang = world.ConvertShotToBang(shot);
                movingItem.ReduceEnergy(shot.Energy);
                if (movingItem.IsAlive())
                    bang.PlaySound(GameSound.PlayerInjured);
                return true;
                }

            var monster = movingItem as Monster;
            if (monster != null)
                {
                var result = ShotHitsMonster(world, shot, monster);
                return result;
                }

            var items = new[] { shot, movingItem };
            var explosion = items.OfType<Explosion>().FirstOrDefault();
            if (explosion != null)
                {
                var otherItem = items.Single(item => item != explosion);
                if (otherItem is Shot)
                    {
                    shot.ReduceEnergy(explosion.Energy);
                    return true;
                    }
                }

            var standardShot1 = shot as StandardShot;
            var standardShot2 = movingItem as StandardShot;
            if (standardShot1 != null && standardShot2 != null)
                {
                var result = ShotHitsShot(world, standardShot1, standardShot2);
                return result;
                }

            return false;
            }

        private static bool ShotHitsShot(World world, StandardShot standardShot1, StandardShot standardShot2)
            {
            if (standardShot2.ShotType == standardShot1.ShotType ||
                standardShot2.Direction != standardShot1.Direction.Reversed())
                {
                return false;
                }
            
            // todo: check what the original game does. It may not create more than one bang.
            int minEnergy = Math.Min(standardShot2.Energy, standardShot1.Energy);
            Bang bang = null;
            standardShot2.ReduceEnergy(minEnergy);
            if (!standardShot2.IsExtant)
                bang = world.ConvertShotToBang(standardShot2);
            standardShot1.ReduceEnergy(minEnergy);
            if (!standardShot1.IsExtant)
                bang = world.ConvertShotToBang(standardShot1);
            if (bang != null)   // bang will always be not null
                bang.PlaySound(GameSound.StaticObjectShotAndInjured);
            return true;
            }

        private static bool ShotHitsMonster(World world, Shot shot, Monster monster)
            {
            if (!monster.IsActive)
                monster.IsActive = true;

            if (monster.Mobility == MonsterMobility.Patrolling)
                monster.Mobility = MonsterMobility.Placid;

            var standardShot = shot as StandardShot;
            if (standardShot != null && monster.ShotsBounceOff)
                {
                if (!standardShot.HasRebounded)
                    standardShot.Reverse();
                return false;
                }

            var score = ((Math.Min(shot.Energy, monster.Energy) >> 1) + 1)*10;
            world.IncreaseScore(score);
            monster.ReduceEnergy(shot.Energy);
            var bang = world.ConvertShotToBang(shot);
            if (monster.IsAlive())
                {
                var sound = monster.IsEgg ? GameSound.PlayerShootsAndInjuresEgg : GameSound.PlayerShootsAndInjuresMonster;
                bang.PlaySound(sound);
                }
            return true;
            }
        }
    }
