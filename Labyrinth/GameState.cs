using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameState
        {
        private readonly IGameObjectCollection _gameObjectCollection;

        public Player Player { get; private set; }

        public GameState(IGameObjectCollection gameObjectCollection)
            {
            this._gameObjectCollection = gameObjectCollection ?? throw new ArgumentNullException(nameof(gameObjectCollection));
            }

         /// <summary>
        /// Returns a list of all extant game objects that could interact with other game objects
        /// </summary>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<MovingItem> GetSurvivingInteractiveItems()
            {
            List<MovingItem> itemsToRemove = null;

            foreach (var item in this._gameObjectCollection.InteractiveGameItems)
                {
                if (item.IsExtant)
                    {
                    yield return item;
                    continue;
                    }

                if (item is Player) 
                    continue;

                if (itemsToRemove == null)
                    itemsToRemove = new List<MovingItem> {item};
                else
                    itemsToRemove.Add(item);
                }

            if (itemsToRemove == null)
                yield break;

            foreach (var item in itemsToRemove)
                {
                this._gameObjectCollection.Remove(item);
                }
            }

        /// <summary>
        /// Returns a list of all game objects of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<T> DistinctItemsOfType<T>() where T: StaticItem
            {
            var result = this._gameObjectCollection.DistinctItems().OfType<T>();
            return result;
            }

        /// <summary>
        /// Returns a list of all extant game objects that are located on the specified tile
        /// </summary>
        /// <param name="tp">Specifes the tile position to inspect</param>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<StaticItem> GetItemsOnTile(TilePos tp)
            {
            var listOfItems = this._gameObjectCollection.ItemsAtPosition(tp);
            var result = listOfItems.Where(gi => gi.IsExtant && gi.TilePosition == tp);
            return result;
            }

        /// <summary>
        /// Retrieves all game objects within the specified rectangle
        /// </summary>
        /// <param name="tr">Top left starting point for the rectangle</param>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<StaticItem> AllItemsInRectangle(TileRect tr)
            {
            for (int j = 0; j < tr.Height; j++)
                {
                int y = tr.TopLeft.Y + j;
                for (int i = 0; i < tr.Width; i++)
                    {
                    int x = tr.TopLeft.X + i;

                    var listOfItems = this.GetItemsOnTile(new TilePos(x, y));
                    if (listOfItems == null)
                        continue;

                    var copyOfList = listOfItems.ToArray();
                    foreach (var item in copyOfList)
                        yield return item;
                    }
                }
            }

        /// <summary>
        /// Removes all bangs and shots from the game object collection
        /// </summary>
        public void RemoveBangsAndShots()
            {
            foreach (var item in this._gameObjectCollection.DistinctItems())
                {
                if (item is Bang || item is Shot)
                    {
                    item.InstantlyExpire();
                    }
                }
            }

        /// <summary>
        /// Returns whether there are any shots currently in the collection
        /// </summary>
        /// <returns>True if a shot exits in the collection, otherwise False.</returns>
        public bool DoesShotExist()
            {
            var result = (this._gameObjectCollection.CountOfShots > 0);
            return result;
            }

        /// <summary>
        /// Tests whether any non-moving items are currently occupying the specified position
        /// </summary>
        /// <remarks>This is to be used when looking to place another non-moving item</remarks>
        /// <param name="tp">The tile position to test</param>
        /// <returns>True if there is already a static item occupying the specified position</returns>
        public bool IsStaticItemOnTile(TilePos tp)
            {
            var objectsAtPosition = this.GetItemsOnTile(tp);
            var result = objectsAtPosition.Any(gi => gi.Solidity != ObjectSolidity.Insubstantial);
            return result;
            }

        /// <summary>
        /// Tests whether an impassable object is occupying the specified position
        /// </summary>
        /// <remarks>This is to be used when looking for a clear path or line of sight</remarks>
        /// <param name="tp">The tile position to test</param>
        /// <returns>True if there is an impassable object occupying the specified position</returns>
        public bool IsImpassableItemOnTile(TilePos tp)
            {
            var objectsAtPosition = this.GetItemsOnTile(tp);
            var result = objectsAtPosition.Any(gi => gi.Solidity == ObjectSolidity.Impassable);
            return result;
            }

        public void UpdatePosition(MovingItem gameObject)
            {
            this._gameObjectCollection.UpdatePosition(gameObject);
            }

        /// <summary>
        /// Place short bang at a shot's position and remove the shot
        /// </summary>
        /// <param name="s">An instance of a shot to convert</param>
        public Bang ConvertShotToBang(Shot s)
            {
            var result = AddBang(s.Position, BangType.Short);
            s.InstantlyExpire();
            return result;
            }
        
        public void AddDiamondDemon(Vector2 p)
            {
            MonsterDef md = new MonsterDef
                {
                Type = typeof(DiamondDemon),
                Position = p,
                Energy = 30,
                Mobility = MonsterMobility.Aggressive,
                LaysEggs = true,
                ChangeRooms = ChangeRooms.FollowsPlayer,
                ShootsAtPlayer = true
                };
            AddMonster(md);
            }

        /// <summary>
        /// Add bang
        /// </summary>
        /// <param name="p">Position to place the sprite</param>
        /// <param name="bangType">The type of bang to create</param>
        public Bang AddBang(Vector2 p, BangType bangType)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var b = new Bang(ap, p, bangType);
            this._gameObjectCollection.Add(b);
            return b;
            }

        public Bang AddBang(Vector2 p, BangType bangType, GameSound gameSound)
            {
            var b = AddBang(p, bangType);
            b.PlaySound(gameSound);
            return b;
            }

        public Grave AddGrave(TilePos tp)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var g = new Grave(ap, tp.ToPosition());
            this._gameObjectCollection.Add(g);
            return g;
            }
        
        public void AddMushroom(TilePos tp)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var m = new Mushroom(ap, tp.ToPosition());
            this._gameObjectCollection.Add(m);
            }
        
        public Crystal AddCrystal(Vector2 position, int id, int score, int energy)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var crystal = new Crystal(ap, position, id, score, energy);
            this._gameObjectCollection.Add(crystal);
            return crystal;
            }

        public void AddExplosion(Vector2 position, int energy)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var e = new Explosion(ap, position, energy);
            this._gameObjectCollection.Add(e);
            }
        
        public void AddMine(Vector2 position)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var m = new Mine(ap, position);
            this._gameObjectCollection.Add(m);
            }

        public Monster AddMonster(MonsterDef monsterDef)
            {
            var animationPlayer = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var constructorInfo = monsterDef.Type.GetConstructor(new[] {typeof (AnimationPlayer), typeof (Vector2), typeof (int)});
            if (constructorInfo == null)
                throw new InvalidOperationException("Failed to get matching constructor information for " + monsterDef.MonsterType + " object.");
            var constructorArguments = new object[] {animationPlayer, monsterDef.Position, monsterDef.Energy};
            var result = (Monster) constructorInfo.Invoke(constructorArguments);
            if (monsterDef.InitialDirection.HasValue)
                result.InitialDirection = monsterDef.InitialDirection.Value;
            if (monsterDef.Mobility.HasValue)
                result.Mobility = monsterDef.Mobility.Value;
            if (monsterDef.ChangeRooms.HasValue)
                result.ChangeRooms = monsterDef.ChangeRooms.Value;
            if (monsterDef.IsEgg.GetValueOrDefault() && monsterDef.TimeBeforeHatching.HasValue)
                result.SetDelayBeforeHatching(monsterDef.TimeBeforeHatching.Value);
            if (monsterDef.LaysMushrooms.HasValue)
                result.LaysMushrooms = monsterDef.LaysMushrooms.Value;
            if (monsterDef.LaysEggs.HasValue)
                result.LaysEggs = monsterDef.LaysEggs.Value;
            if (monsterDef.SplitsOnHit.HasValue)
                result.SplitsOnHit = monsterDef.SplitsOnHit.Value;
            if (monsterDef.ShootsAtPlayer.HasValue)
                result.ShootsAtPlayer = monsterDef.ShootsAtPlayer.Value;
            if (monsterDef.ShootsOnceProvoked.HasValue)
                result.ShootsOnceProvoked = monsterDef.ShootsOnceProvoked.Value;
            if (monsterDef.ShotsBounceOff.HasValue)
                result.ShotsBounceOff = monsterDef.ShotsBounceOff.Value;
            if (monsterDef.IsActive.HasValue)
                result.IsActive = monsterDef.IsActive.Value;

            this._gameObjectCollection.Add(result);
            return result;
            }

        public StandardShot AddStandardShot(Vector2 startPos, Direction direction, int energy, StaticItem originator)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var shot = new StandardShot(ap, startPos, direction, energy, originator);
            this._gameObjectCollection.Add(shot);
            return shot;
            }

        public Player AddPlayer(Vector2 position, int energy, int? initialWorldAreaId)
            {
            if (this.Player != null)
                throw new InvalidOperationException("Cannot add more than one Player.");
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Player(ap, position, energy, initialWorldAreaId);
            this._gameObjectCollection.Add(result);
            this.Player = result;
            return result;
            }

        public Wall AddWall(Vector2 position, string textureName)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Wall(ap, position, textureName);
            this._gameObjectCollection.Add(result);
            return result;
            }

        public Boulder AddBoulder(Vector2 position)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Boulder(ap, position);
            this._gameObjectCollection.Add(result);
            return result;
            }

        public ForceField AddForceField(Vector2 position, int crystalRequired)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new ForceField(ap, position, crystalRequired);
            this._gameObjectCollection.Add(result);
            return result;
            }

        public CrumblyWall AddCrumblyWall(Vector2 position, string textureName, int energy)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new CrumblyWall(ap, position, textureName, energy);
            this._gameObjectCollection.Add(result);
            return result;
            }

        public Fruit AddFruit(Vector2 position, FruitType fruitType, int energy)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Fruit(ap, position, fruitType, energy);
            this._gameObjectCollection.Add(result);
            return result;
            }

        public TileReservation AddTileReservation(Vector2 position)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new TileReservation(ap, position);
            this._gameObjectCollection.Add(result);
            return result;
            }
        }
    }
