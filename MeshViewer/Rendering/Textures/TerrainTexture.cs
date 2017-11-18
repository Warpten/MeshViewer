using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace MeshViewer.Rendering.Textures
{
    public class TerrainTexture
    {
        private int _textureID;
        public int TextureID
        {
            get
            {
                if (_textureID == 0)
                    Generate();
                return _textureID;
            }
            private set
            {
                _textureID = value;
            }
        }


        public TerrainTexture()
        {
            TextureID = 0;
        }

        ~TerrainTexture()
        {
            // if (TextureID != 0)
            //     GL.DeleteTexture(TextureID);
        }

        public unsafe void Generate()
        {
            const int TEXTURE_SIZE = 256;

            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            var bitmap = new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE);
            for (var y = 0; y < TEXTURE_SIZE; ++y)
                for (var x = 0; x < TEXTURE_SIZE; ++x)
                    bitmap.SetPixel(x, y, (x == 0 || y == 0 || x == TEXTURE_SIZE - 1 || y == TEXTURE_SIZE - 1) ? Color.Black : Color.LightBlue);

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, TEXTURE_SIZE, TEXTURE_SIZE), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            // var minFilter = (int)All.LinearMipmapLinear;
            // var magFilter = (int)All.LinearMipmapLinear;
            // GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref minFilter);
            // GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref magFilter);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Bind()
        {
            if (TextureID == 0)
                Generate();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
        }
    }
}
