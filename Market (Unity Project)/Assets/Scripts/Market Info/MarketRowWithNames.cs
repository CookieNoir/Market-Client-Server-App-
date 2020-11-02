public class MarketRowWithNames
{
    public int marketId;
    public int itemId;
    public string itemName;
    public int typeId;
    public string typeName;
    public int playerId;
    public string nickname;
    public int quantity;
    public int priceForUnit;

    public MarketRowWithNames(int _marketId, int _itemId, string _itemName, int _typeId, string _typeName, int _playerId, string _nickname, int _quantity, int _priceForUnit)
    {
        marketId = _marketId;
        itemId = _itemId;
        itemName = _itemName;
        typeId = _typeId;
        typeName = _typeName;
        playerId = _playerId;
        nickname = _nickname;
        quantity = _quantity;
        priceForUnit = _priceForUnit;
    }
}
