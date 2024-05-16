using Godot;
using godotmultiplayerplatformer.Scripts.General;
using System;

public partial class Player : CharacterBody2D
{

	[Export(PropertyHint.Range, "1,1000,1")]
	private float movementSpeed = 300f;

    [Export(PropertyHint.Range, "1,1000,1")]
    private float gravity = 30f;

    [Export(PropertyHint.Range, "1,1000,1")]
    private float jumpStrength = 600f;

    [Export(PropertyHint.Range, "1,10,1")]
    private int maxJumps = 2;

    [Export]
    private AnimatedSprite2D playerSprite;

    [Export]
    private PackedScene playerCamera;

    [Export]
    private float cameraHeight = 20;

    private Vector2 initialSpriteScale;
    private int jumpCount = 0;
    private Camera2D cameraInstance;
    private int ownerId = 1;
    public override void _EnterTree()
    {
        base._EnterTree();
        initialSpriteScale = playerSprite.Scale;
        playerSprite.AnimationFinished += PlayerSprite_AnimationFinished;
        

        GD.Print($"My name is: {Name}");
        ownerId = int.Parse(Name);
        SetMultiplayerAuthority(ownerId);

        if(ownerId != Multiplayer.GetUniqueId())
        {
            return;
        }

        AddCameraToPlayer();
    }

    public override void _PhysicsProcess(double delta)
	{
        if (ownerId != Multiplayer.GetUniqueId())
        {
            return;
        }

        var horizontalInput = Input.GetActionStrength(Constants.INPUT_MOVE_RIGHT) 
            - Input.GetActionStrength(Constants.INPUT_MOVE_LEFT);

        float xVelocity = horizontalInput * movementSpeed;
        float yVelocity = Velocity.Y + gravity;

        HandleMovementState(xVelocity, yVelocity);
        MoveAndSlide();

        FaceMovementDirection(horizontalInput);
	}

    private void HandleMovementState(float xVelocity, float yVelocity)
    {

        var isFalling = Velocity.Y > 0 && !IsOnFloor();
        var isJumping = Input.IsActionJustPressed(Constants.INPUT_JUMP) && IsOnFloor();
        var isDoubleJumping = Input.IsActionJustPressed(Constants.INPUT_JUMP) && isFalling;
        var isJumpCancelled = Input.IsActionJustReleased(Constants.INPUT_JUMP) && Velocity.Y < 0;
        var isIdle = IsOnFloor() && Mathf.IsZeroApprox(Velocity.X);
        var isWalking = IsOnFloor() && !Mathf.IsZeroApprox(Velocity.X);

        if (isJumping)
        {
            playerSprite.Play(Constants.ANIMATION_JUMP_START);
        }
        if (isDoubleJumping)
        {
            playerSprite.Play(Constants.ANIMATION_DOUBLE_JUMP_START);
        }
        else if (isWalking)
        {
            playerSprite.Play(Constants.ANIMATION_Walk);
        }
        else if (isFalling)
        {
            playerSprite.Play(Constants.ANIMATION_FALL);
        }
        else if (isIdle)
        {
            playerSprite.Play(Constants.ANIMATION_IDLE);
        }

        if (isJumping)
        {
            jumpCount++;
            yVelocity = -jumpStrength;
        }
        else if (isDoubleJumping)
        {
            jumpCount++;
            if (jumpCount <= maxJumps)
            {
                yVelocity = -jumpStrength;
            }
        }
        else if (isJumpCancelled)
        {
            yVelocity = 0;
        }

        else if (IsOnFloor())
        {
            jumpCount = 0;
        }

        Velocity = new Vector2(xVelocity, yVelocity);
    }

    private void FaceMovementDirection(float horizontalInput)
    {
        if (Mathf.IsZeroApprox(horizontalInput))
        {
            return;
        }

        if (horizontalInput < 0)
        {
            playerSprite.Scale = new Vector2(-initialSpriteScale.X, initialSpriteScale.Y);
        }
        else if (horizontalInput > 0)
        {
            playerSprite.Scale = initialSpriteScale;
        }
    }

    private void PlayerSprite_AnimationFinished()
    {
        playerSprite.Play(Constants.ANIMATION_JUMP);
    }

    private void AddCameraToPlayer()
    {
        cameraInstance = playerCamera.Instantiate<Camera2D>();
        cameraInstance.GlobalPosition = new Vector2(cameraInstance.GlobalPosition.X, cameraHeight);
        //TODO: See if there are better ways to add child with deferred call in c#,
        //as this GetTree().CurrentScene.AddChild(cameraInstance); doesnt work
        GetTree().CurrentScene.CallDeferred("add_child", cameraInstance);
    }

    private void UpdateCamera()
    {
        cameraInstance.GlobalPosition = new Vector2(this.GlobalPosition.X, cameraHeight);
    }

    public override void _Process(double delta)
    {
        if (Multiplayer.MultiplayerPeer == null)
        {
            return;
        }

        if (ownerId != Multiplayer.GetUniqueId())
        {
            return;
        }

        base._Process(delta);
        UpdateCamera();
    }
}
