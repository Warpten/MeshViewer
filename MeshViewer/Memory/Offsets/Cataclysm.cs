namespace MeshViewer.Memory.Offsets
{
    public static class Cataclysm
    {
        // 8B 34 8A 8B 0D ?? ?? ?? ?? 89 81 ?? ?? ?? ?? 8B 15 ?? ?? ?? ??
        // http://i.imgur.com/LIGX6AY.png
        public static int CurMgrPointer = 0x9BE7E0;
        public static int CurMgrOffset = 0x463C;

        public static int CurMapoffset = 0xD4;

        // Script_IsLoggedIn
        public static int OnLoginScreen = 0x00ED7427;

        // http://i.imgur.com/0hBzvZD.png
        // Script_GetPlayerFacing
        public static int WorldFramePtr = 0xAD7A10;
        public static int CurrentCameraOffset = 0x80D0;
    }
}
