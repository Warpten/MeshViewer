using MeshViewer.Interface.ComponentModel;
using MeshViewer.Memory.Enums;
using MeshViewer.Memory.Enums.UpdateFields;
using MeshViewer.Memory.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MeshViewer.Memory.Entities
{
    // http://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/347720-wow-4-3-4-15595-info-dump-thread-post2236393.html#post2236393
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CGUnit_C : CGObject_C
    {
        public CGUnit_C(IntPtr offset) : base(offset)
        {
        }
        
        public virtual void Update()
        {

        }

        public override string ToString() => $"Unit: {OBJECT_FIELD_GUID} Name: {Name}";

        #region General
        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public virtual string Name => ReadCString(Read<IntPtr>(Read<IntPtr>(BaseAddress + 0x91C) + 0x64), 256);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public virtual float X => Read<float>(BaseAddress + 0x790);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public virtual float Y => Read<float>(BaseAddress + 0x794);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public virtual float Z => Read<float>(BaseAddress + 0x798);

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public virtual float Facing => Read<float>(BaseAddress + 0x7A0);

        [Category("General")]
        public virtual UnitRace Race => (UnitRace)((UNIT_FIELD_BYTES_0 >> 0) & 0xFF);

        [Browsable(false), RefreshProperties(RefreshProperties.All)]
        public virtual float Speed => Read<float>(Read<IntPtr>(BaseAddress + 0x100) + 0x80);

        [Category("General")]
        public virtual Gender Gender => (Gender)((UNIT_FIELD_BYTES_0 >> 16) & 0xFF);

        [Category("General")]
        public virtual Class Class => (Class)((UNIT_FIELD_BYTES_0 >> 8) & 0xFF);
        #endregion

        #region Unit
        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All)]
        public virtual CGUnit_C Target     => Game.GetEntity<CGUnit_C>(UNIT_FIELD_TARGET);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All)]
        public virtual CGUnit_C CharmedBy => Game.GetEntity<CGUnit_C>(UNIT_FIELD_CHARMEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All)]
        public virtual CGUnit_C SummonedBy => Game.GetEntity<CGUnit_C>(UNIT_FIELD_SUMMONEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All)]
        public virtual CGUnit_C CreatedBy  => Game.GetEntity<CGUnit_C>(UNIT_FIELD_CREATEDBY);

        [Category("Unit"), TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All)]
        public virtual CGObject_C ChannelObject => Game.GetEntity<CGUnit_C>(UNIT_FIELD_CHANNEL_OBJECT);
        #endregion

        #region Descriptors
        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_CHARM => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHARM);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_SUMMON => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_SUMMON);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_CRITTER => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CRITTER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_CHARMEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHARMEDBY);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_SUMMONEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_SUMMONEDBY);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_CREATEDBY => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CREATEDBY);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_TARGET => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_TARGET);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid UNIT_FIELD_CHANNEL_OBJECT => GetUpdateField<ObjectGuid>(UnitFields.UNIT_FIELD_CHANNEL_OBJECT);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_CHANNEL_SPELL => GetUpdateField<int>(UnitFields.UNIT_CHANNEL_SPELL);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_BYTES_0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_BYTES_0);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_HEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_HEALTH);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_POWER1 => GetUpdateField<int>(UnitFields.UNIT_FIELD_POWER1, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_MAXHEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXHEALTH);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_MAXPOWER1 => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXPOWER1, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] UNIT_FIELD_POWER_REGEN_INTERRUPTED_FLAT_MODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_REGEN_INTERRUPTED_FLAT_MODIFIER, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_LEVEL => GetUpdateField<int>(UnitFields.UNIT_FIELD_LEVEL);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_FACTIONTEMPLATE => GetUpdateField<int>(UnitFields.UNIT_FIELD_FACTIONTEMPLATE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_VIRTUAL_ITEM_SLOT_ID => GetUpdateField<int>(UnitFields.UNIT_VIRTUAL_ITEM_SLOT_ID, 3);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_FLAGS => GetUpdateField<int>(UnitFields.UNIT_FIELD_FLAGS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_FLAGS_2 => GetUpdateField<int>(UnitFields.UNIT_FIELD_FLAGS_2);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_AURASTATE => GetUpdateField<int>(UnitFields.UNIT_FIELD_AURASTATE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_BASEATTACKTIME => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASEATTACKTIME, 2);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_RANGEDATTACKTIME => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGEDATTACKTIME);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_BOUNDINGRADIUS => GetUpdateField<float>(UnitFields.UNIT_FIELD_BOUNDINGRADIUS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_COMBATREACH => GetUpdateField<float>(UnitFields.UNIT_FIELD_COMBATREACH);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_DISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_DISPLAYID);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_NATIVEDISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_NATIVEDISPLAYID);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_MOUNTDISPLAYID => GetUpdateField<int>(UnitFields.UNIT_FIELD_MOUNTDISPLAYID);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MINDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MAXDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MINOFFHANDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINOFFHANDDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MAXOFFHANDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public byte[] UNIT_FIELD_BYTES_1 => GetUpdateField<byte>(UnitFields.UNIT_FIELD_BYTES_1, 4);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_PETNUMBER => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETNUMBER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_PET_NAME_TIMESTAMP => GetUpdateField<int>(UnitFields.UNIT_FIELD_PET_NAME_TIMESTAMP);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_PETEXPERIENCE => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETEXPERIENCE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_PETNEXTLEVELEXP => GetUpdateField<int>(UnitFields.UNIT_FIELD_PETNEXTLEVELEXP);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_DYNAMIC_FLAGS => GetUpdateField<int>(UnitFields.UNIT_DYNAMIC_FLAGS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_MOD_CAST_SPEED => GetUpdateField<float>(UnitFields.UNIT_MOD_CAST_SPEED);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_MOD_CAST_HASTE => GetUpdateField<float>(UnitFields.UNIT_MOD_CAST_HASTE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_CREATED_BY_SPELL => GetUpdateField<int>(UnitFields.UNIT_CREATED_BY_SPELL);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_NPC_FLAGS => GetUpdateField<int>(UnitFields.UNIT_NPC_FLAGS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_NPC_EMOTESTATE => GetUpdateField<int>(UnitFields.UNIT_NPC_EMOTESTATE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_STAT => GetUpdateField<int>(UnitFields.UNIT_FIELD_STAT0, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_POSSTAT0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_POSSTAT0, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_NEGSTAT0 => GetUpdateField<int>(UnitFields.UNIT_FIELD_NEGSTAT0, 5);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_RESISTANCES => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCES, 7);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, 7);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE => GetUpdateField<int>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, 7);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_BASE_MANA => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASE_MANA);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_BASE_HEALTH => GetUpdateField<int>(UnitFields.UNIT_FIELD_BASE_HEALTH);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public byte[] UNIT_FIELD_BYTES_2 => GetUpdateField<byte>(UnitFields.UNIT_FIELD_BYTES_2, 4);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_ATTACK_POWER => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_ATTACK_POWER_MOD_POS => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER_MOD_POS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_ATTACK_POWER_MOD_NEG => GetUpdateField<int>(UnitFields.UNIT_FIELD_ATTACK_POWER_MOD_NEG);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_ATTACK_POWER_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_ATTACK_POWER_MULTIPLIER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_RANGED_ATTACK_POWER => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG => GetUpdateField<int>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MINRANGEDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MINRANGEDDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MAXRANGEDDAMAGE => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXRANGEDDAMAGE);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] UNIT_FIELD_POWER_COST_MODIFIER => GetUpdateField<int>(UnitFields.UNIT_FIELD_POWER_COST_MODIFIER, 7);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] UNIT_FIELD_POWER_COST_MULTIPLIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_POWER_COST_MULTIPLIER, 7);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_MAXHEALTHMODIFIER => GetUpdateField<float>(UnitFields.UNIT_FIELD_MAXHEALTHMODIFIER);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float UNIT_FIELD_HOVERHEIGHT => GetUpdateField<float>(UnitFields.UNIT_FIELD_HOVERHEIGHT);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_MAXITEMLEVEL => GetUpdateField<int>(UnitFields.UNIT_FIELD_MAXITEMLEVEL);

        [Category("Unit Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int UNIT_FIELD_PADDING => GetUpdateField<int>(UnitFields.UNIT_FIELD_PADDING);
        #endregion

        public override CGUnit_C ToUnit()
        {
            return this;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual IEnumerable<CGUnit_C> Minions
        {
            get
            {
                foreach (var unit in Game.Units)
                    if (unit.UNIT_FIELD_SUMMONEDBY == OBJECT_FIELD_GUID)
                        yield return unit;
            }
        }


        public JamClientAuraCollection Auras
        {
            get
            {
                var auraCount = Read<int>(BaseAddress + 0xE90);
                
                if (auraCount == -1)
                {
                    auraCount = Read<int>(BaseAddress + 0xC14);

                    var auraTable = Read<IntPtr>(BaseAddress + 0xC18);
                    for (var i = 0; i < auraCount; ++i)
                    {
                        var entry = Read<JamClientAuraInfo>(auraTable + SizeCache<JamClientAuraInfo>.Size * i);
                        if (entry.SpellID != 0)
                            yield return entry;
                    }
                }
                else
                {
                    for (var i = 0; i < auraCount; ++i)
                    {
                        var entry = Read<JamClientAuraInfo>(BaseAddress + 0xC10 + SizeCache<JamClientAuraInfo>.Size * i);
                        if (entry.SpellID != 0)
                            yield return entry;
                    }
                }
            }
        }
    }
}
