public class ItemInfoScrollView : ItemInfo
{
    public void OnSellButton()
    {
        SellWindow.instance.OnSellButtonFromInventory(itemId, typeId, quantity);
    }
}
