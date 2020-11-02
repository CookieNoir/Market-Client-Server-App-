using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.id = _id;
        ClientSend.WelcomeReceived();
    }

    public static void PlayersReceived(Packet _packet)
    {
        int _count = _packet.ReadInt();
        if (_count < 0)
        {
            Client.instance.Disconnect();
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Не удалось найти пользователя с указаннымы электронным адресом и паролем.");
        }
        else
        {
            if (UiManager.instance.GetActiveCanvas() == (int)CanvasNames.addPlayerWindow)
                NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.positive, "Новый персонаж успешно создан.");
            UiManager.instance.ClearChildren(UiManager.instance.playerScrollViewContent);
            for (int i = 0; i < _count; ++i)
            {
                UiManager.instance.AddPlayerToScrollView(_packet.ReadInt(), _packet.ReadString(), _packet.ReadInt(), _packet.ReadBool());
            }
            UiManager.instance.SetActiveCanvas(CanvasNames.playerSelectWindow);
        }
        UiManager.instance.SetNicknameFieldInteractable();
    }

    public static void PlayerAlreadyExists(Packet _packet)
    {
        NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Персонаж с указанным псевдонимом уже существует.");
        UiManager.instance.SetNicknameFieldInteractable();
    }

    public static void PersonalDataCreatedFlagReceived(Packet _packet)
    {
        bool _result = _packet.ReadBool();
        Client.instance.Disconnect();
        if (_result)
        {
            UiManager.instance.SetActiveCanvas(CanvasNames.logInWindow);
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.positive, "Вы успешно зарегистрировались.");
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Указанный электронный адрес уже используется.");
        }
    }

    public static void PlayerInventoryReceived(Packet _packet)
    {
        bool _isAdmin = _packet.ReadBool();
        CurrentPlayer.instance.isAdmin = _isAdmin;
        CurrentPlayer.instance.currentPlayerUiHandler.SetActive(true);
        if (_isAdmin)
        {
            int _count = IdNameMatcher.itemTypeMatch.Count;
            UiManager.instance.ClearChildren(UiManager.instance.itemScrollViewContent);
            for (int i = 1; i < _count; ++i)
            {
                UiManager.instance.AddItemToScrollView(i, IdNameMatcher.itemTypeMatch[i], -1001);
            }
        }
        else
        {
            int _balance = _packet.ReadInt();
            CurrentPlayer.instance.Balance = _balance;
            int _count = _packet.ReadInt();
            UiManager.instance.ClearChildren(UiManager.instance.itemScrollViewContent);
            for (int i = 0; i < _count; ++i)
            {
                UiManager.instance.AddItemToScrollView(_packet.ReadInt(), _packet.ReadInt(), _packet.ReadInt());
            }
        }
        UiManager.instance.SetActiveCanvas(CanvasNames.inventoryWindow);
    }

    public static void ValidationError(Packet _packet)
    {
        NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Ошибка. Не найдено данных для совокупности Аккаунт-Персонаж.");
    }

    public static void SellItemResult(Packet _packet)
    {
        bool _result = _packet.ReadBool();
        if (_result)
        {
            SellWindow.instance.OnCancelButton();
            ClientSend.UpdatePlayerInventory();
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.positive, "Предметы успешно выставлены на продажу.");
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно выставить на продажу указанный предмет в заданном количестве.");
        }
    }

    public static void MarketRowsReceived(Packet _packet)
    {
        bool _isAdmin = _packet.ReadBool();
        CurrentPlayer.instance.isAdmin = _isAdmin;
        CurrentPlayer.instance.currentPlayerUiHandler.SetActive(true);

        int _balance = _packet.ReadInt();
        CurrentPlayer.instance.Balance = _balance;

        int _count = _packet.ReadInt();

        MarketRecordsHandler.instance.marketRows.Clear();
        for (int i = 0; i < _count; ++i)
        {
            int _marketId = _packet.ReadInt();
            int _itemId = _packet.ReadInt();
            int _typeId = _packet.ReadInt();
            int _playerId = _packet.ReadInt();
            string _nickname = _packet.ReadString();
            int _quantity = _packet.ReadInt();
            int _priceForUnit = _packet.ReadInt();
            MarketRecordsHandler.instance.marketRows.Add(new MarketRowWithNames(_marketId, _itemId, IdNameMatcher.itemNames[_itemId], _typeId, IdNameMatcher.typeNames[_typeId], _playerId, _nickname, _quantity, _priceForUnit));
        }
        MarketRecordsHandler.instance.SortAndShow();
        UiManager.instance.SetActiveCanvas(CanvasNames.marketWindow);
    }

    public static void BuyItemResult(Packet _packet)
    {
        bool _result = _packet.ReadBool();
        if (_result)
        {
            BuyWindow.instance.OnCancelButton();
            ClientSend.UpdateMarketRecords(UiManager.instance.itemNameField.text);
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.positive, "Предметы успешно приобретены.");
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно приобрести указанный предмет в заданном количестве.");
        }
    }

    public static void RemoveMarketRecordResult(Packet _packet)
    {
        bool _result = _packet.ReadBool();
        if (_result)
        {
            ClientSend.UpdateMarketRecords(UiManager.instance.itemNameField.text);
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.positive, "Предметы успешно сняты с продажи.");
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно снять предметы с продажи.");
        }
    }
}
