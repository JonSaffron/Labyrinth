﻿using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.ClockEvents
    {
    public class ReplenishFruit : IClockEvent
        {
        private readonly RandomFruitDistribution _fruitDistList;
        private readonly GameState _gameState;
        private readonly Dictionary<FruitType, bool[]> _population;
        private readonly Dictionary<Fruit, int> _reverseLookup;

        public ReplenishFruit(RandomFruitDistribution fruitDistList, GameState gameState)
            {
            this._fruitDistList = fruitDistList ?? throw new ArgumentNullException(nameof(fruitDistList));
            this._gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            this._population = new Dictionary<FruitType, bool[]>();
            var totalFruitCount = 0;
            foreach (var fd in fruitDistList.Definitions)
                {
                this._population.Add(fd.Type, new bool[fd.Quantity]);
                totalFruitCount += fd.Quantity;
                }
            this._reverseLookup = new Dictionary<Fruit, int>(totalFruitCount);
            gameState.GameObjectRemoved += OnRemoveFruit;
            }

        public void Update(int ticks)
            {
            if (GlobalServices.Randomness.Next(4) == 0)
                return;

            foreach (var fd in this._fruitDistList.Definitions)
                {
                int slotNumber = (ticks % fd.Quantity);
                AddFruit(slotNumber, this._fruitDistList.Area, fd.Type, fd.Energy);
                }
            }

        private void AddFruit(int slotNumber, Rectangle area, FruitType fruitType, int energy)
            {
            if (!this._population.TryGetValue(fruitType, out var population))
                throw new InvalidOperationException("Population dictionary does not contain an entry for " + fruitType);

            ref bool populated = ref population[slotNumber];
            if (populated)
                return;

            var rnd = GlobalServices.Randomness;
            var tp = new TilePos(area.X + rnd.Next(area.Width), area.Y + rnd.Next(area.Height));
            if (this._gameState.GetItemsOnTile(tp).Any())
                return;

            // Don't put a new fruit near to the player.
            // This differs from the original game's methodology which would not place it in the same room as the player or the previous room that the player had visited.
            if (Math.Abs(tp.X - this._gameState.Player.TilePosition.X) < Constants.RoomSizeInTiles.X
                && Math.Abs(tp.Y - this._gameState.Player.TilePosition.Y) < Constants.RoomSizeInTiles.Y)
                {
                return;
                }

            var fruit = GlobalServices.GameState.AddFruit(tp.ToPosition(), fruitType, energy);
            populated = true;
            this._reverseLookup.Add(fruit, slotNumber);
            }

        private void OnRemoveFruit(object? sender, GameObjectEventArgs args)
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