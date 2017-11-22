using MeshViewer.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeshViewer.Rendering
{
    public sealed class ShaderProgram
    {
        public int Program { get; private set; }

        private Dictionary<ShaderType, int> _shaders = new Dictionary<ShaderType, int>();

        public Dictionary<string, int> Attributes { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Uniforms { get; } = new Dictionary<string, int>();

        public void AddShader(ShaderType shaderType, string fileName)
        {
            var shader = Shader.Compile(shaderType, fileName);
            if (shader != -1)
                _shaders[shaderType] = shader;
        }

        public bool Link()
        {
            if (!_shaders.ContainsKey(ShaderType.FragmentShader) || !_shaders.ContainsKey(ShaderType.VertexShader))
                return false;

            Program = GL.CreateProgram();
            foreach (var kv in _shaders)
                GL.AttachShader(Program, kv.Value);
            GL.LinkProgram(Program);

            var logInfo = GL.GetProgramInfoLog(Program);
            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
                throw new InvalidOperationException(logInfo);

            GL.UseProgram(Program);
            GL.ValidateProgram(Program);

            GL.GetProgram(Program, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            for (var i = 0; i < attributeCount; ++i)
            {
                var sb = new StringBuilder(255);
                GL.GetActiveAttrib(Program, i, 255, out int length, out int size, out ActiveAttribType attributeType, sb);
                Attributes[sb.ToString()] = GL.GetAttribLocation(Program, sb.ToString());
            }

            GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            for (var i = 0; i < uniformCount; ++i)
            {
                GL.GetActiveUniform(Program, i, out int size, out ActiveUniformType uniformType);
                var uniformName = GL.GetActiveUniformName(Program, i);
                Uniforms[GL.GetActiveUniformName(Program, i)] = GL.GetUniformLocation(Program, uniformName);
            }

            foreach (var shader in _shaders)
            {
                GL.DetachShader(Program, shader.Value);
                GL.DeleteShader(shader.Value);
            }

            _shaders.Clear();

            return true;
        }

        public void VertexAttribPointer<T>(string attributeName, int size, VertexAttribPointerType type, bool normalized = false, int offset = 0) where T : struct
        {
            var attribute = Attributes[attributeName];

            if (typeof(T) == typeof(Matrix4))
            {
                GL.VertexAttribPointer(attribute + 0, 4, type, normalized, SizeCache<T>.Size, offset + 0 * SizeCache<Vector4>.Size);
                GL.VertexAttribPointer(attribute + 1, 4, type, normalized, SizeCache<T>.Size, offset + 1 * SizeCache<Vector4>.Size);
                GL.VertexAttribPointer(attribute + 2, 4, type, normalized, SizeCache<T>.Size, offset + 2 * SizeCache<Vector4>.Size);
                GL.VertexAttribPointer(attribute + 3, 4, type, normalized, SizeCache<T>.Size, offset + 3 * SizeCache<Vector4>.Size);
            }
            else
                GL.VertexAttribPointer(attribute, size, type, normalized, SizeCache<T>.Size, offset);
        }

        public void UniformMatrix4(string uniformName, bool transpose, ref Matrix4 matrix)
        {
            GL.UniformMatrix4(Uniforms[uniformName], false, ref matrix);
        }

        public void UniformVector3(string uniformName, ref Vector3 vector)
        {
            GL.Uniform3(Uniforms[uniformName], ref vector);
        }

        public void UniformVector2(string uniformName, ref Vector2 vector)
        {
            GL.Uniform2(Uniforms[uniformName], ref vector);
        }

        public void UniformInteger(string uniformName, int value)
        {
            GL.Uniform1(Uniforms[uniformName], value);
        }

        public void Use() => GL.UseProgram(Program);

        public void EnableVertexAttribArray<T>(string attributeName) where T : struct
        {
            var attribute = Attributes[attributeName];
            if (typeof(T) == typeof(Matrix4))
            {
                GL.EnableVertexAttribArray(attribute);
                GL.EnableVertexAttribArray(attribute + 1);
                GL.EnableVertexAttribArray(attribute + 2);
                GL.EnableVertexAttribArray(attribute + 3);
            }
            else
                GL.EnableVertexAttribArray(attribute);
        }

        public void VertexAttribDivisor<T>(string attributeName, int divisor) where T : struct
        {
            var attribute = Attributes[attributeName];
            if (typeof(T) == typeof(Matrix4))
            {
                GL.VertexAttribDivisor(attribute + 0, divisor);
                GL.VertexAttribDivisor(attribute + 1, divisor);
                GL.VertexAttribDivisor(attribute + 2, divisor);
                GL.VertexAttribDivisor(attribute + 3, divisor);
            }
            else
                GL.VertexAttribDivisor(attribute, divisor);
        }
    }

    public class ShaderProgramCache
    {
        private Dictionary<string, ShaderProgram> _programs = new Dictionary<string, ShaderProgram>();

        public ShaderProgram Get(string programName) => _programs[programName];
        public void Add(string programName, ShaderProgram program) => _programs[programName] = program;

        public static ShaderProgramCache Instance { get; } = new ShaderProgramCache();
    }
}
