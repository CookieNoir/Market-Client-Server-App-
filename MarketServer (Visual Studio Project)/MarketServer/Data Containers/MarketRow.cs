namespace MarketServer
{
    public class MarketRow
    {
        public int marketId;
        public int itemId;
        public int typeId;
        public int playerId;
        public string nickname;
        public int quantity;
        public int priceForUnit;

        public MarketRow(int _marketId, int _itemId, int _typeId, int _playerId, string _nickname, int _quantity, int _priceForUnit)
        {
            marketId = _marketId;
            itemId = _itemId;
            typeId = _typeId;
            playerId = _playerId;
            nickname = _nickname;
            quantity = _quantity;
            priceForUnit = _priceForUnit;
        }
    }
}
