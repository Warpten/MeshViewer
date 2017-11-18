namespace MeshViewer.Memory.Enums.UpdateFields
{
    public enum ContainerFields
    {
        CONTAINER_FIELD_NUM_SLOTS = ItemFields.ITEM_END + 0x0000, // Size: 1, Type: INT, Flags: PUBLIC
        CONTAINER_ALIGN_PAD = ItemFields.ITEM_END + 0x0001, // Size: 1, Type: BYTES, Flags: NONE
        CONTAINER_FIELD_SLOT_1 = ItemFields.ITEM_END + 0x0002, // Size: 72, Type: LONG, Flags: PUBLIC
        CONTAINER_END = ItemFields.ITEM_END + 0x004A
    }
}
