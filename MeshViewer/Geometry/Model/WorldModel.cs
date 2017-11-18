using MeshViewer.Rendering;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Model
{
    public sealed class WorldModel
    {
        public GroupModel[] GroupModels { get; set; }
        // public BIH GroupTree { get; }
        public int RootWmoID { get; }

        public WorldModel(string directory, string modelName)
        {
            var filePath = Path.Combine(directory, "vmaps", $"{modelName}.vmo");
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                reader.BaseStream.Position += 8;

                if (reader.ReadInt32() == 0x444F4D57) // WMOD
                {
                    var chunkSize = reader.ReadInt32(); // 8
                    RootWmoID = reader.ReadInt32();
                }

                if (reader.ReadInt32() == 0x444F4D47) // GMOD
                {
                    GroupModels = new GroupModel[reader.ReadInt32()];
                    for (var i = 0; i < GroupModels.Length; ++i)
                    {
                        GroupModels[i] = new GroupModel(reader)
                        {
                            Program = ShaderProgramCache.Instance.Get("wmo"),
                            VerticeAttribute = "vertexPosition_modelSpace",
                            InstancePositionAttribute = "instance_position",
                        };
                    }

                    if (reader.ReadInt32() == 0x48494247) // GIH
                        BIH.Skip(reader);
                }
            }
        }
    }

    public class WorldModelCache
    {
        private Dictionary<string, WorldModel> _store = new Dictionary<string, WorldModel>();

        public static WorldModel OpenInstance(string directory, string modelName)
        {
            lock (_instance._store)
                if (!_instance._store.ContainsKey(modelName))
                    _instance._store.Add(modelName, new WorldModel(directory, modelName));
            return _instance._store[modelName];
        }

        private static WorldModelCache _instance = new WorldModelCache();

        ~WorldModelCache()
        {
            _store.Clear();
            _store = null;
        }
    }
}
