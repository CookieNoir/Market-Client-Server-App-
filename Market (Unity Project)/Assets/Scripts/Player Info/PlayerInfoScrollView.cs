public class PlayerInfoScrollView : PlayerInfo
{
    public void OnSelectButtonPressed()
    {
        CurrentPlayer.instance.SetValues(playerId, playerName, balance, isAdmin);
        ClientSend.SelectCurrentPlayer();
    }

    public void OnRemoveButtonPressed()
    {
        ClientSend.RemovePlayer(playerId);
    }
}
