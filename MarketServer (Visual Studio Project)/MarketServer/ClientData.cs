namespace MarketServer
{
    public class ClientData
    {
        public int personalDataId;
        public int playerId;
        public int balance;
        public bool isAdmin;

        public ClientData(int _personalDataId)
        {
            personalDataId = _personalDataId;
        }

        public void SetPlayerData(int _playerId, int _balance, bool _isAdmin)
        {
            playerId = _playerId;
            balance = _balance;
            isAdmin = _isAdmin;
        }
    }
}
