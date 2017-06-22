using MeshViewer.Memory.Entities.UpdateFields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MeshViewer.Memory.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class CGPlayer_C : CGUnit_C
    {
        public CGPlayer_C(IntPtr offset) : base(offset)
        {
        }

        public override string ToString()
        {
            return $"Player: {OBJECT_FIELD_GUID} Level: {UNIT_FIELD_LEVEL} Name: {Name} Race: {Race} Class: {Class} Gender: {Gender}";
        }

        [Category("General"), RefreshProperties(RefreshProperties.All)]
        public override string Name
        {
            get
            {
                var maskOffset = Game.Read<int>(0x9980B0 + 0x024);
                if (maskOffset == -1)
                    return "Unknown";

                var objectGuid = OBJECT_FIELD_GUID.Value;

                // Get a pointer to the start of the linked list
                var ptr = Game.Read<int>(0x9980B0 + 0x01C);
                maskOffset &= (int)objectGuid;
                maskOffset += maskOffset * 2;
                maskOffset = ptr + maskOffset * 4 + 4;
                maskOffset = Read<int>(maskOffset + 4);

                // Iterate through the linked list until we find the correct GUID
                while (Read<int>(maskOffset) != (int)objectGuid)
                {
                    var nextOffset = Game.Read<int>(0x9980B0 + 0x01C);
                    ptr = (int)objectGuid;
                    ptr &= Game.Read<int>(0x9980B0 + 0x024);
                    ptr += ptr * 2;
                    ptr = Read<int>(nextOffset + ptr * 4);
                    ptr += maskOffset;
                    maskOffset = Read<int>(ptr + 4);
                }

                return ReadCString(new IntPtr(maskOffset + 0x20), 40);
            }
        }

        [Browsable(false)]
        public CGGameObject_C DuelArbiter => Game.GetEntity<CGGameObject_C>(PLAYER_DUEL_ARBITER);

        #region Bank, buyback and bag management
        [Category("Inventory"), DisplayName("Equipped Items"), RefreshProperties(RefreshProperties.All)]
        public IEnumerable<CGItem_C> EquippedItems
        {
            get
            {
                var items = PLAYER_FIELD_INV_SLOT_HEAD.Take(19);
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }

        [Category("Inventory"), RefreshProperties(RefreshProperties.All)]
        public IEnumerable<CGItem_C> Backpack
        {
            get
            {
                var items = PLAYER_FIELD_PACK_SLOT_1;
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }

        [Category("Inventory"), RefreshProperties(RefreshProperties.All)]
        public IEnumerable<CGItem_C> Bank
        {
            get
            {
                var items = PLAYER_FIELD_BANK_SLOT_1;
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }
        
        public IEnumerable<CGItem_C> OpenBag(int bagIndex)
        {
            if (bagIndex < 0 || bagIndex >= 4)
                return null;
            var bag = PLAYER_FIELD_INV_SLOT_HEAD[19 + bagIndex];
            return Game.Items.Where(item => item.ITEM_FIELD_CONTAINED == bag);
        }

        public IEnumerable<CGItem_C> OpenBankBag(int bagIndex)
        {
            if (bagIndex < 0 || bagIndex >= 7)
                return null;
            var bag = PLAYER_FIELD_BANKBAG_SLOT_1[bagIndex];
            return Game.Items.Where(item => item.ITEM_FIELD_CONTAINED == bag);
        }

        public IEnumerable<CGItem_C> GetBuybackItems()
        {
            var bag = PLAYER_FIELD_VENDORBUYBACK_SLOT_1;
            return Game.Items.Where(item => bag.Contains(item.OBJECT_FIELD_GUID));
        }
        #endregion

        #region Descriptors
        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid PLAYER_DUEL_ARBITER                    => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_DUEL_ARBITER);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FLAGS                                  => GetUpdateField<int>(PlayerFields.PLAYER_FLAGS);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_GUILDRANK                              => GetUpdateField<int>(PlayerFields.PLAYER_GUILDRANK);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_GUILDDELETE_DATE                       => GetUpdateField<int>(PlayerFields.PLAYER_GUILDDELETE_DATE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_GUILDLEVEL                             => GetUpdateField<int>(PlayerFields.PLAYER_GUILDLEVEL);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_BYTES                                  => GetUpdateField<int>(PlayerFields.PLAYER_BYTES);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_BYTES_2                                => GetUpdateField<int>(PlayerFields.PLAYER_BYTES_2);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_BYTES_3                                => GetUpdateField<int>(PlayerFields.PLAYER_BYTES_3);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_DUEL_TEAM                              => GetUpdateField<int>(PlayerFields.PLAYER_DUEL_TEAM);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_GUILD_TIMESTAMP                        => GetUpdateField<int>(PlayerFields.PLAYER_GUILD_TIMESTAMP);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public QuestDescriptor[] PLAYER_QUEST_LOG                => GetUpdateField<QuestDescriptor>(PlayerFields.PLAYER_QUEST_LOG_1_1, 50);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ItemEnchantmentDescriptor[] PLAYER_VISIBLE_ITEM   => GetUpdateField<ItemEnchantmentDescriptor>(PlayerFields.PLAYER_VISIBLE_ITEM_1_ENTRYID, 19);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_CHOSEN_TITLE                           => GetUpdateField<int>(PlayerFields.PLAYER_CHOSEN_TITLE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FAKE_INEBRIATION                       => GetUpdateField<int>(PlayerFields.PLAYER_FAKE_INEBRIATION);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_PAD_0                            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_PAD_0);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] PLAYER_FIELD_INV_SLOT_HEAD           => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_FIELD_INV_SLOT_HEAD, 23);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] PLAYER_FIELD_PACK_SLOT_1             => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_FIELD_PACK_SLOT_1, 16);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] PLAYER_FIELD_BANK_SLOT_1             => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_FIELD_BANK_SLOT_1, 24);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] PLAYER_FIELD_BANKBAG_SLOT_1          => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_FIELD_BANKBAG_SLOT_1, 7);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ObjectGuid[] PLAYER_FIELD_VENDORBUYBACK_SLOT_1    => GetUpdateField<ObjectGuid>(PlayerFields.PLAYER_FIELD_VENDORBUYBACK_SLOT_1, 12);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER_FARSIGHT                             => GetUpdateField<ulong>(PlayerFields.PLAYER_FARSIGHT);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER__FIELD_KNOWN_TITLES                  => GetUpdateField<ulong>(PlayerFields.PLAYER__FIELD_KNOWN_TITLES);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER__FIELD_KNOWN_TITLES1                 => GetUpdateField<ulong>(PlayerFields.PLAYER__FIELD_KNOWN_TITLES1);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER__FIELD_KNOWN_TITLES2                 => GetUpdateField<ulong>(PlayerFields.PLAYER__FIELD_KNOWN_TITLES2);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER__FIELD_KNOWN_TITLES3                 => GetUpdateField<ulong>(PlayerFields.PLAYER__FIELD_KNOWN_TITLES3);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_XP                                     => GetUpdateField<int>(PlayerFields.PLAYER_XP);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_NEXT_LEVEL_XP                          => GetUpdateField<int>(PlayerFields.PLAYER_NEXT_LEVEL_XP);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_LINEID_0                       => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_LINEID_0, 64);      // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_STEP_0                         => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_STEP_0, 64);        // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_RANK_0                         => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_RANK_0, 64);        // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_MAX_RANK_0                     => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_MAX_RANK_0, 64);    // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_MODIFIER_0                     => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_MODIFIER_0, 64);    // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_SKILL_TALENT_0                       => GetUpdateField<int>(PlayerFields.PLAYER_SKILL_TALENT_0, 64);      // two short

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_CHARACTER_POINTS                       => GetUpdateField<int>(PlayerFields.PLAYER_CHARACTER_POINTS);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_TRACK_CREATURES                        => GetUpdateField<int>(PlayerFields.PLAYER_TRACK_CREATURES);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_TRACK_RESOURCES                        => GetUpdateField<int>(PlayerFields.PLAYER_TRACK_RESOURCES);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_EXPERTISE                              => GetUpdateField<int>(PlayerFields.PLAYER_EXPERTISE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_OFFHAND_EXPERTISE                      => GetUpdateField<int>(PlayerFields.PLAYER_OFFHAND_EXPERTISE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_BLOCK_PERCENTAGE                     => GetUpdateField<float>(PlayerFields.PLAYER_BLOCK_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_DODGE_PERCENTAGE                     => GetUpdateField<float>(PlayerFields.PLAYER_DODGE_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_PARRY_PERCENTAGE                     => GetUpdateField<float>(PlayerFields.PLAYER_PARRY_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_CRIT_PERCENTAGE                      => GetUpdateField<float>(PlayerFields.PLAYER_CRIT_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_RANGED_CRIT_PERCENTAGE               => GetUpdateField<float>(PlayerFields.PLAYER_RANGED_CRIT_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_OFFHAND_CRIT_PERCENTAGE              => GetUpdateField<float>(PlayerFields.PLAYER_OFFHAND_CRIT_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_SPELL_CRIT_PERCENTAGE1               => GetUpdateField<float>(PlayerFields.PLAYER_SPELL_CRIT_PERCENTAGE1);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_SHIELD_BLOCK                           => GetUpdateField<int>(PlayerFields.PLAYER_SHIELD_BLOCK);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_SHIELD_BLOCK_CRIT_PERCENTAGE         => GetUpdateField<float>(PlayerFields.PLAYER_SHIELD_BLOCK_CRIT_PERCENTAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_MASTERY                              => GetUpdateField<float>(PlayerFields.PLAYER_MASTERY);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public byte[] PLAYER_EXPLORED_ZONES_1                    => GetUpdateField<byte>(PlayerFields.PLAYER_EXPLORED_ZONES_1, 156);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_REST_STATE_EXPERIENCE                  => GetUpdateField<int>(PlayerFields.PLAYER_REST_STATE_EXPERIENCE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public ulong PLAYER_FIELD_COINAGE                        => GetUpdateField<ulong>(PlayerFields.PLAYER_FIELD_COINAGE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_MOD_DAMAGE_DONE_POS            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_POS, 7);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_MOD_DAMAGE_DONE_NEG            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG, 7);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] PLAYER_FIELD_MOD_DAMAGE_DONE_PCT          => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT, 7);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_MOD_HEALING_DONE_POS             => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MOD_HEALING_DONE_POS);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_HEALING_PCT                => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_HEALING_PCT);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_HEALING_DONE_PCT           => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_HEALING_DONE_PCT);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] PLAYER_FIELD_WEAPON_DMG_MULTIPLIERS       => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_WEAPON_DMG_MULTIPLIERS, 3);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_SPELL_POWER_PCT            => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_SPELL_POWER_PCT);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_OVERRIDE_SPELL_POWER_BY_AP_PCT => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_OVERRIDE_SPELL_POWER_BY_AP_PCT);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_MOD_TARGET_RESISTANCE            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MOD_TARGET_RESISTANCE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE   => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_BYTES                            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_BYTES);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_SELF_RES_SPELL                         => GetUpdateField<int>(PlayerFields.PLAYER_SELF_RES_SPELL);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_PVP_MEDALS                       => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_PVP_MEDALS);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_BUYBACK_PRICE_1                => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_BUYBACK_PRICE_1, 12);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_BUYBACK_TIMESTAMP_1            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_BUYBACK_TIMESTAMP_1, 12);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public short[] PLAYER_FIELD_KILLS                        => GetUpdateField<short>(PlayerFields.PLAYER_FIELD_KILLS, 2);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_LIFETIME_HONORABLE_KILLS         => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_LIFETIME_HONORABLE_KILLS);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_BYTES2                           => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_BYTES2);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_WATCHED_FACTION_INDEX            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_WATCHED_FACTION_INDEX);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_COMBAT_RATING_1                => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_COMBAT_RATING_1, 26);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_ARENA_TEAM_INFO_1_1            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_ARENA_TEAM_INFO_1_1, 21);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_BATTLEGROUND_RATING              => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_BATTLEGROUND_RATING);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_MAX_LEVEL                        => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_MAX_LEVEL);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_DAILY_QUESTS_1                 => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_DAILY_QUESTS_1, 25);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float[] PLAYER_RUNE_REGEN_1                       => GetUpdateField<float>(PlayerFields.PLAYER_RUNE_REGEN_1, 4);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_NO_REAGENT_COST_1                    => GetUpdateField<int>(PlayerFields.PLAYER_NO_REAGENT_COST_1, 3);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_GLYPH_SLOTS_1                  => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_GLYPH_SLOTS_1, 9);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_GLYPHS_1                       => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_GLYPHS_1, 9);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_GLYPHS_ENABLED                         => GetUpdateField<int>(PlayerFields.PLAYER_GLYPHS_ENABLED);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_PET_SPELL_POWER                        => GetUpdateField<int>(PlayerFields.PLAYER_PET_SPELL_POWER);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public short[] PLAYER_FIELD_RESEARCHING_1                => GetUpdateField<short>(PlayerFields.PLAYER_FIELD_RESEARCHING_1, 16); // two shorts

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_FIELD_RESEARCH_SITE_1                => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_RESEARCH_SITE_1, 8); // two shorts

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int[] PLAYER_PROFESSION_SKILL_LINE_1              => GetUpdateField<int>(PlayerFields.PLAYER_PROFESSION_SKILL_LINE_1, 2);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_UI_HIT_MODIFIER                => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_UI_HIT_MODIFIER);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_UI_SPELL_HIT_MODIFIER          => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_UI_SPELL_HIT_MODIFIER);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public int PLAYER_FIELD_HOME_REALM_TIME_OFFSET           => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_HOME_REALM_TIME_OFFSET);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_HASTE                      => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_HASTE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_RANGED_HASTE               => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_RANGED_HASTE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_PET_HASTE                  => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_PET_HASTE);

        [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        public float PLAYER_FIELD_MOD_HASTE_REGEN                => GetUpdateField<float>(PlayerFields.PLAYER_FIELD_MOD_HASTE_REGEN);
        #endregion
        
        private enum PlayerFields
        {
            PLAYER_DUEL_ARBITER                              = UnitFields.UNIT_END + 0x0000, // Size: 2, Type: LONG, Flags: PUBLIC
            PLAYER_FLAGS                                     = UnitFields.UNIT_END + 0x0002, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_GUILDRANK                                 = UnitFields.UNIT_END + 0x0003, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_GUILDDELETE_DATE                          = UnitFields.UNIT_END + 0x0004, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_GUILDLEVEL                                = UnitFields.UNIT_END + 0x0005, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_BYTES                                     = UnitFields.UNIT_END + 0x0006, // Size: 1, Type: BYTES, Flags: PUBLIC
            PLAYER_BYTES_2                                   = UnitFields.UNIT_END + 0x0007, // Size: 1, Type: BYTES, Flags: PUBLIC
            PLAYER_BYTES_3                                   = UnitFields.UNIT_END + 0x0008, // Size: 1, Type: BYTES, Flags: PUBLIC
            PLAYER_DUEL_TEAM                                 = UnitFields.UNIT_END + 0x0009, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_GUILD_TIMESTAMP                           = UnitFields.UNIT_END + 0x000A, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_QUEST_LOG_1_1                             = UnitFields.UNIT_END + 0x000B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_1_2                             = UnitFields.UNIT_END + 0x000C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_1_3                             = UnitFields.UNIT_END + 0x000D, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_1_4                             = UnitFields.UNIT_END + 0x000F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_2_1                             = UnitFields.UNIT_END + 0x0010, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_2_2                             = UnitFields.UNIT_END + 0x0011, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_2_3                             = UnitFields.UNIT_END + 0x0012, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_2_5                             = UnitFields.UNIT_END + 0x0014, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_3_1                             = UnitFields.UNIT_END + 0x0015, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_3_2                             = UnitFields.UNIT_END + 0x0016, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_3_3                             = UnitFields.UNIT_END + 0x0017, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_3_5                             = UnitFields.UNIT_END + 0x0019, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_4_1                             = UnitFields.UNIT_END + 0x001A, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_4_2                             = UnitFields.UNIT_END + 0x001B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_4_3                             = UnitFields.UNIT_END + 0x001C, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_4_5                             = UnitFields.UNIT_END + 0x001E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_5_1                             = UnitFields.UNIT_END + 0x001F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_5_2                             = UnitFields.UNIT_END + 0x0020, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_5_3                             = UnitFields.UNIT_END + 0x0021, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_5_5                             = UnitFields.UNIT_END + 0x0023, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_6_1                             = UnitFields.UNIT_END + 0x0024, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_6_2                             = UnitFields.UNIT_END + 0x0025, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_6_3                             = UnitFields.UNIT_END + 0x0026, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_6_5                             = UnitFields.UNIT_END + 0x0028, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_7_1                             = UnitFields.UNIT_END + 0x0029, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_7_2                             = UnitFields.UNIT_END + 0x002A, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_7_3                             = UnitFields.UNIT_END + 0x002B, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_7_5                             = UnitFields.UNIT_END + 0x002D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_8_1                             = UnitFields.UNIT_END + 0x002E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_8_2                             = UnitFields.UNIT_END + 0x002F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_8_3                             = UnitFields.UNIT_END + 0x0030, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_8_5                             = UnitFields.UNIT_END + 0x0032, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_9_1                             = UnitFields.UNIT_END + 0x0033, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_9_2                             = UnitFields.UNIT_END + 0x0034, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_9_3                             = UnitFields.UNIT_END + 0x0035, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_9_5                             = UnitFields.UNIT_END + 0x0037, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_10_1                            = UnitFields.UNIT_END + 0x0038, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_10_2                            = UnitFields.UNIT_END + 0x0039, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_10_3                            = UnitFields.UNIT_END + 0x003A, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_10_5                            = UnitFields.UNIT_END + 0x003C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_11_1                            = UnitFields.UNIT_END + 0x003D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_11_2                            = UnitFields.UNIT_END + 0x003E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_11_3                            = UnitFields.UNIT_END + 0x003F, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_11_5                            = UnitFields.UNIT_END + 0x0041, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_12_1                            = UnitFields.UNIT_END + 0x0042, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_12_2                            = UnitFields.UNIT_END + 0x0043, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_12_3                            = UnitFields.UNIT_END + 0x0044, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_12_5                            = UnitFields.UNIT_END + 0x0046, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_13_1                            = UnitFields.UNIT_END + 0x0047, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_13_2                            = UnitFields.UNIT_END + 0x0048, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_13_3                            = UnitFields.UNIT_END + 0x0049, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_13_5                            = UnitFields.UNIT_END + 0x004B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_14_1                            = UnitFields.UNIT_END + 0x004C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_14_2                            = UnitFields.UNIT_END + 0x004D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_14_3                            = UnitFields.UNIT_END + 0x004E, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_14_5                            = UnitFields.UNIT_END + 0x0050, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_15_1                            = UnitFields.UNIT_END + 0x0051, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_15_2                            = UnitFields.UNIT_END + 0x0052, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_15_3                            = UnitFields.UNIT_END + 0x0053, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_15_5                            = UnitFields.UNIT_END + 0x0055, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_16_1                            = UnitFields.UNIT_END + 0x0056, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_16_2                            = UnitFields.UNIT_END + 0x0057, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_16_3                            = UnitFields.UNIT_END + 0x0058, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_16_5                            = UnitFields.UNIT_END + 0x005A, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_17_1                            = UnitFields.UNIT_END + 0x005B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_17_2                            = UnitFields.UNIT_END + 0x005C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_17_3                            = UnitFields.UNIT_END + 0x005D, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_17_5                            = UnitFields.UNIT_END + 0x005F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_18_1                            = UnitFields.UNIT_END + 0x0060, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_18_2                            = UnitFields.UNIT_END + 0x0061, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_18_3                            = UnitFields.UNIT_END + 0x0062, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_18_5                            = UnitFields.UNIT_END + 0x0064, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_19_1                            = UnitFields.UNIT_END + 0x0065, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_19_2                            = UnitFields.UNIT_END + 0x0066, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_19_3                            = UnitFields.UNIT_END + 0x0067, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_19_5                            = UnitFields.UNIT_END + 0x0069, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_20_1                            = UnitFields.UNIT_END + 0x006A, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_20_2                            = UnitFields.UNIT_END + 0x006B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_20_3                            = UnitFields.UNIT_END + 0x006C, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_20_5                            = UnitFields.UNIT_END + 0x006E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_21_1                            = UnitFields.UNIT_END + 0x006F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_21_2                            = UnitFields.UNIT_END + 0x0070, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_21_3                            = UnitFields.UNIT_END + 0x0071, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_21_5                            = UnitFields.UNIT_END + 0x0073, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_22_1                            = UnitFields.UNIT_END + 0x0074, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_22_2                            = UnitFields.UNIT_END + 0x0075, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_22_3                            = UnitFields.UNIT_END + 0x0076, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_22_5                            = UnitFields.UNIT_END + 0x0078, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_23_1                            = UnitFields.UNIT_END + 0x0079, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_23_2                            = UnitFields.UNIT_END + 0x007A, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_23_3                            = UnitFields.UNIT_END + 0x007B, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_23_5                            = UnitFields.UNIT_END + 0x007D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_24_1                            = UnitFields.UNIT_END + 0x007E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_24_2                            = UnitFields.UNIT_END + 0x007F, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_24_3                            = UnitFields.UNIT_END + 0x0080, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_24_5                            = UnitFields.UNIT_END + 0x0082, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_25_1                            = UnitFields.UNIT_END + 0x0083, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_25_2                            = UnitFields.UNIT_END + 0x0084, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_25_3                            = UnitFields.UNIT_END + 0x0085, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_25_5                            = UnitFields.UNIT_END + 0x0087, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_26_1                            = UnitFields.UNIT_END + 0x0088, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_26_2                            = UnitFields.UNIT_END + 0x0089, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_26_3                            = UnitFields.UNIT_END + 0x008A, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_26_5                            = UnitFields.UNIT_END + 0x008C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_27_1                            = UnitFields.UNIT_END + 0x008D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_27_2                            = UnitFields.UNIT_END + 0x008E, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_27_3                            = UnitFields.UNIT_END + 0x008F, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_27_5                            = UnitFields.UNIT_END + 0x0091, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_28_1                            = UnitFields.UNIT_END + 0x0092, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_28_2                            = UnitFields.UNIT_END + 0x0093, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_28_3                            = UnitFields.UNIT_END + 0x0094, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_28_5                            = UnitFields.UNIT_END + 0x0096, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_29_1                            = UnitFields.UNIT_END + 0x0097, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_29_2                            = UnitFields.UNIT_END + 0x0098, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_29_3                            = UnitFields.UNIT_END + 0x0099, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_29_5                            = UnitFields.UNIT_END + 0x009B, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_30_1                            = UnitFields.UNIT_END + 0x009C, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_30_2                            = UnitFields.UNIT_END + 0x009D, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_30_3                            = UnitFields.UNIT_END + 0x009E, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_30_5                            = UnitFields.UNIT_END + 0x00A0, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_31_1                            = UnitFields.UNIT_END + 0x00A1, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_31_2                            = UnitFields.UNIT_END + 0x00A2, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_31_3                            = UnitFields.UNIT_END + 0x00A3, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_31_5                            = UnitFields.UNIT_END + 0x00A5, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_32_1                            = UnitFields.UNIT_END + 0x00A6, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_32_2                            = UnitFields.UNIT_END + 0x00A7, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_32_3                            = UnitFields.UNIT_END + 0x00A8, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_32_5                            = UnitFields.UNIT_END + 0x00AA, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_33_1                            = UnitFields.UNIT_END + 0x00AB, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_33_2                            = UnitFields.UNIT_END + 0x00AC, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_33_3                            = UnitFields.UNIT_END + 0x00AD, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_33_5                            = UnitFields.UNIT_END + 0x00AF, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_34_1                            = UnitFields.UNIT_END + 0x00B0, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_34_2                            = UnitFields.UNIT_END + 0x00B1, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_34_3                            = UnitFields.UNIT_END + 0x00B2, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_34_5                            = UnitFields.UNIT_END + 0x00B4, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_35_1                            = UnitFields.UNIT_END + 0x00B5, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_35_2                            = UnitFields.UNIT_END + 0x00B6, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_35_3                            = UnitFields.UNIT_END + 0x00B7, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_35_5                            = UnitFields.UNIT_END + 0x00B9, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_36_1                            = UnitFields.UNIT_END + 0x00BA, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_36_2                            = UnitFields.UNIT_END + 0x00BB, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_36_3                            = UnitFields.UNIT_END + 0x00BC, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_36_5                            = UnitFields.UNIT_END + 0x00BE, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_37_1                            = UnitFields.UNIT_END + 0x00BF, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_37_2                            = UnitFields.UNIT_END + 0x00C0, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_37_3                            = UnitFields.UNIT_END + 0x00C1, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_37_5                            = UnitFields.UNIT_END + 0x00C3, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_38_1                            = UnitFields.UNIT_END + 0x00C4, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_38_2                            = UnitFields.UNIT_END + 0x00C5, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_38_3                            = UnitFields.UNIT_END + 0x00C6, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_38_5                            = UnitFields.UNIT_END + 0x00C8, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_39_1                            = UnitFields.UNIT_END + 0x00C9, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_39_2                            = UnitFields.UNIT_END + 0x00CA, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_39_3                            = UnitFields.UNIT_END + 0x00CB, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_39_5                            = UnitFields.UNIT_END + 0x00CD, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_40_1                            = UnitFields.UNIT_END + 0x00CE, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_40_2                            = UnitFields.UNIT_END + 0x00CF, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_40_3                            = UnitFields.UNIT_END + 0x00D0, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_40_5                            = UnitFields.UNIT_END + 0x00D2, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_41_1                            = UnitFields.UNIT_END + 0x00D3, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_41_2                            = UnitFields.UNIT_END + 0x00D4, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_41_3                            = UnitFields.UNIT_END + 0x00D5, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_41_5                            = UnitFields.UNIT_END + 0x00D7, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_42_1                            = UnitFields.UNIT_END + 0x00D8, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_42_2                            = UnitFields.UNIT_END + 0x00D9, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_42_3                            = UnitFields.UNIT_END + 0x00DA, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_42_5                            = UnitFields.UNIT_END + 0x00DC, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_43_1                            = UnitFields.UNIT_END + 0x00DD, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_43_2                            = UnitFields.UNIT_END + 0x00DE, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_43_3                            = UnitFields.UNIT_END + 0x00DF, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_43_5                            = UnitFields.UNIT_END + 0x00E1, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_44_1                            = UnitFields.UNIT_END + 0x00E2, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_44_2                            = UnitFields.UNIT_END + 0x00E3, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_44_3                            = UnitFields.UNIT_END + 0x00E4, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_44_5                            = UnitFields.UNIT_END + 0x00E6, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_45_1                            = UnitFields.UNIT_END + 0x00E7, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_45_2                            = UnitFields.UNIT_END + 0x00E8, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_45_3                            = UnitFields.UNIT_END + 0x00E9, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_45_5                            = UnitFields.UNIT_END + 0x00EB, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_46_1                            = UnitFields.UNIT_END + 0x00EC, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_46_2                            = UnitFields.UNIT_END + 0x00ED, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_46_3                            = UnitFields.UNIT_END + 0x00EE, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_46_5                            = UnitFields.UNIT_END + 0x00F0, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_47_1                            = UnitFields.UNIT_END + 0x00F1, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_47_2                            = UnitFields.UNIT_END + 0x00F2, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_47_3                            = UnitFields.UNIT_END + 0x00F3, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_47_5                            = UnitFields.UNIT_END + 0x00F5, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_48_1                            = UnitFields.UNIT_END + 0x00F6, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_48_2                            = UnitFields.UNIT_END + 0x00F7, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_48_3                            = UnitFields.UNIT_END + 0x00F8, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_48_5                            = UnitFields.UNIT_END + 0x00FA, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_49_1                            = UnitFields.UNIT_END + 0x00FB, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_49_2                            = UnitFields.UNIT_END + 0x00FC, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_49_3                            = UnitFields.UNIT_END + 0x00FD, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_49_5                            = UnitFields.UNIT_END + 0x00FF, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_50_1                            = UnitFields.UNIT_END + 0x0100, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_50_2                            = UnitFields.UNIT_END + 0x0101, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_50_3                            = UnitFields.UNIT_END + 0x0102, // Size: 2, Type: TWO_SHORT, Flags: PARTY_MEMBER
            PLAYER_QUEST_LOG_50_5                            = UnitFields.UNIT_END + 0x0104, // Size: 1, Type: INT, Flags: PARTY_MEMBER
            PLAYER_VISIBLE_ITEM_1_ENTRYID                    = UnitFields.UNIT_END + 0x0105, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_1_ENCHANTMENT                = UnitFields.UNIT_END + 0x0106, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_2_ENTRYID                    = UnitFields.UNIT_END + 0x0107, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_2_ENCHANTMENT                = UnitFields.UNIT_END + 0x0108, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_3_ENTRYID                    = UnitFields.UNIT_END + 0x0109, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_3_ENCHANTMENT                = UnitFields.UNIT_END + 0x010A, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_4_ENTRYID                    = UnitFields.UNIT_END + 0x010B, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_4_ENCHANTMENT                = UnitFields.UNIT_END + 0x010C, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_5_ENTRYID                    = UnitFields.UNIT_END + 0x010D, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_5_ENCHANTMENT                = UnitFields.UNIT_END + 0x010E, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_6_ENTRYID                    = UnitFields.UNIT_END + 0x010F, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_6_ENCHANTMENT                = UnitFields.UNIT_END + 0x0110, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_7_ENTRYID                    = UnitFields.UNIT_END + 0x0111, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_7_ENCHANTMENT                = UnitFields.UNIT_END + 0x0112, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_8_ENTRYID                    = UnitFields.UNIT_END + 0x0113, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_8_ENCHANTMENT                = UnitFields.UNIT_END + 0x0114, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_9_ENTRYID                    = UnitFields.UNIT_END + 0x0115, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_9_ENCHANTMENT                = UnitFields.UNIT_END + 0x0116, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_10_ENTRYID                   = UnitFields.UNIT_END + 0x0117, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_10_ENCHANTMENT               = UnitFields.UNIT_END + 0x0118, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_11_ENTRYID                   = UnitFields.UNIT_END + 0x0119, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_11_ENCHANTMENT               = UnitFields.UNIT_END + 0x011A, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_12_ENTRYID                   = UnitFields.UNIT_END + 0x011B, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_12_ENCHANTMENT               = UnitFields.UNIT_END + 0x011C, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_13_ENTRYID                   = UnitFields.UNIT_END + 0x011D, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_13_ENCHANTMENT               = UnitFields.UNIT_END + 0x011E, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_14_ENTRYID                   = UnitFields.UNIT_END + 0x011F, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_14_ENCHANTMENT               = UnitFields.UNIT_END + 0x0120, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_15_ENTRYID                   = UnitFields.UNIT_END + 0x0121, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_15_ENCHANTMENT               = UnitFields.UNIT_END + 0x0122, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_16_ENTRYID                   = UnitFields.UNIT_END + 0x0123, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_16_ENCHANTMENT               = UnitFields.UNIT_END + 0x0124, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_17_ENTRYID                   = UnitFields.UNIT_END + 0x0125, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_17_ENCHANTMENT               = UnitFields.UNIT_END + 0x0126, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_18_ENTRYID                   = UnitFields.UNIT_END + 0x0127, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_18_ENCHANTMENT               = UnitFields.UNIT_END + 0x0128, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_19_ENTRYID                   = UnitFields.UNIT_END + 0x0129, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_VISIBLE_ITEM_19_ENCHANTMENT               = UnitFields.UNIT_END + 0x012A, // Size: 1, Type: TWO_SHORT, Flags: PUBLIC
            PLAYER_CHOSEN_TITLE                              = UnitFields.UNIT_END + 0x012B, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_FAKE_INEBRIATION                          = UnitFields.UNIT_END + 0x012C, // Size: 1, Type: INT, Flags: PUBLIC
            PLAYER_FIELD_PAD_0                               = UnitFields.UNIT_END + 0x012D, // Size: 1, Type: INT, Flags: NONE
            PLAYER_END_NOT_SELF                              = UnitFields.UNIT_END + 0x012E,

            PLAYER_FIELD_INV_SLOT_HEAD                       = UnitFields.UNIT_END + 0x012E, // Size: 46, Type: LONG, Flags: PRIVATE
            PLAYER_FIELD_PACK_SLOT_1                         = UnitFields.UNIT_END + 0x015C, // Size: 32, Type: LONG, Flags: PRIVATE
            PLAYER_FIELD_BANK_SLOT_1                         = UnitFields.UNIT_END + 0x017C, // Size: 56, Type: LONG, Flags: PRIVATE
            PLAYER_FIELD_BANKBAG_SLOT_1                      = UnitFields.UNIT_END + 0x01B4, // Size: 14, Type: LONG, Flags: PRIVATE
            PLAYER_FIELD_VENDORBUYBACK_SLOT_1                = UnitFields.UNIT_END + 0x01C2, // Size: 24, Type: LONG, Flags: PRIVATE
            PLAYER_FARSIGHT                                  = UnitFields.UNIT_END + 0x01DA, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER__FIELD_KNOWN_TITLES                       = UnitFields.UNIT_END + 0x01DC, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER__FIELD_KNOWN_TITLES1                      = UnitFields.UNIT_END + 0x01DE, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER__FIELD_KNOWN_TITLES2                      = UnitFields.UNIT_END + 0x01E0, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER__FIELD_KNOWN_TITLES3                      = UnitFields.UNIT_END + 0x01E2, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER_XP                                        = UnitFields.UNIT_END + 0x01E4, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_NEXT_LEVEL_XP                             = UnitFields.UNIT_END + 0x01E5, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_SKILL_LINEID_0                            = UnitFields.UNIT_END + 0x01E6, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_SKILL_STEP_0                              = UnitFields.UNIT_END + 0x0226, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_SKILL_RANK_0                              = UnitFields.UNIT_END + 0x0266, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_SKILL_MAX_RANK_0                          = UnitFields.UNIT_END + 0x02A6, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_SKILL_MODIFIER_0                          = UnitFields.UNIT_END + 0x02E6, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_SKILL_TALENT_0                            = UnitFields.UNIT_END + 0x0326, // Size: 64, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_CHARACTER_POINTS                          = UnitFields.UNIT_END + 0x0366, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_TRACK_CREATURES                           = UnitFields.UNIT_END + 0x0367, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_TRACK_RESOURCES                           = UnitFields.UNIT_END + 0x0368, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_EXPERTISE                                 = UnitFields.UNIT_END + 0x0369, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_OFFHAND_EXPERTISE                         = UnitFields.UNIT_END + 0x036A, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_BLOCK_PERCENTAGE                          = UnitFields.UNIT_END + 0x036B, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_DODGE_PERCENTAGE                          = UnitFields.UNIT_END + 0x036C, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_PARRY_PERCENTAGE                          = UnitFields.UNIT_END + 0x036D, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_CRIT_PERCENTAGE                           = UnitFields.UNIT_END + 0x036E, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_RANGED_CRIT_PERCENTAGE                    = UnitFields.UNIT_END + 0x036F, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_OFFHAND_CRIT_PERCENTAGE                   = UnitFields.UNIT_END + 0x0370, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_SPELL_CRIT_PERCENTAGE1                    = UnitFields.UNIT_END + 0x0371, // Size: 7, Type: float, Flags: PRIVATE
            PLAYER_SHIELD_BLOCK                              = UnitFields.UNIT_END + 0x0378, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_SHIELD_BLOCK_CRIT_PERCENTAGE              = UnitFields.UNIT_END + 0x0379, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_MASTERY                                   = UnitFields.UNIT_END + 0x037A, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_EXPLORED_ZONES_1                          = UnitFields.UNIT_END + 0x037B, // Size: 156, Type: BYTES, Flags: PRIVATE
            PLAYER_REST_STATE_EXPERIENCE                     = UnitFields.UNIT_END + 0x0417, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_COINAGE                             = UnitFields.UNIT_END + 0x0418, // Size: 2, Type: LONG, Flags: PRIVATE
            PLAYER_FIELD_MOD_DAMAGE_DONE_POS                 = UnitFields.UNIT_END + 0x041A, // Size: 7, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_DAMAGE_DONE_NEG                 = UnitFields.UNIT_END + 0x0421, // Size: 7, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_DAMAGE_DONE_PCT                 = UnitFields.UNIT_END + 0x0428, // Size: 7, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_HEALING_DONE_POS                = UnitFields.UNIT_END + 0x042F, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_HEALING_PCT                     = UnitFields.UNIT_END + 0x0430, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_HEALING_DONE_PCT                = UnitFields.UNIT_END + 0x0431, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_WEAPON_DMG_MULTIPLIERS              = UnitFields.UNIT_END + 0x0432, // Size: 3, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_SPELL_POWER_PCT                 = UnitFields.UNIT_END + 0x0435, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_OVERRIDE_SPELL_POWER_BY_AP_PCT      = UnitFields.UNIT_END + 0x0436, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_TARGET_RESISTANCE               = UnitFields.UNIT_END + 0x0437, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE      = UnitFields.UNIT_END + 0x0438, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_BYTES                               = UnitFields.UNIT_END + 0x0439, // Size: 1, Type: BYTES, Flags: PRIVATE
            PLAYER_SELF_RES_SPELL                            = UnitFields.UNIT_END + 0x043A, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_PVP_MEDALS                          = UnitFields.UNIT_END + 0x043B, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_BUYBACK_PRICE_1                     = UnitFields.UNIT_END + 0x043C, // Size: 12, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_BUYBACK_TIMESTAMP_1                 = UnitFields.UNIT_END + 0x0448, // Size: 12, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_KILLS                               = UnitFields.UNIT_END + 0x0454, // Size: 1, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_FIELD_LIFETIME_HONORABLE_KILLS            = UnitFields.UNIT_END + 0x0455, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_BYTES2                              = UnitFields.UNIT_END + 0x0456, // Size: 1, Type: 6, Flags: PRIVATE
            PLAYER_FIELD_WATCHED_FACTION_INDEX               = UnitFields.UNIT_END + 0x0457, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_COMBAT_RATING_1                     = UnitFields.UNIT_END + 0x0458, // Size: 26, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_ARENA_TEAM_INFO_1_1                 = UnitFields.UNIT_END + 0x0472, // Size: 21, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_BATTLEGROUND_RATING                 = UnitFields.UNIT_END + 0x0487, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MAX_LEVEL                           = UnitFields.UNIT_END + 0x0488, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_DAILY_QUESTS_1                      = UnitFields.UNIT_END + 0x0489, // Size: 25, Type: INT, Flags: PRIVATE
            PLAYER_RUNE_REGEN_1                              = UnitFields.UNIT_END + 0x04A2, // Size: 4, Type: float, Flags: PRIVATE
            PLAYER_NO_REAGENT_COST_1                         = UnitFields.UNIT_END + 0x04A6, // Size: 3, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_GLYPH_SLOTS_1                       = UnitFields.UNIT_END + 0x04A9, // Size: 9, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_GLYPHS_1                            = UnitFields.UNIT_END + 0x04B2, // Size: 9, Type: INT, Flags: PRIVATE
            PLAYER_GLYPHS_ENABLED                            = UnitFields.UNIT_END + 0x04BB, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_PET_SPELL_POWER                           = UnitFields.UNIT_END + 0x04BC, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_RESEARCHING_1                       = UnitFields.UNIT_END + 0x04BD, // Size: 8, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_FIELD_RESEARCH_SITE_1                     = UnitFields.UNIT_END + 0x04C5, // Size: 8, Type: TWO_SHORT, Flags: PRIVATE
            PLAYER_PROFESSION_SKILL_LINE_1                   = UnitFields.UNIT_END + 0x04CD, // Size: 2, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_UI_HIT_MODIFIER                     = UnitFields.UNIT_END + 0x04CF, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_UI_SPELL_HIT_MODIFIER               = UnitFields.UNIT_END + 0x04D0, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_HOME_REALM_TIME_OFFSET              = UnitFields.UNIT_END + 0x04D1, // Size: 1, Type: INT, Flags: PRIVATE
            PLAYER_FIELD_MOD_HASTE                           = UnitFields.UNIT_END + 0x04D2, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_RANGED_HASTE                    = UnitFields.UNIT_END + 0x04D3, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_PET_HASTE                       = UnitFields.UNIT_END + 0x04D4, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_FIELD_MOD_HASTE_REGEN                     = UnitFields.UNIT_END + 0x04D5, // Size: 1, Type: float, Flags: PRIVATE
            PLAYER_END                                       = UnitFields.UNIT_END + 0x04D6
        }
    }
}
