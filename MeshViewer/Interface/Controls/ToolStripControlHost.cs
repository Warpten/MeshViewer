using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MeshViewer.Interface.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripCheckBox : ToolStripControlHost
    {
        private CheckBox combo;

        public ToolStripCheckBox() : base(new CheckBox())
        {
            combo = Control as CheckBox;
        }

        public bool Checked {
            get => combo.Checked;
            set => combo.Checked = value;
        }
        
        public event EventHandler CheckedChanged
        {
            add => combo.CheckedChanged += value;
            remove => combo.CheckedChanged -= value;
        }

        // Add properties, events etc. you want to expose...
    }
}
