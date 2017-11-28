using DBFilesClient.NET.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MeshViewer.Data.Structures
{
    [TypeConverter(typeof(ForeignKeyConverter))]
    public sealed class ForeignKey<T> : IObjectType<int>, ICustomTypeDescriptor, IForeignKey where T : class, new()
    {
        public ForeignKey(int underlyingValue) : base(underlyingValue)
        {
            
        }

        public Type ValueType { get; } = typeof(T);
        public bool HasValue => Value != null;

        private T _value;
        public T Value
        {
            get
            {
                if (_value != null)
                    return null;

                var storage = DBC.TryGetStorage<T>();
                if (storage == null || !storage.ContainsKey(Key))
                    return null;

                return _value = storage[Key];
            }
        }

        private PropertyDescriptorCollection _properties;

        #region Implementation of ICustomTypeDescriptor
        public object GetPropertyOwner(PropertyDescriptor pd) => this;
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(Value, true);
        public string GetClassName() => TypeDescriptor.GetClassName(Value, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(Value, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(Value, true);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(Value, true);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(Value, true);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(Value, editorBaseType, true);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(Value, attributes, true);
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(Value, true);

        public PropertyDescriptorCollection GetProperties() => GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Populate the cache now
            // Cannot be done at construction time because of the order in which files load - which cannot be predicted.
            if (Value != null)
            {
                if (_properties != null)
                    return _properties;

                var filteredPropertyCollection = new List<PropertyDescriptor>();

                foreach (var propertyDescriptor in TypeDescriptor.GetProperties(ValueType).Cast<PropertyDescriptor>())
                {
                    var baseType = propertyDescriptor.PropertyType;
                    if (baseType.IsArray)
                        baseType = baseType.GetElementType();

                    var propValue = propertyDescriptor.GetValue(Value);

                    if (!propertyDescriptor.PropertyType.IsArray)
                    {
                        if (typeof(IForeignKey).IsAssignableFrom(baseType))
                        {
                            if (!(propValue as IForeignKey).HasValue)
                                continue;
                        }

                        if (baseType == typeof(string))
                        {
                            if (string.IsNullOrEmpty(propValue as string))
                                continue;
                        }
                    }

                    filteredPropertyCollection.Add(propertyDescriptor);
                }

                _properties = new PropertyDescriptorCollection(filteredPropertyCollection.ToArray());
                return _properties;
            }

            return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
        }
        #endregion

    }

    internal class ForeignKeyConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var valueType = value.GetType();
                if (valueType.IsGenericType)
                    return $"{valueType.GetGenericArguments()[0].Name} (#{value})";
                return $"{valueType.Name} (#{value})";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public interface IForeignKey
    {
        Type ValueType { get; }
        bool HasValue { get; }
    }
}
