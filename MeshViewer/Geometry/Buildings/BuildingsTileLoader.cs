using MeshViewer.Geometry.Model;
using OpenTK;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Buildings
{
    public sealed class BuildingsTileLoader
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private string _directory;

        private ConcurrentDictionary<string, ConcurrentDictionary<ulong, Matrix4>> _instanceGUIDs = new ConcurrentDictionary<string, ConcurrentDictionary<ulong, Matrix4>>();
        private bool _instancesRendered = false;

        public IEnumerable<string> Models => _instanceGUIDs.Keys;

        public void RemoveInstances()
        {
            if (!_instancesRendered)
                return;

            _instancesRendered = false;
            
            foreach (var kv in _instanceGUIDs)
            {
                var worldModel = WorldModelCache.OpenInstance(_directory, kv.Key);
                if (worldModel == null)
                    continue;

                foreach (var kv2 in kv.Value)
                    worldModel.RemoveInstance(kv2.Key);
            }
        }

        public void AddInstances()
        {
            if (_instancesRendered)
                return;

            _instancesRendered = true;

            foreach (var kv in _instanceGUIDs)
            {
                var worldModel = WorldModelCache.OpenInstance(_directory, kv.Key);
                if (worldModel == null)
                    continue;

                foreach (var kv2 in kv.Value)
                {
                    var instanceMatrix = kv2.Value;
                    worldModel.AddInstance(ref instanceMatrix);
                }
            }
        }

        public void LoadInstances(string directory, int mapID, int tileX, int tileY)
        {
            _directory = directory;

            X = tileX;
            Y = tileY;

            var filePath = Path.Combine(directory, "vmaps", $"{mapID:D3}_{tileX:D2}_{tileY:D2}.vmtile");

            if (!File.Exists(filePath))
                return;

            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                reader.BaseStream.Position += 8; // Skip magic

                var numModelBatches = reader.ReadInt32();
                
                for (var i = 0; i < numModelBatches; ++i)
                {
                    var modelSpawn = new ModelSpawn(directory, reader);
                    reader.BaseStream.Position += 4;

                    var modelName     = modelSpawn.ModelName;
                    var modelPosition = modelSpawn.PositionMatrix;

                    var worldModel = WorldModelCache.OpenInstance(directory, modelName);
                    if (!_instanceGUIDs.TryGetValue(modelName, out var storageDictionary))
                        storageDictionary = _instanceGUIDs[modelName] = new ConcurrentDictionary<ulong, Matrix4>();

                    storageDictionary.TryAdd(worldModel.AddInstance(ref modelPosition), modelPosition);
                }
            }

            _instancesRendered = true;
        }
    }
}
