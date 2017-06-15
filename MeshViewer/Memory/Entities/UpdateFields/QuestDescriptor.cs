using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MeshViewer.Memory.Entities.UpdateFields
{
    [StructLayout(LayoutKind.Sequential)]
    public struct QuestDescriptor
    {
        public int QuestID;
        public int State;
        public short CounterA;
        public short UnkShort0;
        public short CounterB;
        public short UnkShort1;
        public int Time;

        public override string ToString()
        {
            return $"Quest ID: {QuestID}";
        }
    }
}
