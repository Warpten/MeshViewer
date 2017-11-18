namespace MeshViewer.Memory.Entities.UpdateFields
{
    public struct ItemEnchantmentDescriptor
    {
        public int EnchantmentID;
        public short Duration;
        public short Charges;

        public override string ToString()
        {
            return $"Enchantment ID: {EnchantmentID} Duration: {Duration} Charges: {Charges}";
        }
    }
}
