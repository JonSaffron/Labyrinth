using System;
using System.Linq;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractionWithMovingItems : IInteraction
        {
        private readonly MovingItem _movingItem1;
        private readonly MovingItem _movingItem2;

        public InteractionWithMovingItems(MovingItem movingItem1, MovingItem movingItem2)
            {
            if (movingItem1 == null)
                throw new ArgumentNullException("movingItem1");
            if (movingItem2 == null)
                throw new ArgumentNullException("movingItem2");

            this._movingItem1 = movingItem1;
            this._movingItem2 = movingItem2;
            }

        public void Collide()
            {
            if (!this._movingItem1.IsExtant || !this._movingItem2.IsExtant)
                return;

            var items = new[] { this._movingItem1, this._movingItem2 };

            var explosion = items.OfType<Explosion>().FirstOrDefault();
            if (explosion != null)
                {
                var otherItem = items.Single(item => item != explosion);
                InteractionInvolvesExplosion(explosion, otherItem);
                return;
                }
                
            var shot = items.OfType<StandardShot>().FirstOrDefault();
            if (shot != null)
                {
                var otherItem = items.Single(item => item != shot);
                if (InteractionInvolvingShot(shot, otherItem))
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
                    GlobalServices.GameState.AddBang(monster.Position, BangType.Long, GameSound.PlayerCollidesWithMonster);
                    return;
                    }
                }
            
            var moveableObject = items.FirstOrDefault(item => item.Solidity == ObjectSolidity.Moveable);
            var movingObject = items.FirstOrDefault(item => item != moveableObject && item.Capability.CanMoveAnother());
            if (moveableObject != null && movingObject != null)
                {
                var actionTaken = PushOrBounceObject(moveableObject, movingObject);
                if (actionTaken)
                    // ReSharper disable once RedundantJumpStatement
                    return;
                }

            //todo move crush code

            // any other interaction here...
            }

        private static bool ShouldStartPushOrBounce(MovingItem moveableObject, MovingItem movingObject)
            {
            bool isMoveableObjectAlreadyInMotion = moveableObject.CurrentMovement.IsMoving;
            bool isMovingObjectMotionless = !movingObject.CurrentMovement.IsMoving;
            if (isMoveableObjectAlreadyInMotion || isMovingObjectMotionless) 
                return false;

            float currentDistanceApart = Vector2.Distance(movingObject.Position, moveableObject.Position);
            if (!(currentDistanceApart < 40)) 
                return false;

            Vector2 positionWithMovement = movingObject.Position + movingObject.CurrentMovement.Direction.ToVector();
            float potentialDistanceApart = Vector2.Distance(positionWithMovement, moveableObject.Position);
            bool isGettingCloser = (potentialDistanceApart < currentDistanceApart);
            return isGettingCloser;
            }

        private static bool PushOrBounceObject(MovingItem moveableObject, MovingItem movingObject)
            {
            if (ShouldStartPushOrBounce(moveableObject, movingObject))
                {
                moveableObject.PushOrBounce(movingObject, movingObject.CurrentMovement.Direction);
                var standardShot = movingObject as StandardShot;
                if (standardShot != null)
                    GlobalServices.GameState.ConvertShotToBang(standardShot);
                return true;
                }

            // test whether the moveable object crushing the insubstantial object
            if (moveableObject.CurrentMovement.IsMoving && moveableObject.CurrentMovement.Direction != movingObject.CurrentMovement.Direction)
                {
                var insubstantialObjectPosition = movingObject.CurrentMovement.Direction == Direction.None ? movingObject.Position : movingObject.CurrentMovement.MovingTowards;
                var insubtantialObjectTile = TilePos.TilePosFromPosition(insubstantialObjectPosition);
                var moveableObjectTile = TilePos.TilePosFromPosition(moveableObject.CurrentMovement.MovingTowards);
                if (insubtantialObjectTile == moveableObjectTile)
                    {
                    var energy = movingObject.InstantlyExpire();
                    var b = GlobalServices.GameState.AddBang(movingObject.Position, BangType.Long);
                    var monster = movingObject as Monster;
                    if (monster != null)
                        {
                        b.PlaySound(GameSound.MonsterDies);
                        GlobalServices.ScoreKeeper.EnemyCrushed(monster, energy);
                        }
                    return true;
                    }
                }
            return false;
            }

        private static void InteractionInvolvesExplosion(Explosion explosion, MovingItem movingItem)
            {
            var shot = movingItem as Shot;
            if (shot != null)
                {
                shot.ReduceEnergy(explosion.Energy);
                explosion.InstantlyExpire();
                }

            var monster = movingItem as Monster;
            if (monster != null)
                {
                var energyRemoved = Math.Min(explosion.Energy, monster.Energy);
                GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemoved);
                monster.ReduceEnergy(explosion.Energy);
                explosion.InstantlyExpire();
                return;
                }

            var player = movingItem as Player;
            if (player != null)
                {
                var explosionEnergy = explosion.Energy;
                explosion.InstantlyExpire();
                player.ReduceEnergy(explosionEnergy);
                if (movingItem.IsAlive())
                    player.PlaySound(GameSound.PlayerInjured);
                // ReSharper disable once RedundantJumpStatement
                return;
                }

            // any other interaction here...
            }

        private static bool InteractionInvolvingShot(StandardShot shot, MovingItem movingItem)
            {
            if (movingItem is Player && shot.HasRebounded)
                {
                var shotEnergy = shot.Energy;
                GlobalServices.GameState.ConvertShotToBang(shot);
                movingItem.ReduceEnergy(shotEnergy);
                if (movingItem.IsAlive())
                    movingItem.PlaySound(GameSound.PlayerInjured);
                return true;
                }

            var monster = movingItem as Monster;
            if (monster != null)
                {
                var result = ShotHitsMonster(shot, monster);
                return result;
                }

            var standardShot2 = movingItem as StandardShot;
            if (standardShot2 != null)
                {
                var result = ShotHitsShot(shot, standardShot2);
                return result;
                }

            return false;
            }

        private static bool ShotHitsShot(StandardShot standardShot1, StandardShot standardShot2)
            {
            if (standardShot2.ShotType == standardShot1.ShotType ||
                standardShot2.DirectionOfTravel != standardShot1.DirectionOfTravel.Reversed())
                {
                return false;
                }
            
            // todo: check what the original game does. It may not create more than one bang.
            int minEnergy = Math.Min(standardShot2.Energy, standardShot1.Energy);
            Bang bang = null;
            standardShot2.ReduceEnergy(minEnergy);
            if (!standardShot2.IsExtant)
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot2);
            standardShot1.ReduceEnergy(minEnergy);
            if (!standardShot1.IsExtant)
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot1);
            if (bang != null)   // bang will always be not null
                bang.PlaySound(GameSound.StaticObjectShotAndInjured);
            return true;
            }

        private static bool ShotHitsMonster(StandardShot shot, Monster monster)
            {
            if (monster.ShotsBounceOff)
                {
                if (!shot.HasRebounded)
                    shot.Reverse();
                return false;
                }

            var energyRemoved = Math.Min(shot.Energy, monster.Energy);
            GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemoved);
            monster.ReduceEnergy(shot.Energy);
            GlobalServices.GameState.ConvertShotToBang(shot);
            return true;
            }
        }
    }
