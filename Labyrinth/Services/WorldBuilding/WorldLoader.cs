using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Messages;

namespace Labyrinth.Services.WorldBuilding
    {
    public class WorldLoader : IWorldLoader
        {
        private XmlElement _xmlRoot;
        private XmlNamespaceManager _xnm;

        [NotNull] private readonly PlayerStartStateCollection _playerStartStates = new PlayerStartStateCollection();
        [NotNull] private readonly List<TileDefinitionCollection> _tileDefinitionCollections = new List<TileDefinitionCollection>();
        [NotNull] private readonly List<RandomMonsterDistribution> _randomMonsterDistributions = new List<RandomMonsterDistribution>();
        [NotNull] private readonly List<RandomFruitDistribution> _randomFruitDistributions = new List<RandomFruitDistribution>();

        public TilePos WorldSize { get; private set; }

        public void LoadWorld(string levelName)
            {
            OnProgress("Loading and validating world design");

            string worldDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Worlds");

            string pathToWorld = $"{worldDirectory}/{levelName}.xml";
            if (!File.Exists(pathToWorld))
                throw new ArgumentOutOfRangeException(pathToWorld);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(pathToWorld);

            var validator = new WorldValidator();
            var pathToXsd = worldDirectory + "/WorldSchema.xsd";
            validator.Validate(xmlDoc.OuterXml, pathToXsd);

            this._xmlRoot = xmlDoc.DocumentElement;
            if (this._xmlRoot == null)
                throw new InvalidOperationException("Empty xml document.");
            this._xnm = new XmlNamespaceManager(xmlDoc.NameTable);
            this._xnm.AddNamespace("ns", "http://JonSaffron/Labyrinth");

            int width = int.Parse(this._xmlRoot.GetAttribute("Width"));
            int height = int.Parse(this._xmlRoot.GetAttribute("Height"));
            this.WorldSize = new TilePos(width, height);

            LoadAreas();
            ValidateAreas();
            }

        public Tile[,] FloorTiles
            {
            get
                {
                var result = GetFloorLayout(this._playerStartStates);
                return result;
                }
            }

        public bool RestartInSameRoom
            {
            get
                {
                var text = this._xmlRoot.GetAttribute("RestartInSameRoom");
                if (string.IsNullOrWhiteSpace(text))
                    return false;
                bool result = XmlConvert.ToBoolean(text);
                return result;
                }
            }

        public bool UnlockLevels
            {
            get
                {
                var text = this._xmlRoot.GetAttribute("UnlockLevels");
                if (string.IsNullOrWhiteSpace(text))
                    return false;
                bool result = XmlConvert.ToBoolean(text);
                return result;
                }
            }

        public Dictionary<int, PlayerStartState> PlayerStartStates => this._playerStartStates.StartStates;

        public List<RandomFruitDistribution> FruitDistributions => this._randomFruitDistributions;

        public void AddGameObjects(GameState gameState)
            {
            OnProgress("Adding game objects");

            var layout = GetLayout();

            var pgo = new ProcessGameObjects(gameState);
            pgo.AddWalls(this._tileDefinitionCollections, layout);
            pgo.AddPlayerAndStartPositions(this._playerStartStates.StartStates.Values);

            var objects = this._xmlRoot.SelectNodes(@"ns:Objects/ns:*", this._xnm);
            if (objects != null)
                {
                var objectList = objects.Cast<XmlElement>();
                pgo.AddGameObjects(objectList, this._xnm);
                }

            OnProgress("Validating layout against gamestate");
            ValidateGameState(ref layout, gameState);

            OnProgress("Adding random elements");
            pgo.AddMonstersFromRandomDistributions(this._randomMonsterDistributions);
            pgo.AddFruitFromRandomDistributions(this._randomFruitDistributions);
            }

        private string[] GetLayout()
            {
            var layoutDef = (XmlElement)this._xmlRoot.SelectSingleNode("ns:Layout", this._xnm);
            if (layoutDef == null)
                throw new InvalidOperationException("No Layout element found.");
            var layout = layoutDef.InnerText;
            var result = layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
            return result;
            }

        private void LoadAreas()
            {
            OnProgress("Building world areas");

            var areas = this._xmlRoot.SelectSingleNode("ns:Areas", this._xnm);
            if (areas == null)
                throw new InvalidOperationException();

            XmlNodeList areaList = areas.SelectNodes("ns:Area", this._xnm);
            if (areaList == null)
                throw new InvalidOperationException();

            this._playerStartStates.Clear();
            this._tileDefinitionCollections.Clear();
            this._randomMonsterDistributions.Clear();
            this._randomFruitDistributions.Clear();

            foreach (XmlElement area in areaList)
                {
                var areaRect = RectangleExtensions.GetRectangleFromDefinition(area);

                var startPos = (XmlElement) area.SelectSingleNode("ns:PlayerStartState", this._xnm);
                if (startPos != null)
                    {
                    var pss = PlayerStartState.FromXml(startPos);
                    pss.Area = areaRect;
                    this._playerStartStates.Add(pss);
                    }

                var tileDefinitions = area.SelectNodes("ns:TileDefinitions/ns:*", this._xnm);
                if (tileDefinitions != null && tileDefinitions.Count != 0)
                    {
                    var td = TileDefinitionCollection.FromXml(tileDefinitions);
                    td.Area = areaRect;
                    this._tileDefinitionCollections.Add(td);
                    }

                var fruitPopulation = (XmlElement) area.SelectSingleNode("ns:FruitDefinitions", this._xnm);
                if (fruitPopulation != null && fruitPopulation.ChildNodes.Count != 0)
                    {
                    var fd = RandomFruitDistribution.FromXml(fruitPopulation);
                    fd.Area = areaRect;
                    this._randomFruitDistributions.Add(fd);
                    }

                var randomMonsterDistribution = (XmlElement) area.SelectSingleNode("ns:RandomMonsterDistribution", this._xnm);
                if (randomMonsterDistribution != null)
                    {
                    var md = RandomMonsterDistribution.FromXml(randomMonsterDistribution, this._xnm);
                    md.Area = areaRect;
                    this._randomMonsterDistributions.Add(md);
                    }
                }
            }

        private void ValidateAreas()
            {
            CheckForOverlappingAreasOrAreasOutsideTheWorld(this._playerStartStates.Values, "player start state");
            CheckForOverlappingAreasOrAreasOutsideTheWorld(this._tileDefinitionCollections, "tile definitions");
            CheckForOverlappingAreasOrAreasOutsideTheWorld(this._randomMonsterDistributions, "random monster distribution");
            CheckForOverlappingAreasOrAreasOutsideTheWorld(this._randomFruitDistributions, "random fruit distribution");
            ValidatePlayerStartStates();
            }

        private void CheckForOverlappingAreasOrAreasOutsideTheWorld(IEnumerable<IHasArea> areaList, string areaType)
            {
            var l = areaList.ToList();
            var c = l.Count;
            for (int i = 0; i < c; i++)
                {
                var sourceArea = l[i].Area;
                if (sourceArea.Top < 0 || sourceArea.Left < 0 || sourceArea.Width > this.WorldSize.X || sourceArea.Height > this.WorldSize.Y)
                    {
                    throw new InvalidOperationException($"Area for {areaType} {sourceArea} extends beyond the size of the world.");
                    }

                for (int j = i + 1; j < c; j++)
                    {
                    var otherArea = l[j].Area;
                    if (sourceArea.Intersects(otherArea))
                        {
                        throw new InvalidOperationException($"Area for {areaType} {sourceArea} overlaps with {otherArea}.");
                        }
                    }
                }
            }

        private void ValidatePlayerStartStates()
            {
            foreach (var pss in this._playerStartStates.Values)
                {
                if (!pss.Area.ContainsTile(pss.Position))
                    throw new InvalidOperationException($"Invalid player start position - co-ordinate {pss.Position} is not within the area {pss.Area}.");
                }

            if (this._playerStartStates.StartStates.Values.Count(wa => wa.IsInitialArea) != 1)
                throw new InvalidOperationException("One and only one world player start state should be marked as the initial area.");
            }

        private Tile[,] GetFloorLayout(PlayerStartStateCollection playerStartStateCollection)
            {
            var layout = GetLayout();
            ValidateLayout(layout);

            var result = new Tile[this.WorldSize.X, this.WorldSize.Y];
            foreach (var tdc in this._tileDefinitionCollections)
                {
                string defaultFloorName = tdc.GetDefaultFloor();
                foreach (TilePos tp in tdc.Area.PointsInside())
                    {
                    char symbol = layout[tp.Y][tp.X];
                    var td = tdc[symbol];

                    var textureName =
                        td is TileFloorDefinition definition
                            ? definition.TextureName
                            : defaultFloorName;

                    if (!playerStartStateCollection.TryGetStartState(tp, out PlayerStartState pss))
                        throw new InvalidOperationException();
                    result[tp.X, tp.Y] = new Tile(textureName, pss.Id);
                    }
                }
            return result;
            }
            
        private void ValidateLayout(string[] lines)
            {
            TilePos size = this.WorldSize;
            int countOfLines = lines.GetLength(0);
            if (countOfLines != size.Y)
                throw new InvalidOperationException($"The world layout has {countOfLines} lines whilst the Height property equals {size.Y}. The two should match.");

            for (int y = 0; y < size.Y; y++)
                {
                var lineLength = lines[y].Length;
                if (lineLength != size.X)
                    throw new InvalidOperationException($"Line {y} has {lineLength} characters, whilst the Width property equals {size.X}. The two should match.");
                }
            }

        private void ValidateGameState(ref string[] layout, GameState gameState)
            {
            var tileUsage = BuildTileUsageByMap(ref layout);

            var issues = new List<string>();
            var cy = this.WorldSize.Y;
            var cx = this.WorldSize.X;
            for (int y = 0; y < cy; y++)
                {
                for (int x = 0; x < cx; x++)
                    {
                    var tp = new TilePos(x, y);
                    var hasItems = gameState.GetItemsOnTile(tp).Any();
                    TileUsage t = tileUsage[x, y];
                    if (t.TileTypeByMap == TileTypeByMap.Object && !hasItems)
                        {
                        issues.Add(tp + ": Map had tile marked as occupied by an object '" + t.Description + "', but nothing is there.");
                        }
                    else if (t.TileTypeByMap == TileTypeByMap.Floor && hasItems)
                        {
                        var items = gameState.GetItemsOnTile(tp).ToList();
                        issues.Add(tp + ": Map had tile marked as unoccupied, but contains " + items.Count +
                                   " item(s): " + string.Join(", ", items.Select(item => item.GetType().Name)) + ".");
                        }
                    }
                }

            if (issues.Count == 0)
                return;

            var message = string.Join("\r\n", issues);
            Trace.WriteLine(message);
            var dr = MessageBox.Show(message, "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr == DialogResult.Cancel)
                throw new InvalidOperationException(message);
            }

        private TileUsage[,] BuildTileUsageByMap(ref string[] layout)
            {
            var result = new TileUsage[this.WorldSize.X, this.WorldSize.Y];
            foreach (var tdc in this._tileDefinitionCollections)
                {
                foreach (TilePos tp in tdc.Area.PointsInside())
                    {
                    char symbol = layout[tp.Y][tp.X];
                    var tileDefinition = tdc[symbol];

                    TileUsage tu;
                    switch (tileDefinition)
                        {
                        case TileWallDefinition _:
                            tu = TileUsage.Wall(symbol);
                            break;
                        case TileFloorDefinition _:
                            tu = TileUsage.Floor(symbol);
                            break;
                        case TileObjectDefinition objectDef:
                            tu = TileUsage.Object(symbol, objectDef.Description);
                            break;
                        default:
                            throw new InvalidOperationException();
                        }

                    result[tp.X, tp.Y] = tu;
                    }
                }
            return result;
            }

        private void OnProgress(string message)
            {
            var msg = new WorldLoaderProgress(message);
            Messenger.Default.Send(msg);
            }
        }
    }
