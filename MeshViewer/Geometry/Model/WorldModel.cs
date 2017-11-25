using MeshViewer.Rendering;
using OpenTK;
using System.Collections.Concurrent;
using System.IO;

namespace MeshViewer.Geometry.Model
{
    public sealed class WorldModel : InstancedBatchedIndexedModel<uint, Vector3, Matrix4>
    {
        public WorldModel(string directory, string modelName, string shaderProgramName) : base(shaderProgramName)
        {
            var filePath = Path.Combine(directory, "vmaps", $"{modelName}.vmo");
            if (!File.Exists(filePath))
                return;

            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                reader.BaseStream.Position += 8;

                if (reader.ReadInt32() == 0x444F4D57) // WMOD
                    reader.BaseStream.Position += 8; // chunk size + root wmo id

                if (reader.ReadInt32() == 0x444F4D47) // GMOD
                {
                    var modelCount = reader.ReadInt32();
                    for (var i = 0; i < modelCount; ++i)
                    {
                        var model = new GroupModel(reader)
                        {
                            Program = ShaderProgramCache.Instance.Get("wmo")
                        };

                        if (modelName.EndsWith(".m2"))
                            model.InvertIndices();

                        AddBatch(model);
                    }
                }
            }
        }

    }

    public class WorldModelCache
    {
        private static WorldModelCache _instance = new WorldModelCache();

        private ConcurrentDictionary<string, WorldModel> _store = new ConcurrentDictionary<string, WorldModel>();

        public static WorldModel OpenInstance(string directory, string modelName)
        {
            if (!_instance._store.TryGetValue(modelName, out var modelInstance))
            {
                modelInstance = new WorldModel(directory, modelName, "wmo");

                _instance._store.TryAdd(modelName, modelInstance);
            }

            return modelInstance;
        }

        ~WorldModelCache()
        {
            _store.Clear();
            _store = null;
        }
    }
}
