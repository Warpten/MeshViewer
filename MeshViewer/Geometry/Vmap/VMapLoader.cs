using MeshViewer.Geometry.Model;
using MeshViewer.Memory;
using MeshViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MeshViewer.Geometry.Vmap
{
    public sealed class VMapLoader
    {
        public bool IsTiled { get; }
        // private BIH BIH { get; }
        public int MapID { get; }

        public ModelSpawn GlobalModel { get; }

        private Dictionary<int, VMapTileLoader> Grids { get; } = new Dictionary<int, VMapTileLoader>();

        private string Directory { get; }

        public VMapLoader(string directory, int mapID)
        {
            Directory = directory;

            using (var reader = new BinaryReader(File.OpenRead(Path.Combine(directory, "vmaps", $"{mapID:D3}.vmtree"))))
            {
                if (reader == null)
                    return;

                MapID = mapID;

                reader.BaseStream.Position += 8;
                IsTiled = reader.ReadByte() == 1;

                if (reader.ReadInt32() == 0x45444F4E) // NODE
                    BIH.Skip(reader);
                
                if (!IsTiled && reader.ReadInt32() == 0x4A424F47) // GOBJ
                    GlobalModel = new ModelSpawn(directory, reader);
            }
        }

        public void LoadTile(int tileX, int tileY)
        {
            var gridHash = PackTile(tileX, tileY);
            if (Grids.ContainsKey(gridHash))
                return;

            // Placeholder until loaded.
            Grids[gridHash] = null;
            Task.Factory.StartNew(() =>
            {
                var gridLoader = new VMapTileLoader(Directory, MapID, tileX, tileY);
                if (gridLoader.FileExists)
                    lock (Grids) Grids[gridHash] = gridLoader;
            });
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        static Vector3 WMO_COLOR = new Vector3(0.0f, 0.7f, 0.0f);
        public void Render(int centerTileX, int centerTileY)
        {
            if (IsTiled)
            {
                var wmoProgram = ShaderProgramCache.Instance.Get("wmo");
                var view = Matrix4.Mult(Game.Camera.View, Game.Camera.Projection);
                var cameraDirection = Game.Camera.Forward;

                wmoProgram.Use();
                wmoProgram.UniformMatrix4("projection_view", false, ref view);
                wmoProgram.UniformVector3("camera_direction", ref cameraDirection);
                wmoProgram.UniformVector3("object_color", ref WMO_COLOR);

                const int MAX_CHUNK_DISTANCE = 1; /// Debugging

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
                    {
                        if (mapGrid == null || !(Math.Abs(centerTileX - mapGrid.X) <= MAX_CHUNK_DISTANCE && Math.Abs(centerTileY - mapGrid.Y) <= MAX_CHUNK_DISTANCE))
                            continue;

                        mapGrid.Render();
                    }
                }
            }
            //else
                //GlobalModel.Render();
        }
    }
}
