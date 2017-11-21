using MeshViewer.Memory;
using MeshViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MeshViewer.Geometry.Terrain
{
    public sealed class TerrainLoader
    {
        public int MapID { get; }

        public Dictionary<int, TerrainGridLoader> Grids { get; set; } = new Dictionary<int, TerrainGridLoader>();
        public string Directory { get; }

        public TerrainLoader(string directory, int mapID)
        {
            MapID = mapID;

            Directory = Path.Combine(directory, "maps");
        }

        public void LoadTile(int tileX, int tileY)
        {
            var gridHash = PackTile(tileX, tileY);
            if (Grids.ContainsKey(gridHash))
                return;
            
            // Placeholder until the task executes
            Grids[gridHash] = null;
            Task.Run(() =>
            {
                var gridLoader = new TerrainGridLoader(Directory, MapID, tileX, tileY)
                {
                    Program = ShaderProgramCache.Instance.Get("terrain")
                };
                if (gridLoader.FileExists)
                    lock (Grids) Grids[gridHash] = gridLoader;
            });
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        private static Vector3 TERRAIN_COLOR = new Vector3(0.7f, 0.7f, 0.0f);

        public void Render(int centerTileX, int centerTileY)
        {
            const int MAX_CHUNK_DISTANCE = 1; /// Debugging

            var terrainProgram = ShaderProgramCache.Instance.Get("terrain");
            var projModelView = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
            var cameraDirection = Game.Camera.Forward;

            terrainProgram.Use();
            terrainProgram.UniformMatrix4("modelViewProjection", false, ref projModelView);
            terrainProgram.UniformVector3("camera_direction", ref cameraDirection);
            terrainProgram.UniformVector3("object_color", ref TERRAIN_COLOR);

            lock (Grids)
            {
                foreach (var grid in Grids)
                    if (grid.Value != null && Math.Abs(grid.Value.X - centerTileX) > MAX_CHUNK_DISTANCE && Math.Abs(grid.Value.Y - centerTileY) > MAX_CHUNK_DISTANCE)
                        grid.Value.Unload();

                for (var i = centerTileY - MAX_CHUNK_DISTANCE; i <= centerTileY + MAX_CHUNK_DISTANCE; ++i)
                    for (var j = centerTileX - MAX_CHUNK_DISTANCE; j <= centerTileX + MAX_CHUNK_DISTANCE; ++j)
                        if (!Grids.ContainsKey(PackTile(j, i)))
                            LoadTile(j, i);

                foreach (var mapGrid in Grids.Values)
                    if (mapGrid != null && Math.Abs(centerTileX - mapGrid.X) <= MAX_CHUNK_DISTANCE && Math.Abs(centerTileY - mapGrid.Y) <= MAX_CHUNK_DISTANCE)
                        mapGrid.Render();
            }
        }

        ~TerrainLoader()
        {
            Grids.Clear();
            Grids = null;
        }
    }
}
