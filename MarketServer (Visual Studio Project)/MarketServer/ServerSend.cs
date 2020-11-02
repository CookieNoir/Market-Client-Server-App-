using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; ++i)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; ++i)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                string _msg = "Welcome to the Market server! Checking your data...";
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendPlayers(int _toClient, int _result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendPlayers))
            {
                string _msg;
                int _count = -1;
                List<PlayerRow> _players = new List<PlayerRow>();

                if (_result > -1)
                {
                    _msg = $"Everything is Ok! Sending data to player with ID = {_toClient}";
                    _players = Postgres.GetPlayersByPersonalDataId(_result);
                    _count = _players.Count();
                }
                else
                {
                    _msg = "Couldn't find user with received data.";
                }
                Console.WriteLine(_msg);

                _packet.Write(_count);

                for (int i = 0; i < _count; ++i)
                {
                    _packet.Write(_players[i].id);
                    _packet.Write(_players[i].nickname);
                    _packet.Write(_players[i].balance);
                    _packet.Write(_players[i].isAdmin);
                }

                SendTCPData(_toClient, _packet);
            }
        }

        public static void PlayerAlreadyExists(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerAlreadyExists))
            {
                string _msg = $"User with ID = {_toClient} tried to create a new player, but the player with the specified nickname already exists.";
                Console.WriteLine(_msg);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void PersonalDataCreated(int _toClient, bool _result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.personalDataCreated))
            {
                _packet.Write(_result);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendBalanceAndInventory(int _toClient, int _playerId, int _balance, bool _isAdmin)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendPlayerBalanceAndInventory))
            {
                _packet.Write(_isAdmin);
                _packet.Write(_balance);
                List<InventorySlotRow> inventorySlotRows = Postgres.GetInventoryByPlayerId(_playerId);
                int _count = inventorySlotRows.Count;
                _packet.Write(_count);
                for (int i = 0; i < _count; ++i)
                {
                    _packet.Write(inventorySlotRows[i].itemId);
                    _packet.Write(inventorySlotRows[i].typeId);
                    _packet.Write(inventorySlotRows[i].quantity);
                }
                SendTCPData(_toClient, _packet);
            }
        }

        public static void ValidationError(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.validationError))
            {
                string _msg = $"Couldn't match Player ID and Personal Data ID for User with ID = {_toClient}.";
                Console.WriteLine(_msg);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void SellItemResult(int _toClient, bool _result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sellItemResult))
            {
                _packet.Write(_result);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendBalanceAndMarketRecords(int _toClient, int _playerId, int _balance, bool _isAdmin, string _pattern)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendPlayerBalanceAndMarketRecords))
            {
                _packet.Write(_isAdmin);
                _packet.Write(_balance);
                List<MarketRow> marketRows = Postgres.GetMarketRecords(_pattern);
                int _count = marketRows.Count;
                _packet.Write(_count);
                for (int i = 0; i < _count; ++i)
                {
                    _packet.Write(marketRows[i].marketId);
                    _packet.Write(marketRows[i].itemId);
                    _packet.Write(marketRows[i].typeId);
                    _packet.Write(marketRows[i].playerId);
                    _packet.Write(marketRows[i].nickname);
                    _packet.Write(marketRows[i].quantity);
                    _packet.Write(marketRows[i].priceForUnit);
                }
                SendTCPData(_toClient, _packet);
            }
        }

        public static void BuyItemResult(int _toClient, bool _result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.buyItemResult))
            {
                _packet.Write(_result);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void RemoveMarketRecordResult(int _toClient, bool _result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.removeMarketRecordResult))
            {
                _packet.Write(_result);
                SendTCPData(_toClient, _packet);
            }
        }
    }
}
