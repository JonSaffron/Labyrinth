using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.GameObjects;
using Labyrinth.Services.Messages;

namespace Labyrinth
    {
    class InteractionWithStaticItems : IInteraction
        {
        private readonly World _world;
        private readonly IGameObject _staticItem;
        private readonly IMovingItem _movingItem;

        public InteractionWithStaticItems(World world, IGameObject staticItem, IMovingItem movingItem)
            {
            this._world = world ?? throw new ArgumentNullException(nameof(world));

            if (staticItem == null)
                throw new ArgumentNullException(nameof(staticItem));

            if (staticItem is IMovingItem)
                throw new ArgumentOutOfRangeException(nameof(staticItem));

            this._staticItem = staticItem;

            this._movingItem = movingItem ?? throw new ArgumentNullException(nameof(movingItem));
            }

        public void Collide()
            {
            if (!this._movingItem.IsExtant || !this._staticItem.IsExtant)
                return;

            if (this._movingItem is Player player)
                {
                InteractionInvolvesPlayer(this._world, player, this._staticItem);
                return;
                }

            if (this._movingItem is Explosion explosion)
                {
                InteractionInvolvesExplosion(explosion, this._staticItem);
                return;
                }

            if (this._movingItem is IShot shot)
                {
                InteractionInvolvesShot(shot, this._staticItem);
                // return;
                }
            }

        private static void InteractionInvolvesPlayer(World world, Player player, IGameObject staticItem)
            {
            if (staticItem is Crystal crystal)
                {
                PlayerTakesCrystal(world, player, crystal);
                return;
                }

            if (staticItem is ForceField)
                {
                player.InstantlyExpire();
                return;
                }

            if (staticItem is Fruit fruit)
                {
                PlayerEatsFruit(player, fruit);
                return;
                }

            if (staticItem is Mushroom mushroom)
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
            // todo this didn't seem to end the world - test that checking IsExtant fixes the problem
            player.AddEnergy(crystal.Energy);
            var crystalTaken = new CrystalTaken(crystal);
            Messenger.Default.Send(crystalTaken);
            player.CrystalCollected(crystal);
            crystal.SetTaken();
            bool doAnyCrystalsRemain = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Any(c => c.IsExtant);
            if (doAnyCrystalsRemain)
                {
                GlobalServices.SoundPlayer.Play(GameSound.PlayerCollectsCrystal);
                }
            else
                {
                GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerFinishesWorld,
                    (sender, args) => world.SetLevelReturnType(LevelReturnType.FinishedWorld));
                world.SetDoNotUpdate();
                }
            }

        private static void InteractionInvolvesShot(IShot shot, IGameObject staticItem)
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

            if (!(shot is StandardShot standardShot))
                return;
            if (staticItem is ForceField)
                {
                if (!standardShot.HasRebounded)
                    standardShot.Reverse();
                //return;
                }

            }

        private static void InteractionInvolvesExplosion(Explosion explosion, IGameObject staticItem)
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
