using MeshViewer.Memory.Entities.UpdateFields;
using MeshViewer.Memory.Enums.UpdateFields;
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

        [Category("General")]
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
        [Category("Inventory"), DisplayName("Equipped Items")]
        public IEnumerable<CGItem_C> EquippedItems
        {
            get
            {
                var items = PLAYER_FIELD_INV_SLOT_HEAD.Take(19);
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }

        [Category("Inventory")]
        public IEnumerable<CGItem_C> Backpack
        {
            get
            {
                var items = PLAYER_FIELD_PACK_SLOT_1;
                return Game.Items.Where(item => items.Contains(item.OBJECT_FIELD_GUID));
            }
        }

        [Category("Inventory")]
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

        // [Category("Player Descriptors"), RefreshProperties(RefreshProperties.All)]
        // public int PLAYER_FIELD_PAD_0                            => GetUpdateField<int>(PlayerFields.PLAYER_FIELD_PAD_0);

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

        [Category("Player Descriptors")]
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

        public override CGPlayer_C ToPlayer() => this;
        public override CGUnit_C ToUnit() => this;
    }
}
