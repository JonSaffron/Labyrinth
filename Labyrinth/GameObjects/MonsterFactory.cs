﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class MonsterFactory
        {
        private static readonly Dictionary<string, Breed> Breeds = BuildBreedDefinitions();

        public IMonster BuildMonster(MonsterDef monsterDef)
            {
            if (monsterDef.IsEgg)
                {
                return BuildEgg(monsterDef);
                }

            var breedInfo = Breeds[monsterDef.Breed];
            var result = BuildMonsterFromBreed(breedInfo, monsterDef.Position, monsterDef.Energy);
            AddInherentBehaviours(result, breedInfo);
            AddMovements(result, breedInfo, monsterDef.InitialDirection.GetValueOrDefault(Direction.None));
            AddInherentProperties(result, breedInfo);

            if (monsterDef.Mobility.HasValue)
                result.Mobility = monsterDef.Mobility.Value;
            if (monsterDef.ChangeRooms.HasValue)
                result.ChangeRooms = monsterDef.ChangeRooms.Value;
            if (monsterDef.MobilityAfterInjury.HasValue)
                {
                result.Behaviours.Remove<MobilityAfterInjury>();
                if (monsterDef.MobilityAfterInjury.Value == "")
                    {
                    result.Behaviours.Add(new MobilityAfterInjury(result, monsterDef.MobilityAfterInjury.Value));
                    }
                }
            if (monsterDef.ChangeRoomsAfterInjury.HasValue)
                {
                result.Behaviours.Remove<ChangeRoomsAfterInjury>();
                if (monsterDef.ChangeRoomsAfterInjury.Value == "")
                    {
                    result.Behaviours.Add(new ChangeRoomsAfterInjury(result, monsterDef.ChangeRoomsAfterInjury.Value));
                    }
                }
            if (monsterDef.LaysMushrooms.HasValue)
                result.Behaviours.Set<LaysMushroom>(monsterDef.LaysMushrooms.Value);
            if (monsterDef.LaysEggs.HasValue)
                result.Behaviours.Set<LaysEgg>(monsterDef.LaysEggs.Value);
            if (monsterDef.SplitsOnHit.HasValue)
                result.Behaviours.Set<SpawnsUponDeath>(monsterDef.SplitsOnHit.Value);
            if (monsterDef.ShootsAtPlayer.HasValue)
                result.Behaviours.Set<ShootsAtPlayer>(monsterDef.ShootsAtPlayer.Value);
            if (monsterDef.ShootsOnceProvoked.HasValue)
                result.Behaviours.Set<StartsShootingWhenHurt>(monsterDef.ShootsOnceProvoked.Value);
            if (monsterDef.ShotsBounceOff.HasValue)
                {
                if (monsterDef.ShotsBounceOff.Value)
                    result.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Reflection);
                else
                    result.Properties.Remove(GameObjectProperties.EffectOfShot);
                }
            if (monsterDef.IsActive.HasValue)
                result.IsActive = monsterDef.IsActive.Value;

            if (!result.IsActive)
                {
                result.Behaviours.Add<ActivateWhenHurt>();
                result.Behaviours.Add<ActivateWhenMeetsPlayer>();
                }

            if (result.Mobility != MonsterMobility.Stationary && result.MovementBoundary == null)
                {
                throw new InvalidOperationException("Monster of type " + monsterDef.Breed + " at " + monsterDef.Position + " has no movement boundary. Presumably ChangeRooms has not been set.");
                }

            var initialDirection = monsterDef.InitialDirection.GetValueOrDefault(Direction.None);
            result.SetMonsterMotion(initialDirection);
            
            return result;
            }

        private static IMonster BuildEgg(MonsterDef monsterDef)
            {
            if (!monsterDef.TimeBeforeHatching.HasValue)
                throw new InvalidOperationException("MonsterDef has IsEgg set without a TimeBeforeHatching value.");
            var animationPlayer = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var underlyingMonster = monsterDef;
            underlyingMonster.IsEgg = false;
            underlyingMonster.TimeBeforeHatching = null;
            var result = new MonsterEgg(animationPlayer, underlyingMonster, monsterDef.TimeBeforeHatching.Value);
            return result;
            }

        private static Monster BuildMonsterFromBreed([NotNull] Breed breedInfo, Vector2 position, int energy)
            {
            var animationPlayer = new AnimationPlayer(GlobalServices.SpriteLibrary);
            var pathToTexture = "Sprites/Monsters/" + breedInfo.Texture;
            animationPlayer.PlayAnimation(Animation.LoopingAnimation(pathToTexture, breedInfo.BaseMovementsPerFrame));
            var result = new Monster(breedInfo.Name, animationPlayer, position, energy);
            return result;
            }

        private static void AddInherentBehaviours(Monster monster, Breed breedInfo)
            {
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
                var constructorArguments = new object[] {monster};
                var behaviour = (IBehaviour) constructorInfo.Invoke(constructorArguments);

                monster.Behaviours.Add(behaviour);
                }
            }

        private static void AddMovements(Monster monster, Breed breedInfo, Direction initialDirection)
            {
            var movement = breedInfo.BreedMovement;
            if (movement.ChangeRooms.HasValue)
                {
                monster.ChangeRooms = movement.ChangeRooms.Value;
                }
            if (movement.Speed.HasValue)
                {
                monster.CurrentSpeed = (int) (Constants.BaseSpeed * movement.Speed.Value);
                }

            foreach (var move in breedInfo.BreedMovement.Moves)
                {
                string typeName = "Labyrinth.GameObjects.Movement." + move.Value;
                Type movementType = Type.GetType(typeName);
                if (movementType == null || !movementType.GetInterfaces().Contains(typeof(IMonsterMotion)))
                    {
                    throw new InvalidOperationException("Could not find movement Type " + move.Value);
                    }

                IMonsterMotion implementation = GetImplementationForMotion(monster, movementType, initialDirection);
                monster.MovementMethods.Add(move.Key, implementation);
                }
            if (movement.DefaultMobility.HasValue)
                {
                monster.Mobility = movement.DefaultMobility.Value;
                }
            }

        private static void AddInherentProperties(Monster monster, Breed breedInfo)
            {
            foreach (var property in breedInfo.InherentProperties)
                {
                AddToPropertyBag(monster.Properties, property.Key, property.Value);
                }
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
                        breed.InherentProperties.Add(property.Name, propertyValue);
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

                    foreach (XmlElement moveElement in movement.SelectNodesEx("ns:Move", xnm))
                        {
                        var type = (MonsterMobility) Enum.Parse(typeof(MonsterMobility), moveElement.GetAttribute("Type"));
                        var implementation = moveElement.GetAttribute("Implementation");
                        breed.BreedMovement.Moves.Add(type, implementation);
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
            object value;
            if (propertyType.IsEnum)
                {
                value = Enum.Parse(propertyType, propertyValue);
                }
            else if (propertyType == typeof(bool))
                {
                value = XmlConvert.ToBoolean(propertyValue);
                }
            else
                {
                throw new InvalidOperationException($"Property definition {propertyName} has type {propertyType.FullName} which isn't supported.");
                }

            MethodInfo methodInfo = typeof(PropertyBag).GetMethod("Set")?.MakeGenericMethod(propertyType);
            if (methodInfo == null)
                {
                throw new InvalidOperationException("Failed to get generic PropertyBag method definition");
                }

            methodInfo.Invoke(propertyBag, new[] {propertyDef, value});
            }

        private static IMonsterMotion GetImplementationForMotion(Monster monster, Type type, Direction initialDirection)
            {
            if (!type.GetInterfaces().Contains(typeof(IMonsterMotion)))
                {
                throw new InvalidOperationException("Type " + type.Name + " does not implement IMonsterMotion.");
                }

            var constructorArgTypes = new List<Type> {typeof(Monster)};
            var constructorArguments = new List<object> {monster};
            if (initialDirection != Direction.None)
                {
                constructorArgTypes.Add(typeof(Direction));
                constructorArguments.Add(initialDirection);
                }

            var constructorInfo = type.GetConstructor(constructorArgTypes.ToArray());
            if (constructorInfo == null)
                throw new InvalidOperationException("Failed to get matching constructor information for " + type.Name + " class.");

            var movementImplementation = (IMonsterMotion) constructorInfo.Invoke(constructorArguments.ToArray());
            return movementImplementation;
            }

        private class Breed
            {
            public string Name;
            public string Texture;
            public int BaseMovementsPerFrame;
            public readonly HashSet<string> InherentBehaviours = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public readonly Dictionary<string, string> InherentProperties = new Dictionary<string, string>();
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
