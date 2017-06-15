using System.Text;

namespace MeshViewer.Memory
{
    public struct ObjectGuid
    {
        public ulong Value { get; }

        bool IsEmpty()             => Value == 0L;
        bool IsCreature()          => GetHigh() == HighGuid.Unit;
        bool IsPet()               => GetHigh() == HighGuid.Pet;
        bool IsVehicle()           => GetHigh() == HighGuid.Vehicle;
        bool IsCreatureOrPet()     => IsCreature() || IsPet();
        bool IsCreatureOrVehicle() => IsCreature() || IsVehicle();
        bool IsAnyTypeCreature()   => IsCreature() || IsPet() || IsVehicle();
        bool IsPlayer()            => !IsEmpty() && GetHigh() == HighGuid.Player;
        bool IsUnit()              => IsAnyTypeCreature() || IsPlayer();
        bool IsItem()              => GetHigh() == HighGuid.Item;
        bool IsGameObject()        => GetHigh() == HighGuid.GameObject;
        bool IsDynamicObject()     => GetHigh() == HighGuid.DynamicObject;
        bool IsCorpse()            => GetHigh() == HighGuid.Corpse;
        bool IsAreaTrigger()       => GetHigh() == HighGuid.AreaTrigger;
        bool IsBattleground()      => GetHigh() == HighGuid.Battleground;
        bool IsTransport()         => GetHigh() == HighGuid.Transport;
        bool IsMOTransport()       => GetHigh() == HighGuid.MoTransport;
        bool IsAnyTypeGameObject() => IsGameObject() || IsTransport() || IsMOTransport();
        bool IsInstance()          => GetHigh() == HighGuid.Instance;
        bool IsGroup()             => GetHigh() == HighGuid.Group;
        bool IsGuild()             => GetHigh() == HighGuid.Guild;
        
        HighGuid GetHigh()
        {
            HighGuid temp = (HighGuid)((Value >> 48) & 0x0000FFFF);
            return (HighGuid)((temp == HighGuid.Corpse || temp == HighGuid.AreaTrigger) ? temp : (HighGuid)(((int) temp >> 4) & 0x00000FFF));
        }

        public uint GetEntry() => HasEntry() ? (uint)((Value >> 32) & 0x00000000000FFFFFL) : 0;
        public uint GetCounter() => (uint)(Value & 0x00000000FFFFFFFFL);

        public byte this[int index] => (byte)((Value >> (8 * index)) & 0xFF);

        public static implicit operator ulong(ObjectGuid guid) => guid.Value;

        private static bool HasEntry(HighGuid high)
        {
            switch (high)
            {
                case HighGuid.Item:
                case HighGuid.Player:
                case HighGuid.DynamicObject:
                case HighGuid.Corpse:
                case HighGuid.MoTransport:
                case HighGuid.Instance:
                case HighGuid.Group:
                    return false;
                case HighGuid.GameObject:
                case HighGuid.Transport:
                case HighGuid.Unit:
                case HighGuid.Pet:
                case HighGuid.Vehicle:
                default:
                    return true;
            }
        }

        public bool HasEntry() => HasEntry(GetHigh());

        public enum HighGuid
        {
            Item          = 0x0400, // blizz 4000
            Container     = 0x0400, // blizz 4000
            Player        = 0x0000, // blizz 0000
            GameObject    = 0x0F11, // blizz F110
            Transport     = 0x0F12, // blizz F120 (for GAMEOBJECT_TYPE_TRANSPORT)
            Unit          = 0x0F13, // blizz F130
            Pet           = 0x0F14, // blizz F140
            Vehicle       = 0x0F15, // blizz F550
            DynamicObject = 0x0F10, // blizz F100
            Corpse        = 0xF101, // blizz F100
            AreaTrigger   = 0xF102,
            Battleground  = 0x01F1,
            MoTransport   = 0x01FC, // blizz 1FC0 (for GAMEOBJECT_TYPE_MO_TRANSPORT)
            Instance      = 0x01F4, // blizz 1F40
            Group         = 0x01F5,
            Guild         = 0x01FF
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append($"GUID Full: 0x{Value:X16} Type: {GetHigh().ToString()}");
            if (HasEntry())
                str.Append(IsPet() ? $" Pet number: {GetEntry()}" : $" Entry: {GetEntry()}");
            str.Append($" Low: {GetCounter()}");
            return str.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ObjectGuid))
                return false;

            return Value == ((ObjectGuid)obj).Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
