using MeshViewer.Memory;
using MeshViewer.Rendering;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace MeshViewer.Geometry.Terrain
{
    /// <summary>
    /// This class loads ADT geometry for the terrain.
    /// </summary>
    public sealed class TerrainLoader
    {
        /// <summary>
        /// The current map's ID.
        /// </summary>
        public int MapID { get; }

        private ConcurrentDictionary<int, TerrainGridLoader> _terrainGrids { get; set; } = new ConcurrentDictionary<int, TerrainGridLoader>();

        public string Directory { get; }

        public TerrainLoader(string directory, int mapID)
        {
            MapID = mapID;

            Directory = Path.Combine(directory, "maps");
        }

        private async void LoadTile(int tileX, int tileY)
        {
            await Task.Run(() =>
            {
                return _terrainGrids.GetOrAdd(PackTile(tileX, tileY), (hash) =>
                {
                    return new TerrainGridLoader(Directory, MapID, tileX, tileY);
                });
            }).ConfigureAwait(false);
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        private static Vector3 TERRAIN_COLOR = new Vector3(1.0f, 0.435f, 0.071f);

        public void Render(int centerTileX, int centerTileY, int renderRange)
        {
            var terrainProgram = ShaderProgramCache.Instance.Get("terrain");
            var projModelView = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
            var cameraDirection = Game.Camera.Forward;

            terrainProgram.Use();
            terrainProgram.UniformMatrix("modelViewProjection", false, ref projModelView);
            terrainProgram.UniformVector("camera_direction", ref cameraDirection);
            terrainProgram.UniformVector("object_color", ref TERRAIN_COLOR);

            foreach (var grid in _terrainGrids)
            {
                if (!grid.Value.Valid)
                    continue;

                var xInRange = Math.Abs(grid.Value.X - centerTileX) <= renderRange;
                var yInRange = Math.Abs(grid.Value.Y - centerTileY) <= renderRange;

                if (!xInRange || !yInRange)
                {
                    grid.Value.Unload(); // Unload geometry
                    ((IDictionary)_terrainGrids).Remove(grid.Key);
                }
            }

            for (var i = centerTileY - renderRange; i <= centerTileY + renderRange; ++i)
            {
                for (var j = centerTileX - renderRange; j <= centerTileX + renderRange; ++j)
                {
                    if (!_terrainGrids.TryGetValue(PackTile(j, i), out var gridLoader))
                        LoadTile(j, i);
                    else
                        gridLoader.Render();
                }
            }
        }

        ~TerrainLoader()
        {
            _terrainGrids.Clear();
            _terrainGrids = null;
        }
    }
}
