using MeshViewer.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MeshViewer.Geometry.Terrain
{
    public sealed class TerrainGridLoader : IndexedModel<float, int>
    {
        public int X { get; }
        public int Y { get; }
        public int MapID { get; }

        #region File data
        private ushort _gridArea;
        private ushort[] _areaMap;

        public float[] V9 { get; private set; }
        public float[] V8 { get; private set; }

        public ushort[] LiquidEntry { get; private set; }
        public byte[] LiquidFlags { get; private set; }
        public float[] LiquidMap { get; private set; }
        public MapLiquidHeader LiquidHeader { get; private set; }

        public ushort[] Holes { get; private set; }

        public bool FileExists { get; }
        public MapFileHeader FileHeader { get; }
        #endregion

        public TerrainGridLoader(string filePath, int mapID, int x, int y)
        {
            MapID = mapID;
            X = x;
            Y = y;

            Program = ShaderProgramCache.Instance.Get("terrain");

            var file = Path.Combine(filePath, $"{MapID:D3}{y:D2}{x:D2}.map");
            if (!(FileExists = File.Exists(file)))
                return;

            using (var reader = new BinaryReader(File.OpenRead(file)))
            {
                FileHeader = reader.Read<MapFileHeader>();

                // if (header.AreaMapOffset != 0 && !LoadAreaData(reader, header.AreaMapOffset, header.AreaMapSize))
                //     throw new InvalidOperationException();

                // if (FileHeader.LiquidMapOffset != 0 && !LoadLiquidData(reader, FileHeader.LiquidMapOffset, FileHeader.LiquidMapSize))
                //     throw new InvalidOperationException();

                if (FileHeader.HeightMapOffset != 0 && !LoadHeightData(reader, FileHeader.HeightMapOffset, FileHeader.HeightMapSize))
                    throw new InvalidOperationException();

                if (FileHeader.HolesOffset != 0 && !LoadHolesData(reader, FileHeader.HolesOffset, FileHeader.HolesSize))
                    throw new InvalidOperationException();
            }
        }

        ~TerrainGridLoader()
        {
            // Unload();
        }
    
        // public override void Unload()
        // {
        //     LiquidEntry = null;
        //     LiquidFlags = null;
        //     LiquidMap = null;
        //     Holes = null;
        // 
        //     V9 = null;
        //     V8 = null;
        // 
        //     _areaMap = null;
        // 
        //     base.Unload();
        // }

        private const int V9_SIZE = 129;
        private const int V9_SIZE_SQ = V9_SIZE * V9_SIZE;
        private const int V8_SIZE = 128;
        private const int V8_SIZE_SQ = V8_SIZE * V8_SIZE;
        private const float GRID_SIZE = 533.333313f;
        private const float GRID_PART_SIZE = GRID_SIZE / V8_SIZE;

        protected override bool BindData(ref float[] vertices, ref int[] indices)
        {
            vertices = new float[0];
            indices = new int[0];
            if (V9 == null || V8 == null)
                return false;

            var stackedVertices = new List<float>();
            var stackedIndices = new List<int>();

            var terrainVertices = new List<float>();
            var terrainIndices = new List<int>();

            var xOffset = (X - 32) * GRID_SIZE;
            var yOffset = (Y - 32) * GRID_SIZE;

            for (var index = 0; index < V9_SIZE_SQ; ++index)
            {
                var x = (xOffset + (index % V9_SIZE) * GRID_PART_SIZE) * -1.0f;
                var y = (yOffset + (index / V9_SIZE) * GRID_PART_SIZE) * -1.0f;
                terrainVertices.AddRange(new[] { y, x, V9[index] });
            }

            for (var index = 0; index < V8_SIZE_SQ; ++index)
            {
                var x = (xOffset + (index % V8_SIZE) * GRID_PART_SIZE + GRID_PART_SIZE / 2.0f) * -1.0f;
                var y = (yOffset + (index / V8_SIZE) * GRID_PART_SIZE + GRID_PART_SIZE / 2.0f) * -1.0f;
                terrainVertices.AddRange(new[] { y, x, V8[index] });
            }

            for (var square = 0; square < V8_SIZE_SQ; ++square)
            {
                for (var corner = 1; corner <= 4; ++corner)
                {
                    GetHeightTriangle(square, corner, out int[] terrainIndice);
                    terrainIndices.AddRange(terrainIndice);
                }
            }

            if (terrainIndices.Count == 0)
                return false;

            if (Holes.Length > 0 && Holes.Any(h => h != 0))
            {
                var terrainIndiceOffset = 0;

                ushort[] holetab_h = { 0x1111, 0x2222, 0x4444, 0x8888 };
                ushort[] holetab_v = { 0x000F, 0x00F0, 0x0F00, 0xF000 };

                for (int square = 0; square < V8_SIZE_SQ; ++square)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        var useTerrain = true /* stackedIndices.Count != 0 */;
                        if (useTerrain)
                        {
                            var row = square / 128;
                            var col = square % 128;
                            var cellRow = row / 8;     // 8 squares per cell
                            var cellCol = col / 8;
                            var holeRow = (row % 8) / 2;
                            var holeCol = (square - (row * 128 + cellCol * 8)) / 2;

                            var hole = Holes[cellRow * 16 + cellCol];

                            useTerrain = (hole & holetab_h[holeCol] & holetab_v[holeRow]) == 0;
                        }

                        if (useTerrain)
                        {
                            for (var k = 0; k < 3 * 4 / 2; ++k)
                                stackedIndices.Add(terrainIndices[k + terrainIndiceOffset]);
                        }

                        terrainIndiceOffset += 3 * 4 / 2;
                    }
                }
                stackedVertices.AddRange(terrainVertices);
            }
            else
            {
                stackedIndices.AddRange(terrainIndices);
                stackedVertices.AddRange(terrainVertices);
            }

            CleanVertices(stackedIndices, stackedVertices);

            for (var i = 0; i < stackedIndices.Count; i += 3)
            {
                var tmp = stackedIndices[i + 1];
                stackedIndices[i + 1] = stackedIndices[i + 2];
                stackedIndices[i + 2] = tmp;
            }

            vertices = stackedVertices.ToArray();
            indices = stackedIndices.ToArray();
            
            return true;
        }

        private void CleanVertices(List<int> tris, List<float> verts)
        {
            var cleanVerts = new List<float>();
            var cleanIndices = new List<int>();
            var indiceMap = new Dictionary<int, int>();

            // 1. Iterate all the old indices.
            //    a. If that indice has never been seen before, push the vertice to the new list
            //       as well as the indice, and update the new value.
            //    b. If that indice has already been seen, push the indice.

            var count = 0;
            for (var i = 0; i < tris.Count; ++i)
            {
                var oldIndice = tris[i];

                if (!indiceMap.ContainsKey(oldIndice))
                {
                    indiceMap[oldIndice] = count++;

                    cleanVerts.AddRange(new[] {
                        verts[oldIndice * 3 + 0],
                        verts[oldIndice * 3 + 1],
                        verts[oldIndice * 3 + 2],
                    });
                }
                cleanIndices.Add(indiceMap[oldIndice]);
            }

            verts = cleanVerts;
            tris = cleanIndices;
        }

        private void GetHeightTriangle(int square, int corner, out int[] terrainIndice)
        {
            var rowOffset = square / V8_SIZE;
            terrainIndice = new int[3];
            switch (corner)
            {
                case 1: // Top
                    terrainIndice[2] = square + rowOffset;               //  0-----1 .... 128
                    terrainIndice[0] = square + rowOffset + 1;           //  |\ T /|
                    terrainIndice[1] = V9_SIZE_SQ + square;              //  | \ / |
                    break;                                               //  |L 0 R| .. 127
                case 3: // Left                                          //  | / \ |
                    terrainIndice[2] = square + rowOffset;               //  |/ B \|
                    terrainIndice[0] = V9_SIZE_SQ + square;              // 129---130 ... 386
                    terrainIndice[1] = square + V9_SIZE + rowOffset;     //  |\   /|
                    break;                                               //  | \ / |
                case 2: // Right                                         //  | 128 | .. 255
                    terrainIndice[2] = square + 1 + rowOffset;           //  | / \ |
                    terrainIndice[0] = square + V9_SIZE + rowOffset + 1; //  |/   \|
                    terrainIndice[1] = V9_SIZE_SQ + square;              // 258---259 ... 515
                    break;
                case 4: // Bottom
                    terrainIndice[2] = V9_SIZE_SQ + square;
                    terrainIndice[0] = square + V9_SIZE + 1 + rowOffset;
                    terrainIndice[1] = square + V9_SIZE + rowOffset;
                    break;
                default:
                    break;
            }
        }

        #region Loading files
        private bool LoadHolesData(BinaryReader reader, uint offset, uint size)
        {
            reader.BaseStream.Position = offset;
            Holes = reader.Read<ushort>((int)size);
            return true;
        }

        private bool LoadAreaData(BinaryReader reader, uint offset, uint size)
        {
            reader.BaseStream.Position = offset;
            var header = reader.Read<MapAreaHeader>();

            _gridArea = header.GridArea;

            if ((header.Flags & 0x0001) == 0) // MAP_AREA_NO_AREA
            {
                _areaMap = reader.Read<ushort>(16 * 16);
                return _areaMap.Length == 16 * 16;
            }

            return true;
        }

        private bool LoadHeightData(BinaryReader reader, uint offset, uint size)
        {
            reader.BaseStream.Position = offset;
            var header = reader.Read<MapHeightHeader>();
            
            if ((header.Flags & 0x0001) == 0) // MAP_HEIGHT_NO_HEIGHT
            {
                if ((header.Flags & 0x0002) != 0) // MAP_HEIGHT_AS_INT16
                {
                    var gridHeightMultiplier = (header.GridMaxHeight - header.GridHeight) / 65535;
                    V9 = reader.Read<ushort>(129 * 129).Select(v => v * gridHeightMultiplier + header.GridHeight).ToArray();
                    V8 = reader.Read<ushort>(128 * 128).Select(v => v * gridHeightMultiplier + header.GridHeight).ToArray();
                }
                else if ((header.Flags & 0x0004) != 0) // MAP_HEIGHT_AS_INT8
                {
                    var gridHeightMultiplier = (header.GridMaxHeight - header.GridHeight) / 255;
                    V9 = reader.ReadBytes(129 * 129).Select(v => v * gridHeightMultiplier + header.GridHeight).ToArray();
                    V9 = reader.ReadBytes(128 * 128).Select(v => v * gridHeightMultiplier + header.GridHeight).ToArray();
                }
                else
                {
                    V9 = reader.Read<float>(129 * 129);
                    V8 = reader.Read<float>(128 * 128);
                }
            }

            return true;
        }

        private bool LoadLiquidData(BinaryReader reader, uint offset, uint size)
        {
            reader.BaseStream.Position = offset;
            LiquidHeader = reader.Read<MapLiquidHeader>();

            if ((LiquidHeader.Flags & 0x0001) == 0) // MAP_LIQUID_NO_TYPE
            {
                LiquidEntry = reader.Read<ushort>(16 * 16);
                LiquidFlags = reader.ReadBytes(16 * 16);
            }

            if ((LiquidHeader.Flags & 0x0002) == 0) // MAP_LIQUID_NO_HEIGHT
            {
                LiquidMap = reader.Read<float>(LiquidHeader.Height * LiquidHeader.Width);
            }

            return true;
        }

        public struct MapLiquidHeader
        {
            public int FourCC { get; set; }
            public ushort Flags { get; set; }
            public ushort LiquidType { get; set; }
            public byte OffsetX { get; set; }
            public byte OffsetY { get; set; }
            public byte Width { get; set; }
            public byte Height { get; set; }
            public float LiquidLevel { get; set; }
        }

        private struct MapAreaHeader
        {
            public uint FourCC { get; set; }
            public ushort Flags { get; set; }
            public ushort GridArea { get; set; }
        }

        private struct MapHeightHeader
        {
            public uint FourCC { get; set; }
            public uint Flags { get; set; }
            public float GridHeight { get; set; }
            public float GridMaxHeight { get; set; }
        }

        public struct MapFileHeader
        {
            public uint MapMagic { get; set; }
            public uint VersionMagic { get; set; }
            public uint BuildMagic { get; set; }
            public uint AreaMapOffset { get; set; }
            public uint AreaMapSize { get; set; }
            public uint HeightMapOffset { get; set; }
            public uint HeightMapSize { get; set; }
            public uint LiquidMapOffset { get; set; }
            public uint LiquidMapSize { get; set; }
            public uint HolesOffset { get; set; }
            public uint HolesSize { get; set; }
        }
        #endregion
    }
}
