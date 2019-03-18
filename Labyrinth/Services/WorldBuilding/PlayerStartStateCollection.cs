using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.DataStructures;
using JetBrains.Annotations;

namespace Labyrinth.Services.WorldBuilding
    {
    class PlayerStartStateCollection
        {
        public readonly Dictionary<int, PlayerStartState> StartStates = new Dictionary<int, PlayerStartState>();

        public void Add([NotNull] PlayerStartState pss)
            {
            if (pss == null) throw new ArgumentNullException(nameof(pss));
            this.StartStates.Add(pss.Id, pss);
            }

        public IList<PlayerStartState> Values => this.StartStates.Values.ToList();

        public bool TryGetStartState(TilePos tp, out PlayerStartState pss)
            {
            pss = this.StartStates.Values.SingleOrDefault(item => item.Area.ContainsTile(tp));
            return pss != null;
            }

        public void Clear()
            {
            this.StartStates.Clear();
            }
        }
    }
