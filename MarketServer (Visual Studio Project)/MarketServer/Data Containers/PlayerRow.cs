namespace MarketServer
{
    public class PlayerRow
    {
        public int id;
        public string nickname;
        public int balance;
        public bool isAdmin;

        public PlayerRow(int _id, string _nickname, int _balance, bool _isAdmin)
        {
            id = _id;
            nickname = _nickname;
            balance = _balance;
            isAdmin = _isAdmin;
        }
    }
}
