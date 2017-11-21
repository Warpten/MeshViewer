using System;

namespace MeshViewer.Memory.Enums
{
    [Flags]
    public enum GameObjectFlags : uint
    {
        InUse             = 0x00000001,
        Locked            = 0x00000002,
        InteractCondition = 0x00000004,
        Transport         = 0x00000008,
        NotSelectable     = 0x00000010,
        Triggered         = 0x00000020,
        Unknown7          = 0x00000040,
        Unknown8          = 0x00000080,
        Unknown9          = 0x00000100,
        Damaged           = 0x00000200,
        Destroyed         = 0x00000400,
        Unknown12         = 0x00000800,
        Unknown13         = 0x00001000,
        Unknown14         = 0x00002000,
        Unknown15         = 0x00004000,
        Unknown16         = 0x00008000,
        Unknown17         = 0x00010000,
        Unknown18         = 0x00020000,
        Unknown19         = 0x00040000,
        Unknown20         = 0x00080000,
        Unknown21         = 0x00100000,
        Unknown22         = 0x00200000,
        Unknown23         = 0x00400000,
        Unknown24         = 0x01000000,
        Unknown25         = 0x02000000,
        Unknown26         = 0x04000000,
        Unknown27         = 0x08000000,
        Unknown28         = 0x10000000,
        Unknown29         = 0x20000000,
        Unknown30         = 0x40000000,
        Unknown31         = 0x80000000,
    }
}
