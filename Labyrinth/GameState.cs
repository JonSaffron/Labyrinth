using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameState
        {
        private readonly IGameObjectCollection _gameObjectCollection;

        public Player Player { get; private set; }

        public GameState(IGameObjectCollection gameObjectCollection)
            {
            if (gameObjectCollection == null)
                throw new ArgumentNullException("gameObjectCollection");
            this._gameObjectCollection = gameObjectCollection;
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
                if (!item.IsExtant && !(item is Player))
                    {
                    if (itemsToRemove == null)
                        itemsToRemove = new List<MovingItem> { item };
                    else
                        itemsToRemove.Add(item);
                    continue;
                    }

                yield return item;
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
            if (!IsTileWithinWorld(tr.TopLeft) || tr.Width <= 0 || tr.Height <= 0 || !IsTileWithinWorld(new TilePos(tr.TopLeft.X + tr.Width - 1, tr.TopLeft.Y + tr.Height - 1)))
                throw new ArgumentOutOfRangeException("tr");

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
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the World and fall off the bottom.
        /// </summary>
        public bool IsTileWithinWorld(TilePos tp)
            {
            var x = tp.X;
            var y = tp.Y;
            var result = !(x < 0 || x >= this._gameObjectCollection.Width || y < 0 || y >= this._gameObjectCollection.Height);
            return result;
            }

        /// <summary>
        /// Tests whether the specified position can be occupied.
        /// </summary>
        /// <remarks>This is only to be used to determine whether moving objects can (temporarily) occupy the tile</remarks>
        /// <param name="tp">The tile position to test</param>
        /// <param name="treatMoveableItemsAsImpassable">If true then an item such as the boulder will be treated as impassable</param>
        /// <returns>True if the specified tile can be occupied</returns>
        public bool CanTileBeOccupied(TilePos tp, bool treatMoveableItemsAsImpassable)
            {
            if (!IsTileWithinWorld(tp))
                return false;

            var objectsAtPosition = this.GetItemsOnTile(tp);
            var isTileAlreadyOccupied = treatMoveableItemsAsImpassable
                ? objectsAtPosition.Any(gi => gi.Solidity == ObjectSolidity.Impassable || gi.Solidity == ObjectSolidity.Moveable)
                : objectsAtPosition.Any(gi => gi.Solidity == ObjectSolidity.Impassable);
            var result = !isTileAlreadyOccupied;
            return result;
            }

        /// <summary>
        /// Tests whether any non-moving items are currently occupying the specified position
        /// </summary>
        /// <remarks>This is only to be used when looking to place non-moving items</remarks>
        /// <param name="tp">The tile position to test</param>
        /// <returns>True if there is already a static item occupying the specified position</returns>
        public bool IsStaticItemOnTile(TilePos tp)
            {
            var objectsAtPosition = this.GetItemsOnTile(tp);
            var result = objectsAtPosition.Any(gi => gi.Solidity != ObjectSolidity.Insubstantial);
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
            var dd = Create("DiamondDemon", p, 30);
            dd.Mobility = MonsterMobility.Aggressive;
            dd.LaysEggs = true;
            dd.ChangeRooms = ChangeRooms.FollowsPlayer;
            dd.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
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
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var b = new Bang(ap, p, bangType);
            this._gameObjectCollection.Add(b);
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
        
        public void AddMonster(Monster m)
            {
            this._gameObjectCollection.Add(m);
            }
        
        public void AddShot(StandardShot s)
            {
            this._gameObjectCollection.Add(s);
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

        public Monster Create(string type, Vector2 position, int energy)
            {
            var animationPlayer = new AnimationPlayer(GlobalServices.SpriteLibrary);
            Monster monster;
            switch (type)
                {
                case "ThresherBrown": monster = new ThresherBrown(animationPlayer, position, energy); break;
                case "RotaFloaterBrown": monster = new RotaFloaterBrown(animationPlayer, position, energy); break;
                case "DeathCube": monster = new DeathCube(animationPlayer, position, energy); break;
                case "RotaFloaterCyan": monster = new RotaFloaterCyan(animationPlayer, position, energy); break;
                case "FlitterbugRed": monster = new FlitterbugRed(animationPlayer, position, energy); break;
                case "KillerCubeGreen": monster = new KillerCubeGreen(animationPlayer, position, energy); break;
                case "ThresherCyan": monster = new ThresherCyan(animationPlayer, position, energy); break;
                case "Butterfly": monster = new Butterfly(animationPlayer, position, energy); break;
                case "KillerCubeRed": monster = new KillerCubeRed(animationPlayer, position, energy); break;
                case "FlitterbugCyan": monster = new FlitterbugCyan(animationPlayer, position, energy); break;
                case "DiamondDemon": monster = new DiamondDemon(animationPlayer, position, energy); break;
                case "FlitterbugBrown": monster = new FlitterbugBrown(animationPlayer, position, energy); break;
                case "CrazyCrawler": monster = new CrazyCrawler(animationPlayer, position, energy); break;
                case "TigerMoth": monster = new TigerMoth(animationPlayer, position, energy); break;
                case "Joker": monster = new Joker(animationPlayer, position, energy); break;
                default: throw new InvalidOperationException("No handler exists for creating monster " + type);
                }
            this._gameObjectCollection.Add(monster);
            return monster;
            }

        public StandardShot AddStandardShot(Vector2 startPos, Direction direction, int energy, ShotType shotType)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var shot = new StandardShot(ap, startPos, direction, energy, shotType);
            this._gameObjectCollection.Add(shot);
            return shot;
            }

        public Player AddPlayer(Vector2 position, int energy)
            {
            if (this.Player != null)
                throw new InvalidOperationException("Cannot add more than one Player.");
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Player(ap, position, energy);
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

        public Fruit AddFruit(Vector2 position, FruitType fruitType)
            {
            var ap = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Fruit(ap, position, fruitType);
            this._gameObjectCollection.Add(result);
            return result;
            }
        }
    }
