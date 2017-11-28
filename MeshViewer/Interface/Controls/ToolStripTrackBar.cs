using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MeshViewer.Interface.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripTrackBar : ToolStripControlHost
    {
        private TrackBar combo;

        public ToolStripTrackBar() : base(new TrackBar())
        {
            combo = Control as TrackBar;
            combo.AutoSize = false;
        }

        public TickStyle TickStyle
        {
            get => combo.TickStyle;
            set => combo.TickStyle = value;
        }
        
        public int Minimum
        {
            get => combo.Minimum;
            set => combo.Minimum = value;
        }

        public int Maximum
        {
            get => combo.Maximum;
            set => combo.Maximum = value;
        }

        public int Value
        {
            get => combo.Value;
            set => combo.Value = value;
        }

        public event EventHandler ValueChanged
        {
            add => combo.ValueChanged += value;
            remove => combo.ValueChanged -= value;
        }

        protected override Size DefaultSize { get; } = new Size(100, 20);
    }
}
