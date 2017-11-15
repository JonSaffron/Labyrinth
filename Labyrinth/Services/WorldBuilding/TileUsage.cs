namespace Labyrinth.Services.WorldBuilding
    {
    struct TileUsage
        {
        public readonly TileTypeByMap TileTypeByMap;
        private readonly char _symbol;
        private readonly string _description;

        public static TileUsage Floor(char symbol)
            {
            return new TileUsage(symbol, TileTypeByMap.Floor);
            }

        public static TileUsage Wall(char symbol)
            {
            return new TileUsage(symbol, TileTypeByMap.Wall);
            }

        public static TileUsage Object(char symbol, string description)
            {
            return new TileUsage(symbol, description);
            }

        private TileUsage(char symbol, TileTypeByMap tileTypeByMap)
            {
            this.TileTypeByMap = tileTypeByMap;
            this._symbol = symbol;
            this._description = tileTypeByMap.ToString();
            }
        
        private TileUsage(char symbol, string description)
            {
            this._symbol = symbol;
            this._description = description;
            this.TileTypeByMap = TileTypeByMap.Object;
            }

        public string Description => $"'{this._symbol} {this._description}";
        }
    }
