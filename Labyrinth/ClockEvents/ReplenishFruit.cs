using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.ClockEvents
    {
    public class ReplenishFruit : IClockEvent
        {
        private readonly RandomFruitDistribution _fruitDistList;
        private readonly Dictionary<FruitType, bool[]> _population;
        private readonly Dictionary<Fruit, int> _reverseLookup;

        public ReplenishFruit([NotNull] RandomFruitDistribution fruitDistList)
            {
            this._fruitDistList = fruitDistList ?? throw new ArgumentNullException(nameof(fruitDistList));
            this._population = new Dictionary<FruitType, bool[]>();
            var totalFruitCount = 0;
            foreach (var fd in fruitDistList.Definitions)
                {
                this._population.Add(fd.FruitType, new bool[fd.FruitQuantity]);
                totalFruitCount += fd.FruitQuantity;
                }
            this._reverseLookup = new Dictionary<Fruit, int>(totalFruitCount);
            GlobalServices.GameState.GameObjectRemoved += OnRemoveFruit;
            }

        public void Update(int ticks)
            {
            foreach (var fd in this._fruitDistList.Definitions)
                {
                int slotNumber = (ticks % fd.FruitQuantity);
                AddFruit(slotNumber, this._fruitDistList.Area, fd.FruitType, fd.Energy);
                }            
            }

        private void AddFruit(int slotNumber, Rectangle area, FruitType fruitType, int energy)
            {
            var gameState = GlobalServices.GameState;

            if (!this._population.TryGetValue(fruitType, out var population))
                throw new InvalidOperationException("Population dictionary does not contain an entry for " + fruitType);

            ref bool populated = ref population[slotNumber];
            if (populated)
                return;

            var rnd = GlobalServices.Randomess;
            var tp = new TilePos(area.X + rnd.Next(area.Width), area.Y + rnd.Next(area.Height));
            if (gameState.GetItemsOnTile(tp).Any())
                return;

            if (Math.Abs(tp.X - gameState.Player.TilePosition.X) < Constants.RoomSizeInTiles.X
                && Math.Abs(tp.Y - gameState.Player.TilePosition.Y) < Constants.RoomSizeInTiles.Y)
                {
                return;
                }

            var fruit = GlobalServices.GameState.AddFruit(tp.ToPosition(), fruitType, energy);
            populated = true;
            this._reverseLookup.Add(fruit, slotNumber);
            }

        private void OnRemoveFruit(object sender, GameObjectEventArgs args)
            {
            if (!(args.GameObject is Fruit fruit))
                return;

            if (!this._reverseLookup.TryGetValue(fruit, out var slotNumber))
                return;

            if (!this._population.TryGetValue(fruit.FruitType, out var population))
                throw new InvalidOperationException("Population dictionary does not contain an entry for " + fruit.FruitType);

            ref bool populated = ref population[slotNumber];
            if (!populated)
                throw new InvalidOperationException("Fruit population is not marked as set.");

            populated = false;
            this._reverseLookup.Remove(fruit);
            }
        }
    }