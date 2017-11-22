using MeshViewer.Memory;
using MeshViewer.Rendering;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;

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

        private TerrainGridLoader LoadTile(int tileX, int tileY)
        {
            return _terrainGrids.GetOrAdd(PackTile(tileX, tileY), (hash) =>
            {
                return new TerrainGridLoader(Directory, MapID, tileX, tileY);
            });
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        private static Vector3 TERRAIN_COLOR = new Vector3(1.0f, 0.435f, 0.071f);

        public void Render(int centerTileX, int centerTileY)
        {
            const int MAX_CHUNK_DISTANCE = 1; /// Debugging

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

                var xInRange = Math.Abs(grid.Value.X - centerTileX) <= MAX_CHUNK_DISTANCE;
                var yInRange = Math.Abs(grid.Value.Y - centerTileY) <= MAX_CHUNK_DISTANCE;

                if (!xInRange || !yInRange)
                {
                    grid.Value.Unload(); // Unload geometry
                    ((IDictionary)_terrainGrids).Remove(grid.Key);
                }
            }

            for (var i = centerTileY - MAX_CHUNK_DISTANCE; i <= centerTileY + MAX_CHUNK_DISTANCE; ++i)
            {
                for (var j = centerTileX - MAX_CHUNK_DISTANCE; j <= centerTileX + MAX_CHUNK_DISTANCE; ++j)
                {
                    if (!_terrainGrids.TryGetValue(PackTile(j, i), out var gridLoader))
                        gridLoader = LoadTile(j, i);
                        
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
