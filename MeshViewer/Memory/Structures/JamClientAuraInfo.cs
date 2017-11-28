using MeshViewer.Data;
using MeshViewer.Data.Structures;
using MeshViewer.Interface.ComponentModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MeshViewer.Memory.Structures
{
   
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(JamClientAuraInfoConverter))]
    public struct JamClientAuraInfo // sizeof = 0x40
    {
        public ObjectGuid CasterGUID { get; set; } // 0x00

        [Browsable(false)]
        private int _spellID { get; set; }          // 0x08

        public byte Flags { get; set; }            // 0x0C
        public byte Slot { get; set; }             // 0x0D -- Guessed
        public byte CasterLevel { get; set; }      // 0x0E
        public byte StackAmount { get; set; }      // 0x0F -- Also Charges
        public uint MaxDuration { get; set; }      // 0x10
        public uint Duration { get; set; }         // 0x14
        private int _effectAmount0;                // 0x18
        private int _effectAmount1;                // 0x1C
        private int _effectAmount2;                // 0x20

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SpellRec_C Spell => _spellID == 0 ? null : DBC.Spell[_spellID];

        public bool HasDuration => (Flags & 0x20) != 0;

        [Browsable(false)]
        public bool[] HasEffectAmounts
        {
            get
            {
                return new[] { (Flags & 0x01) != 0, (Flags & 0x02) != 0, (Flags & 0x04) != 0 };
            }
        }

        public bool IsPositive => (Flags & 0x10) != 0;

        public int EffectAmount0 => (Flags & 0x01) != 0 ? _effectAmount0 : 0;
        public int EffectAmount1 => (Flags & 0x02) != 0 ? _effectAmount1 : 0;
        public int EffectAmount2 => (Flags & 0x04) != 0 ? _effectAmount2 : 0;

        public override string ToString()
        {
            return $"Spell #{_spellID}";
        }
    }
}
