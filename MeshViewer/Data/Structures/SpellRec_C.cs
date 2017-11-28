using DBFilesClient.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Data.Structures
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SpellRec_C : IExpandableRecordDescriptor
    {
        [Browsable(false)]
        public uint Id { get; set; }

        public uint Attributes { get; set; }
        public uint AttributesEx { get; set; }
        public uint AttributesEx2 { get; set; }
        public uint AttributesEx3 { get; set; }
        public uint AttributesEx4 { get; set; }
        public uint AttributesEx5 { get; set; }
        public uint AttributesEx6 { get; set; }
        public uint AttributesEx7 { get; set; }
        public uint AttributesEx8 { get; set; }
        public uint AttributesEx9 { get; set; }
        public uint AttributesEx10 { get; set; }
        public uint CastingTimeIndex { get; set; }
        public uint DurationIndex { get; set; }
        public uint PowerType { get; set; }
        public uint RangeIndex { get; set; }
        public float Speed { get; set; }
        [ArraySize(SizeConst = 2)]
        public uint[] SpellVisual { get; set; }
        public uint SpellIconID { get; set; }
        public uint ActiveIconID { get; set; }
        public string SpellName { get; set; }
        public string Rank { get; set; }
        public string Description { get; set; }
        public string ToolTip { get; set; }
        public uint SchoolMask { get; set; }
        public uint RuneCostID { get; set; }
        public uint SpellMissileID { get; set; }
        public uint SpellDescriptionVariableID { get; set; }
        public uint SpellDifficultyId { get; set; }
        public float AttackPowerCoefficient { get; set; }
        public uint SpellScalingId { get; set; }

        public ForeignKey<SpellAuraOptionsRec_C> SpellAuraOptions { get; set; }

        public uint SpellAuraRestrictionsId { get; set; }
        public uint SpellCastingRequirementsId { get; set; }
        public uint SpellCategoriesId { get; set; }
        public uint SpellClassOptionsId { get; set; }
        public uint SpellCooldownsId { get; set; }
        public uint Unknown4 { get; set; }
        public uint SpellEquippedItemsId { get; set; }
        public uint SpellInterruptsId { get; set; }
        public uint SpellLevelsId { get; set; }
        public uint SpellPowerId { get; set; }
        public uint SpellReagentsId { get; set; }
        public uint SpellShapeshiftId { get; set; }
        public uint SpellTargetRestrictionsId { get; set; }
        public uint SpellTotemsId { get; set; }
        public uint ResearchProject { get; set; }

        public override string ToString() => $"#{Id} {SpellName}";
        
    }

    public abstract class IExpandableRecordDescriptor : ICustomTypeDescriptor
    {
        private PropertyDescriptorCollection _properties;

        #region Implementation of ICustomTypeDescriptor
        public object GetPropertyOwner(PropertyDescriptor pd) => this;
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);
        public string GetClassName() => TypeDescriptor.GetClassName(this, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

        public PropertyDescriptorCollection GetProperties() => GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Populate the cache now
            // Cannot be done at construction time because of the order in which files load - which cannot be predicted.
            if (_properties != null)
                return _properties;

            var filteredPropertyCollection = new List<PropertyDescriptor>();

            foreach (var propertyDescriptor in TypeDescriptor.GetProperties(GetType()).Cast<PropertyDescriptor>())
            {
                var baseType = propertyDescriptor.PropertyType;
                if (baseType.IsArray)
                    baseType = baseType.GetElementType();

                var propValue = propertyDescriptor.GetValue(this);

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
        #endregion
    }
}
