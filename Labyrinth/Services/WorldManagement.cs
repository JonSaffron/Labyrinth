using Labyrinth.DataStructures;
using Labyrinth.Services.WorldBuilding;
using System;
using System.Collections.Generic;
using System.IO;

namespace Labyrinth.Services
    {
    internal static class WorldManagement
        {
        public static IEnumerable<WorldInfo> EnumerateAvailableWorlds()
            {
            string worldDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Worlds");
            var dir = new DirectoryInfo(worldDirectory);
            if (!dir.Exists)
                throw new InvalidOperationException("Directory for world files does not exist.");
            var files = dir.EnumerateFiles("*.xml");
            foreach (var file in files)
                {
                var worldInfo = GetWorldInfo(file.Name);
                if (worldInfo != null)
                    {
                    yield return worldInfo;
                    }
                }
            }

        public static WorldInfo? GetWorldInfo(string worldFile)
            {
            try
                {
                var worldLoader = new WorldLoader(worldFile);
                WorldInfo result = new WorldInfo(file: worldFile, name: worldLoader.WorldName, difficulty: string.Empty);
                return result;
                }
            catch (Exception)
                {
                return null;
                }
            }
        }
    }
