using MeshViewer.Memory.Enums;
using MeshViewer.Memory.Enums.UpdateFields;
using System;
using System.ComponentModel;
using System.Text;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGObject_C
    {
        [Browsable(false)]
        public IntPtr BaseAddress { get; private set; }

        private IntPtr _updateFields;

        public CGObject_C(IntPtr offset)
        {
            BaseAddress = offset;

            Refresh();
        }

        public void Refresh()
        {
            _updateFields = Read<IntPtr>(BaseAddress + 0xC);
        }

        #region Memory reading
        protected T[] ReadArray<T>(IntPtr offset, int count) where T : struct => Game.ReadArray<T>(offset, count, true);
        protected T[] ReadArray<T>(int offset, int count) where T : struct => Game.ReadArray<T>(new IntPtr(offset), count, true);
        protected T Read<T>(int offset) where T : struct => Game.Read<T>(offset, true);
        protected T Read<T>(IntPtr offset) where T : struct => Game.Read<T>(offset, true);
        protected string ReadCString(IntPtr offset, int maxLength, Encoding encoding) => Game.ReadCString(offset, maxLength, encoding, true);
        protected string ReadCString(IntPtr offset, int maxLength) => Game.ReadCString(offset, maxLength, Encoding.UTF8, true);
        #endregion

        #region Updatefield readers
        protected T GetUpdateField<T>(int offset) where T : struct => Read<T>(_updateFields + offset * 4);
        protected T GetUpdateField<T>(Enum field) where T : struct => GetUpdateField<T>((int)Convert.ChangeType(field, typeof(int)));

        protected T[] GetUpdateField<T>(int offset, int size) where T : struct => ReadArray<T>(_updateFields + offset * 4, size);
        protected T[] GetUpdateField<T>(Enum field, int size) where T : struct => GetUpdateField<T>((int)Convert.ChangeType(field, typeof(int)), size);
        #endregion

        #region Descriptors
        [Browsable(false)]
        public ObjectGuid OBJECT_FIELD_GUID    => GetUpdateField<ObjectGuid>(ObjectFields.OBJECT_FIELD_GUID);

        // [Browsable(false)]
        // public ulong OBJECT_FIELD_DATA    => GetUpdateField<ulong>(ObjectFields.OBJECT_FIELD_DATA);

        // [Browsable(false)]
        // public int OBJECT_FIELD_TYPE      => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_TYPE);

        // [Browsable(false)]
        // public int OBJECT_FIELD_ENTRY     => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_ENTRY);

        [Browsable(false)]
        public float OBJECT_FIELD_SCALE_X => GetUpdateField<float>(ObjectFields.OBJECT_FIELD_SCALE_X);

        // [Browsable(false)]
        // public int OBJECT_FIELD_PADDING   => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_PADDING);
        #endregion

        public override string ToString() => $"Object: {OBJECT_FIELD_GUID} [Type: {Type}]";

        [Browsable(false)]
        public ObjectType Type => Read<ObjectType>(BaseAddress + 0x14);

        #region Type conversion
        public virtual CGUnit_C ToUnit()
        {
            switch (Type)
            {
                case ObjectType.Unit:
                case ObjectType.Player:
                    return new CGUnit_C(BaseAddress);
                default:
                    return null;
            }
        }

        public virtual CGPlayer_C     ToPlayer()     => Type == ObjectType.Player     ? new CGPlayer_C(BaseAddress) : null;
        public virtual CGGameObject_C ToGameObject() => Type == ObjectType.GameObject ? new CGGameObject_C(BaseAddress) : null;
        public virtual CGContainer_C  ToContainer()  => Type == ObjectType.Container  ? new CGContainer_C(BaseAddress) : null;
        public virtual CGItem_C       ToItem()       => Type == ObjectType.Item       ? new CGItem_C(BaseAddress) : null;
        #endregion

        public void UpdateBaseAddress(IntPtr baseAddr)
        {
            BaseAddress = baseAddr;

            BaseAddressUpdated = true;

            Refresh();
            OnUpdate();
        }

        /// <summary>
        /// Marker property for the object manager to know when a pointer became invalid.
        /// </summary>
        [Browsable(false)]
        public bool BaseAddressUpdated { get; set; } = true;

        /// <summary>
        /// Called when the entity spawns.
        /// </summary>
        public virtual void OnSpawn()
        {
        }

        /// <summary>
        /// Called just before despawning.
        /// </summary>
        public virtual void OnDespawn()
        {
        }

        public virtual void OnUpdate()
        {

        }

        #region Browsable properties
        [Category("Object")]
        public ObjectGuid GUID => OBJECT_FIELD_GUID;

        [Category("Object"), RefreshProperties(RefreshProperties.All)]
        public float Scale => OBJECT_FIELD_SCALE_X;
        #endregion
    }
}
