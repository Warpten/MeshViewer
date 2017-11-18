using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace MeshViewer.Rendering
{
    public static class Shader
    {
        public static int Compile(ShaderType shaderType, string shaderFile)
        {
            if (!File.Exists(shaderFile))
                return -1;

            var Resource = GL.CreateShader(shaderType);
            var shaderSource = File.ReadAllText(shaderFile);
            GL.ShaderSource(Resource, shaderSource);
            GL.CompileShader(Resource);
            GL.GetShader(Resource, ShaderParameter.CompileStatus, out int shaderStatus);
            if (shaderStatus == 0)
            {
                GL.GetShaderInfoLog(Resource, out string shaderInfoLog);
                throw new InvalidOperationException(string.Format("Error while compiling {0}: {1}", shaderType, shaderInfoLog));
            }

            return Resource;
        }
    }
}
