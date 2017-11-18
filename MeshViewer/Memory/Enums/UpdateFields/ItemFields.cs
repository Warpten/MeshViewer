namespace MeshViewer.Memory.Enums.UpdateFields
{
    public enum ItemFields
    {
        ITEM_FIELD_OWNER = ObjectFields.OBJECT_END + 0x0000, // Size: 2, Type: LONG, Flags: PUBLIC
        ITEM_FIELD_CONTAINED = ObjectFields.OBJECT_END + 0x0002, // Size: 2, Type: LONG, Flags: PUBLIC
        ITEM_FIELD_CREATOR = ObjectFields.OBJECT_END + 0x0004, // Size: 2, Type: LONG, Flags: PUBLIC
        ITEM_FIELD_GIFTCREATOR = ObjectFields.OBJECT_END + 0x0006, // Size: 2, Type: LONG, Flags: PUBLIC
        ITEM_FIELD_STACK_COUNT = ObjectFields.OBJECT_END + 0x0008, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
        ITEM_FIELD_DURATION = ObjectFields.OBJECT_END + 0x0009, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
        ITEM_FIELD_SPELL_CHARGES = ObjectFields.OBJECT_END + 0x000A, // Size: 5, Type: INT, Flags: OWNER, ITEM_OWNER
        ITEM_FIELD_FLAGS = ObjectFields.OBJECT_END + 0x000F, // Size: 1, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_1_1 = ObjectFields.OBJECT_END + 0x0010, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_1_3 = ObjectFields.OBJECT_END + 0x0012, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_2_1 = ObjectFields.OBJECT_END + 0x0013, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_2_3 = ObjectFields.OBJECT_END + 0x0015, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_3_1 = ObjectFields.OBJECT_END + 0x0016, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_3_3 = ObjectFields.OBJECT_END + 0x0018, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_4_1 = ObjectFields.OBJECT_END + 0x0019, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_4_3 = ObjectFields.OBJECT_END + 0x001B, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_5_1 = ObjectFields.OBJECT_END + 0x001C, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_5_3 = ObjectFields.OBJECT_END + 0x001E, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_6_1 = ObjectFields.OBJECT_END + 0x001F, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_6_3 = ObjectFields.OBJECT_END + 0x0021, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_7_1 = ObjectFields.OBJECT_END + 0x0022, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_7_3 = ObjectFields.OBJECT_END + 0x0024, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_8_1 = ObjectFields.OBJECT_END + 0x0025, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_8_3 = ObjectFields.OBJECT_END + 0x0027, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_9_1 = ObjectFields.OBJECT_END + 0x0028, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_9_3 = ObjectFields.OBJECT_END + 0x002A, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_10_1 = ObjectFields.OBJECT_END + 0x002B, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_10_3 = ObjectFields.OBJECT_END + 0x002D, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_11_1 = ObjectFields.OBJECT_END + 0x002E, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_11_3 = ObjectFields.OBJECT_END + 0x0030, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_12_1 = ObjectFields.OBJECT_END + 0x0031, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_12_3 = ObjectFields.OBJECT_END + 0x0033, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_13_1 = ObjectFields.OBJECT_END + 0x0034, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_13_3 = ObjectFields.OBJECT_END + 0x0036, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_14_1 = ObjectFields.OBJECT_END + 0x0037, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_14_3 = ObjectFields.OBJECT_END + 0x0039, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_15_1 = ObjectFields.OBJECT_END + 0x003A, // Size: 2, Type: INT, Flags: PUBLIC
        ITEM_FIELD_ENCHANTMENT_15_3 = ObjectFields.OBJECT_END + 0x003C, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
        ITEM_FIELD_PROPERTY_SEED = ObjectFields.OBJECT_END + 0x003D, // Size: 1, Type: INT, Flags: PUBLIC
        ITEM_FIELD_RANDOM_PROPERTIES_ID = ObjectFields.OBJECT_END + 0x003E, // Size: 1, Type: INT, Flags: PUBLIC
        ITEM_FIELD_DURABILITY = ObjectFields.OBJECT_END + 0x003F, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
        ITEM_FIELD_MAXDURABILITY = ObjectFields.OBJECT_END + 0x0040, // Size: 1, Type: INT, Flags: OWNER, ITEM_OWNER
        ITEM_FIELD_CREATE_PLAYED_TIME = ObjectFields.OBJECT_END + 0x0041, // Size: 1, Type: INT, Flags: PUBLIC
        ITEM_END = ObjectFields.OBJECT_END + 0x0042,
    }
}
