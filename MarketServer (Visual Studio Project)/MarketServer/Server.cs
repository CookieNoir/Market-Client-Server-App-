using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MarketServer
{
    class Server
    {
        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            maxPlayers = _maxPlayers;
            port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on port {port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _ar)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_ar);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= maxPlayers; ++i)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server is full!");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= maxPlayers; ++i)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.newPlayerReceived, ServerHandle.NewPlayerReceived },
                {(int)ClientPackets.removePlayerIdReceived, ServerHandle.RemovePlayerIdReceived },
                {(int)ClientPackets.newPersonalDataReceived, ServerHandle.NewPersonalDataReceived },
                {(int)ClientPackets.currentPlayerSelected, ServerHandle.SelectPlayer },
                {(int)ClientPackets.updateCurrentPlayerInventory, ServerHandle.UpdateCurrentPlayerInventory },
                {(int)ClientPackets.updatePlayersList, ServerHandle.UpdatePlayersList },
                {(int)ClientPackets.sellItemReceived, ServerHandle.SellItemReceived },
                {(int)ClientPackets.updateMarketRecords, ServerHandle.UpdateMarketRecords },
                {(int)ClientPackets.buyItemReceived, ServerHandle.BuyItemReceived },
                {(int)ClientPackets.removeMarketRecordReceived, ServerHandle.RemoveMarketRecordReceived },
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
