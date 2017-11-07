using System;
using System.Linq;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class InteractionWithStaticItems : IInteraction
        {
        private readonly World _world;
        private readonly StaticItem _staticItem;
        private readonly MovingItem _movingItem;

        public InteractionWithStaticItems(World world, StaticItem staticItem, MovingItem movingItem)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (movingItem == null)
                throw new ArgumentNullException("movingItem");
            if (staticItem == null)
                throw new ArgumentNullException("staticItem");

            if (staticItem is MovingItem)
                throw new ArgumentOutOfRangeException("staticItem");

            this._world = world;
            this._staticItem = staticItem;
            this._movingItem = movingItem;
            }

        public void Collide()
            {
            if (!this._movingItem.IsExtant || !this._staticItem.IsExtant)
                return;

            var player = this._movingItem as Player;
            if (player != null)
                {
                InteractionInvolvesPlayer(this._world, player, this._staticItem);
                return;
                }

            var explosion = this._movingItem as Explosion;
            if (explosion != null)
                {
                InteractionInvolvesExplosion(explosion, this._staticItem);
                return;
                }

            var shot = this._movingItem as Shot;
            if (shot != null)
                {
                InteractionInvolvesShot(shot, this._staticItem);
                // return;
                }
            }

        private static void InteractionInvolvesPlayer(World world, Player player, StaticItem staticItem)
            {
            var crystal = staticItem as Crystal;
            if (crystal != null)
                {
                PlayerTakesCrystal(world, player, crystal);
                return;
                }

            var forceField = staticItem as ForceField;
            if (forceField != null)
                {
                player.InstantlyExpire();
                return;
                }

            var fruit = staticItem as Fruit;
            if (fruit != null)
                {
                PlayerEatsFruit(player, fruit);
                return;
                }

            var mushroom = staticItem as Mushroom;
            if (mushroom != null)
                {
                PlayerPoisonedByMushroom(mushroom, player);
                // return;
                }
            }

        private static void PlayerPoisonedByMushroom(Mushroom mushroom, Player player)
            {
            player.PlaySound(GameSound.PlayerInjured);
            int r = mushroom.CalculateEnergyToRemove(player);
            if (r > 0)
                player.ReduceEnergy(r);
            mushroom.SetTaken();
            }

        private static void PlayerEatsFruit(Player player, Fruit fruit)
            {
            player.PlaySound(GameSound.PlayerEatsFruit);
            player.AddEnergy(fruit.Energy);
            fruit.SetTaken();
            }

        private static void PlayerTakesCrystal(World world, Player player, Crystal crystal)
            {
            player.AddEnergy(crystal.Energy);
            GlobalServices.ScoreKeeper.CrystalTaken(crystal);
            player.CrystalCollected(crystal);
            crystal.SetTaken();
            int howManyCrystalsRemain = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Count();
            if (howManyCrystalsRemain == 0)
                {
                GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerFinishesWorld,
                    (sender, args) => world.SetLevelReturnType(LevelReturnType.FinishedWorld));
                world.SetDoNotUpdate();
                }
            else
                GlobalServices.SoundPlayer.Play(GameSound.PlayerCollectsCrystal);
            }

        private static void InteractionInvolvesShot(Shot shot, StaticItem staticItem)
            {
            if (staticItem is Wall)
                {
                shot.InstantlyExpire();
                return;
                }

            if (staticItem is Fruit || staticItem is Grave || staticItem is Mushroom || staticItem is CrumblyWall)
                {
                staticItem.ReduceEnergy(shot.Energy);
                Bang bang;
                if (staticItem.IsExtant)
                    bang = GlobalServices.GameState.ConvertShotToBang(shot);
                else
                    {
                    bang = GlobalServices.GameState.AddBang(staticItem.Position, BangType.Short);
                    shot.InstantlyExpire();
                    }
                bang.PlaySound(GameSound.StaticObjectShotAndInjured);
                return;
                }

            var standardShot = shot as StandardShot;
            if (standardShot == null)
                return;
            if (staticItem is ForceField)
                {
                if (!standardShot.HasRebounded)
                    standardShot.Reverse();
                //return;
                }

            }

        private static void InteractionInvolvesExplosion(Explosion explosion, StaticItem staticItem)
            {
            if (staticItem is Wall)
                {
                explosion.InstantlyExpire();
                return;
                }

            if (staticItem is Fruit || staticItem is Grave || staticItem is Mushroom || staticItem is CrumblyWall)
                {
                staticItem.ReduceEnergy(explosion.Energy);
                if (!staticItem.IsExtant)
                    {
                    var bang = GlobalServices.GameState.AddBang(staticItem.Position, BangType.Short);
                    explosion.InstantlyExpire();
                    bang.PlaySound(GameSound.StaticObjectShotAndInjured);
                    }
                //return;
                }
            }
        }
    }
