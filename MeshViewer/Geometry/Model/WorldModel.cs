using MeshViewer.Rendering;
using OpenTK;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Model
{
    public sealed class WorldModel
    {
        private GroupModel[] GroupModels { get; set; }
        // public BIH GroupTree { get; }
        public int RootWmoID { get; }

        public bool Valid => GroupModels != null;

        public WorldModel(string directory, string modelName)
        {
            var filePath = Path.Combine(directory, "vmaps", $"{modelName}.vmo");
            if (!File.Exists(filePath))
                return;

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

                    // if (reader.ReadInt32() == 0x48494247) // GIH
                    //     BIH.Skip(reader);
                }
            }
        }

        public void Render()
        {
            foreach (var model in GroupModels)
                model.Render();
        }

        public void AddInstance(ref Matrix4 positionMatrix)
        {
            foreach (var model in GroupModels)
                model.AddInstance(ref positionMatrix);
        }

        public void RemoveInstance(ref Matrix4 positionMatrix)
        {
            foreach (var model in GroupModels)
                model.RemoveInstance(ref positionMatrix);
        }

        public void InvertIndices()
        {
            foreach (var m in GroupModels)
                m.InvertIndices();
        }
    }

    public class WorldModelCache
    {
        private ConcurrentDictionary<string, WorldModel> _store = new ConcurrentDictionary<string, WorldModel>();

        public static WorldModel OpenInstance(string directory, string modelName)
        {
            if (!_instance._store.TryGetValue(modelName, out var modelInstance))
            {
                modelInstance = new WorldModel(directory, modelName);
                if (modelInstance == null || !modelInstance.Valid)
                    return null;

                _instance._store.TryAdd(modelName, modelInstance);
            }

            return modelInstance;
        }

        private static WorldModelCache _instance = new WorldModelCache();

        ~WorldModelCache()
        {
            _store.Clear();
            _store = null;
        }
    }
}
