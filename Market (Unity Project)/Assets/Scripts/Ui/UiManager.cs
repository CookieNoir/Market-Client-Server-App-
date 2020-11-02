using UnityEngine;
using UnityEngine.UI;

public enum CanvasNames
{
    logInWindow = 0,
    playerSelectWindow = 1,
    addPlayerWindow = 2,
    registerWindow = 3,
    inventoryWindow = 4,
    marketWindow = 5,
};

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject[] canvases;
    public int activeCanvas;

    [Header("Log In Window")] // Canvases[0]
    public InputField logInEmailField;
    public InputField logInPasswordField;
    public GameObject registerButton;

    [Header("Player Select Window")] // Canvases[1]
    public Transform playerScrollViewContent;
    public GameObject playerInfoPrefab;

    [Header("Add Player Window")] // Canvases[2]
    public InputField nicknameField;

    [Header("Register Window")] // Canvases[3]
    public InputField fullNameField;
    public InputField registerEmailField;
    public InputField registerPasswordField;
    public GameObject logInButton;

    [Header("Inventory Window")] // Canvases[4]
    public Transform itemScrollViewContent;
    public GameObject itemInfoPrefab;

    [Header("Market Window")] // Canvases[5]
    public Transform marketRowScrollViewContent;
    public InputField itemNameField;
    public GameObject marketInfoPrefab;
    public GameObject marketOwnedInfoPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            activeCanvas = 0;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    private void Start()
    {
        SetCanvasesAtStart();
    }

    private void SetLogInEmailFieldInteractable()
    {
        logInEmailField.interactable = true;
        logInEmailField.text = "";
    }
    private void SetLogInPasswordFieldInteractable()
    {
        logInPasswordField.interactable = true;
        logInPasswordField.text = "";
    }
    public void SetLogInFieldsInteractable()
    {
        SetLogInEmailFieldInteractable();
        SetLogInPasswordFieldInteractable();
        registerButton.SetActive(true);
    }

    private void SetRegisterEmailFieldInteractable()
    {
        registerEmailField.interactable = true;
        registerEmailField.text = "";
    }
    private void SetFullNameFieldInteractable()
    {
        fullNameField.interactable = true;
        fullNameField.text = "";
    }
    private void SetRegisterPasswordFieldInteractable()
    {
        registerPasswordField.interactable = true;
        registerPasswordField.text = "";
    }
    public void SetRegisterFieldsInteractable()
    {
        SetFullNameFieldInteractable();
        SetRegisterEmailFieldInteractable();
        SetRegisterPasswordFieldInteractable();
        logInButton.SetActive(true);
    }

    public void SetActiveCanvas(int _canvasId)
    {
        if (activeCanvas != _canvasId)
        {
            canvases[activeCanvas].SetActive(false);
            canvases[_canvasId].SetActive(true);
            activeCanvas = _canvasId;
        }
    }

    public void SetActiveCanvas(CanvasNames _canvasName)
    {
        int _canvasId = (int)_canvasName;
        SetActiveCanvas(_canvasId);
    }

    private void SetCanvasesAtStart()
    {
        for (int i = 0; i < canvases.Length; ++i)
        {
            canvases[i].SetActive(false);
        }
        canvases[activeCanvas].SetActive(true);
    }

    public void ClearChildren(Transform _target)
    {
        foreach (Transform child in _target)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddPlayerToScrollView(int _id, string _nickname, int _balance, bool _isAdmin)
    {
        PlayerInfo _playerInfo = Instantiate(playerInfoPrefab, playerScrollViewContent).GetComponent<PlayerInfo>();
        _playerInfo.SetValues(_id, _nickname, _balance, _isAdmin);
    }

    public void AddItemToScrollView(int _itemId, int _typeId, int _quantity)
    {
        ItemInfo _itemInfo = Instantiate(itemInfoPrefab, itemScrollViewContent).GetComponent<ItemInfo>();
        _itemInfo.SetValues(_itemId, _typeId, _quantity);
    }

    public void AddMarketRowToScrollView(int _marketId, int _itemId, string _itemName, int _typeId, string _typeName, int _playerId, string _nickname, int _quantity, int _priceForUnit)
    {
        MarketInfo _marketInfo = Instantiate(marketInfoPrefab, marketRowScrollViewContent).GetComponent<MarketInfo>();
        _marketInfo.SetValues(_marketId, _itemId, _itemName, _typeId, _typeName, _playerId, _nickname, _quantity, _priceForUnit);
    }

    public void AddMarketRowToScrollView(MarketRowWithNames _marketRow, bool _isOwned)
    {
        if (_isOwned)
        {
            MarketInfo _marketInfo = Instantiate(marketOwnedInfoPrefab, marketRowScrollViewContent).GetComponent<MarketInfo>();
            _marketInfo.SetValues(_marketRow);
        }
        else
        {
            MarketInfo _marketInfo = Instantiate(marketInfoPrefab, marketRowScrollViewContent).GetComponent<MarketInfo>();
            _marketInfo.SetValues(_marketRow);
        }
    }

    public int GetActiveCanvas()
    {
        return activeCanvas;
    }

    public void ConnectToServer()
    {
        if (DataHelper.IsEmailCorrect(logInEmailField.text) && DataHelper.IsPasswordCorrect(logInPasswordField.text))
        {
            logInEmailField.interactable = false;
            logInPasswordField.interactable = false;
            registerButton.SetActive(false);
            Client.instance.ConnectToServer(false);
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Поля заполнены некорректно.");
        }
    }

    public void OnAddPlayerButton()
    {
        if (DataHelper.IsPlayerNicknameCorrect(nicknameField.text))
        {
            nicknameField.interactable = false;
            ClientSend.AddNewPlayer(nicknameField.text);
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Невозможно создать персонажа с указанным псевдонимом.");
        }
    }

    public void SetNicknameFieldInteractable()
    {
        nicknameField.interactable = true;
        nicknameField.text = "";
    }

    public void OnCreatePersonalDataButton()
    {
        if (DataHelper.IsFullNameCorrect(fullNameField.text) && DataHelper.IsEmailCorrect(registerEmailField.text) && DataHelper.IsPasswordCorrect(registerPasswordField.text))
        {
            fullNameField.interactable = false;
            registerEmailField.interactable = false;
            registerPasswordField.interactable = false;
            logInButton.SetActive(false);
            Client.instance.ConnectToServer(true);
        }
        else
        {
            NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Поля заполнены некорректно.");
        }
    }

    public void OnUpdateInventoryButton()
    {
        ClientSend.UpdatePlayerInventory();
    }

    public void OnReturnToPlayersListButton()
    {
        CurrentPlayer.instance.currentPlayerUiHandler.SetActive(false);
        ClientSend.UpdatePlayersList();
    }

    public void OnUpdateMarketRecordsButton()
    {
        ClientSend.UpdateMarketRecords(itemNameField.text);
    }
}
