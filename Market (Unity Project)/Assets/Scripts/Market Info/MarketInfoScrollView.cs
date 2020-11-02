public class MarketInfoScrollView : MarketInfo
{
    public void OnBuyButton()
    {
        BuyWindow.instance.OnBuyButtonFromMarket(marketRow);
    }

    public void OnRemoveButton()
    {
        ClientSend.RemoveMarketRecord(marketRow.marketId);
    }
}
