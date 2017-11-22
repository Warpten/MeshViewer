using System;
using OpenTK.Graphics.OpenGL;

namespace MeshViewer.Rendering
{
    public class FrameBuffer
    {
        private int _handle;
        private int _colorBufferHandle;
        private int _depthBufferHandle;

        private int _width, _height;
        private int _sampleCount = 4;

        private bool _dirty;

        /// <summary>
        /// Returns the width of the underlying texture and depth buffers.
        /// </summary>
        public int Width
        {
            get => _width;
            set {
                _width = value;

                _dirty = true;
            }
        }

        /// <summary>
        /// Returns the height of the underlying texture and depth buffers.
        /// </summary>
        public int Height
        {
            get => _height;
            set
            {
                _height = value;

                _dirty = true;
            }
        }

        /// <summary>
        /// Returns the amount of samples used in an anti-aliasing operations.
        /// </summary>
        public int SampleSize
        {
            get => _sampleCount;
            set
            {
                _sampleCount = value;
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
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, _sampleCount, PixelInternalFormat.Rgb, _width, _height, true);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _colorBufferHandle, 0);

            if (!GL.IsRenderbuffer(_depthBufferHandle))
                _depthBufferHandle = GL.GenRenderbuffer();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthBufferHandle);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, _sampleCount, RenderbufferStorage.Depth32fStencil8, _width, _height);

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

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _colorBufferHandle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
        }

        public FrameBuffer()
        {
            _dirty = true;
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
    }
}
