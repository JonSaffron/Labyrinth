using System.Xml;

namespace Labyrinth.Services.WorldBuilding
    {
    public class PlayerStartState
        {
        public readonly TilePos Position;
        public readonly int Energy;

        public static PlayerStartState FromXml(XmlElement startPos)
            {
            int x = int.Parse(startPos.GetAttribute("Left"));
            int y = int.Parse(startPos.GetAttribute("Top"));
            var tp = new TilePos(x, y);
            var e = int.Parse(startPos.GetAttribute("Energy"));
            var result = new PlayerStartState(tp, e);
            return result;
            }

        public PlayerStartState(TilePos position, int energy)
            {
            this.Position = position;
            this.Energy = energy;
            }
        }
    }
