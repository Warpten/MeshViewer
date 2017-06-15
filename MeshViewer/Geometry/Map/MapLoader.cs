using MeshViewer.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Map
{
    public sealed class MapLoader
    {
        public int MapID { get; }

        public Dictionary<int, GridMapLoader> Grids { get; set; } = new Dictionary<int, GridMapLoader>();
        public string Directory { get; }

        public MapLoader(string directory, int mapID)
        {
            MapID = mapID;

            Directory = Path.Combine(directory, "maps");
        }

        public void LoadTile(int tileX, int tileY)
        {
            var gridHash = PackTile(tileX, tileY);
            if (Grids.ContainsKey(gridHash))
                return;

            var gridLoader = new GridMapLoader(Directory, MapID, tileX, tileY);
            if (gridLoader.FileExists)
                Grids[gridHash] = gridLoader;
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        public void Render(int centerTileX, int centerTileY, Camera camera)
        {
            const int MAX_CHUNK_DISTANCE = 0; /// Debugging

            var terrainProgram = ShaderProgramCache.Instance.Get("terrain");
            GL.UseProgram(terrainProgram.Program);
            var projModelView = Matrix4.Mult(camera.View, camera.Projection);
            GL.UniformMatrix4(terrainProgram.Uniforms["modelViewProjection"], false, ref projModelView);
            
            for (var i = centerTileY - MAX_CHUNK_DISTANCE; i <= centerTileY + MAX_CHUNK_DISTANCE; ++i)
                for (var j = centerTileX - MAX_CHUNK_DISTANCE; j <= centerTileX + MAX_CHUNK_DISTANCE; ++j)
                    if (!Grids.ContainsKey(PackTile(j, i)))
                        LoadTile(j, i);

            foreach (var mapGrid in Grids.Values)
                if (Math.Abs(centerTileX - mapGrid.X) <= MAX_CHUNK_DISTANCE && Math.Abs(centerTileY - mapGrid.Y) <= MAX_CHUNK_DISTANCE)
                    mapGrid.Render();
        }

        ~MapLoader()
        {
            Grids.Clear();
            Grids = null;
        }
    }
}
