using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    struct Tile
        {
        public readonly Texture2D Floor;

        private TileTypeByMap _tileTypeByMap;
        private TileTypeByData _tileTypeByData;
        private readonly char _symbol;

        public const int Width = 32;
        public const int Height = 32;
        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D floor, TileTypeByMap tileTypeByMap)
            {
            this.Floor = floor;
            this._tileTypeByMap = tileTypeByMap;
            this._tileTypeByData = TileTypeByData.Free;
            this._symbol = ' ';
            }
        
        public Tile(Texture2D floor, char symbol) : this(floor, TileTypeByMap.PotentiallyOccupied)
            {
            this._symbol = symbol;
            }

        public void SetOccupationByMovingMonster()
            {
            if (this._tileTypeByMap == TileTypeByMap.Wall)
                throw new TileException(TileExceptionType.Error, "Wall tile cannot be occupied.");
            if (this._tileTypeByData == TileTypeByData.Free)
                this._tileTypeByData = TileTypeByData.Moving;
            if (this._tileTypeByMap == TileTypeByMap.Floor)
                {
                this._tileTypeByMap = TileTypeByMap.PotentiallyOccupied;
                throw new TileException(TileExceptionType.Warning, "Tile is not marked as potentially occupied.");
                }
            }

        public void SetOccupationByStaticItem()
            {
            if (this._tileTypeByMap == TileTypeByMap.Wall)
                throw new TileException(TileExceptionType.Error, "Wall tile cannot be occupied.");
            if (this._tileTypeByData == TileTypeByData.Static)
                throw new TileException(TileExceptionType.Error, "Tile is already occupied by a static item.");
            this._tileTypeByData = TileTypeByData.Static;
            if (this._tileTypeByMap == TileTypeByMap.Floor)
                {
                this._tileTypeByMap = TileTypeByMap.PotentiallyOccupied;
                throw new TileException(TileExceptionType.Warning, "Tile is not marked as potentially occupied.");
                }
            }

        public void SetOccupationByFruit()
            {
            if (this._tileTypeByMap == TileTypeByMap.Wall)
                throw new TileException(TileExceptionType.Error, "Wall tile cannot be occupied.");
            if (this._tileTypeByData == TileTypeByData.Static)
                throw new TileException(TileExceptionType.Error, "Tile is already occupied by a static item.");
            this._tileTypeByData = TileTypeByData.Static;
            if (this._tileTypeByMap == TileTypeByMap.Floor)
                {
                this._tileTypeByMap = TileTypeByMap.PotentiallyOccupied;
                }
            }

        public void CheckIfIncorrectlyPotentiallyOccupied()
            {
            if (this._tileTypeByMap == TileTypeByMap.PotentiallyOccupied && this._tileTypeByData == TileTypeByData.Free)
                {
                this._tileTypeByMap = TileTypeByMap.Floor;
                throw new TileException(TileExceptionType.Warning, "Tile is marked as potentially occupied (symbol " + this._symbol + ") but is not occupied.");
                }
            if (this._tileTypeByMap == TileTypeByMap.Floor && this._tileTypeByData != TileTypeByData.Free)
                {
                this._tileTypeByMap = TileTypeByMap.PotentiallyOccupied;
                throw new TileException(TileExceptionType.Warning, "Tile is not marked as potentially occupied but is occupied.");
                }
            }

        public bool IsFree
            {
            get
                {
                bool result = (this._tileTypeByMap == TileTypeByMap.Floor)  && (this._tileTypeByData == TileTypeByData.Free);
                return result;
                }
            }

        //public TileCollision Collision
        //    {
        //    get
        //        {
        //        TileCollision result = this._tileTypeByMap == TileTypeByMap.Wall ? TileCollision.Impassable : TileCollision.Passable;
        //        return result;
        //        }
        //    }
        }
    }
