using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class RandomMonsterDistribution : IHasArea
        {
        public Rectangle Area { get; set; }
        public DiceRoll DiceRoll { get; private set; }
        public int CountOfMonsters { get; private set; }

        public Dictionary<int, MonsterDef> Templates { get; } = new Dictionary<int, MonsterDef>();

        public static RandomMonsterDistribution FromXml(XmlElement node, XmlNamespaceManager xnm)
            {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (xnm == null) throw new ArgumentNullException(nameof(xnm));
            var result = new RandomMonsterDistribution
                {
                DiceRoll = new DiceRoll(node.GetAttribute("DiceToRoll")),
                CountOfMonsters = int.Parse(node.GetAttribute("CountOfMonsters"))
                };

            foreach (XmlElement mDef in node.SelectNodes("ns:MonsterTemplates/ns:Monster", xnm)!)
                {
                var md = MonsterDef.FromXml(mDef, xnm);
                int matchingDiceRoll = int.Parse(mDef.GetAttribute("MatchingDiceRoll"));
                result.Templates.Add(matchingDiceRoll, md);
                }

            return result;
            }
        }
    }
