using System;
using BrightIdeasSoftware;
using MeshViewer.Memory.Entities;

namespace MeshViewer.Interface.Controls.ListViews.Renderers
{
    public abstract class ObjectListRenderer<T> : DescribedTaskRenderer where T : CGObject_C
    {
        public Type Type => typeof(T);

        public override string GetDescription(object model) => GetDescription(model as T);

        protected abstract string GetDescription(T model);
        public virtual void BindExtra(OLVColumn column) { }
    }
}
