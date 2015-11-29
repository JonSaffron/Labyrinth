namespace Labyrinth.Services.WorldBuilding
    {
    class TileUsage
        {
        private TileTypeByMap _tileTypeByMap;
        private TileTypeByData _tileTypeByData;
        private readonly char _symbol;

        public TileUsage(TileTypeByMap tileTypeByMap)
            {
            this._tileTypeByMap = tileTypeByMap;
            this._tileTypeByData = TileTypeByData.Free;
            this._symbol = ' ';
            }
        
        public TileUsage(char symbol) : this(TileTypeByMap.PotentiallyOccupied)
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
                bool result = (this._tileTypeByMap == TileTypeByMap.Floor) && (this._tileTypeByData == TileTypeByData.Free);
                return result;
                }
            }
        }
    }
