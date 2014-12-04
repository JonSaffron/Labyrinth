using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class WorldArea : IEquatable<WorldArea>
        {
        private readonly Dictionary<char, TileDefinition> _tileDefs = new Dictionary<char, TileDefinition>();
        private readonly Dictionary<FruitType, FruitDefinition> _fruitDefs = new Dictionary<FruitType, FruitDefinition>();
        public StartState StartState { get; private set; }
        private int? _id; 
        public Rectangle Area { get; private set; }
        public bool IsInitialArea { get; private set; }
        
        public Dictionary<char, TileDefinition> TileDefinitions
            {
            get
                {
                return this._tileDefs;
                }
            }
        
        public Dictionary<FruitType, FruitDefinition> FruitDefinitions
            {
            get
                {
                return this._fruitDefs;
                }
            }      
        
        public static Rectangle GetRectangleFromDefinition(XmlElement area)
            {
            int x = int.Parse(area.GetAttribute("Left"));
            int y = int.Parse(area.GetAttribute("Top"));
            var width = area.GetAttribute("Width");
            var height = area.GetAttribute("Height");
            var right = area.GetAttribute("Right");
            var bottom = area.GetAttribute("Bottom");
            Rectangle result;
            if (!string.IsNullOrEmpty(width) && !string.IsNullOrEmpty(height) && string.IsNullOrEmpty(right) && string.IsNullOrEmpty(bottom))
                result = new Rectangle(x, y, int.Parse(width), int.Parse(height));
            else if (string.IsNullOrEmpty(width) && string.IsNullOrEmpty(height) && !string.IsNullOrEmpty(right) && !string.IsNullOrEmpty(bottom))
                result = RectangleExtensions.NewRectangle(x, y, int.Parse(right), int.Parse(bottom));
            else
                throw new InvalidOperationException();
            return result;
            }

        public bool HasId
            {
            get
                {
                return this._id.HasValue;
                }
            }
        
        public int Id
            {
            get
                {
                if (!this._id.HasValue)
                    throw new InvalidOperationException();
                return this._id.Value;
                }
            }

        public WorldArea(XmlElement area)
            {
            string id = area.GetAttribute("Id");
            if (!string.IsNullOrEmpty(id))
                this._id = int.Parse(id);
            
            this.Area = GetRectangleFromDefinition(area);
            
            if (area.GetAttribute("WorldStart") == "true")
                this.IsInitialArea = true;

            var tileDefinitions = (XmlElement) area.SelectSingleNode("TileDefinitions");
            if (tileDefinitions != null)
                LoadTileDefs(tileDefinitions);
            
            var fruitPopulation = (XmlElement) area.SelectSingleNode("FruitDefinitions");
            if (fruitPopulation != null)
                LoadFruitPopulation(fruitPopulation);
            
            var startPos = (XmlElement) area.SelectSingleNode("StartPos");
            if (startPos != null)
                LoadStartPos(startPos);
            }

        private void LoadTileDefs(XmlElement tileDefinitions)
            {
            XmlNodeList tiledefs = tileDefinitions.SelectNodes("TileDef");
            if (tiledefs == null)
                throw new InvalidOperationException();
            foreach (XmlElement tiledef in tiledefs)
                {
                string symbol = tiledef.GetAttribute("Symbol");
                string tileDefType = tiledef.GetAttribute("Type");
                string texture = tiledef.GetAttribute("Tile");
                var td = new TileDefinition(symbol, tileDefType, texture);
                this.TileDefinitions.Add(td.Symbol, td);
                }
            }
        
        private void LoadFruitPopulation(XmlElement fruitPopulation)
            {
            XmlNodeList fruitDefs = fruitPopulation.SelectNodes("FruitDef");
            if (fruitDefs == null)
                throw new InvalidOperationException();
            foreach (XmlElement fruitDef in fruitDefs)
                {
                var fruitType = (FruitType)Enum.Parse(typeof(FruitType), fruitDef.GetAttribute("Type"));
                int fruitQuantity = int.Parse(fruitDef.GetAttribute("Quantity"));
                var fd = new FruitDefinition(fruitType, fruitQuantity);
                this.FruitDefinitions.Add(fd.FruitType, fd);
                }
            }
        
        private void LoadStartPos(XmlElement startPos)
            {
            int px = int.Parse(startPos.GetAttribute("PlayerLeft"));
            int py = int.Parse(startPos.GetAttribute("PlayerTop"));
            var p = new TilePos(px, py);
            if (!this.Area.Contains(p))
                throw new InvalidOperationException();

            int startEnergy = int.Parse(startPos.GetAttribute("StartEnergy"));

            this.StartState = new StartState(p, startEnergy);
            }

        public bool Equals(WorldArea other)
            {
            if (other == null)
                return false;
            return this.Area == other.Area;
            }
        }
    }
