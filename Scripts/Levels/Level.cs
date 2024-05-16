using Godot;
using System;
using System.Linq;

public partial class Level : Node2D
{
    [Export]
    private Node2D PlayersContainer {  get; set; }
    
    [Export]
    private PackedScene PlayerScene { get; set; }

    [Export]
    private Node2D[] SpawnPoints { get; set; }

    int nextSpawnPoint = 0;

    public override void _Ready()
    {
        base._Ready();
        
        if (!Multiplayer.IsServer())
        {
            return;
        }

        Multiplayer.PeerConnected += Multiplayer_PeerConnected;
        Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;

        foreach (var id in Multiplayer.GetPeers())
        {
            AddPlayer(id);
        }

        AddPlayer(1);
    }

    public void ExitTree()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }
        Multiplayer.PeerConnected -= Multiplayer_PeerConnected;
        Multiplayer.PeerDisconnected -= Multiplayer_PeerDisconnected;
    }

    private void Multiplayer_PeerDisconnected(long id)
    {
        GD.Print("Removing player who has left the game.");
        DeletePlayer(id);
    }

    private void Multiplayer_PeerConnected(long id)
    {
        GD.Print("Adding late player");
        AddPlayer(id);
    }

    private void AddPlayer(long playerId)
    {
        var playerInstance = PlayerScene.Instantiate<Player>();
        playerInstance.Name = playerId.ToString();
        PlayersContainer.AddChild(playerInstance);
        playerInstance.Position = GetNextSpawnPoint();
    }

    private void DeletePlayer(long playerId)
    {
        if (PlayersContainer.HasNode(playerId.ToString()))
        {
            PlayersContainer.GetNode(playerId.ToString()).QueueFree();
        }
    }

    private Vector2 GetNextSpawnPoint()
    {
        nextSpawnPoint++;
        nextSpawnPoint = Mathf.Wrap(nextSpawnPoint, 1, SpawnPoints.Length);
        GD.Print($"Next calculated spawn point is: {nextSpawnPoint}");
        var spawnPoint = SpawnPoints[nextSpawnPoint].Position;

        return spawnPoint;
    }
}
