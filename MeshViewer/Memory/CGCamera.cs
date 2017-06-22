using MeshViewer.Memory.Offsets;
using OpenTK;
using System;

namespace MeshViewer.Memory
{
    public sealed class CGCamera
    {
        public IntPtr BaseAddress => Game.Read<IntPtr>(Game.Read<IntPtr>(Cataclysm.WorldFramePtr) + 0x80D0, true);

        public float X => Game.Read<float>(BaseAddress + 0x8, true);
        public float Y => Game.Read<float>(BaseAddress + 0xC, true);
        public float Z => Game.Read<float>(BaseAddress + 0x10, true);

        public Vector3 XYZ => new Vector3(X, Y, Z);

        public Matrix3 Matrix => Game.Read<Matrix3>(BaseAddress + 0x14, true);

        public float FoV => Game.Read<float>(BaseAddress + 0x38, true);

        public Vector3 Up      => Matrix.Row2;
        public Vector3 Right   => Matrix.Row1;
        public Vector3 Forward => Matrix.Row0;

        public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(FoV * 0.6f, AspectRatio, NearClip, FarClip);
        public Matrix4 View => Matrix4.LookAt(XYZ, XYZ + Forward, Vector3.UnitZ);

        public float AspectRatio { get; set; } = 800.0f / 600.0f;
        public float NearClip { get; set; } = 0.1f; //  Game.Read<float>(0xE8909C + 44); // CVar (nearClip)
        public float FarClip { get; set; } = 1000.0f; //  Game.Read<float>(0xE890A0 + 44); // CVar (farClip)

    }
}
