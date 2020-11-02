using UnityEngine;
using UnityEngine.UI;

public class MarketInfo : MonoBehaviour
{
    public MarketRowWithNames marketRow;
    [Header("Ui Panel")]
    public Text itemNameText;
    public Text typeNameText;
    public Text nicknameText;
    public Text quantityText;
    public Text priceForUnitText;
    public Text priceForAllText;

    public void SetValues(int _marketId, int _itemId, string _itemName, int _typeId, string _typeName, int _playerId, string _nickname, int _quantity, int _priceForUnit)
    {
        marketRow = new MarketRowWithNames(_marketId, _itemId, _itemName, _typeId, _typeName, _playerId, _nickname, _quantity, _priceForUnit);
        SetText();
    }

    public void SetValues(MarketRowWithNames _marketRowWithNames)
    {
        marketRow = _marketRowWithNames;
        SetText();
    }

    private void SetText()
    {
        if (itemNameText) itemNameText.text = marketRow.itemName;
        if (typeNameText) typeNameText.text = marketRow.typeName;
        if (nicknameText) nicknameText.text = marketRow.nickname;
        if (quantityText) quantityText.text = marketRow.quantity.ToString();
        if (priceForUnitText) priceForUnitText.text = marketRow.priceForUnit.ToString();
        if (priceForAllText) priceForAllText.text = (marketRow.priceForUnit * marketRow.quantity).ToString();
    }
}
