using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Labyrinth.GameObjects.Behaviour;
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
            
            var breedInfo = Breeds[breed];

            var pathToTexture = "Sprites/Monsters/" + breedInfo.Texture;
            result.SetNormalAnimation(Animation.LoopingAnimation(pathToTexture, breedInfo.BaseMovementsPerFrame));

            var inherentBehaviours = breedInfo.InherentBehaviours;
            foreach (var behaviourName in inherentBehaviours)
                {
                string typeName = "Labyrinth.GameObjects.Behaviour." + behaviourName;
                Type behaviourType = Type.GetType(typeName);
                if (behaviourType == null || !behaviourType.GetInterfaces().Contains(typeof(IBehaviour)))
                    {
                    throw new InvalidOperationException("Could not find behaviour Type " + behaviourName);
                    }
                var constructorInfo = behaviourType.GetConstructor(new[] {typeof (Monster)});
                if (constructorInfo == null)
                    throw new InvalidOperationException("Failed to get matching constructor information for " + behaviourName + " class.");
                var constructorArguments = new object[] {result};
                var behaviour = (IBehaviour) constructorInfo.Invoke(constructorArguments);

                result.Behaviours.Add(behaviour);
                }

            var movement = breedInfo.BreedMovement;
            if (movement.ChangeRooms.HasValue)
                {
                result.ChangeRooms = movement.ChangeRooms.Value;
                }
            if (movement.Speed.HasValue)
                {
                result.CurrentSpeed = (int) (Constants.BaseSpeed * movement.Speed.Value);
                }

            foreach (var move in breedInfo.BreedMovement.Moves)
                {
                string typeName = "Labyrinth.GameObjects.Movement." + move.Value;
                Type movementType = Type.GetType(typeName);
                if (movementType == null || !movementType.GetInterfaces().Contains(typeof(IMonsterMotion)))
                    {
                    throw new InvalidOperationException("Could not find movement Type" + move.Value);
                    }

                result.MovementMethods.Add(move.Key, movementType);
                }
            if (movement.DefaultMobility.HasValue)
                {
                result.Mobility = movement.DefaultMobility.Value;
                }

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
            foreach (XmlElement breedElement in xmlRoot.SelectNodesEx("ns:Breed", xnm))
                {
                var breed = new Breed
                    {
                    Name = breedElement.GetAttribute("Name"),
                    Texture = breedElement.GetAttribute("Texture"),
                    BaseMovementsPerFrame = int.Parse(breedElement.GetAttribute("BaseMovementsPerFrame"))
                    };

                XmlElement inherentBehaviours = (XmlElement) breedElement.SelectSingleNode("ns:InherentBehaviours", xnm);
                if (inherentBehaviours != null)
                    {
                    foreach (XmlElement behaviour in inherentBehaviours.ChildNodes)
                        {
                        breed.InherentBehaviours.Add(behaviour.LocalName);
                        }
                    }

                XmlElement inherentProperties = (XmlElement) breedElement.SelectSingleNode("ns:InherentProperties", xnm);
                if (inherentProperties != null)
                    {
                    foreach (XmlElement property in inherentProperties.ChildNodes)
                        {
                        var propertyValue = property.GetAttribute("value");
                        AddToPropertyBag(breed.InherentProperties, property.Name, propertyValue);
                        }
                    }

                XmlElement movement = (XmlElement) breedElement.SelectSingleNode("ns:Movement", xnm);
                if (movement != null)
                    {
                    var defaultMobility = movement.GetAttribute("Default");
                    if (!string.IsNullOrWhiteSpace(defaultMobility)) 
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

                    XmlElement movesElement = (XmlElement) movement.SelectSingleNode("ns:Movement", xnm);
                    if (movesElement != null)
                        {
                        foreach (XmlElement moveElement in movesElement.SelectNodesEx("ns:Move", xnm))
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

        private static void AddToPropertyBag(PropertyBag propertyBag, string propertyName, string propertyValue)
            {
            Type gameObjectPropertiesType = typeof(GameObjectProperties);
            var fieldInfo = gameObjectPropertiesType.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
                {
                throw new InvalidOperationException("Failed to get property definition for " + propertyName);
                }

            object propertyDef = fieldInfo.GetValue(null);
            Type propertyType = propertyDef.GetType().GetGenericArguments()[0];
            if (!propertyType.IsEnum)
                {
                throw new InvalidOperationException($"Property definition {propertyName} has type {propertyType.FullName} which isn't supported.");
                }

            MethodInfo methodInfo = typeof(PropertyBag).GetMethod("Set")?.MakeGenericMethod(propertyType);
            if (methodInfo == null)
                {
                throw new InvalidOperationException("Failed to get generic PropertyBag method definition");
                }

            object value = Enum.Parse(propertyType, propertyValue);
            methodInfo.Invoke(propertyBag, new[] {propertyDef, value});
            }

        private class Breed
            {
            public string Name;
            public string Texture;
            public int BaseMovementsPerFrame;
            public readonly HashSet<string> InherentBehaviours = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly PropertyBag InherentProperties = new PropertyBag();
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
