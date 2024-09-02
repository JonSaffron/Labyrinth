using System;
using System.IO.IsolatedStorage;
using System.IO;

namespace Labyrinth.Services
    {
    internal static class PersistentStorage
        {
        private const string WorldSerialiseFileName = "world.dat";

        public static string? ReadSettings(string fileName)
            {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            using var storage = IsolatedStorageFile.GetUserStoreForDomain();
            if (!storage.FileExists(fileName))
                {
                return null;
                }

            using IsolatedStorageFileStream stream = storage.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            using TextReader reader = new StreamReader(stream);
            var result = reader.ReadLine();
            return result;
            }

        public static void WriteSettings(string fileName, string contents)
            {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (contents == null) throw new ArgumentNullException(nameof(contents));
            using var storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = storage.CreateFile(fileName);
            using TextWriter writer = new StreamWriter(stream);
            writer.WriteLine(contents);
            }

        public static string? GetWorld()
            {
            string? world = ReadSettings(WorldSerialiseFileName);
            if (world != null)
                {
                if (string.IsNullOrWhiteSpace(world))
                    {
                    world = null;
                    }
                }
            return world;
            }

        public static void SetWorld(string world)
            {
            if (world == null) throw new ArgumentNullException(nameof(world));
            WriteSettings(WorldSerialiseFileName, world);
            }
        }
    }
