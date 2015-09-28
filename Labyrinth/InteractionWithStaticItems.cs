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

            var shot = this._movingItem as Shot;
            if (shot != null)
                {
                InteractionInvolvesShot(this._world, shot, this._staticItem);
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
                PlayerEatsFruit(world, player, fruit);
                return;
                }

            var mushroom = staticItem as Mushroom;
            if (mushroom != null)
                {
                PlayerPoisonedByMushroom(world, mushroom, player);
                // return;
                }
            }

        private static void PlayerPoisonedByMushroom(World world, Mushroom mushroom, Player player)
            {
            world.Game.SoundPlayer.Play(GameSound.PlayerInjured);
            int r = mushroom.CalculateEnergyToRemove(player);
            if (r > 0)
                player.ReduceEnergy(r);
            mushroom.SetTaken();
            }

        private static void PlayerEatsFruit(World world, Player player, Fruit fruit)
            {
            world.Game.SoundPlayer.Play(GameSound.PlayerEatsFruit);
            player.AddEnergy(fruit.Energy);
            fruit.SetTaken();
            }

        private static void PlayerTakesCrystal(World world, Player player, Crystal crystal)
            {
            player.AddEnergy(crystal.Energy);
            world.IncreaseScore(crystal.Score);
            player.CrystalCollected(crystal);
            crystal.SetTaken();
            int howManyCrystalsRemain = world.GameObjects.DistinctItemsOfType<Crystal>().Count();
            if (howManyCrystalsRemain == 0)
                {
                world.Game.SoundPlayer.PlayWithCallback(GameSound.PlayerFinishesWorld,
                    (sender, args) => world.SetLevelReturnType(LevelReturnType.FinishedLevel));
                world.SetDoNotUpdate();
                }
            else
                world.Game.SoundPlayer.Play(GameSound.PlayerCollectsCrystal);
            }

        private static void InteractionInvolvesShot(World world, Shot shot, StaticItem staticItem)
            {
            if (staticItem is Wall)
                {
                shot.InstantlyExpire();
                return;
                }

            if (staticItem is Fruit || staticItem is Grave || staticItem is Mushroom || staticItem is CrumblyWall)
                {
                world.Game.SoundPlayer.Play(GameSound.StaticObjectShotAndInjured);
                staticItem.ReduceEnergy(shot.Energy);
                if (staticItem.IsExtant)
                    world.ConvertShotToBang(shot);
                else
                    {
                    world.AddBang(staticItem.Position, BangType.Short);
                    shot.InstantlyExpire();
                    }
                return;
                }

            var standardShot = shot as StandardShot;
            if (standardShot == null)
                return;
            if (staticItem is ForceField)
                {
                if (!standardShot.HasRebounded)
                    standardShot.Reverse();
                return;
                }

            }
        }
    }
