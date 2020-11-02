using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public int playerId;
    public string playerName;
    public int balance;
    public bool isAdmin;
    [Header("Ui Panel")]
    public Text playerNameText;
    public Text balanceText;

    public void SetValues(int _playerId, string _playerName, int _balance, bool _isAdmin)
    {
        playerId = _playerId;
        playerName = _playerName;
        balance = _balance;
        isAdmin = _isAdmin;

        RefreshText();
    }

    public int Balance
    {
        get
        {
            return balance;
        }
        set
        {
            balance = value;
            RefreshText();
        }
    }

    public void RefreshText()
    {
        if (isAdmin)
        {
            if (playerNameText) playerNameText.text = playerName + " (Администратор)";
            if (balanceText) balanceText.text = "Много";
        }
        else
        {
            if (playerNameText) playerNameText.text = playerName;
            if (balanceText) balanceText.text = balance.ToString();
        }
    }
}
