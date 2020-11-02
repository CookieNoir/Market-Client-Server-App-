using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        if (Client.instance.createNewPersonalData) CreateNewPersonalData();
        else LogIn();
    }

    public static void LogIn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(UiManager.instance.logInEmailField.text);
            _packet.Write(UiManager.instance.logInPasswordField.text);
            SendTCPData(_packet);
        }
    }

    public static void AddNewPlayer(string _nickname)
    {
        using (Packet _packet = new Packet((int)ClientPackets.addNewPlayer))
        {
            _packet.Write(_nickname);
            SendTCPData(_packet);
        }
    }

    public static void RemovePlayer(int _id)
    {
        using (Packet _packet = new Packet((int)ClientPackets.removePlayer))
        {
            _packet.Write(_id);
            SendTCPData(_packet);
        }
    }

    public static void CreateNewPersonalData()
    {
        using (Packet _packet = new Packet((int)ClientPackets.addNewPersonalData))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(UiManager.instance.fullNameField.text);
            _packet.Write(UiManager.instance.registerEmailField.text);
            _packet.Write(UiManager.instance.registerPasswordField.text);
            SendTCPData(_packet);
        }
    }

    public static void SelectCurrentPlayer()
    {
        using (Packet _packet = new Packet((int)ClientPackets.selectCurrentPlayer))
        {
            _packet.Write(CurrentPlayer.instance.playerId);
            SendTCPData(_packet);
        }
    }

    public static void UpdatePlayerInventory()
    {
        using (Packet _packet = new Packet((int)ClientPackets.updateCurrentPlayerInventory))
        {
            SendTCPData(_packet);
        }
    }

    public static void UpdatePlayersList()
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePlayersList))
        {
            SendTCPData(_packet);
        }
    }

    public static void SellItem(int _itemId, int _quantity, int _priceForUnit)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sellItem))
        {
            _packet.Write(_itemId);
            _packet.Write(_quantity);
            _packet.Write(_priceForUnit);
            SendTCPData(_packet);
        }
    }

    public static void UpdateMarketRecords(string _itemNamePart)
    {
        using (Packet _packet = new Packet((int)ClientPackets.updateMarketRecords))
        {
            _packet.Write(_itemNamePart);
            SendTCPData(_packet);
        }
    }

    public static void BuyItem(int _marketId, int _quantity)
    {
        using (Packet _packet = new Packet((int)ClientPackets.buyItem))
        {
            _packet.Write(_marketId);
            _packet.Write(_quantity);
            SendTCPData(_packet);
        }
    }

    public static void RemoveMarketRecord(int _marketId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.removeMarketRecord))
        {
            _packet.Write(_marketId);
            SendTCPData(_packet);
        }
    }
    #endregion
}
