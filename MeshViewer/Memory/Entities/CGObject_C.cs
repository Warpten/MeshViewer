using MeshViewer.Memory.Enums;
using System;
using System.ComponentModel;
using System.Text;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGObject_C
    {
        [Browsable(false)]
        public IntPtr BaseAddress { get; }

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
        [Category("Object Descriptors")]
        public ObjectGuid OBJECT_FIELD_GUID    => GetUpdateField<ObjectGuid>(ObjectFields.OBJECT_FIELD_GUID);

        [Category("Object Descriptors")]
        public ulong OBJECT_FIELD_DATA    => GetUpdateField<ulong>(ObjectFields.OBJECT_FIELD_DATA);

        [Category("Object Descriptors")]
        public int OBJECT_FIELD_TYPE      => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_TYPE);

        [Category("Object Descriptors")]
        public int OBJECT_FIELD_ENTRY     => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_ENTRY);

        [Category("Object Descriptors")]
        public float OBJECT_FIELD_SCALE_X => GetUpdateField<float>(ObjectFields.OBJECT_FIELD_SCALE_X);

        [Category("Object Descriptors")]
        public int OBJECT_FIELD_PADDING   => GetUpdateField<int>(ObjectFields.OBJECT_FIELD_PADDING);
        #endregion

        public override string ToString() => $"Object: {OBJECT_FIELD_GUID} [Type: {Type}]";

        [Browsable(false)]
        public ObjectType Type => Read<ObjectType>(BaseAddress + 0x14);

        #region Type conversion
        public CGUnit_C ToUnit()
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

        public CGPlayer_C     ToPlayer()     => Type == ObjectType.Player     ? new CGPlayer_C(BaseAddress) : null;
        public CGGameObject_C ToGameObject() => Type == ObjectType.GameObject ? new CGGameObject_C(BaseAddress) : null;
        public CGContainer_C  ToContainer()  => Type == ObjectType.Container  ? new CGContainer_C(BaseAddress) : null;
        public CGItem_C       ToItem()       => Type == ObjectType.Item       ? new CGItem_C(BaseAddress) : null;
        #endregion

        protected enum ObjectFields : int
        {
            OBJECT_FIELD_GUID = 0x0,
            OBJECT_FIELD_DATA = 0x2,
            OBJECT_FIELD_TYPE = 0x4,
            OBJECT_FIELD_ENTRY = 0x5,
            OBJECT_FIELD_SCALE_X = 0x6,
            OBJECT_FIELD_PADDING = 0x7,
            OBJECT_END = 0x8
        }
    }
}
