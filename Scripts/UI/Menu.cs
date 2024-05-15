using Godot;
using godotmultiplayerplatformer.Scripts.Multiplayer;
using System;

public partial class Menu : Control
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
    }

    private void StartButtonNode_Pressed()
    {
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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
