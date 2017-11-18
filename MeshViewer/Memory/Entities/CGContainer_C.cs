using MeshViewer.Memory.Enums.UpdateFields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class CGContainer_C : CGItem_C
    {
        public CGContainer_C(IntPtr offset) : base(offset)
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
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }
        #endregion

        [Category("Container Descriptors")]
        public int CONTAINER_FIELD_NUM_SLOTS     => GetUpdateField<int>(ContainerFields.CONTAINER_FIELD_NUM_SLOTS);

        // [Category("Container Descriptors"), RefreshProperties(RefreshProperties.All)]
        // public int CONTAINER_ALIGN_PAD           => GetUpdateField<int>(ContainerFields.CONTAINER_ALIGN_PAD);

        [Category("Container Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] CONTAINER_FIELD_SLOT => GetUpdateField<ObjectGuid>(ContainerFields.CONTAINER_FIELD_SLOT_1, 72);

        public override CGContainer_C ToContainer() => this;
        public override CGItem_C ToItem() => this;
    }
}
