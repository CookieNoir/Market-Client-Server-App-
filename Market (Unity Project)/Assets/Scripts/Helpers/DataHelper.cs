using System.Text.RegularExpressions;

public static class DataHelper
{
    public static bool IsPlayerNicknameCorrect(string _target)
    {
        return IsEnoughSymbols(_target, 3) && AreOnlyEnglshLetters(_target);
    }

    public static bool IsEmailCorrect(string _target)
    {
        return IsEnoughSymbols(_target, 6) && IsEmailContainsOnlyAllowedSymbols(_target);
    }

    public static bool IsPasswordCorrect(string _target)
    {
        return IsEnoughSymbols(_target, 8);
    }

    public static bool IsFullNameCorrect(string _target)
    {
        return IsEnoughSymbols(_target, 3);
    }

    private static bool IsEnoughSymbols(string _target, int _length)
    {
        return _target.Length >= _length; // Min _length symbols
    }

    private static bool AreOnlyEnglshLetters(string _target)
    {
        return !Regex.IsMatch(_target, @"[^a-zA-Z]");
    }

    private static bool IsEmailContainsOnlyAllowedSymbols(string _target)
    {
        return !Regex.IsMatch(_target, @"[^a-zA-Z@._0-9]");
    }

    public static bool CanSellItem(int _quantityInventory, int _quantityForSell, int _priceForUnit)
    {
        return _quantityForSell > 0 && _quantityForSell <= _quantityInventory && _priceForUnit > 0;
    }

    public static bool CanSellItemAsAdmin(int _quantityForSell, int _priceForUnit)
    {
        return _quantityForSell > 0 && _priceForUnit > 0;
    }

    public static bool CanBuyItem(int _quantityMarket, int _quantityForSell, int _balance, int _priceForN)
    {
        return _quantityForSell > 0 && _quantityForSell <= _quantityMarket && _priceForN <= _balance;
    }

    public static bool CanBuyItemAsAdmin(int _quantityMarket, int _quantityForSell)
    {
        return _quantityForSell > 0 && _quantityForSell <= _quantityMarket;
    }
}
