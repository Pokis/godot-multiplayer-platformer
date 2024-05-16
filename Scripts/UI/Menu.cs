using Godot;
using godotmultiplayerplatformer.Scripts.Multiplayer;
using System;

public partial class Menu : Node
{
	[Export]
	private Button HostButtonNode { get; set; }

    [Export]
    private Button StartButtonNode { get; set; }

    [Export]
    private Button JoinButtonNode { get; set; }

    [Export]
    private LineEdit IpAddressLineEditNode { get; set; }

    [Export]
    private Label StatusLabel { get; set; }

    [Export]
    private Node LevelContainerNode { get; set; }

    [Export]
    private PackedScene LevelScene { get; set; }

    [Export]
    private HBoxContainer NotConnectedHboxNode { get; set; }

    [Export]
    private HBoxContainer HostHboxNode { get; set; }

    [Export]
    private Control UI { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        HostButtonNode.Pressed += HostButtonNode_Pressed;
        JoinButtonNode.Pressed += JoinButtonNode_Pressed;
        StartButtonNode.Pressed += StartButtonNode_Pressed;
        Multiplayer.ConnectionFailed += Multiplayer_ConnectionFailed;
        Multiplayer.ConnectedToServer += Multiplayer_ConnectedToServer;
    }

    private void Multiplayer_ConnectedToServer()
    {
        StatusLabel.Text = "Connected!";
    }

    private void Multiplayer_ConnectionFailed()
    {
        StatusLabel.Text = "Connection failed.";
        NotConnectedHboxNode.Show();
    }

    private void StartButtonNode_Pressed()
    {
        //ChangeLevel(LevelScene);
        CallDeferred(nameof(ChangeLevel), LevelScene);
        Rpc(nameof(HideMenu));
    }

    private void ChangeLevel(PackedScene scene)
    {
        foreach (var child in LevelContainerNode.GetChildren())
        {
            LevelContainerNode.RemoveChild(child);
            child.QueueFree();
        }

        LevelContainerNode.AddChild(scene.Instantiate());
    }

    private void JoinButtonNode_Pressed()
    {
        var lobby = GetTree().Root.GetChild<Lobby>(0);
        var ipAddress = IpAddressLineEditNode.Text;
        if (lobby != null)
        {
            GD.Print("Found the lobby, joining game.");
            lobby.JoinGame(ipAddress);
            StatusLabel.Text = "Connecting...";
        }

        NotConnectedHboxNode.Hide();
    }

    private void HostButtonNode_Pressed()
    {
        var lobby = GetTree().Root.GetChild<Lobby>(0);
        if(lobby != null)
        {
            GD.Print("Found the lobby, creating game.");
            lobby.CreateGame();
            StatusLabel.Text = "Host Created!";
        }

        NotConnectedHboxNode.Hide();
        HostHboxNode.Show();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void HideMenu()
    {
        UI.Hide();
    }
}
