using MeshViewer.Memory.Entities.UpdateFields;
using MeshViewer.Memory.Enums.UpdateFields;
using System;
using System.ComponentModel;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGItem_C : CGObject_C
    {
        public CGItem_C(IntPtr offset) : base(offset)
        {
        }

        public override string ToString()
        {
            return $"Item: {OBJECT_FIELD_GUID}";
        }

        #region Item
        [Category("Item")]
        public CGPlayer_C Owner => Game.GetEntity<CGPlayer_C>(ITEM_FIELD_OWNER);

        [Category("Item")]
        public CGContainer_C Contained => Game.GetEntity<CGContainer_C>(ITEM_FIELD_CONTAINED);

        [Category("Item")]
        public CGPlayer_C Creator => Game.GetEntity<CGPlayer_C>(ITEM_FIELD_CREATOR);

        [Category("Item")]
        public CGPlayer_C GiftCreator => Game.GetEntity<CGPlayer_C>(ITEM_FIELD_GIFTCREATOR);
        #endregion

        #region Descriptors
        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_OWNER               => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_OWNER);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid ITEM_FIELD_CONTAINED           => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_CONTAINED);

        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_CREATOR             => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_CREATOR);

        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_GIFTCREATOR         => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_GIFTCREATOR);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int ITEM_FIELD_STACK_COUNT                => GetUpdateField<int>(ItemFields.ITEM_FIELD_STACK_COUNT);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int ITEM_FIELD_DURATION                   => GetUpdateField<int>(ItemFields.ITEM_FIELD_DURATION);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] ITEM_FIELD_SPELL_CHARGES            => GetUpdateField<int>(ItemFields.ITEM_FIELD_SPELL_CHARGES, 5);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int ITEM_FIELD_FLAGS                      => GetUpdateField<int>(ItemFields.ITEM_FIELD_FLAGS);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ItemEnchantmentDescriptor[] ITEM_FIELD_ENCHANTMENT => GetUpdateField<ItemEnchantmentDescriptor>(ItemFields.ITEM_FIELD_ENCHANTMENT_1_1, 15);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_PROPERTY_SEED              => GetUpdateField<int>(ItemFields.ITEM_FIELD_PROPERTY_SEED);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_RANDOM_PROPERTIES_ID       => GetUpdateField<int>(ItemFields.ITEM_FIELD_RANDOM_PROPERTIES_ID);

        [Category("Item Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int ITEM_FIELD_DURABILITY                 => GetUpdateField<int>(ItemFields.ITEM_FIELD_DURABILITY);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_MAXDURABILITY              => GetUpdateField<int>(ItemFields.ITEM_FIELD_MAXDURABILITY);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_CREATE_PLAYED_TIME         => GetUpdateField<int>(ItemFields.ITEM_FIELD_CREATE_PLAYED_TIME);
        #endregion

        public override CGItem_C ToItem() => this;
    }
}
