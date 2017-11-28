using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace MeshViewer.Interface.Controls
{
    /// <summary>
    /// A read-only custom implementation of <see cref="PropertyGrid"/>.
    /// Contrary to Microsoft's control, it can render fields just as well.
    /// </summary>
    public partial class PowerPropertyGrid : UserControl
    {
        private List<MemberRenderer> _unassignedMembers = new List<MemberRenderer>();

        private Dictionary<string, CategoryRenderer> _categories = new Dictionary<string, CategoryRenderer>();

        public PowerPropertyGrid()
        {
            InitializeComponent();
        }

        private bool _paintSuspended = false;

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                _InvalidateMembers();
            }
        }

        public bool ShowFields { get; set; } = false;
        public bool ShowProperties { get; set; } = true;

        private void _InvalidateMembers()
        {
            BeginUpdate();

            if (ShowFields)
                _InvalidateFields();
            if (ShowProperties)
                _InvalidateProperties();
            
            EndUpdate();
        }

        private void _InvalidateFields()
        {
            
        }

        private void _InvalidateProperties()
        {
            foreach (PropertyDescriptor propertyInfo in TypeDescriptor.GetProperties(_selectedObject.GetType()))
            {
                var propAttributes = propertyInfo.Attributes;

                var propertyType = propertyInfo.GetType();
                if (propAttributes.OfType<BrowsableAttribute>().FirstOrDefault()?.Browsable ?? false)
                    continue;

                var categoryAttribute = propAttributes.OfType<CategoryAttribute>().FirstOrDefault();
                if (categoryAttribute != default(CategoryAttribute))
                {
                    if (!_categories.TryGetValue(categoryAttribute.Category, out var container))
                        container = _categories[categoryAttribute.Category] = new CategoryRenderer();

                    container.AddMember(new PropertyRenderer(propertyInfo));
                }
                else
                {
                    _unassignedMembers.Add(new PropertyRenderer(propertyInfo));
                }
            }
        }

        public void BeginUpdate()
        {
            _paintSuspended = true;
        }

        public void EndUpdate()
        {
            _paintSuspended = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_paintSuspended)
                base.OnPaint(e);
        }

        private class PropertyRenderer : MemberRenderer
        {
            public PropertyRenderer(MemberDescriptor descriptor) : base(descriptor)
            {
            }

            protected sealed override EventDescriptor Event => base.Event;

            public override void Render()
            {
                throw new NotImplementedException();
            }
        }

        private class CategoryRenderer
        {
            public string Name { get; set; }

            private List<MemberRenderer> _members;

            public CategoryRenderer()
            {
                _members = new List<MemberRenderer>();
            }

            public void AddMember(MemberRenderer member)
            {
                _members.Add(member);
            }
        }
    }
}
