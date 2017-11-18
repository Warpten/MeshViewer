namespace MeshViewer.Memory.Enums.UpdateFields
{
    public enum GameObjectFields
    {
        OBJECT_FIELD_CREATED_BY = ObjectFields.OBJECT_END + 0x0000, // Size: 2, Type: LONG, Flags: PUBLIC
        GAMEOBJECT_DISPLAYID = ObjectFields.OBJECT_END + 0x0002, // Size: 1, Type: INT, Flags: PUBLIC
        GAMEOBJECT_FLAGS = ObjectFields.OBJECT_END + 0x0003, // Size: 1, Type: INT, Flags: PUBLIC
        GAMEOBJECT_PARENTROTATION = ObjectFields.OBJECT_END + 0x0004, // Size: 4, Type: FLOAT, Flags: PUBLIC
        GAMEOBJECT_DYNAMIC = ObjectFields.OBJECT_END + 0x0008, // Size: 1, Type: TWO_SHORT, Flags: DYNAMIC
        GAMEOBJECT_FACTION = ObjectFields.OBJECT_END + 0x0009, // Size: 1, Type: INT, Flags: PUBLIC
        GAMEOBJECT_LEVEL = ObjectFields.OBJECT_END + 0x000A, // Size: 1, Type: INT, Flags: PUBLIC
        GAMEOBJECT_BYTES_1 = ObjectFields.OBJECT_END + 0x000B, // Size: 1, Type: BYTES, Flags: PUBLIC
        GAMEOBJECT_END = ObjectFields.OBJECT_END + 0x000C
    }
}
