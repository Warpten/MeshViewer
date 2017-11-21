using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MeshViewer.Interface.ComponentModel
{
    public abstract class Collection<T> : CollectionBase, IEnumerable, IEnumerable<T>, ICustomTypeDescriptor
    {
        public abstract string Description { get; }

        public void Add(T client) => List.Add(client);
        public void Remove(T client) => List.Remove(client);

        public T this[int index]
        {
            get => (T)List[index];
        }

        public override string ToString() => Description;

        #region ICustomTypeDescriptor
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection
            var pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < List.Count; i++)
            {
                // For each element create a property descriptor and add it to the 
                // PropertyDescriptorCollection instance
                var pd = new CollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            return pds;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return List.GetEnumerator() as IEnumerator<T>;
        }

        private class CollectionPropertyDescriptor : PropertyDescriptor
        {
            private readonly Collection<T> _collection;
            private readonly int _index;

            public CollectionPropertyDescriptor(Collection<T> collection, int index) : base($"[{index}]", null)
            {
                _collection = collection;
                _index = index;

                DisplayName = $"[{index}]";
            }

            public override string DisplayName { get; }

            private static AttributeCollection EmptyAttributeCollection = new AttributeCollection(null);

            public override AttributeCollection Attributes => EmptyAttributeCollection;

            public override bool CanResetValue(object component) => false;

            public override object GetValue(object component) => _collection[_index];

            public override void ResetValue(object component) { }
            public override void SetValue(object component, object value) { }
            public override bool ShouldSerializeValue(object component) => false;
            public override Type ComponentType => _collection.GetType();
            public override bool IsReadOnly => true;

            public override Type PropertyType { get; } = typeof(T);
        }
    }
}
