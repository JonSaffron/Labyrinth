using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public class PlayerStartState : IHasArea
        {
        public Rectangle Area { get; set; }
        public int Id { get; }
        public bool IsInitialArea { get; }
        public readonly TilePos Position;
        public readonly int Energy;

        public static PlayerStartState FromXml(XmlElement startPos)
            {
            int id = int.Parse(startPos.GetAttribute("Id"));

            string worldStart = startPos.GetAttribute("WorldStart");
            var isInitialArea = !string.IsNullOrWhiteSpace(worldStart) && XmlConvert.ToBoolean(worldStart);

            int x = int.Parse(startPos.GetAttribute("Left"));
            int y = int.Parse(startPos.GetAttribute("Top"));
            var tp = new TilePos(x, y);

            var e = int.Parse(startPos.GetAttribute("Energy"));
            var result = new PlayerStartState(id, isInitialArea, tp, e);

            return result;
            }

        public PlayerStartState(int id, bool isInitialArea, TilePos position, int energy)
            {
            this.Id = id;
            this.IsInitialArea = isInitialArea;
            this.Position = position;
            this.Energy = energy;
            }
        }
    }
