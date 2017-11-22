using MeshViewer.Data;
using MeshViewer.Geometry.Model;
using MeshViewer.Memory;
using MeshViewer.Memory.Entities;
using MeshViewer.Rendering;
using OpenTK;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        static Vector3 GAMEOBJECT_COLOR = new Vector3(0.55f, 0.0f, 0.0f);

        public void Render()
        {
            if (!Enabled)
                return;

            var gameobjectProgram = ShaderProgramCache.Instance.Get("gameobject");
            var view = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
            var cameraDirection = Game.Camera.Forward;

            gameobjectProgram.Use();
            gameobjectProgram.UniformMatrix4("projection_view", false, ref view);
            gameobjectProgram.UniformVector3("camera_direction", ref cameraDirection);
            gameobjectProgram.UniformVector3("object_color", ref GAMEOBJECT_COLOR);
            
            foreach (var kv in _models)
            {
                kv.Value.Render();
                // break;
            }
        }

        public bool RemoveInstance(CGGameObject_C gameObject)
        {
            if (gameObject.DisplayInfo == null || !_models.TryGetValue(gameObject.DisplayInfo.ID, out var modelInstance))
                return false;

            var positionMatrix = Matrix4.CreateTranslation(gameObject.X, gameObject.Y, gameObject.Z);

            var rotationQuaternion = new Quaternion(gameObject.GAMEOBJECT_PARENTROTATION[0],
                gameObject.GAMEOBJECT_PARENTROTATION[1],
                gameObject.GAMEOBJECT_PARENTROTATION[2],
                gameObject.GAMEOBJECT_PARENTROTATION[3]);
            var rotationMatrix = Matrix4.CreateFromQuaternion(rotationQuaternion);

            var scaleMatrix = Matrix4.CreateScale(gameObject.OBJECT_FIELD_SCALE_X);
            positionMatrix = rotationMatrix * scaleMatrix * positionMatrix;

            modelInstance.RemoveInstance(ref positionMatrix);
            return true;
        }

        public bool AddInstance(CGGameObject_C gameObject)
        {
            if (gameObject.DisplayInfo == null)
                return false;

            var positionMatrix = gameObject.PositionMatrix;

            // var positionMatrix = Matrix4.CreateTranslation(gameObject.X, gameObject.Y, gameObject.Z);
            // 
            // var rotationQuaternion = new Quaternion(gameObject.GAMEOBJECT_PARENTROTATION[0],
            //     gameObject.GAMEOBJECT_PARENTROTATION[1],
            //     gameObject.GAMEOBJECT_PARENTROTATION[2],
            //     gameObject.GAMEOBJECT_PARENTROTATION[3]);
            // var rotationMatrix = Matrix4.CreateFromQuaternion(rotationQuaternion);
            // 
            // var scaleMatrix = Matrix4.CreateScale(gameObject.OBJECT_FIELD_SCALE_X);
            // positionMatrix = rotationMatrix * scaleMatrix * positionMatrix;

            var entry = gameObject.DisplayInfo.ID;

            if (_models.TryGetValue(entry, out WorldModel instance))
            {
                instance.AddInstance(ref positionMatrix);
            }
            else
            {
                var modelSpawn = WorldModelCache.OpenInstance(_directory, Path.GetFileName(gameObject.DisplayInfo.Filename).Replace(".mdx", ".m2"));
                if (modelSpawn == null)
                    return false;

                modelSpawn.AddInstance(ref positionMatrix);

                // reference type, so it is irrelevant if we didn't manage to save it - a thread did before us!
                _models.TryAdd(gameObject.DisplayInfo.ID, modelSpawn);
            }

            return true;
        }
    }
}
