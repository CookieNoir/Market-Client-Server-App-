using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 80;
    public int id = 0;
    public TCP tcp;

    public bool createNewPersonalData = false;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer(bool _createNewPersonalData)
    {
        InitializeClientData();
        isConnected = true;
        createNewPersonalData = _createNewPersonalData;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient { ReceiveBufferSize = dataBufferSize, SendBufferSize = dataBufferSize };

            receiveBuffer = new byte[dataBufferSize];
            IAsyncResult result = socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            bool success = result.AsyncWaitHandle.WaitOne(4000, true);
            if (!socket.Connected)
            {
                NotificationSystem.instance.Notify(NotificationSystem.NotificationTypes.negative, "Отсутствует соединение с сервером.");
                instance.Disconnect();
            }
        }

        private void ConnectCallback(IAsyncResult _ar)
        {
            socket.EndConnect(_ar);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server with TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _ar)
        {
            try
            {
                int _byteLength = stream.EndRead(_ar);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome},
            {(int)ServerPackets.sendPlayers, ClientHandle.PlayersReceived},
            {(int)ServerPackets.playerAlreadyExists, ClientHandle.PlayerAlreadyExists},
            {(int)ServerPackets.personalDataCreatedFlagReceived, ClientHandle.PersonalDataCreatedFlagReceived},
            {(int)ServerPackets.playerInventoryReceived, ClientHandle.PlayerInventoryReceived},
            {(int)ServerPackets.validationError, ClientHandle.ValidationError},
            {(int)ServerPackets.sellItemResult, ClientHandle.SellItemResult},
            {(int)ServerPackets.MarketRecordsReceived, ClientHandle.MarketRowsReceived},
            {(int)ServerPackets.buyItemResultReceived, ClientHandle.BuyItemResult},
            {(int)ServerPackets.removeMarketRecordResultReceived, ClientHandle.RemoveMarketRecordResult},
        };
        Debug.Log("Initialized packets.");
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            if (createNewPersonalData)
            {
                UiManager.instance.SetActiveCanvas(CanvasNames.registerWindow);
            }
            else
            {
                UiManager.instance.SetActiveCanvas(CanvasNames.logInWindow);
            }
            UiManager.instance.SetLogInFieldsInteractable();
            UiManager.instance.SetRegisterFieldsInteractable();
            Debug.Log("Disconnected from server.");
            tcp.socket.Close();
            isConnected = false;
        }
    }
}
