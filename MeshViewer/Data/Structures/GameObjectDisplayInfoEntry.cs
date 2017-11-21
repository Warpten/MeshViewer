using DBFilesClient.NET;
using System.ComponentModel;

namespace MeshViewer.Data.Structures
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class GameObjectDisplayInfoEntry
    {
        public int ID { get; set; }
        public string Filename { get; set; }
        [ArraySize(SizeConst = 10)]
        public int[] SoundId { get; set; }
        [ArraySize(SizeConst = 6)]
        public float[] BoundingBox { get; set; }
        [ArraySize(SizeConst = 3)]
        public float[] UnkFloats0 { get; set; }
    }
}
