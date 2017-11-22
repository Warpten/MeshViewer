using MeshViewer.Geometry.Buildings;
using MeshViewer.Geometry.GameObjects;
using MeshViewer.Geometry.Terrain;
using MeshViewer.Rendering;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MeshViewer.Geometry
{
    public static class GeometryLoader
    {
        public static BuildingsLoader Buildings { get; private set; }
        public static TerrainLoader Terrain { get; private set; }
        public static GameObjectLoader GameObjects { get; private set; }

        public static FrameBuffer Buffer { get; private set; } = new FrameBuffer();

        public static bool Initialized => Buildings != null;

        public static void Initialize(string directory, int mapID)
        {
            Buildings = new BuildingsLoader(directory, mapID);
            Terrain = new TerrainLoader(directory, mapID);
            GameObjects = new GameObjectLoader(directory);
        }

        public static void LoadTile(int tileX, int tileY)
        {
            Terrain.LoadTile(tileX, tileY);
            Buildings.LoadTile(tileX, tileY);
        }

        public static void Render(int centerTileX, int centerTileY)
        {
            Buffer.Bind();

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Terrain.Render(centerTileX, centerTileY);
            Buildings.Render(centerTileX, centerTileY);

            GameObjects.Render();

            Buffer.RenderTexture();
        }
    }
}
