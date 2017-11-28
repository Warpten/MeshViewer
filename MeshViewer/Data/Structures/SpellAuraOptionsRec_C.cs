using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Data.Structures
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SpellAuraOptionsRec_C
    {
        [Browsable(false)]
        public uint Id { get; set; }

        public uint StackAmount { get; set; }
        public uint ProcChance { get; set; }
        public uint ProcCharges { get; set; }
        public uint ProcFlags { get; set; }
    }
}
