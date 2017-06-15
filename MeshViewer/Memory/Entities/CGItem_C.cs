using MeshViewer.Memory.Entities.UpdateFields;
using System;
using System.ComponentModel;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGItem_C : CGObject_C
    {
        public CGItem_C(Process game, IntPtr offset) : base(game, offset)
        {
        }

        public override string ToString()
        {
            return $"Item: {OBJECT_FIELD_GUID}";
        }

        #region Item
        [Category("Item")]
        public CGPlayer_C Owner => Game.Manager.GetEntity<CGPlayer_C>(ITEM_FIELD_OWNER);

        [Category("Item")]
        public CGContainer_C Contained => Game.Manager.GetEntity<CGContainer_C>(ITEM_FIELD_CONTAINED);

        [Category("Item")]
        public CGPlayer_C Creator => Game.Manager.GetEntity<CGPlayer_C>(ITEM_FIELD_CREATOR);

        [Category("Item")]
        public CGPlayer_C GiftCreator => Game.Manager.GetEntity<CGPlayer_C>(ITEM_FIELD_GIFTCREATOR);
        #endregion

        #region Descriptors
        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_OWNER               => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_OWNER);

        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_CONTAINED           => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_CONTAINED);

        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_CREATOR             => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_CREATOR);

        [Category("Item Descriptors")]
        public ObjectGuid ITEM_FIELD_GIFTCREATOR         => GetUpdateField<ObjectGuid>(ItemFields.ITEM_FIELD_GIFTCREATOR);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_STACK_COUNT                => GetUpdateField<int>(ItemFields.ITEM_FIELD_STACK_COUNT);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_DURATION                   => GetUpdateField<int>(ItemFields.ITEM_FIELD_DURATION);

        [Category("Item Descriptors")]
        public int[] ITEM_FIELD_SPELL_CHARGES            => GetUpdateField<int>(ItemFields.ITEM_FIELD_SPELL_CHARGES, 5);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_FLAGS                      => GetUpdateField<int>(ItemFields.ITEM_FIELD_FLAGS);

        [Category("Item Descriptors")]
        public ItemEnchantmentDescriptor[] ITEM_FIELD_ENCHANTMENT => GetUpdateField<ItemEnchantmentDescriptor>(ItemFields.ITEM_FIELD_ENCHANTMENT_1_1, 15);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_PROPERTY_SEED              => GetUpdateField<int>(ItemFields.ITEM_FIELD_PROPERTY_SEED);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_RANDOM_PROPERTIES_ID       => GetUpdateField<int>(ItemFields.ITEM_FIELD_RANDOM_PROPERTIES_ID);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_DURABILITY                 => GetUpdateField<int>(ItemFields.ITEM_FIELD_DURABILITY);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_MAXDURABILITY              => GetUpdateField<int>(ItemFields.ITEM_FIELD_MAXDURABILITY);

        [Category("Item Descriptors")]
        public int ITEM_FIELD_CREATE_PLAYED_TIME         => GetUpdateField<int>(ItemFields.ITEM_FIELD_CREATE_PLAYED_TIME);
        #endregion

        protected enum ItemFields
        {
            ITEM_FIELD_OWNER                                 = ObjectFields.OBJECT_END + 0x0000, // Size: 2, Type: LONG, Flags: PUBLIC
            ITEM_FIELD_CONTAINED                             = ObjectFields.OBJECT_END + 0x0002, // Size: 2, Type: LONG, Flags: PUBLIC
            ITEM_FIELD_CREATOR                               = ObjectFields.OBJECT_END + 0x0004, // Size: 2, Type: LONG, Flags: PUBLIC
            ITEM_FIELD_GIFTCREATOR                           = ObjectFields.OBJECT_END + 0x0006, // Size: 2, Type: LONG, Flags: PUBLIC
            ITEM_FIELD_STACK_COUNT                           = ObjectFields.OBJECT_END + 0x0008, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
            ITEM_FIELD_DURATION                              = ObjectFields.OBJECT_END + 0x0009, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
            ITEM_FIELD_SPELL_CHARGES                         = ObjectFields.OBJECT_END + 0x000A, // Size: 5, Type: INT, Flags: OWNER, ITEM_OWNER
            ITEM_FIELD_FLAGS                                 = ObjectFields.OBJECT_END + 0x000F, // Size: 1, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_1_1                       = ObjectFields.OBJECT_END + 0x0010, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_1_3                       = ObjectFields.OBJECT_END + 0x0012, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_2_1                       = ObjectFields.OBJECT_END + 0x0013, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_2_3                       = ObjectFields.OBJECT_END + 0x0015, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_3_1                       = ObjectFields.OBJECT_END + 0x0016, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_3_3                       = ObjectFields.OBJECT_END + 0x0018, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_4_1                       = ObjectFields.OBJECT_END + 0x0019, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_4_3                       = ObjectFields.OBJECT_END + 0x001B, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_5_1                       = ObjectFields.OBJECT_END + 0x001C, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_5_3                       = ObjectFields.OBJECT_END + 0x001E, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_6_1                       = ObjectFields.OBJECT_END + 0x001F, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_6_3                       = ObjectFields.OBJECT_END + 0x0021, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_7_1                       = ObjectFields.OBJECT_END + 0x0022, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_7_3                       = ObjectFields.OBJECT_END + 0x0024, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_8_1                       = ObjectFields.OBJECT_END + 0x0025, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_8_3                       = ObjectFields.OBJECT_END + 0x0027, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_9_1                       = ObjectFields.OBJECT_END + 0x0028, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_9_3                       = ObjectFields.OBJECT_END + 0x002A, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_10_1                      = ObjectFields.OBJECT_END + 0x002B, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_10_3                      = ObjectFields.OBJECT_END + 0x002D, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_11_1                      = ObjectFields.OBJECT_END + 0x002E, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_11_3                      = ObjectFields.OBJECT_END + 0x0030, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_12_1                      = ObjectFields.OBJECT_END + 0x0031, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_12_3                      = ObjectFields.OBJECT_END + 0x0033, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_13_1                      = ObjectFields.OBJECT_END + 0x0034, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_13_3                      = ObjectFields.OBJECT_END + 0x0036, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_14_1                      = ObjectFields.OBJECT_END + 0x0037, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_14_3                      = ObjectFields.OBJECT_END + 0x0039, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_15_1                      = ObjectFields.OBJECT_END + 0x003A, // Size: 2, Type: INT, Flags: PUBLIC
            ITEM_FIELD_ENCHANTMENT_15_3                      = ObjectFields.OBJECT_END + 0x003C, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            ITEM_FIELD_PROPERTY_SEED                         = ObjectFields.OBJECT_END + 0x003D, // Size: 1, Type: INT, Flags: PUBLIC
            ITEM_FIELD_RANDOM_PROPERTIES_ID                  = ObjectFields.OBJECT_END + 0x003E, // Size: 1, Type: INT, Flags: PUBLIC
            ITEM_FIELD_DURABILITY                            = ObjectFields.OBJECT_END + 0x003F, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
            ITEM_FIELD_MAXDURABILITY                         = ObjectFields.OBJECT_END + 0x0040, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
            ITEM_FIELD_CREATE_PLAYED_TIME                    = ObjectFields.OBJECT_END + 0x0041, // Size: 1, Type: INT, Flags: PUBLIC
            ITEM_END                                         = ObjectFields.OBJECT_END + 0x0042,
        }
    }
}
