using MeshViewer.Memory.Structures;
using System.ComponentModel;

namespace MeshViewer.Interface.ComponentModel
{
    [TypeConverter(typeof(CollectionConverter<JamClientAuraCollection>))]
    public sealed class JamClientAuraCollection : Collection<JamClientAuraInfo>
    {
        public override string Description => "Auras";
    }
}
