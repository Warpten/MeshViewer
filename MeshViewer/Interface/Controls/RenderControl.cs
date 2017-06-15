using System;
using MeshViewer.Geometry;
using MeshViewer.Rendering;
using MeshViewer.Rendering.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using MeshViewer.Rendering.Textures;
using MeshViewer.Memory;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MeshViewer.Interface.Controls
{
    public class RenderControl : GLControl
    {
        public Camera Camera { get; private set; }
        public ShaderProgram Program { get; private set; }
        private TerrainTexture Texture { get; }
        
        private int VAO;
        private int VBO;
        private int VerticeCount;

        public RenderControl() : this(new GraphicsMode(32, 24), 2, 0, GraphicsContextFlags.Debug)
        {
            
        }

        public RenderControl(GraphicsMode mode) : this(mode, 2, 0, GraphicsContextFlags.Debug)
        {

        }

        public RenderControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags) : base(mode, major, minor, flags)
        {
            if (DesignMode)
                return;

            Resize += (sender, args) =>
            {
                GL.Viewport(0, 0, Width, Height);
                Camera?.SetViewport(Width, Height);
            };

            Context.MakeCurrent(null);
            MakeCurrent();
            Context.ErrorChecking = true;

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            SwapBuffers();

            Program = new ShaderProgram();
            Program.AddVertexShader("./shaders/vertex.shader");
            Program.AddFragmentShader("./shaders/fragment.shader");
            Program.Link();

            Texture = new TerrainTexture();
        }

        public void SetCamera(float x, float y, float z, float facing)
        {
            Camera = new Camera(new Vector3(x, y, z), new Vector3((float)Math.Cos(facing), (float)Math.Sin(facing), 0),  Width, Height);
            Camera.OnMovement += Update;

            Invalidate();
        }

        public async void LoadMap(int mapID)
        {
            var vertices = await Task.Factory.StartNew(() => {
                return new GeometryLoader(@"D:\Repositories\omfg.gg\Build\bin\RelWithDebInfo", mapID).Vertices;
            });
            VerticeCount = vertices.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * SizeCache<Vector3>.Size, vertices, BufferUsageHint.StaticDraw);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeCache<Vector3>.Size, 0);
            GL.EnableVertexAttribArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode)
                return;

            SetView();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureID);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticeCount); // I don't even know what the proper primitive is, but whatever

            SwapBuffers();
        }

        private void SetView()
        {
            if (Camera == null)
                return;

            var projModelView = Matrix4.Mult(Camera.View, Camera.Projection);
            GL.UniformMatrix4(Program.Uniforms["modelViewProjection"], false, ref projModelView);
        }
    }
}
