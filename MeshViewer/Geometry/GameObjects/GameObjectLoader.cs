using MeshViewer.Geometry.Model;
using MeshViewer.Memory;
using MeshViewer.Memory.Entities;
using MeshViewer.Rendering;
using OpenTK;
using System.Collections.Concurrent;
using System.IO;

namespace MeshViewer.Geometry.GameObjects
{
    public sealed class GameObjectLoader
    {
        private string _directory;
        private ConcurrentDictionary<int, WorldModel> _models = new ConcurrentDictionary<int, WorldModel>();

        public bool Enabled { get; set; } = true;

        public GameObjectLoader(string directory)
        {
            _directory = directory;
        }

        public void Clear() => _models.Clear();

        static Vector3 GAMEOBJECT_COLOR = new Vector3(1.00f, 0.118f, 0.071f);

        public void Render()
        {
            if (!Enabled)
                return;

            var gameobjectProgram = ShaderProgramCache.Instance.Get("gameobject");
            var view = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
            var cameraDirection = Game.Camera.Forward;

            gameobjectProgram.Use();
            gameobjectProgram.UniformMatrix("projection_view", false, ref view);
            gameobjectProgram.UniformVector("camera_direction", ref cameraDirection);
            gameobjectProgram.UniformVector("object_color", ref GAMEOBJECT_COLOR);
            
            foreach (var kv in _models)
            {
                kv.Value.Render();
                // break;
            }
        }

        public bool RemoveInstance(ulong instanceGUID, CGGameObject_C gameObject)
        {
            if (gameObject.DisplayInfo == null || !_models.TryGetValue(gameObject.DisplayInfo.ID, out var modelInstance))
                return false;

            modelInstance.RemoveInstance(instanceGUID);
            return true;
        }

        public ulong AddInstance(CGGameObject_C gameObject)
        {
            // return ulong.MaxValue;

            if (gameObject.DisplayInfo == null)
                return ulong.MaxValue;

            var gameObjectDisplayInfo = gameObject.DisplayInfo;

            var positionMatrix = gameObject.PositionMatrix;
            var entry = gameObjectDisplayInfo.ID;

            if (_models.TryGetValue(entry, out WorldModel instance))
                return instance.AddInstance(ref positionMatrix);

            var modelSpawn = WorldModelCache.OpenInstance(_directory, Path.GetFileName(gameObjectDisplayInfo.Filename).Replace(".mdx", ".m2"));
            if (modelSpawn == null)
                return ulong.MaxValue;

            var modelInstanceGUID = modelSpawn.AddInstance(ref positionMatrix);

            // reference type, so it is irrelevant if we didn't manage to save it - a thread did before us!
            _models.TryAdd(gameObject.DisplayInfo.ID, modelSpawn);

            return modelInstanceGUID;
        }
    }
}
