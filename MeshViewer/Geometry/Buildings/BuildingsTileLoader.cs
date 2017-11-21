using MeshViewer.Geometry.Model;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Buildings
{
    public sealed class BuildingsTileLoader
    {
        private Dictionary<string, ModelSpawn> Spawns { get; set; } = new Dictionary<string, ModelSpawn>();
        public int MapID { get; }
        public int X { get; }
        public int Y { get; }
        public bool FileExists { get; }

        public BuildingsTileLoader(string directory, int mapID, int tileX, int tileY)
        {
            X = tileX;
            Y = tileY;

            var refCounter = new Dictionary<int, int>();

            var filePath = Path.Combine(directory, "vmaps", $"{mapID:D3}_{tileX:D2}_{tileY:D2}.vmtile");
            if (!(FileExists = File.Exists(filePath)))
                return;

            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                reader.BaseStream.Position += 8; // Skip magic

                var numSpawns = reader.ReadInt32();
                for (var i = 0; i < numSpawns; ++i)
                {
                    var modelSpawn = new ModelSpawn(directory, reader);
                    var referencedValue = reader.ReadInt32();

                    if (!Spawns.ContainsKey(modelSpawn.Name))
                        Spawns.Add(modelSpawn.Name, modelSpawn);
                    // No else statement, ctor adds the instance already
                }
            }

            foreach (var modelInstance in Spawns.Values)
            {
                if (!modelInstance.Name.EndsWith(".m2"))
                    continue;

                modelInstance.InvertIndices();
            }
        }

        public void Unload()
        {
            Spawns.Clear();
        }

        public void Render()
        {
            foreach (var modelInstance in Spawns.Values)
                modelInstance.Render();
        }
    }
}
