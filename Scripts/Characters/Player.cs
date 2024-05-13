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

    [Export]
    private AnimatedSprite2D playerSprite;

    private Vector2 initialSpriteScale;

    public override void _Ready()
    {
        base._Ready();
        initialSpriteScale = playerSprite.Scale;
    }

    public override void _PhysicsProcess(double delta)
	{
        var horizontalInput = Input.GetActionStrength(Constants.INPUT_MOVE_RIGHT) 
            - Input.GetActionStrength(Constants.INPUT_MOVE_LEFT);

        float xVelocity = horizontalInput * movementSpeed;
        float yVelocity = Velocity.Y + gravity;

        var isFalling = Velocity.Y > 0 && !IsOnFloor();
        var isJumping = Input.IsActionJustPressed(Constants.INPUT_JUMP) && IsOnFloor();
        var isJumpCancelled = Input.IsActionJustReleased(Constants.INPUT_JUMP) && Velocity.Y < 0;
        var isIdle = IsOnFloor() && Mathf.IsZeroApprox(Velocity.X);
        var isWalking = IsOnFloor() && !Mathf.IsZeroApprox(Velocity.X);

        if (isJumping)
        {
            yVelocity = -jumpStrength;
        }

        Velocity = new Vector2(xVelocity, yVelocity);

        MoveAndSlide();

        if(isJumping)
        {
            GD.Print($"Playing " + Constants.ANIMATION_JUMP_START);
            playerSprite.Play(Constants.ANIMATION_JUMP_START);
        }
        else if(isWalking)
        {

            GD.Print($"Playing " + Constants.ANIMATION_Walk);
            playerSprite.Play(Constants.ANIMATION_Walk);
        }
        else if (isFalling)
        {

            GD.Print($"Playing " + Constants.ANIMATION_FALL);
            playerSprite.Play(Constants.ANIMATION_FALL);
        }
        else if (isIdle)
        {
            GD.Print($"Playing " + Constants.ANIMATION_IDLE);
            playerSprite.Play(Constants.ANIMATION_IDLE);
        }


        if (Mathf.IsZeroApprox(horizontalInput))
        {
            return;
        }

        if(horizontalInput < 0)
        {
            playerSprite.Scale = new Vector2(-initialSpriteScale.X, initialSpriteScale.Y);
        }
        else if (horizontalInput > 0)
        {
            playerSprite.Scale = initialSpriteScale;
        }
	}
}
