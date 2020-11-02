using System;

namespace MarketServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _email = _packet.ReadString();
            string _password = _packet.ReadString();

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"User with email \"{_email}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            else
            {
                Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} with email {_email} connected and is having ID = {_fromClient}. Checking user data...");
                int _result = Postgres.CheckEmailAndPassword(_email, _password);
                Server.clients[_fromClient].clientData.personalDataId = _result;
                ServerSend.SendPlayers(_fromClient, _result);
            }
        }

        public static void NewPlayerReceived(int _fromClient, Packet _packet)
        {
            string _nickname = _packet.ReadString();
            int _PD_ID = Server.clients[_fromClient].clientData.personalDataId;
            if (Postgres.AddNewPlayer(_nickname, _PD_ID))
            {
                ServerSend.SendPlayers(_fromClient, _PD_ID);
            }
            else
            {
                ServerSend.PlayerAlreadyExists(_fromClient);
            }
        }

        public static void RemovePlayerIdReceived(int _fromClient, Packet _packet)
        {
            int _id = _packet.ReadInt();
            Postgres.RemovePlayer(_id);
            ServerSend.SendPlayers(_fromClient, Server.clients[_fromClient].clientData.personalDataId);
        }

        public static void NewPersonalDataReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _fullName = _packet.ReadString();
            string _email = _packet.ReadString();
            string _password = _packet.ReadString();

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"User (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            else
            {
                ServerSend.PersonalDataCreated(_fromClient, Postgres.AddNewPersonalData(_fullName, _email, _password));
            }
        }

        public static void SelectPlayer(int _fromClient, Packet _packet)
        {
            int _playerId = _packet.ReadInt();
            Server.clients[_fromClient].clientData.playerId = _playerId;
            int _balance = Postgres.GetBalanceAndAdminStatusByPLayerIdAndPersonalDataId(
                _playerId,
                Server.clients[_fromClient].clientData.personalDataId,
                _fromClient);
            bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
            if (_balance > -1)
            {
                ServerSend.SendBalanceAndInventory(_fromClient, _playerId, _balance, _isAdmin);
            }
            else
            {
                ServerSend.ValidationError(_fromClient);
            }
        }

        public static void UpdateCurrentPlayerInventory(int _fromClient, Packet _packet)
        {
            int _playerId = Server.clients[_fromClient].clientData.playerId;
            int _balance = Postgres.GetBalanceAndAdminStatusByPLayerIdAndPersonalDataId(
                _playerId,
                Server.clients[_fromClient].clientData.personalDataId,
                _fromClient);
            bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
            if (_balance > -1)
            {
                ServerSend.SendBalanceAndInventory(_fromClient, _playerId, _balance, _isAdmin);
            }
            else
            {
                ServerSend.ValidationError(_fromClient);
            }
        }

        public static void UpdatePlayersList(int _fromClient, Packet _packet)
        {
            ServerSend.SendPlayers(_fromClient, Server.clients[_fromClient].clientData.personalDataId);
        }

        public static void SellItemReceived(int _fromClient, Packet _packet)
        {
            int _itemId = _packet.ReadInt();
            int _quantityForSell = _packet.ReadInt();
            int _priceForUnit = _packet.ReadInt();
            if (_itemId <= 0 || _quantityForSell <= 0 || _priceForUnit <= 0)
            {
                ServerSend.SellItemResult(_fromClient, false);
            }
            else
            {
                int _playerId = Server.clients[_fromClient].clientData.playerId;
                bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
                if (_isAdmin)
                {
                    ServerSend.SellItemResult(_fromClient, Postgres.CreateNewMarketRecord(_itemId, _playerId, _quantityForSell, _priceForUnit));
                }
                else
                {
                    int _quantityLeft = Postgres.CheckItemQuantityInInventoryBeforeSelling(_playerId, _itemId, _quantityForSell);
                    if (_quantityLeft < 0)
                    {
                        ServerSend.SellItemResult(_fromClient, false);
                    }
                    else
                    {
                        ServerSend.SellItemResult(_fromClient, Postgres.CreateNewMarketRecordAndSetQuantityForItemInPlayerInventory(_playerId, _itemId, _quantityLeft, _quantityForSell, _priceForUnit));
                    }
                }
            }
        }

        public static void UpdateMarketRecords(int _fromClient, Packet _packet)
        {
            string _pattern = _packet.ReadString();
            int _playerId = Server.clients[_fromClient].clientData.playerId;
            int _balance = Postgres.GetBalanceAndAdminStatusByPLayerIdAndPersonalDataId(
                _playerId,
                Server.clients[_fromClient].clientData.personalDataId,
                _fromClient);
            bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
            if (_balance > -1)
            {
                ServerSend.SendBalanceAndMarketRecords(_fromClient, _playerId, _balance, _isAdmin, _pattern);
            }
            else
            {
                ServerSend.ValidationError(_fromClient);
            }
        }

        public static void BuyItemReceived(int _fromClient, Packet _packet)
        {
            int _marketId = _packet.ReadInt();
            int _quantityForBuy = _packet.ReadInt();
            if (_marketId <= 0 || _quantityForBuy <= 0)
            {
                ServerSend.BuyItemResult(_fromClient, false);
            }
            else
            {
                int _playerId = Server.clients[_fromClient].clientData.playerId;
                int _itemId = -1;
                int _sellerId = -1;
                int _priceForQuantity = int.MaxValue;
                int _quantityLeft = Postgres.CheckItemQuantityInMarketBeforeBuying(_marketId, _quantityForBuy, ref _itemId, ref _sellerId, ref _priceForQuantity);
                if (_quantityLeft < 0 || _playerId == _sellerId)
                {
                    ServerSend.BuyItemResult(_fromClient, false);
                }
                else
                {                   
                    int _personalDataId = Server.clients[_fromClient].clientData.personalDataId;
                    bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
                    if (_isAdmin)
                    {
                        ServerSend.BuyItemResult(_fromClient, 
                            Postgres.ChangeBalanceOfSellerAndUpdateMarketRecord(_marketId, _playerId, _quantityLeft, _priceForQuantity));
                    }
                    else
                    {
                        int _newBalance = Postgres.GetBalanceAndAdminStatusByPLayerIdAndPersonalDataId(_playerId, _personalDataId, _fromClient);
                        _newBalance -= _priceForQuantity;
                        if (_newBalance <= 0)
                        {
                            ServerSend.BuyItemResult(_fromClient, false);
                        }
                        else
                        {
                            ServerSend.BuyItemResult(_fromClient, 
                                Postgres.ChangeBalanceOfBuyerAndSellerAndUpdateMarketRecordAndInventory(
                                    _marketId, _itemId, _playerId, _sellerId, _quantityForBuy, _quantityLeft, _priceForQuantity));
                        }
                    }
                }
            }
        }

        public static void RemoveMarketRecordReceived(int _fromClient, Packet _packet)
        {
            int _marketId = _packet.ReadInt();
            if (_marketId > 0)
            {
                int _itemId = -1, _quantity = -1;
                int _sellerId = Postgres.GetMarketRecordOwnerItemIdAndQuantity(_marketId, ref _itemId, ref _quantity);
                int _playerId = Server.clients[_fromClient].clientData.playerId;
                if (_sellerId == _playerId)
                {
                    bool _isAdmin = Server.clients[_fromClient].clientData.isAdmin;
                    ServerSend.RemoveMarketRecordResult(_fromClient, 
                        Postgres.RemoveMarketRecordAndReturnItemsToInventory(_marketId, _itemId, _playerId, _quantity, _isAdmin));
                }
                else
                {
                    ServerSend.RemoveMarketRecordResult(_fromClient, false);
                }
            }
            else
            {
                ServerSend.RemoveMarketRecordResult(_fromClient, false);
            }
        }
    }
}
