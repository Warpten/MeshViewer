using MeshViewer.Memory.Structures;
using System;
using System.ComponentModel;
using System.Globalization;

namespace MeshViewer.Interface.ComponentModel
{
    public class CollectionConverter<T> : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(Collection<T>).IsAssignableFrom(value.GetType()))
                return ((Collection<T>)value).Description;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class JamClientAuraInfoConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is JamClientAuraInfo auraInfo)
                return auraInfo.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
