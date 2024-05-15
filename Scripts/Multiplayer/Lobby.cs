using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Godot.Projection;

namespace godotmultiplayerplatformer.Scripts.Multiplayer
{
    internal partial class Lobby : Node
    {
        const int PORT = 7000;
        const int MAX_CONNECTIONS = 2;

        public event Action<long, PlayerInformation> PlayerConnected;
        public event Action<long> PlayerDisconnected;
        public event Action ServerDisconnected;

        private Dictionary<long, PlayerInformation> players = new Dictionary<long, PlayerInformation>();

        private PlayerInformation playerInfo { get; set; } = new PlayerInformation(-1);

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
            Multiplayer.MultiplayerPeer = null;
            players.Clear();
            InvokeServerDisconnected();
        }

        private void Multiplayer_ConnectedToServer()
        {
            var peerId = Multiplayer.GetUniqueId();
            //players.Add(peerId, new PlayerInformation(peerId));
            //AddPlayer(peerId);
            GD.Print($"[{Multiplayer.GetUniqueId()}]: {peerId} Connected to server");
        }

        private void Multiplayer_ConnectionFailed()
        {
            Multiplayer.MultiplayerPeer = null; 
            GD.Print("Connection failed, removing peer");
        }

        private void Multiplayer_PeerDisconnected(long id)
        {
            GD.Print($"[{Multiplayer.GetUniqueId()}]: Peer {id} disconnected");
            RemovePlayer(id);
        }

        private void Multiplayer_PeerConnected(long id)
        {
            GD.Print($"[{Multiplayer.GetUniqueId()}]: Peer {id} connected, calling RPC in {Multiplayer.GetUniqueId()}! sending my id of {playerInfo.Id}!");
            //RpcId(id, nameof(EchoRPC), 420);
            //var playerInfoJson = System.Text.Json.JsonSerializer.Serialize(playerInfo);
            string playerInfoJson = JsonSerializer.Serialize(playerInfo);
            RpcId(id, nameof(RegisterPlayer), playerInfoJson);

            //GD.Print($"[{Multiplayer.GetUniqueId()}]: Peer {id} connected, Adding him to my internal players list!");
            //AddPlayer(id);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void RegisterPlayer(string playerInfoJson)
        {
            //var playerInfo = System.Text.Json.JsonSerializer.Deserialize<PlayerInformation>(playerInfoJson);
            PlayerInformation playerInfo = JsonSerializer.Deserialize<PlayerInformation>(playerInfoJson);
            var newPlayerId = Multiplayer.GetRemoteSenderId();
            GD.Print($"[{Multiplayer.GetUniqueId()}]:RPC received!, adding player {newPlayerId}/{playerInfo.Id}:{playerInfo.Name}, Adding him to my internal players list!");
            AddPlayer(newPlayerId, playerInfo);
        }

        public void CreateGame()
        {
            var peer = new ENetMultiplayerPeer();
            var error = peer.CreateServer(PORT, MAX_CONNECTIONS);
            
            if(error != Error.Ok)
            {
                GD.Print($"Error when creating server: {error}");
                throw new Exception("Error when creating server");
            }

            Multiplayer.MultiplayerPeer = peer;

            AddPlayer(1, "The Host");
            playerInfo = new PlayerInformation(1, "The Host");
            GD.Print($"[{Multiplayer.GetUniqueId()}]: Game created! Im the host!");
        }

        public void JoinGame(string ipAddress)
        {
            GD.Print("Joining game...");
            var peer = new ENetMultiplayerPeer();
            var error = peer.CreateClient(ipAddress, PORT);

            if (error != Error.Ok)
            {
                GD.Print($"Error when creating server: {error}");
                throw new Exception("Error when joining a server");
            }
            playerInfo = new PlayerInformation(peer.GetUniqueId());

            Multiplayer.MultiplayerPeer = peer;
            GD.Print($"[{Multiplayer.GetUniqueId()}]: Joined game {peer.GetUniqueId()}");
        }

        private void AddPlayer(long id)
        {
            AddPlayer(id, $"Player{id}");
        }

        private void AddPlayer(long id, string playerName)
        {
            var playerInfo = new PlayerInformation(id, playerName);
            AddPlayer(id, playerInfo);
        }

        private void AddPlayer(long id, PlayerInformation playerInfo)
        {
            GD.Print($"[{Multiplayer.GetUniqueId()}] Adding player {id} to dictionary");
            players.Add(id, playerInfo);
            InvokePlayerConnected(id, playerInfo);
        }

        private void RemovePlayer(long id)
        {
            players.Remove(id);
            InvokePlayerDisconnected(id);
        }

        private void InvokePlayerConnected(long id, PlayerInformation info)
        {
            PlayerConnected?.Invoke(id, info);
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
