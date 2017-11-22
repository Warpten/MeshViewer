using MeshViewer.Rendering;
using OpenTK;
using System.Collections.Concurrent;
using System.IO;

namespace MeshViewer.Geometry.Model
{
    public sealed class WorldModel
    {
        private GroupModel[] GroupModels { get; set; }
        private ulong _instanceGUID = 0;
        public bool Valid => GroupModels != null && GroupModels.Length != 0;

        public WorldModel(string directory, string modelName)
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
                    GroupModels = new GroupModel[reader.ReadInt32()];
                    for (var i = 0; i < GroupModels.Length; ++i)
                    {
                        GroupModels[i] = new GroupModel(reader)
                        {
                            Program = ShaderProgramCache.Instance.Get("wmo"),
                            VerticeAttribute = "vertexPosition_modelSpace",
                            InstancePositionAttribute = "instance_position",
                        };

                        if (modelName.EndsWith(".m2"))
                            GroupModels[i].InvertIndices();
                    }
                }
            }
        }

        public void Render()
        {
            foreach (var model in GroupModels)
                model.Render();
        }

        public ulong AddInstance(ref Matrix4 positionMatrix)
        { 
            foreach (var model in GroupModels)
                model.AddInstance(ref positionMatrix, _instanceGUID);
            return _instanceGUID++;
        }

        public ulong AddInstance(ref Matrix4 positionMatrix, ulong existingGUID)
        {
            foreach (var model in GroupModels)
                model.AddInstance(ref positionMatrix, existingGUID);
            return existingGUID;
        }

        public void RemoveInstance(ulong instanceGUID)
        {
            foreach (var model in GroupModels)
                model.RemoveInstance(instanceGUID);
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
                modelInstance = new WorldModel(directory, modelName);
                if (modelInstance == null || !modelInstance.Valid)
                    return null;

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
