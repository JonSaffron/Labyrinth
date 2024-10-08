﻿using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public struct MonsterDef
        {
        public string Breed { get; set; }
        public Vector2 Position { get; set; }
        public int Energy { get; set; }
        public bool IsEgg { get; set; }
        public int? TimeBeforeHatching { get; set; }
        public MonsterMobility? Mobility { get; set;}
        public Direction? InitialDirection { get; set; }
        public Setting<MonsterMobility> MobilityAfterInjury { get; set; }
        public ChangeRooms? ChangeRooms { get; set; }
        public Setting<ChangeRooms> ChangeRoomsAfterInjury { get; set; }
        public bool? LaysMushrooms { get; set; }
        public bool? LaysEggs { get; set; }
        public bool? SplitsOnHit { get; set; }
        public Setting<ShootsAtPlayer> ShootsAtPlayer { get; set; }
        public bool? ShotsBounceOff { get; set; }
        public bool? IsActive { get; set; }

        internal static MonsterDef FromXml(XmlElement mDef, XmlNamespaceManager namespaceManager)
            {
            if (mDef == null) throw new ArgumentNullException(nameof(mDef));
            MonsterDef result = new MonsterDef
                {
                Breed = mDef.GetAttribute(nameof(result.Breed)),
                Energy = int.Parse(mDef.GetAttribute(nameof(result.Energy)))
                };

            string mobility = mDef.GetAttribute(nameof(result.Mobility));
            if (!string.IsNullOrEmpty(mobility))
                {
                result.Mobility = (MonsterMobility)Enum.Parse(typeof(MonsterMobility), mobility);
                }

            string initialDirection = mDef.GetAttribute(nameof(result.InitialDirection));
            if (!string.IsNullOrEmpty(initialDirection))
                {
                result.InitialDirection = (Direction)Enum.Parse(typeof(Direction), initialDirection);
                }

            string changeRooms = mDef.GetAttribute(nameof(result.ChangeRooms));
            if (!string.IsNullOrEmpty(changeRooms))
                {
                result.ChangeRooms = (ChangeRooms)Enum.Parse(typeof(ChangeRooms), changeRooms);
                }

            string isActiveAttribute = mDef.GetAttribute(nameof(result.IsActive));
            if (!string.IsNullOrEmpty(isActiveAttribute))
                {
                result.IsActive = XmlConvert.ToBoolean(isActiveAttribute);
                }

            // additional behaviours
            var apply = (XmlElement?) mDef.SelectSingleNode("ns:Apply", namespaceManager);
            if (apply != null)
                {
                ApplyNewBehaviours(ref result, apply, namespaceManager);
                }

            // remove behaviours
            var remove = (XmlElement?) mDef.SelectSingleNode("ns:Remove", namespaceManager);
            if (remove != null)
                {
                RemoveUnwantedBehaviours(ref result, remove, namespaceManager);
                }

            return result;
            }

        private static void ApplyNewBehaviours(ref MonsterDef result, XmlElement apply, XmlNamespaceManager namespaceManager)
            {
            var mobilityAfterInjury = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(result.MobilityAfterInjury), namespaceManager);
            if (mobilityAfterInjury != null)
                {
                result.MobilityAfterInjury = Setting<MonsterMobility>.NewSetting((MonsterMobility) Enum.Parse(typeof(MonsterMobility), mobilityAfterInjury.GetAttribute("value")));
                }

            var changeRoomsAfterInjury = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(ChangeRoomsAfterInjury), namespaceManager);
            if (changeRoomsAfterInjury != null)
                {
                result.ChangeRoomsAfterInjury = Setting<ChangeRooms>.NewSetting((ChangeRooms) Enum.Parse(typeof(ChangeRooms), changeRoomsAfterInjury.GetAttribute("value")));
                }

            var isEgg = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(result.IsEgg), namespaceManager);
            if (isEgg != null)
                {
                result.IsEgg = true;
                result.TimeBeforeHatching = int.Parse(isEgg.GetAttribute(nameof(TimeBeforeHatching)));
                result.TimeBeforeHatching |= 1; // not sure why the original game does this...
                }

            var laysEggs = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(result.LaysEggs), namespaceManager);
            if (laysEggs != null)
                {
                result.LaysEggs = true;
                }

            var laysMushrooms = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(result.LaysMushrooms), namespaceManager);
            if (laysMushrooms != null)
                {
                result.LaysMushrooms = true;
                }

            var splitsOnHit = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(SplitsOnHit), namespaceManager);
            if (splitsOnHit != null)
                {
                result.SplitsOnHit = true;
                }

            var shootsAtPlayer = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(ShootsAtPlayer), namespaceManager);
            if (shootsAtPlayer != null)
                {
                result.ShootsAtPlayer = Setting<ShootsAtPlayer>.NewSetting((ShootsAtPlayer) Enum.Parse(typeof(ShootsAtPlayer), shootsAtPlayer.GetAttribute("value")));
                }

            var shotsBounceOff = (XmlElement?) apply.SelectSingleNode("ns:" + nameof(ShotsBounceOff), namespaceManager);
            if (shotsBounceOff != null)
                {
                result.ShotsBounceOff = true;
                }
            }

        private static void RemoveUnwantedBehaviours(ref MonsterDef result, XmlElement remove, XmlNamespaceManager namespaceManager)
            {
            var mobilityAfterInjury = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.MobilityAfterInjury), namespaceManager);
            if (mobilityAfterInjury != null)
                {
                if (!result.MobilityAfterInjury.UseBreedDefault)
                    throw new InvalidOperationException("Contradictory setting for MobilityAfterInjury");
                result.MobilityAfterInjury = Setting<MonsterMobility>.SettingNoBehaviour();
                }

            var changeRoomsAfterInjury = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(ChangeRoomsAfterInjury), namespaceManager);
            if (changeRoomsAfterInjury != null)
                {
                if (!result.ChangeRoomsAfterInjury.UseBreedDefault)
                    throw new InvalidOperationException("Contradictory setting for ChangeRoomsAfterInjury");
                result.ChangeRoomsAfterInjury = Setting<ChangeRooms>.SettingNoBehaviour();
                }

            var laysEggs = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.LaysEggs), namespaceManager);
            if (laysEggs != null)
                {
                if (result.LaysEggs.HasValue)
                    throw new InvalidOperationException("Contradictory setting for LaysEggs");
                result.LaysEggs = false;
                }

            var laysMushrooms = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.LaysMushrooms), namespaceManager);
            if (laysMushrooms != null)
                {
                if (result.LaysMushrooms.HasValue)
                    throw new InvalidOperationException("Contradictory setting for LaysMushrooms");
                result.LaysMushrooms = false;
                }

            var splitsOnHit = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.SplitsOnHit), namespaceManager);
            if (splitsOnHit != null)
                {
                if (result.SplitsOnHit.HasValue)
                    throw new InvalidOperationException("Contradictory setting for SplitsOnHit");
                result.SplitsOnHit = false;
                }

            var shootsAtPlayer = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.ShootsAtPlayer), namespaceManager);
            if (shootsAtPlayer != null)
                {
                if (!result.ShootsAtPlayer.UseBreedDefault)
                    throw new InvalidOperationException("Contradictory setting for ShootsAtPlayer");
                result.ShootsAtPlayer = Setting<ShootsAtPlayer>.SettingNoBehaviour();
                }

            var shotsBounceOffAttribute = (XmlElement?) remove.SelectSingleNode("ns:" + nameof(result.ShotsBounceOff), namespaceManager);
            if (shotsBounceOffAttribute != null)
                {
                if (result.ShotsBounceOff.HasValue)
                    throw new InvalidOperationException("Contradictory setting for ShotsBounceOff");
                result.ShotsBounceOff = false;
                }
            }
        }
    }
