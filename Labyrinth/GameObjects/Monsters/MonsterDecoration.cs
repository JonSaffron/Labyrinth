using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class MonsterDecoration : IMonster
        {
        [NotNull] private readonly IMonster _monster;

        public MonsterDecoration([NotNull] IMonster monster)
            {
            this._monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public Vector2 Position => this._monster.Position;

        public TilePos TilePosition => this._monster.TilePosition;

        public Rectangle BoundingRectangle => this._monster.BoundingRectangle;

        public int Energy => this._monster.Energy;

        public bool IsExtant => this._monster.IsExtant;

        public ObjectSolidity Solidity => this._monster.Solidity;

        public int DrawOrder => this._monster.DrawOrder;

        public void ReduceEnergy(int energyToRemove)
            {
            this._monster.ReduceEnergy(energyToRemove);
            }

        public void InstantlyExpire()
            {
            this._monster.InstantlyExpire();
            }

        public void Draw(GameTime gt, ISpriteBatch spriteBatch)
            {
            this._monster.Draw(gt, spriteBatch);
            }

        public PropertyBag Properties => this._monster.Properties;

        public bool HasBehaviour<T>() where T : IBehaviour
            {
            return this._monster.HasBehaviour<T>();
            }

        public bool IsStationary => this._monster.IsStationary;

        public bool IsEgg => this._monster.IsEgg;
        }
    }
