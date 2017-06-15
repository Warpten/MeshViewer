using OpenTK;
using System;

namespace MeshViewer.Memory
{
    public sealed class CGCamera_C
    {
        private Process _game;

        public CGCamera_C(Process game)
        {
            _game = game;
        }

        public IntPtr BaseAddress => _game.Read<IntPtr>(_game.Read<IntPtr>(0xAD7A10) + 0x80D0, true);

        public float X => _game.Read<float>(BaseAddress + 0x8, true);
        public float Y => _game.Read<float>(BaseAddress + 0xC, true);
        public float Z => _game.Read<float>(BaseAddress + 0x10, true);

        public Matrix3 Matrix => _game.Read<Matrix3>(BaseAddress + 0x14, true);

        public float FoV => _game.Read<float>(BaseAddress + 0x40, true);
    }
}
