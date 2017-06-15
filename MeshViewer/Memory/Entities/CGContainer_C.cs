using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class CGContainer_C : CGItem_C
    {
        public CGContainer_C(Process game, IntPtr offset) : base(game, offset)
        {
        }

        public override string ToString()
        {
            return $"Container: {OBJECT_FIELD_GUID}";
        }

        #region General
        [Category("General")]
        public IEnumerable<CGItem_C> Items
        {
            get
            {
                var items = CONTAINER_FIELD_SLOT; // Caching for perf
                return Game.Manager.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }
        #endregion

        [Category("Container Descriptors")]
        public int CONTAINER_FIELD_NUM_SLOTS     => GetUpdateField<int>(ContainerFields.CONTAINER_FIELD_NUM_SLOTS);

        [Category("Container Descriptors")]
        public int CONTAINER_ALIGN_PAD           => GetUpdateField<int>(ContainerFields.CONTAINER_ALIGN_PAD);

        [Category("Container Descriptors")]
        public ObjectGuid[] CONTAINER_FIELD_SLOT => GetUpdateField<ObjectGuid>(ContainerFields.CONTAINER_FIELD_SLOT_1, 72);

        private enum ContainerFields
        {
            CONTAINER_FIELD_NUM_SLOTS                        = ItemFields.ITEM_END + 0x0000, // Size: 1, Type: INT, Flags: PUBLIC
            CONTAINER_ALIGN_PAD                              = ItemFields.ITEM_END + 0x0001, // Size: 1, Type: BYTES, Flags: NONE
            CONTAINER_FIELD_SLOT_1                           = ItemFields.ITEM_END + 0x0002, // Size: 72, Type: LONG, Flags: PUBLIC
            CONTAINER_END                                    = ItemFields.ITEM_END + 0x004A
        }
    }
}
