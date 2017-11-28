using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshViewer.Interface.Controls
{
    public abstract partial class MemberRenderer : UserControl
    {
        protected MemberRenderer(MemberDescriptor descriptor) : this()
        {
            _descriptor = descriptor;
        }

        public MemberRenderer()
        {
            InitializeComponent();
        }

        public abstract void Render();

        private List<MemberRenderer> _children = new List<MemberRenderer>();
        private MemberDescriptor _descriptor;

        protected virtual PropertyDescriptor Property => _descriptor as PropertyDescriptor;
        protected virtual EventDescriptor Event => _descriptor as EventDescriptor;
    }
}
