using UnityEngine;
using UnityEngine.UI;

public class BuyWindow : MarketInfo
{
    public static BuyWindow instance;

    public GameObject buyWindow;
    public InputField quantityField;
    public string textBeforeNickname;
    public string textAfterNickname;
    public Text priceForNValue;
    public int priceForN;

    private int quantityForBuy;
    private bool _isAdmin;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            buyWindow.SetActive(false);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    public void OnValuesChanged()
    {
        quantityForBuy = int.Parse(quantityField.text);
        if (quantityForBuy < 0) quantityField.text = "0";
        else if (quantityForBuy > marketRow.quantity) quantityField.text = marketRow.quantity.ToString();
        priceForN = quantityForBuy * marketRow.priceForUnit;
        priceForNValue.text = priceForN.ToString();
    }

    public void OnCancelButton()
    {
        buyWindow.SetActive(false);
        quantityField.interactable = true;
    }

    public void OnBuyButtonFromMarket(MarketRowWithNames _marketRowWithNames)
    {
        SetValues(_marketRowWithNames);
        _isAdmin = CurrentPlayer.instance.isAdmin;
        nicknameText.text = textBeforeNickname + ' ' + marketRow.nickname + ' ' + textAfterNickname;
        quantityField.text = "0";
        buyWindow.SetActive(true);
    }

    public void OnBuyButtonFromBuyWindow()
    {
        quantityField.interactable = false;
        if (DataHelper.CanBuyItem(marketRow.quantity, quantityForBuy, CurrentPlayer.instance.balance, priceForN) || (_isAdmin && DataHelper.CanBuyItemAsAdmin(marketRow.quantity, quantityForBuy)))
        {
            ClientSend.BuyItem(marketRow.marketId, quantityForBuy);
        }
        else
        {
            quantityField.interactable = true;
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно приобрести указанный предмет в заданном количестве.");
        }
    }
}
