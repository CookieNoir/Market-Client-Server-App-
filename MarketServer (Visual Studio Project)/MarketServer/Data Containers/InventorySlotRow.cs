namespace MarketServer
{
    public class InventorySlotRow
    {
        public int itemId;
        public int typeId;
        public int quantity;

        public InventorySlotRow(int _itemId, int _typeId, int _quantity)
        {
            itemId = _itemId;
            typeId = _typeId;
            quantity = _quantity;
        }
    }
}
