using MeshViewer.Memory.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Interface.ComponentModel
{
    public abstract class EnumerableConverter<T> : ExpandableObjectConverter
    {
        public virtual string Description { get; } = $"{typeof(T).Name}[]";

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(IEnumerable<T>).IsAssignableFrom(value.GetType()))
                return Description;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class JamClientAuraEnumerableConverter : EnumerableConverter<JamClientAuraInfo>
    {
        public override string Description => "Auras";
    }
}
