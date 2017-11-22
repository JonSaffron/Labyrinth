using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    class RandomMonsterDistribution : IHasArea
        {
        public Rectangle Area { get; set; }
        public DiceRoll DiceRoll { get; set; }
        public int CountOfMonsters { get; set; }

        public Dictionary<int, MonsterDef> Templates { get; } = new Dictionary<int, MonsterDef>();

        public static RandomMonsterDistribution FromXml(XmlElement node, XmlNamespaceManager xnm)
            {
            var result = new RandomMonsterDistribution
                {
                DiceRoll = new DiceRoll(node.GetAttribute("DiceToRoll")),
                CountOfMonsters = int.Parse(node.GetAttribute("CountOfMonsters"))
                };

            // ReSharper disable once PossibleNullReferenceException
            foreach (XmlElement mdef in node.SelectNodes("ns:MonsterTemplates/ns:Monster", xnm))
                {
                var md = MonsterDef.FromXml(mdef);
                int matchingDiceRoll = int.Parse(mdef.GetAttribute("MatchingDiceRoll"));
                result.Templates.Add(matchingDiceRoll, md);
                }

            return result;
            }
        }
    }
