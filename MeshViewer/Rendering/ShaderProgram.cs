using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Rendering
{
    public sealed class ShaderProgram
    {
        public int Program { get; private set; }

        private int VertexShader { get; set; }
        private int FragmentShader { get; set; }

        public Dictionary<string, int> Attributes { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Uniforms { get; } = new Dictionary<string, int>();

        public void AddVertexShader(string fileName)
        {
            var vertexShader = Shader.Compile(ShaderType.VertexShader, fileName);
            if (vertexShader != -1)
                VertexShader = vertexShader;
        }

        public void AddFragmentShader(string fileName)
        {
            var fragmentShader = Shader.Compile(ShaderType.FragmentShader, fileName);
            if (FragmentShader != -1)
                FragmentShader = fragmentShader;
        }

        public bool Link()
        {
            if (VertexShader <= 0 || FragmentShader <= 0)
                return false;

            Program = GL.CreateProgram();
            GL.AttachShader(Program, VertexShader);
            GL.AttachShader(Program, FragmentShader);
            GL.LinkProgram(Program);

            var logInfo = GL.GetProgramInfoLog(Program);
            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
                throw new InvalidOperationException(logInfo);

            GL.UseProgram(Program);
            GL.ValidateProgram(Program);

            GL.DetachShader(Program, VertexShader);
            GL.DeleteShader(VertexShader);

            GL.DetachShader(Program, FragmentShader);
            GL.DeleteShader(FragmentShader);

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

            return true;
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
