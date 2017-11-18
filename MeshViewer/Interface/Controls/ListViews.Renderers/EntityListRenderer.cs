using BrightIdeasSoftware;
using MeshViewer.Memory.Entities;
using System.Drawing;
using MeshViewer.Properties;
using MeshViewer.Memory.Enums;

namespace MeshViewer.Interface.Controls.ListViews.Renderers
{
    /// <summary>
    /// A renderer suited for rendering both players and units.
    /// </summary>
    public sealed class EntityListRenderer : ObjectListRenderer<CGUnit_C>
    {
        public override void BindExtra(OLVColumn column)
        {
            column.ImageGetter = GetClassImage;
        }

        private static Bitmap Warrior     = new Bitmap(Resources.Warrior,     new Size(45, 45));
        private static Bitmap Paladin     = new Bitmap(Resources.Paladin,     new Size(45, 45));
        private static Bitmap Hunter      = new Bitmap(Resources.Hunter,      new Size(45, 45));
        private static Bitmap Rogue       = new Bitmap(Resources.Rogue,       new Size(45, 45));
        private static Bitmap Priest      = new Bitmap(Resources.Priest,      new Size(45, 45));
        private static Bitmap DeathKnight = new Bitmap(Resources.DeathKnight, new Size(45, 45));
        private static Bitmap Shaman      = new Bitmap(Resources.Shaman,      new Size(45, 45));
        private static Bitmap Mage        = new Bitmap(Resources.Mage,        new Size(45, 45));
        private static Bitmap Warlock     = new Bitmap(Resources.Warlock,     new Size(45, 45));
        private static Bitmap Druid       = new Bitmap(Resources.Druid,       new Size(45, 45));

        public object GetClassImage(object model)
        {
            if (!(model is CGPlayer_C))
                return null;

            var player = model as CGUnit_C;
            switch (player.Class)
            {
                case Class.Warrior:     return Warrior;
                case Class.Paladin:     return Paladin;
                case Class.Hunter:      return Hunter;
                case Class.Rogue:       return Rogue;
                case Class.Priest:      return Priest;
                case Class.DeathKnight: return DeathKnight;
                case Class.Shaman:      return Shaman;
                case Class.Mage:        return Mage;
                case Class.Warlock:     return Warlock;
                case Class.Druid:       return Druid;
            }
            return null;
        }

        protected override string GetDescription(CGUnit_C model)
        {
            if (model.Type == ObjectType.Player)
                return $"Level {model.UNIT_FIELD_LEVEL} {model.Gender} {model.Race} {model.Class}";
            return $"Level {model.UNIT_FIELD_LEVEL} {model.Gender} {model.Class}";
        }
    }
}
