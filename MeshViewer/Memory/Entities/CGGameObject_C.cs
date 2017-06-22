using MeshViewer.Memory.Enums;
using System;
using System.ComponentModel;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGGameObject_C : CGObject_C
    {
        public CGGameObject_C(IntPtr offset) : base(offset)
        {
        }

        public override string ToString()
        {
            return $"GameObject: {OBJECT_FIELD_GUID} Name: {Name} Type: {ObjectType} Level: {GAMEOBJECT_LEVEL}";
        }

        #region General
        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public string Name => ReadCString(Read<IntPtr>(Read<IntPtr>(BaseAddress + 0x1CC) + 0xB4), 100);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public float X => Read<float>(0x110);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public float Y => Read<float>(0x114);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public float Z => Read<float>(0x118);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public CGUnit_C CreatedBy => Game.GetEntity<CGUnit_C>(OBJECT_FIELD_CREATED_BY);
        #endregion

        #region Descriptors
        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid OBJECT_FIELD_CREATED_BY=> GetUpdateField<ObjectGuid>(GameObjectFields.OBJECT_FIELD_CREATED_BY);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int GAMEOBJECT_DISPLAYID          => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_DISPLAYID);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int GAMEOBJECT_FLAGS              => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_FLAGS);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] GAMEOBJECT_PARENTROTATION => GetUpdateField<float>(GameObjectFields.GAMEOBJECT_PARENTROTATION, 4);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public short[] GAMEOBJECT_DYNAMIC        => GetUpdateField<short>(GameObjectFields.GAMEOBJECT_DYNAMIC, 2);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int GAMEOBJECT_FACTION            => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_FACTION);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int GAMEOBJECT_LEVEL              => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_LEVEL);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int GAMEOBJECT_BYTES_1            => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_BYTES_1);
        #endregion

        [Browsable(false)]
        public bool IsBobbing => Game.Read<byte>(0xD4) != 0;

        #region GameObject
        [Category("GameObject")]
        public byte State => (byte)((GAMEOBJECT_BYTES_1 >> 0) & 0xFF);

        [Category("GameObject")]
        public GameObjectType ObjectType => (GameObjectType)((GAMEOBJECT_BYTES_1 >> 8) & 0xFF);

        [Category("GameObject")]
        public byte ArtKit => (byte)((GAMEOBJECT_BYTES_1 >> 16) & 0xFF);

        [Category("GameObject")]
        public byte AnimProgress => (byte)((GAMEOBJECT_BYTES_1 >> 24) & 0xFF);
        #endregion

        protected enum GameObjectFields
        {
            OBJECT_FIELD_CREATED_BY   = ObjectFields.OBJECT_END + 0x0000, // Size: 2, Type: LONG, Flags: PUBLIC
            GAMEOBJECT_DISPLAYID      = ObjectFields.OBJECT_END + 0x0002, // Size: 1, Type: INT, Flags: PUBLIC
            GAMEOBJECT_FLAGS          = ObjectFields.OBJECT_END + 0x0003, // Size: 1, Type: INT, Flags: PUBLIC
            GAMEOBJECT_PARENTROTATION = ObjectFields.OBJECT_END + 0x0004, // Size: 4, Type: FLOAT, Flags: PUBLIC
            GAMEOBJECT_DYNAMIC        = ObjectFields.OBJECT_END + 0x0008, // Size: 1, Type: TWO_SHORT, Flags: DYNAMIC
            GAMEOBJECT_FACTION        = ObjectFields.OBJECT_END + 0x0009, // Size: 1, Type: INT, Flags: PUBLIC
            GAMEOBJECT_LEVEL          = ObjectFields.OBJECT_END + 0x000A, // Size: 1, Type: INT, Flags: PUBLIC
            GAMEOBJECT_BYTES_1        = ObjectFields.OBJECT_END + 0x000B, // Size: 1, Type: BYTES, Flags: PUBLIC
            GAMEOBJECT_END            = ObjectFields.OBJECT_END + 0x000C
        }
    }
}
