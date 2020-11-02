using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public int itemId;
    public int typeId;
    public int quantity;
    [Header("Ui Panel")]
    public Text itemNameText;
    public Text typeNameText;
    public Text quantityText;

    public void SetValues(int _itemId, int _typeId, int _quantity)
    {
        itemId = _itemId;
        typeId = _typeId;
        quantity = _quantity;

        if (itemNameText) itemNameText.text = IdNameMatcher.itemNames[itemId];
        if (typeNameText) typeNameText.text = IdNameMatcher.typeNames[typeId];
        if (quantityText)
        {
            if (quantity < -1000) quantityText.text = "Много";
            else quantityText.text = quantity.ToString();
        }
    }
}
