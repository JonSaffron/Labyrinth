using System;
using System.Collections.Generic;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.GameObjects
    {
    class MonsterFactory
        {
        private readonly static Dictionary<string, Breed> breeds = BuildBreedDefinitions();

        public Monster BuildMonster(MonsterDef monsterDef)
            {
            // as was, we could instantiate the class which would apply the class defaults
            // these would then be overridden by the monsterDef

            return null;
            }

        private static Dictionary<string, Breed> BuildBreedDefinitions()
            {
            return null;
            }

        private class Breed
            {
            public string Name;
            public string Texture;
            public int BaseMovementsPerFrame;
            public readonly HashSet<string> InherentBehaviours = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly BreedMovement BreedMovement = new BreedMovement();
            }

        private class BreedMovement
            {
            public MonsterMobility? DefaultMobility;
            public ChangeRooms? ChangeRooms;
            public decimal? Speed;
            public readonly Dictionary<MonsterMobility, string> Moves = new Dictionary<MonsterMobility, string>();
            }
        }
    }
