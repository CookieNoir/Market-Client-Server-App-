using System;
using UnityEngine;
using UnityEngine.UI;

public class SellWindow : ItemInfo
{
    public static SellWindow instance;

    public GameObject sellWindow;
    public InputField quantityField;
    public InputField priceField;
    [Header("Price For N Items")]
    public Text priceForNText;
    public string leftText;
    public string rightText;
    public Text priceForNValue;

    private int quantityForSell;
    private int price;
    private bool _isAdmin;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            sellWindow.SetActive(false);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    public void OnValuesChanged()
    {
        quantityForSell = int.Parse(quantityField.text);
        if (quantityForSell < 0) quantityField.text = "0";
        else if (!_isAdmin && quantityForSell > quantity) quantityField.text = quantity.ToString();
        price = int.Parse(priceField.text);
        if (price < 0) priceField.text = "0";
        priceForNText.text = leftText + ' ' + quantityForSell.ToString() + ' ' + rightText;
        priceForNValue.text = (quantityForSell * price).ToString();
    }

    public void OnSellButtonFromInventory(int _itemId, int _typeId, int _quantity)
    {
        SetValues(_itemId, _typeId, _quantity);
        _isAdmin = CurrentPlayer.instance.isAdmin;
        quantityField.text = "0";
        priceField.text = "0";
        sellWindow.SetActive(true);
    }

    public void OnCancelButton()
    {
        sellWindow.SetActive(false);
        quantityField.interactable = true;
        priceField.interactable = true;
    }

    public void OnSellButtonFromSellWindow()
    {
        quantityField.interactable = false;
        priceField.interactable = false;
        if (DataHelper.CanSellItem(quantity, quantityForSell, price) || (_isAdmin && DataHelper.CanSellItemAsAdmin(quantityForSell, price)))
        {
            ClientSend.SellItem(itemId, quantityForSell, price);
        }
        else
        {
            quantityField.interactable = true;
            priceField.interactable = true;
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно выставить на продажу указанный предмет в заданном количестве.");
        }
    }
}
