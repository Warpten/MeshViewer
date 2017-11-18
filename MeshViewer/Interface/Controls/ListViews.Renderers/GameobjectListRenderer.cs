using MeshViewer.Memory.Entities;

namespace MeshViewer.Interface.Controls.ListViews.Renderers
{
    public sealed class GameobjectListRenderer : ObjectListRenderer<CGGameObject_C>
    {
        protected override string GetDescription(CGGameObject_C model)
        {
            return $"Type: {model.ObjectType}";
        }
    }
}
