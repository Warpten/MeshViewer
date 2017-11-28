using MeshViewer.Geometry.Model;
using MeshViewer.Memory;
using MeshViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeshViewer.Geometry.Buildings
{
    public sealed class BuildingsLoader
    {
        private ConcurrentDictionary<int, BuildingsTileLoader> _grids = new ConcurrentDictionary<int, BuildingsTileLoader>();
        private bool _isTiled;

        private string _directory;
        private int _mapId;

        public BuildingsLoader(string directory, int mapID)
        {
            _directory = directory;
            _mapId = mapID;

            using (var reader = new BinaryReader(File.OpenRead(Path.Combine(directory, "vmaps", $"{mapID:D3}.vmtree"))))
            {
                reader.BaseStream.Position += 8; // Skip signature
                _isTiled = reader.ReadByte() == 1;

                if (reader.ReadInt32() == 0x45444F4E) // NODE
                    BIH.Skip(reader);

                if (!_isTiled && reader.ReadInt32() == 0x4A424F47) // GOBJ
                {
                    // load global model
                }
            }
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        static Vector3 WMO_COLOR = new Vector3(0.063f, 0.886f, 0.243f);

        public void Render(int centerTileX, int centerTileY, int renderRange)
        {
            // todo implement global model
            if (!_isTiled)
                return;

            var wmoProgram = ShaderProgramCache.Instance.Get("wmo");
            var view = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
            var cameraDirection = Game.Camera.Forward;

            wmoProgram.Use();
            wmoProgram.UniformMatrix("projection_view", false, ref view);
            wmoProgram.UniformVector("camera_direction", ref cameraDirection);
            wmoProgram.UniformVector("object_color", ref WMO_COLOR);

            foreach (var kv in _grids)
            {
                var xInRange = Math.Abs(kv.Value.X - centerTileX) <= renderRange;
                var yInRange = Math.Abs(kv.Value.Y - centerTileY) <= renderRange;

                if (!xInRange || !yInRange)
                    kv.Value.RemoveInstances();
            }

            for (var i = centerTileY - renderRange; i <= centerTileY + renderRange; ++i)
            {
                for (var j = centerTileX - renderRange; j <= centerTileX + renderRange; ++j)
                {
                    if (!_grids.TryGetValue(PackTile(j, i), out var gridRenderer))
                        LoadGrid(j, i);
                    else
                        gridRenderer.AddInstances();
                }
            }

            // Now that all instances are added, render everything in one single pass
            foreach (var kv in _grids.SelectMany(grid => grid.Value.Models).Distinct())
            {
                var worldModel = WorldModelCache.OpenInstance(_directory, kv);
                if (worldModel == null)
                    continue;

                worldModel.Render();
            }
        }

        private async void LoadGrid(int tileX, int tileY)
        {
            await Task.Run(() =>
            {
                var tileLoader = new BuildingsTileLoader();
                tileLoader.LoadInstances(_directory, _mapId, tileX, tileY);

                _grids[PackTile(tileX, tileY)] = tileLoader;
            });
        }
    }
}
