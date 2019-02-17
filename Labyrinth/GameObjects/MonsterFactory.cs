using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Labyrinth.GameObjects.Actions;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class MonsterFactory
        {
        private static readonly Dictionary<string, Breed> Breeds = BuildBreedDefinitions();

        public Monster BuildMonster(MonsterDef monsterDef)
            {

            // as was, we could instantiate the class which would apply the class defaults
            // these would then be overridden by the monsterDef

            var result = BuildMonsterFromDefaults(monsterDef.Breed, monsterDef.Position, monsterDef.Energy);
            if (monsterDef.InitialDirection.HasValue)
                result.InitialDirection = monsterDef.InitialDirection.Value;
            if (monsterDef.Mobility.HasValue)
                result.Mobility = monsterDef.Mobility.Value;
            if (monsterDef.ChangeRooms.HasValue)
                result.ChangeRooms = monsterDef.ChangeRooms.Value;
            if (monsterDef.IsEgg.GetValueOrDefault() && monsterDef.TimeBeforeHatching.HasValue)
                result.SetDelayBeforeHatching(monsterDef.TimeBeforeHatching.Value);
            if (monsterDef.LaysMushrooms.HasValue)
                result.Behaviours.Set<LaysMushroom>(monsterDef.LaysMushrooms.Value);
            if (monsterDef.LaysEggs.HasValue)
                result.Behaviours.Set<LaysEgg>(monsterDef.LaysEggs.Value);
            if (monsterDef.SplitsOnHit.HasValue)
                result.Behaviours.Set<SpawnsUponDeath>(monsterDef.SplitsOnHit.Value);
            if (monsterDef.ShootsAtPlayer.HasValue)
                result.SetShootsAtPlayer(monsterDef.ShootsAtPlayer.Value);
            if (monsterDef.ShootsOnceProvoked.HasValue)
                result.Behaviours.Set<StartsShootingWhenHurt>(monsterDef.ShootsOnceProvoked.Value);
            if (monsterDef.ShotsBounceOff.HasValue)
                result.ShotsBounceOff = monsterDef.ShotsBounceOff.Value;
            if (monsterDef.IsActive.HasValue)
                result.IsActive = monsterDef.IsActive.Value;
            if (!result.IsActive)
                {
                result.Behaviours.Add<ActivateWhenHurt>();
                }

            return result;
            }


        private Monster BuildMonsterFromDefaults(string breed, Vector2 position, int energy)
            {
            var animationPlayer = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var result = new Monster(breed, animationPlayer, position, energy);
            // todo apply inherent defaults and movement implementations
            return result;
            }

        private static Dictionary<string, Breed> BuildBreedDefinitions()
            {
            string worldDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/StandingData");

            string pathToWorld = worldDirectory + "/Monsters.xml";
            if (!File.Exists(pathToWorld))
                throw new ArgumentOutOfRangeException(pathToWorld);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(pathToWorld);

            var validator = new WorldValidator();
            var pathToXsd = worldDirectory + "/MonstersSchema.xsd";
            validator.Validate(xmlDoc.OuterXml, pathToXsd);

            var xmlRoot = xmlDoc.DocumentElement;
            if (xmlRoot == null)
                throw new InvalidOperationException("Empty xml document.");
            var xnm = new XmlNamespaceManager(xmlDoc.NameTable);
            xnm.AddNamespace("ns", "http://JonSaffron/Labyrinth/Monsters");

            var result = new Dictionary<string, Breed>();
            foreach (XmlElement breedElement in xmlRoot.SelectNodes("ns:Breed", xnm))
                {
                var breed = new Breed();
                breed.Name = breedElement.GetAttribute("Name");
                breed.Texture = breedElement.GetAttribute("Texture");
                breed.BaseMovementsPerFrame = int.Parse(breedElement.GetAttribute("BaseMovementsPerFrame"));
                XmlElement inherentBehaviours = (XmlElement) breedElement.SelectSingleNode("ns:InherentBehaviours", xnm);
                if (inherentBehaviours != null)
                    {
                    foreach (XmlElement behaviour in inherentBehaviours.ChildNodes)
                        {
                        breed.InherentBehaviours.Add(behaviour.LocalName);
                        }
                    }
                XmlElement movement = (XmlElement) breedElement.SelectSingleNode("ns:Movement", xnm);
                if (movement != null)
                    {
                    var defaultMobility = movement.GetAttribute("DefaultMobility");
                    if (!string.IsNullOrWhiteSpace( defaultMobility)) 
                        {
                        breed.BreedMovement.DefaultMobility = (MonsterMobility) Enum.Parse(typeof(MonsterMobility), defaultMobility);
                        }
                    var changeRooms = movement.GetAttribute("ChangeRooms");
                    if (!string.IsNullOrWhiteSpace(changeRooms))
                        {
                        breed.BreedMovement.ChangeRooms = (ChangeRooms) Enum.Parse(typeof(ChangeRooms), changeRooms);
                        }
                    var speed = movement.GetAttribute("Speed");
                    if (!string.IsNullOrWhiteSpace(speed))
                        {
                        breed.BreedMovement.Speed = decimal.Parse(speed);
                        }

                    var movesElement = movement.SelectSingleNode("ns:Movement", xnm);
                    if (movesElement != null)
                        {
                        foreach (XmlElement moveElement in movesElement.SelectNodes("ns:Move", xnm))
                            {
                            var type = (MonsterMobility) Enum.Parse(typeof(MonsterMobility), moveElement.GetAttribute("Type"));
                            var implementation = moveElement.GetAttribute("Implementation");
                            breed.BreedMovement.Moves.Add(type, implementation);
                            }
                        }
                    }

                result.Add(breed.Name, breed);
                }

            return result;
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
