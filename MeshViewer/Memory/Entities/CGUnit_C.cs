using MeshViewer.Memory.Enums;
using System;
using System.ComponentModel;

namespace MeshViewer.Memory.Entities
{
    // http://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/347720-wow-4-3-4-15595-info-dump-thread-post2236393.html#post2236393
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGUnit_C : CGObject_C
    {
        public CGUnit_C(Process game, IntPtr offset) : base(game, offset)
        {
        }

        public override string ToString() => $"Unit: {OBJECT_FIELD_GUID} Name: {Name}";

        #region General
        [Category("General")]
        public virtual string Name => ReadCString(Read<IntPtr>(Read<IntPtr>(BaseAddress + 0x91C) + 0x64), 256);

        [Category("General")]
        public virtual float X => Read<float>(BaseAddress + 0x790);

        [Category("General")]
        public virtual float Y => Read<float>(BaseAddress + 0x794);

        [Category("General")]
        public virtual float Z => Read<float>(BaseAddress + 0x798);

        [Category("General")]
        public virtual float Facing => Read<float>(BaseAddress + 0x7A0);

        [Category("General")]
        public virtual UnitRace Race => (UnitRace)((UNIT_FIELD_BYTES_0 >> 0) & 0xFF);

        [Browsable(false)]
        public virtual float Speed => Read<float>(Read<IntPtr>(BaseAddress + 0x100) + 0x80);

        [Category("General")]
        public virtual Gender Gender => (Gender)((UNIT_FIELD_BYTES_0 >> 16) & 0xFF);

        [Category("General")]
        public virtual Class Class => (Class)((UNIT_FIELD_BYTES_0 >> 8) & 0xFF);
        #endregion

        #region Unit
        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CGUnit_C Target     => Game.Manager.GetEntity<CGUnit_C>(UNIT_FIELD_TARGET);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CGUnit_C CharmedBy => Game.Manager.GetEntity<CGUnit_C>(UNIT_FIELD_CHARMEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CGUnit_C SummonedBy => Game.Manager.GetEntity<CGUnit_C>(UNIT_FIELD_SUMMONEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CGUnit_C CreatedBy  => Game.Manager.GetEntity<CGUnit_C>(UNIT_FIELD_CREATEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CGObject_C ChannelObject => Game.Manager.GetEntity<CGUnit_C>(UNIT_FIELD_CHANNEL_OBJECT);
        #endregion

        #region Descriptors
        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_CHARM => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHARM);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_SUMMON => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_SUMMON);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_CRITTER => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CRITTER);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_CHARMEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHARMEDBY);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_SUMMONEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_SUMMONEDBY);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_CREATEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CREATEDBY);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_TARGET => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_TARGET);

        [Category("Unit Descriptors")]
        public ObjectGuid UNIT_FIELD_CHANNEL_OBJECT => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHANNEL_OBJECT);

        [Category("Unit Descriptors")]
        public int UNIT_CHANNEL_SPELL => GetUpdateField<int>(UnitFields.UNIT_CHANNEL_SPELL);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_BYTES_0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_BYTES_0);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_HEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_HEALTH);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_POWER1 => GetUpdateField<int>(UnitFields.UNIT_FIELD_POWER1, 5);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_MAXHEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXHEALTH);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_MAXPOWER1 => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXPOWER1, 5);

        [Category("Unit Descriptors")]
        public float[] UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER, 5);

        [Category("Unit Descriptors")]
        public float[] UNIT_FIELD_POWER_REGEN_INTERRUPTED_FLAT_MODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_REGEN_INTERRUPTED_FLAT_MODIFIER, 5);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_LEVEL => GetUpdateField<int>(UnitFields.UNIT_FIELD_LEVEL);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_FACTIONTEMPLATE => GetUpdateField<int>(UnitFields.UNIT_FIELD_FACTIONTEMPLATE);

        [Category("Unit Descriptors")]
        public int[] UNIT_VIRTUAL_ITEM_SLOT_ID => GetUpdateField<int>(UnitFields.UNIT_VIRTUAL_ITEM_SLOT_ID, 3);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_FLAGS => GetUpdateField<int>(UnitFields.UNIT_FIELD_FLAGS);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_FLAGS_2 => GetUpdateField<int>(UnitFields.UNIT_FIELD_FLAGS_2);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_AURASTATE => GetUpdateField<int>(UnitFields.UNIT_FIELD_AURASTATE);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_BASEATTACKTIME => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASEATTACKTIME, 2);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_RANGEDATTACKTIME => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGEDATTACKTIME);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_BOUNDINGRADIUS => GetUpdateField<float>(UnitFields.UNIT_FIELD_BOUNDINGRADIUS);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_COMBATREACH => GetUpdateField<float>(UnitFields.UNIT_FIELD_COMBATREACH);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_DISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_DISPLAYID);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_NATIVEDISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_NATIVEDISPLAYID);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_MOUNTDISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_MOUNTDISPLAYID);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MINDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINDAMAGE);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MAXDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXDAMAGE);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MINOFFHANDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINOFFHANDDAMAGE);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MAXOFFHANDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE);

        [Category("Unit Descriptors")]
        public byte[] UNIT_FIELD_BYTES_1 => GetUpdateField<byte>(UnitFields.UNIT_FIELD_BYTES_1, 4);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_PETNUMBER => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETNUMBER);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_PET_NAME_TIMESTAMP => GetUpdateField<int>(UnitFields.UNIT_FIELD_PET_NAME_TIMESTAMP);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_PETEXPERIENCE => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETEXPERIENCE);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_PETNEXTLEVELEXP => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETNEXTLEVELEXP);

        [Category("Unit Descriptors")]
        public int UNIT_DYNAMIC_FLAGS => GetUpdateField<int>(UnitFields.UNIT_DYNAMIC_FLAGS);

        [Category("Unit Descriptors")]
        public float UNIT_MOD_CAST_SPEED => GetUpdateField<float>(UnitFields.UNIT_MOD_CAST_SPEED);

        [Category("Unit Descriptors")]
        public float UNIT_MOD_CAST_HASTE => GetUpdateField<float>(UnitFields.UNIT_MOD_CAST_HASTE);

        [Category("Unit Descriptors")]
        public int UNIT_CREATED_BY_SPELL => GetUpdateField<int>(UnitFields.UNIT_CREATED_BY_SPELL);

        [Category("Unit Descriptors")]
        public int UNIT_NPC_FLAGS => GetUpdateField<int>(UnitFields.UNIT_NPC_FLAGS);

        [Category("Unit Descriptors")]
        public int UNIT_NPC_EMOTESTATE => GetUpdateField<int>(UnitFields.UNIT_NPC_EMOTESTATE);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_STAT => GetUpdateField<int>(UnitFields.UNIT_FIELD_STAT0, 5);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_POSSTAT0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_POSSTAT0, 5);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_NEGSTAT0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_NEGSTAT0, 5);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_RESISTANCES => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCES, 7);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, 7);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, 7);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_BASE_MANA => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASE_MANA);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_BASE_HEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASE_HEALTH);

        [Category("Unit Descriptors")]
        public byte[] UNIT_FIELD_BYTES_2 => GetUpdateField<byte>(UnitFields.UNIT_FIELD_BYTES_2, 4);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_ATTACK_POWER => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_ATTACK_POWER_MOD_POS => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER_MOD_POS);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_ATTACK_POWER_MOD_NEG => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER_MOD_NEG);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_ATTACK_POWER_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_ATTACK_POWER_MULTIPLIER);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_RANGED_ATTACK_POWER => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MINRANGEDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINRANGEDDAMAGE);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MAXRANGEDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXRANGEDDAMAGE);

        [Category("Unit Descriptors")]
        public int[] UNIT_FIELD_POWER_COST_MODIFIER => GetUpdateField<int>(UnitFields.UNIT_FIELD_POWER_COST_MODIFIER, 7);

        [Category("Unit Descriptors")]
        public float[] UNIT_FIELD_POWER_COST_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_COST_MULTIPLIER, 7);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_MAXHEALTHMODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXHEALTHMODIFIER);

        [Category("Unit Descriptors")]
        public float UNIT_FIELD_HOVERHEIGHT => GetUpdateField<float>(UnitFields.UNIT_FIELD_HOVERHEIGHT);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_MAXITEMLEVEL => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXITEMLEVEL);

        [Category("Unit Descriptors")]
        public int UNIT_FIELD_PADDING => GetUpdateField<int>(UnitFields.UNIT_FIELD_PADDING);
        #endregion

        protected enum UnitFields
        {
	        UNIT_FIELD_CHARM = ObjectFields.OBJECT_END + 0x0,
	        UNIT_FIELD_SUMMON = ObjectFields.OBJECT_END + 0x2,
	        UNIT_FIELD_CRITTER = ObjectFields.OBJECT_END + 0x4,
	        UNIT_FIELD_CHARMEDBY = ObjectFields.OBJECT_END + 0x6,
	        UNIT_FIELD_SUMMONEDBY = ObjectFields.OBJECT_END + 0x8,
	        UNIT_FIELD_CREATEDBY = ObjectFields.OBJECT_END + 0xA,
	        UNIT_FIELD_TARGET = ObjectFields.OBJECT_END + 0xC,
	        UNIT_FIELD_CHANNEL_OBJECT = ObjectFields.OBJECT_END + 0xE,
	        UNIT_CHANNEL_SPELL = ObjectFields.OBJECT_END + 0x10,
	        UNIT_FIELD_BYTES_0 = ObjectFields.OBJECT_END + 0x11,
	        UNIT_FIELD_HEALTH = ObjectFields.OBJECT_END + 0x12,
	        UNIT_FIELD_POWER1 = ObjectFields.OBJECT_END + 0x13,
	        UNIT_FIELD_POWER2 = ObjectFields.OBJECT_END + 0x14,
	        UNIT_FIELD_POWER3 = ObjectFields.OBJECT_END + 0x15,
	        UNIT_FIELD_POWER4 = ObjectFields.OBJECT_END + 0x16,
	        UNIT_FIELD_POWER5 = ObjectFields.OBJECT_END + 0x17,
	        UNIT_FIELD_MAXHEALTH = ObjectFields.OBJECT_END + 0x18,
	        UNIT_FIELD_MAXPOWER1 = ObjectFields.OBJECT_END + 0x19,
	        UNIT_FIELD_MAXPOWER2 = ObjectFields.OBJECT_END + 0x1A,
	        UNIT_FIELD_MAXPOWER3 = ObjectFields.OBJECT_END + 0x1B,
	        UNIT_FIELD_MAXPOWER4 = ObjectFields.OBJECT_END + 0x1C,
	        UNIT_FIELD_MAXPOWER5 = ObjectFields.OBJECT_END + 0x1D,
	        UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER = ObjectFields.OBJECT_END + 0x1E,
	        UNIT_FIELD_POWER_REGEN_INTERRUPTED_FLAT_MODIFIER = ObjectFields.OBJECT_END + 0x23,
	        UNIT_FIELD_LEVEL = ObjectFields.OBJECT_END + 0x28,
	        UNIT_FIELD_FACTIONTEMPLATE = ObjectFields.OBJECT_END + 0x29,
	        UNIT_VIRTUAL_ITEM_SLOT_ID = ObjectFields.OBJECT_END + 0x2A,
	        UNIT_FIELD_FLAGS = ObjectFields.OBJECT_END + 0x2D,
	        UNIT_FIELD_FLAGS_2 = ObjectFields.OBJECT_END + 0x2E,
	        UNIT_FIELD_AURASTATE = ObjectFields.OBJECT_END + 0x2F,
	        UNIT_FIELD_BASEATTACKTIME = ObjectFields.OBJECT_END + 0x30,
	        UNIT_FIELD_RANGEDATTACKTIME = ObjectFields.OBJECT_END + 0x32,
	        UNIT_FIELD_BOUNDINGRADIUS = ObjectFields.OBJECT_END + 0x33,
	        UNIT_FIELD_COMBATREACH = ObjectFields.OBJECT_END + 0x34,
	        UNIT_FIELD_DISPLAYID = ObjectFields.OBJECT_END + 0x35,
	        UNIT_FIELD_NATIVEDISPLAYID = ObjectFields.OBJECT_END + 0x36,
	        UNIT_FIELD_MOUNTDISPLAYID = ObjectFields.OBJECT_END + 0x37,
	        UNIT_FIELD_MINDAMAGE = ObjectFields.OBJECT_END + 0x38,
	        UNIT_FIELD_MAXDAMAGE = ObjectFields.OBJECT_END + 0x39,
	        UNIT_FIELD_MINOFFHANDDAMAGE = ObjectFields.OBJECT_END + 0x3A,
	        UNIT_FIELD_MAXOFFHANDDAMAGE = ObjectFields.OBJECT_END + 0x3B,
	        UNIT_FIELD_BYTES_1 = ObjectFields.OBJECT_END + 0x3C,
	        UNIT_FIELD_PETNUMBER = ObjectFields.OBJECT_END + 0x3D,
	        UNIT_FIELD_PET_NAME_TIMESTAMP = ObjectFields.OBJECT_END + 0x3E,
	        UNIT_FIELD_PETEXPERIENCE = ObjectFields.OBJECT_END + 0x3F,
	        UNIT_FIELD_PETNEXTLEVELEXP = ObjectFields.OBJECT_END + 0x40,
	        UNIT_DYNAMIC_FLAGS = ObjectFields.OBJECT_END + 0x41,
	        UNIT_MOD_CAST_SPEED = ObjectFields.OBJECT_END + 0x42,
	        UNIT_MOD_CAST_HASTE = ObjectFields.OBJECT_END + 0x43,
	        UNIT_CREATED_BY_SPELL = ObjectFields.OBJECT_END + 0x44,
	        UNIT_NPC_FLAGS = ObjectFields.OBJECT_END + 0x45,
	        UNIT_NPC_EMOTESTATE = ObjectFields.OBJECT_END + 0x46,
	        UNIT_FIELD_STAT0 = ObjectFields.OBJECT_END + 0x47,
	        UNIT_FIELD_STAT1 = ObjectFields.OBJECT_END + 0x48,
	        UNIT_FIELD_STAT2 = ObjectFields.OBJECT_END + 0x49,
	        UNIT_FIELD_STAT3 = ObjectFields.OBJECT_END + 0x4A,
	        UNIT_FIELD_STAT4 = ObjectFields.OBJECT_END + 0x4B,
	        UNIT_FIELD_POSSTAT0 = ObjectFields.OBJECT_END + 0x4C,
	        UNIT_FIELD_POSSTAT1 = ObjectFields.OBJECT_END + 0x4D,
	        UNIT_FIELD_POSSTAT2 = ObjectFields.OBJECT_END + 0x4E,
	        UNIT_FIELD_POSSTAT3 = ObjectFields.OBJECT_END + 0x4F,
	        UNIT_FIELD_POSSTAT4 = ObjectFields.OBJECT_END + 0x50,
	        UNIT_FIELD_NEGSTAT0 = ObjectFields.OBJECT_END + 0x51,
	        UNIT_FIELD_NEGSTAT1 = ObjectFields.OBJECT_END + 0x52,
	        UNIT_FIELD_NEGSTAT2 = ObjectFields.OBJECT_END + 0x53,
	        UNIT_FIELD_NEGSTAT3 = ObjectFields.OBJECT_END + 0x54,
	        UNIT_FIELD_NEGSTAT4 = ObjectFields.OBJECT_END + 0x55,
	        UNIT_FIELD_RESISTANCES = ObjectFields.OBJECT_END + 0x56,
	        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE = ObjectFields.OBJECT_END + 0x5D,
	        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE = ObjectFields.OBJECT_END + 0x64,
	        UNIT_FIELD_BASE_MANA = ObjectFields.OBJECT_END + 0x6B,
	        UNIT_FIELD_BASE_HEALTH = ObjectFields.OBJECT_END + 0x6C,
	        UNIT_FIELD_BYTES_2 = ObjectFields.OBJECT_END + 0x6D,
	        UNIT_FIELD_ATTACK_POWER = ObjectFields.OBJECT_END + 0x6E,
	        UNIT_FIELD_ATTACK_POWER_MOD_POS = ObjectFields.OBJECT_END + 0x6F,
	        UNIT_FIELD_ATTACK_POWER_MOD_NEG = ObjectFields.OBJECT_END + 0x70,
	        UNIT_FIELD_ATTACK_POWER_MULTIPLIER = ObjectFields.OBJECT_END + 0x71,
	        UNIT_FIELD_RANGED_ATTACK_POWER = ObjectFields.OBJECT_END + 0x72,
	        UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS = ObjectFields.OBJECT_END + 0x73,
	        UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG = ObjectFields.OBJECT_END + 0x74,
	        UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER = ObjectFields.OBJECT_END + 0x75,
	        UNIT_FIELD_MINRANGEDDAMAGE = ObjectFields.OBJECT_END + 0x76,
	        UNIT_FIELD_MAXRANGEDDAMAGE = ObjectFields.OBJECT_END + 0x77,
	        UNIT_FIELD_POWER_COST_MODIFIER = ObjectFields.OBJECT_END + 0x78,
	        UNIT_FIELD_POWER_COST_MULTIPLIER = ObjectFields.OBJECT_END + 0x7F,
	        UNIT_FIELD_MAXHEALTHMODIFIER = ObjectFields.OBJECT_END + 0x86,
	        UNIT_FIELD_HOVERHEIGHT = ObjectFields.OBJECT_END + 0x87,
	        UNIT_FIELD_MAXITEMLEVEL = ObjectFields.OBJECT_END + 0x88,
	        UNIT_FIELD_PADDING = ObjectFields.OBJECT_END + 0x89,
	        UNIT_END = ObjectFields.OBJECT_END + 0x8A
        }
    }
}
