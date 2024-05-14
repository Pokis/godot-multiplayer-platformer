using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace godotmultiplayerplatformer.Scripts.Multiplayer
{
    internal partial class Lobby : Node
    {
        const int PORT = 7000;
        const int MAX_CONNECTIONS = 2;

        public event Action<long, string> PlayerConnected;
        public event Action<long> PlayerDisconnected;
        public event Action ServerDisconnected;

        private Dictionary<long, string> players = new Dictionary<long, string>();

        public override void _Ready()
        {
            base._Ready();
            Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
            Multiplayer.ConnectionFailed += Multiplayer_ConnectionFailed;
            Multiplayer.ConnectedToServer += Multiplayer_ConnectedToServer;
            Multiplayer.ServerDisconnected += Multiplayer_ServerDisconnected;
        }

        private void Multiplayer_ServerDisconnected()
        {
            GD.Print("Server disconnected");
        }

        private void Multiplayer_ConnectedToServer()
        {
            GD.Print("Connected to server");
        }

        private void Multiplayer_ConnectionFailed()
        {
            GD.Print("Connection failed");
        }

        private void Multiplayer_PeerDisconnected(long id)
        {
            RemovePlayer(id);
        }

        private void Multiplayer_PeerConnected(long id)
        {
            AddPlayer(id);
        }

        public void CreateGame()
        {
            var peer = new ENetMultiplayerPeer();
            var error = peer.CreateServer(PORT, MAX_CONNECTIONS);
            
            if(error != null)
            {
                GD.Print(error);
            }

            GD.Print(error);

            Multiplayer.MultiplayerPeer = peer;

            AddPlayer(1, "The Host");
        }

        private void AddPlayer(long id)
        {
            AddPlayer(id, $"Player{id}");
        }

        private void AddPlayer(long id, string playerName)
        {
            players.Add(1, playerName);
            InvokePlayerConnected(id, playerName);
        }

        private void RemovePlayer(long id)
        {
            players.Remove(id);
            InvokePlayerDisconnected(id);
        }

        private void InvokePlayerConnected(long id, string name)
        {
            PlayerConnected?.Invoke(id, name);
        }

        private void InvokePlayerDisconnected(long id)
        {
            PlayerDisconnected?.Invoke(id);
        }

        private void InvokeServerDisconnected()
        {
            ServerDisconnected?.Invoke();
        }
    }
}
