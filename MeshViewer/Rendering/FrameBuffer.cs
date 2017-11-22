using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using MeshViewer.Memory;
using OpenTK;

namespace MeshViewer.Rendering
{
    public class FrameBuffer
    {
        private int _handle;
        private int _colorBufferHandle;
        private int _depthBufferHandle;
        private int _surfaceVAO;
        private int _surfaceVBO;

        private int _width, _height;

        private bool _dirty;

        public int Width
        {
            get => _width;
            set {
                _width = value;

                _dirty = true;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;

                _dirty = true;
            }
        }

        public int Texture => _colorBufferHandle;
        public int Depth => _depthBufferHandle;

        public void Bind()
        {
            if (!_dirty)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
                return;
            }

            // Delete the previous framebuffer if it existed
            if (GL.IsFramebuffer(_handle))
                GL.DeleteFramebuffer(_handle);

            _handle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);

            if (!GL.IsTexture(_colorBufferHandle))
                _colorBufferHandle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2DMultisample, _colorBufferHandle);
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 4, PixelInternalFormat.Rgb, _width, _height, true); //  0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            // GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            // GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _colorBufferHandle, 0);

            if (!GL.IsRenderbuffer(_depthBufferHandle))
                _depthBufferHandle = GL.GenRenderbuffer();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthBufferHandle);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.Depth32fStencil8, _width, _height);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _depthBufferHandle);
            
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new InvalidOperationException();

            _dirty = false;
        }

        public void RenderTexture()
        {
            var screenShader = ShaderProgramCache.Instance.Get("quad");
            screenShader.Use();
            screenShader.UniformInteger("screenTexture", 0);

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // GL.BindVertexArray(_surfaceVAO);
            // GL.Disable(EnableCap.DepthTest);
            // GL.BindTexture(TextureTarget.Texture2D, _colorBufferHandle);
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _colorBufferHandle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
        }

        public FrameBuffer()
        {
            _dirty = true;

            // Initialize the render data for the texture
            _surfaceVAO = GL.GenVertexArray();
            _surfaceVBO = GL.GenBuffer();

            GL.BindVertexArray(_surfaceVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _surfaceVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_textureVertices.Length * SizeCache<Vector4>.Size), _textureVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, SizeCache<Vector4>.Size, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, SizeCache<Vector4>.Size, SizeCache<float>.Size * 2);
        }

        ~FrameBuffer()
        {
            // if (!GL.IsFramebuffer(_handle))
            //     return;
            // 
            // GL.DeleteFramebuffer(_handle);
            // 
            // GL.DeleteTexture(_colorBufferHandle);
            // GL.DeleteRenderbuffer(_depthBufferHandle);
        }

        private static Vector4[] _textureVertices = { // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
            new Vector4(-1.0f,  1.0f,  0.0f, 1.0f),
            new Vector4(-1.0f, -1.0f,  0.0f, 0.0f),
            new Vector4( 1.0f, -1.0f,  1.0f, 0.0f),

            new Vector4(-1.0f,  1.0f,  0.0f, 1.0f),
            new Vector4( 1.0f, -1.0f,  1.0f, 0.0f),
            new Vector4( 1.0f,  1.0f,  1.0f, 1.0f),
        };
    }
}
