using MeshViewer.Data;
using MeshViewer.Data.Structures;
using MeshViewer.Geometry;
using MeshViewer.Memory.Enums;
using MeshViewer.Memory.Enums.UpdateFields;
using OpenTK;
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
        [Category("General")]
        public string Name => ReadCString(Read<IntPtr>(Read<IntPtr>(BaseAddress + 0x1CC) + 0xB4), 100);

        [Category("General")]
        public float X => Read<float>(BaseAddress + 0x110);

        [Category("General")]
        public float Y => Read<float>(BaseAddress + 0x114);

        [Category("General")]
        public float Z => Read<float>(BaseAddress + 0x118);

        [Category("General")]
        public CGUnit_C CreatedBy => Game.GetEntity<CGUnit_C>(OBJECT_FIELD_CREATED_BY);
        #endregion

        #region Descriptors
        [Category("GameObject Descriptors")]
        public ObjectGuid OBJECT_FIELD_CREATED_BY => GetUpdateField<ObjectGuid>(GameObjectFields.OBJECT_FIELD_CREATED_BY);

        [Browsable(false)]
        public int GAMEOBJECT_DISPLAYID          => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_DISPLAYID);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] GAMEOBJECT_PARENTROTATION => GetUpdateField<float>(GameObjectFields.GAMEOBJECT_PARENTROTATION, 4);

        [Category("GameObject Descriptors"), RefreshProperties(RefreshProperties.All)]
        public short[] GAMEOBJECT_DYNAMIC        => GetUpdateField<short>(GameObjectFields.GAMEOBJECT_DYNAMIC, 2);

        [Category("GameObject Descriptors")]
        public int GAMEOBJECT_FACTION            => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_FACTION);

        [Category("GameObject Descriptors")]
        public int GAMEOBJECT_LEVEL              => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_LEVEL);

        [Browsable(false)]
        public int GAMEOBJECT_BYTES_1            => GetUpdateField<int>(GameObjectFields.GAMEOBJECT_BYTES_1);
        #endregion

        [Browsable(false)]
        public bool IsBobbing => Game.Read<byte>(0xD4) != 0;
        
        public override CGGameObject_C ToGameObject() => this;

        public override void OnSpawn()
        {
            if (GeometryLoader.GameObjects == null || _spawned)
                return;

            _spawned = GeometryLoader.GameObjects.AddInstance(this);
        }

        public override void OnUpdate()
        {
            if (GeometryLoader.GameObjects == null || _spawned)
                return;

            _spawned = GeometryLoader.GameObjects.AddInstance(this);
        }

        public override void OnDespawn()
        {
            if (GeometryLoader.GameObjects == null || !_spawned)
                return;

            if (GeometryLoader.GameObjects.RemoveInstance(this))
                _spawned = false;
        }

        #region Browsable properties
        [Category("GameObject"), RefreshProperties(RefreshProperties.All)]
        public GameObjectFlags Flags => GetUpdateField<GameObjectFlags>(GameObjectFields.GAMEOBJECT_FLAGS);

        [Category("GameObject"), RefreshProperties(RefreshProperties.All)]
        public byte State => (byte)((GAMEOBJECT_BYTES_1 >> 0) & 0xFF);

        [Category("GameObject"), RefreshProperties(RefreshProperties.All)]
        public GameObjectType ObjectType => (GameObjectType)((GAMEOBJECT_BYTES_1 >> 8) & 0xFF);

        [Category("GameObject"), RefreshProperties(RefreshProperties.All)]
        public byte ArtKit => (byte)((GAMEOBJECT_BYTES_1 >> 16) & 0xFF);

        [Category("GameObject"), RefreshProperties(RefreshProperties.All)]
        public byte AnimProgress => (byte)((GAMEOBJECT_BYTES_1 >> 24) & 0xFF);
        
        [Category("GameObject")]
        public GameObjectDisplayInfoEntry DisplayInfo => GAMEOBJECT_DISPLAYID != 0 && DBC.GameObjectDisplayInfo != null ? DBC.GameObjectDisplayInfo[GAMEOBJECT_DISPLAYID] : null;
        #endregion

        public Matrix4 PositionMatrix => Read<Matrix4>(BaseAddress + 0x1D0);
        private bool _spawned = false;
    }
}
