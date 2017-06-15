using MeshViewer.Geometry.Model;
using MeshViewer.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace MeshViewer.Geometry.Vmap
{
    public sealed class VMapLoader
    {
        public bool IsTiled { get; }
        private BIH BIH { get; }
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
                    BIH = new BIH(reader);
                
                if (!IsTiled && reader.ReadInt32() == 0x4A424F47) // GOBJ
                    GlobalModel = new ModelSpawn(directory, reader);
            }
        }

        public void LoadTile(int tileX, int tileY)
        {
            var gridHash = PackTile(tileX, tileY);
            if (Grids.ContainsKey(gridHash))
                return;

            var gridLoader = new VMapTileLoader(Directory, MapID, tileX, tileY);
            if (gridLoader.FileExists)
                Grids[gridHash] = gridLoader;
        }

        private int PackTile(int x, int y) => ((x & 0xFF) << 8) | (y & 0xFF);

        public void Render(int centerTileX, int centerTileY, Camera camera)
        {
            if (IsTiled)
            {
                var wmoProgram = ShaderProgramCache.Instance.Get("wmo");
                GL.UseProgram(wmoProgram.Program);
                var view = Matrix4.Mult(camera.View, camera.Projection);
                GL.UniformMatrix4(wmoProgram.Uniforms["projection_view"], false, ref view);

                const int MAX_CHUNK_DISTANCE = 0; /// Debugging

                for (var i = centerTileY - MAX_CHUNK_DISTANCE; i <= centerTileY + MAX_CHUNK_DISTANCE; ++i)
                    for (var j = centerTileX - MAX_CHUNK_DISTANCE; j <= centerTileX + MAX_CHUNK_DISTANCE; ++j)
                        if (!Grids.ContainsKey(PackTile(j, i)))
                            LoadTile(j, i);

                foreach (var mapGrid in Grids.Values)
                {
                    if (!(Math.Abs(centerTileX - mapGrid.X) <= MAX_CHUNK_DISTANCE && Math.Abs(centerTileY - mapGrid.Y) <= MAX_CHUNK_DISTANCE))
                        continue;

                    mapGrid.Render(camera);
                }
            }
            //else
                //GlobalModel.Render();
        }
    }
}
