using System;
using System.Collections.Generic;
using Npgsql;

namespace MarketServer
{
    class Postgres
    {
        public static NpgsqlConnection connection;

        public static void SetConnection()
        {
            string connectionString = "Server=127.0.0.1;Port=5433;Username=Nikolay;Password=Nikolay;Database=Market";
            connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string sql = "Select version()";
            NpgsqlCommand simpleCommand = new NpgsqlCommand(sql, connection);
            string version = simpleCommand.ExecuteScalar().ToString();
            Console.WriteLine($"PostgreSQL version: {version}");
        }

        public static int CheckEmailAndPassword(string _email, string _password)
        {
            int result = -1;
            string sql = $"Select \"Personal_Data_ID\" from \"Market_Schema\".\"Personal_Data\" where \"Email\"='{_email}' and \"Password\"='{_password}'";
            NpgsqlCommand checkDataCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = checkDataCommand.ExecuteReader();

            try
            {
                if (reader.Read()) result = (int)reader[0];
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public static List<PlayerRow> GetPlayersByPersonalDataId(int _PD_ID)
        {
            List<PlayerRow> resultList = new List<PlayerRow>();
            string sql =
                "Select \"Player_ID\", \"Nickname\", \"Balance\", \"Is_Admin\"" +
                "from \"Market_Schema\".\"Player\"" +
                $"where \"Personal_Data_ID\"={_PD_ID}" +
                "order by \"Player_ID\"";
            NpgsqlCommand getPlayersCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getPlayersCommand.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    resultList.Add(new PlayerRow((int)reader[0], reader[1].ToString(), (int)reader[2], (bool)reader[3]));
                }
            }
            finally
            {
                reader.Close();
            }

            return resultList;
        }

        public static bool AddNewPlayer(string _nickname, int _PD_ID)
        {
            int result = -1;
            string sql = $"Select \"Player_ID\" from \"Market_Schema\".\"Player\" where \"Nickname\"='{_nickname}'";
            NpgsqlCommand getPlayersCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getPlayersCommand.ExecuteReader();
            try
            {
                if (reader.Read()) result = (int)reader[0];
            }
            finally
            {
                reader.Close();
            }
            if (result > -1)
            {
                Console.WriteLine($"The player with nickname {_nickname} already exists.");
            }
            else
            {
                NpgsqlTransaction transaction = null;
                NpgsqlCommand command = null;
                try
                {
                    sql = $"insert into \"Market_Schema\".\"Player\" values (nextval('\"Market_Schema\".\"Player_Player_ID_seq\"'), '{_nickname}', 1000, {_PD_ID}, false)";
                    transaction = connection.BeginTransaction();
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Console.WriteLine($"The player with nickname {_nickname} successfully added.");
                    return true;
                }
                catch
                {
                    if (transaction != null) transaction.Rollback();
                }
            }
            return false;
        }

        public static bool RemovePlayer(int _id)
        {
            int result = -1;
            string sql = $"Select \"Player_ID\" from \"Market_Schema\".\"Player\" where \"Player_ID\"={_id}";
            NpgsqlCommand getPlayersCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getPlayersCommand.ExecuteReader();
            try
            {
                if (reader.Read()) result = (int)reader[0];
            }
            finally
            {
                reader.Close();
            }
            if (result < 0)
            {
                Console.WriteLine($"The player with ID {_id} doesn't exist.");
            }
            else
            {
                NpgsqlTransaction transaction = null;
                NpgsqlCommand command = null;
                try
                {
                    transaction = connection.BeginTransaction();

                    sql = $"Delete from \"Market_Schema\".\"Inventory_Slot\" where \"Player_ID\"={_id}";
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();

                    sql = $"Delete from \"Market_Schema\".\"Player\" where \"Player_ID\"={_id}";
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine($"The player with ID {_id} successfully removed.");
                    return true;
                }
                catch
                {
                    if (transaction != null) transaction.Rollback();
                }
            }
            return false;
        }

        public static bool AddNewPersonalData(string _fio, string _email, string _password)
        {
            int result = -1;
            string sql = $"Select \"Personal_Data_ID\" from \"Market_Schema\".\"Personal_Data\" where \"Email\"='{_email}'";
            NpgsqlCommand getPlayersCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getPlayersCommand.ExecuteReader();
            bool success = false;
            try
            {
                if (reader.Read()) result = (int)reader[0];
            }
            finally
            {
                reader.Close();
            }
            if (result > -1)
            {
                Console.WriteLine($"Email {_email} is already used.");
            }
            else
            {
                NpgsqlTransaction transaction = null;
                NpgsqlCommand command = null;
                try
                {
                    sql =
                        "insert into \"Market_Schema\".\"Personal_Data\"" +
                        $"values (nextval('\"Market_Schema\".\"Personal_Data_Personal_Data_ID_seq\"'), '{_fio}', '{_email}', '{_password}')";
                    transaction = connection.BeginTransaction();
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    success = true;
                }
                catch
                {
                    if (transaction != null) transaction.Rollback();
                }
            }
            if (success)
            {
                Console.WriteLine($"Personal data with full name {_fio} and email {_email} is successfully created.");
            }
            return success;
        }

        public static List<InventorySlotRow> GetInventoryByPlayerId(int _playerId)
        {
            List<InventorySlotRow> inventorySlots = new List<InventorySlotRow>();
            string sql =
                "select \"Inventory_Slot\".\"Item_ID\", \"Item\".\"Type_ID\", \"Inventory_Slot\".\"Quantity\"" +
                "from \"Market_Schema\".\"Inventory_Slot\"" +
                "join \"Market_Schema\".\"Item\"" +
                "on \"Inventory_Slot\".\"Item_ID\"=\"Item\".\"Item_ID\"" +
                $"where \"Player_ID\" = {_playerId} and \"Quantity\" > 0" +
                "order by \"Item_ID\"";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    inventorySlots.Add(new InventorySlotRow((int)reader[0], (int)reader[1], (int)reader[2]));
                }
            }
            finally
            {
                reader.Close();
            }

            return inventorySlots;
        }

        public static int GetBalanceAndAdminStatusByPLayerIdAndPersonalDataId(int _playerId, int _personalDataId, int _toClient)
        {
            int result = -1;

            string sql =
                "select \"Balance\",\"Is_Admin\" from \"Market_Schema\".\"Player\"" +
                $"where \"Player_ID\"={_playerId} and \"Personal_Data_ID\"={_personalDataId}";
            NpgsqlCommand getBalanceCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getBalanceCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    result = (int)reader[0];
                    Server.clients[_toClient].clientData.SetPlayerData(_playerId, result, (bool)reader[1]);
                }
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public static int CheckItemQuantityInInventoryBeforeSelling(int _playerId, int _itemId, int _quantityForSell)
        {
            int result = -1;

            string sql =
                "select \"Inventory_Slot\".\"Quantity\"" +
                "from \"Market_Schema\".\"Inventory_Slot\"" +
                $"where \"Player_ID\" = {_playerId} and \"Item_ID\" = {_itemId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    result = (int)reader[0];
                }
            }
            finally
            {
                reader.Close();
            }

            result -= _quantityForSell;
            return result;
        }

        public static bool CreateNewMarketRecordAndSetQuantityForItemInPlayerInventory(int _playerId, int _itemId, int _newQuantity, int _quantityForSell, int _priceForUnit)
        {
            NpgsqlTransaction transaction = null;
            NpgsqlCommand command = null;
            bool success = false;
            try
            {
                transaction = connection.BeginTransaction();
                string sql =
                    "update \"Market_Schema\".\"Inventory_Slot\"" +
                    $"set \"Quantity\"={_newQuantity}" +
                    $"where \"Player_ID\"={_playerId} and \"Item_ID\"={_itemId}";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                sql =
                    "insert into \"Market_Schema\".\"Market\"" +
                    $"values (nextval('\"Market_Schema\".\"Market_Market_ID_seq\"'), {_itemId}, {_playerId}, {_quantityForSell}, {_priceForUnit})";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();
                transaction.Commit();
                success = true;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
            }
            if (success)
            {
                Console.WriteLine($"Игрок {GetPlayerNameByPlayerId(_playerId)} создал предложение о продаже предмета {GetItemNameByItemId(_itemId)} в количестве {_quantityForSell} по цене {_priceForUnit}.");
            }
            return success;
        }

        public static bool CreateNewMarketRecord(int _itemId, int _playerId, int _quantityForSell, int _priceForUnit)
        {
            NpgsqlTransaction transaction = null;
            NpgsqlCommand command = null;
            bool success = false;
            try
            {
                string sql =
                    "insert into \"Market_Schema\".\"Market\"" +
                    $"values (nextval('\"Market_Schema\".\"Market_Market_ID_seq\"'), {_itemId}, {_playerId}, {_quantityForSell}, {_priceForUnit})";
                transaction = connection.BeginTransaction();
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();
                transaction.Commit();                
                success = true;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
            }
            if (success)
            {
                Console.WriteLine($"Игрок {GetPlayerNameByPlayerId(_playerId)} создал предложение о продаже предмета {GetItemNameByItemId(_itemId)} в количестве {_quantityForSell} по цене {_priceForUnit}.");
            }
            return success;
        }

        public static List<MarketRow> GetMarketRecords(string _pattern, int _playerId = -1)
        {
            List<MarketRow> marketRecords = new List<MarketRow>();
            string sql =
                "select" +
                "\"Market\".\"Market_ID\"," +
                "\"Market\".\"Item_ID\"," +
                "\"Item\".\"Type_ID\"," +
                "\"Market\".\"Player_ID\"," +
                "\"Player\".\"Nickname\"," +
                "\"Market\".\"Quantity\"," +
                "\"Market\".\"Price_For_Unit\"" +
                "from \"Market_Schema\".\"Market\"" +
                "join \"Market_Schema\".\"Item\"" +
                "on \"Market\".\"Item_ID\" = \"Item\".\"Item_ID\"" +
                "join \"Market_Schema\".\"Player\"" +
                "on \"Market\".\"Player_ID\" = \"Player\".\"Player_ID\"" +
                $"where \"Item\".\"Name\" like '%'||'{_pattern}'||'%'";
            if (_playerId > 0) sql += $"and \"Market\".\"Player_ID\"={_playerId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    marketRecords.Add(new MarketRow((int)reader[0], (int)reader[1], (int)reader[2], (int)reader[3], reader[4].ToString(), (int)reader[5], (int)reader[6]));
                }
            }
            finally
            {
                reader.Close();
            }

            return marketRecords;
        }

        public static int CheckItemQuantityInMarketBeforeBuying(int _marketId, int _quantityForBuy, ref int _itemId, ref int _playerId, ref int _priceForQuantity)
        {
            int result = -1;

            string sql =
                "select \"Item_ID\",\"Player_ID\",\"Quantity\",\"Price_For_Unit\"" +
                "from \"Market_Schema\".\"Market\"" +
                $"where \"Market_ID\" = {_marketId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    _itemId = (int)reader[0];
                    _playerId = (int)reader[1];
                    result = (int)reader[2] - _quantityForBuy;
                    _priceForQuantity = (int)reader[3] * _quantityForBuy;
                }
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public static bool ChangeBalanceOfSellerAndUpdateMarketRecord(int _marketId, int _sellerId, int _quantityLeft, int _priceForQuantity)
        {
            NpgsqlTransaction transaction = null;
            NpgsqlCommand command = null;
            bool success = false;
            try
            {
                transaction = connection.BeginTransaction();

                string sql =
                    "update \"Market_Schema\".\"Player\"" +
                    $"set \"Balance\"=\"Balance\"+{_priceForQuantity}" +
                    $"where \"Player_ID\"={_sellerId} and \"Is_Admin\"=false";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                if (_quantityLeft > 0)
                {
                    sql =
                       "update \"Market_Schema\".\"Market\"" +
                       $"set \"Quantity\"={_quantityLeft}" +
                       $"where \"Market_ID\"={_marketId}";
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                }
                else
                {
                    sql =
                       "delete from \"Market_Schema\".\"Market\"" +
                       $"where \"Market_ID\"={_marketId}";
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                success = true;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
            }
            if (success)
            {
                Console.WriteLine($"Администратор приобрел предметы игрока {GetPlayerNameByPlayerId(_sellerId)}.");
            }
            return success;
        }

        public static bool ChangeBalanceOfBuyerAndSellerAndUpdateMarketRecordAndInventory(int _marketId, int _itemId, int _playerId, int _sellerId, int _quantity, int _quantityLeft, int _priceForQuantity)
        {
            NpgsqlTransaction transaction = null;
            NpgsqlCommand command = null;
            bool success = false;
            try
            {
                transaction = connection.BeginTransaction();

                string sql =
                    "update \"Market_Schema\".\"Player\"" +
                    $"set \"Balance\"=\"Balance\"+{_priceForQuantity}" +
                    $"where \"Player_ID\"={_sellerId} and \"Is_Admin\"=false";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                sql =
                    "update \"Market_Schema\".\"Player\"" +
                    $"set \"Balance\"=\"Balance\"-{_priceForQuantity}" +
                    $"where \"Player_ID\"={_playerId} and \"Is_Admin\"=false";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                int _itemQuantityInInventory = -1;
                sql = "select \"Quantity\" from \"Market_Schema\".\"Inventory_Slot\"" +
                      $"where \"Player_ID\" = {_playerId} and \"Item_ID\" = {_itemId}";
                command = new NpgsqlCommand(sql, connection, transaction);
                NpgsqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        _itemQuantityInInventory = (int)reader[0];
                    }
                }
                finally
                {
                    reader.Close();
                }
                if (_itemQuantityInInventory < 0)
                {
                    sql = $"insert into \"Market_Schema\".\"Inventory_Slot\" values ({_playerId}, {_itemId}, {_quantity})";
                }
                else
                {
                    sql = $"update \"Market_Schema\".\"Inventory_Slot\" set \"Quantity\"=\"Quantity\"+{_quantity} " +
                          $"where \"Player_ID\"={_playerId} and \"Item_ID\"={_itemId}";
                }
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                if (_quantityLeft > 0)
                {
                    sql =
                       "update \"Market_Schema\".\"Market\"" +
                       $"set \"Quantity\"={_quantityLeft}" +
                       $"where \"Market_ID\"={_marketId}";
                }
                else
                {
                    sql =
                       "delete from \"Market_Schema\".\"Market\"" +
                       $"where \"Market_ID\"={_marketId}";
                }
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                transaction.Commit();               
                success = true;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
            }
            if (success)
            {
                Console.WriteLine($"Игрок {GetPlayerNameByPlayerId(_playerId)} приобрел предмет {GetItemNameByItemId(_itemId)} игрока {GetPlayerNameByPlayerId(_sellerId)} в количестве {_quantity}.");
            }
            return success;
        }

        public static int GetMarketRecordOwnerItemIdAndQuantity(int _marketId, ref int _itemId, ref int _quantity)
        {
            int result = -1;

            string sql =
                "select \"Player_ID\",\"Item_ID\",\"Quantity\" from \"Market_Schema\".\"Market\"" +
                $"where \"Market_ID\" = {_marketId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    result = (int)reader[0];
                    _itemId = (int)reader[1];
                    _quantity = (int)reader[2];
                }
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public static bool RemoveMarketRecordAndReturnItemsToInventory(int _marketId, int _itemId, int _playerId, int _quantity, bool _isAdmin)
        {
            NpgsqlTransaction transaction = null;
            NpgsqlCommand command = null;
            bool success = false;
            try
            {
                transaction = connection.BeginTransaction();
                string sql;

                if (!_isAdmin)
                {
                    int _itemQuantityInInventory = -1;
                    sql = "select \"Quantity\" from \"Market_Schema\".\"Inventory_Slot\"" +
                          $"where \"Player_ID\" = {_playerId} and \"Item_ID\" = {_itemId}";
                    command = new NpgsqlCommand(sql, connection, transaction);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.Read())
                        {
                            _itemQuantityInInventory = (int)reader[0];
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                    if (_itemQuantityInInventory < 0)
                    {
                        sql = $"insert into \"Market_Schema\".\"Inventory_Slot\" values ({_playerId}, {_itemId}, {_quantity})";
                    }
                    else
                    {
                        sql = $"update \"Market_Schema\".\"Inventory_Slot\" set \"Quantity\"=\"Quantity\"+{_quantity} " +
                              $"where \"Player_ID\"={_playerId} and \"Item_ID\"={_itemId}";
                    }
                    command = new NpgsqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                }

                sql = "delete from \"Market_Schema\".\"Market\"" +
                      $"where \"Market_ID\"={_marketId}";
                command = new NpgsqlCommand(sql, connection, transaction);
                command.ExecuteNonQuery();

                transaction.Commit();                
                success = true;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
            }
            if (success)
            {
                Console.WriteLine($"Игрок {GetPlayerNameByPlayerId(_playerId)} отменил предложение о продаже предмета {GetItemNameByItemId(_itemId)} в количестве {_quantity}.");
            }
            return success;
        }

        public static string GetPlayerNameByPlayerId(int _playerId)
        {
            string result = "{Deleted_Player}";

            string sql =
                "select \"Nickname\" from \"Market_Schema\".\"Player\"" +
                $"where \"Player_ID\" = {_playerId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    result = reader[0].ToString();
                }
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public static string GetItemNameByItemId(int _itemId)
        {
            string result = "{Unknown_Item}";

            string sql =
                "select \"Name\" from \"Market_Schema\".\"Item\"" +
                $"where \"Item_ID\" = {_itemId}";
            NpgsqlCommand getInventoryCommand = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = getInventoryCommand.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    result = reader[0].ToString();
                }
            }
            finally
            {
                reader.Close();
            }

            return result;
        }
    }
}