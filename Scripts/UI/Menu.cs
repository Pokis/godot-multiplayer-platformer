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


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        HostButtonNode.Pressed += HostButtonNode_Pressed;
        JoinButtonNode.Pressed += JoinButtonNode_Pressed;
        StartButtonNode.Pressed += StartButtonNode_Pressed;

    }

    private void StartButtonNode_Pressed()
    {
    }

    private void JoinButtonNode_Pressed()
    {
    }

    private void HostButtonNode_Pressed()
    {
        Lobby.CreateGame();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
