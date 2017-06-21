using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Rendering
{
    public static class RenderingExtensions
    {
        /// <summary>
        /// Builds a Matrix4 from the given euler angles
        /// </summary>
        /// <param name="pitch">The pitch (attitude), rotation around X axis</param>
        /// <param name="yaw">The yaw (heading), rotation around Y axis</param>
        /// <param name="roll">The roll (bank), rotation around Z axis</param>
        /// <returns></returns>
        /// <summary>
        /// Construct a new Matrix4 from given Euler angles
        /// </summary>
        /// <param name="pitch">The pitch (attitude), rotation around X axis</param>
        /// <param name="yaw">The yaw (heading), rotation around Y axis</param>
        /// <param name="roll">The roll (bank), rotation around Z axis</param>
        public static Matrix4 FromEulerAngles(float pitch, float yaw, float roll)
        {
            var xRotation = Matrix4.CreateRotationX(pitch);
            var yRotation = Matrix4.CreateRotationY(yaw);
            var zRotation = Matrix4.CreateRotationZ(roll);

            return xRotation * (yRotation * zRotation);
        }

    }
}
