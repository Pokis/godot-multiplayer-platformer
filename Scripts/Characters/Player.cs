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

    public override void _PhysicsProcess(double delta)
	{
        var horizontalInput = Input.GetActionStrength(Constants.INPUT_MOVE_RIGHT) 
            - Input.GetActionStrength(Constants.INPUT_MOVE_LEFT);

        float xVelocity = horizontalInput * movementSpeed;
        float yVelocity = Velocity.Y + gravity;
        //Velocity.X = horizontalInput * movementSpeed;
        //Velocity.Y += gravity;

        if(Input.IsActionJustPressed(Constants.INPUT_JUMP) && IsOnFloor())
        {
            yVelocity = -jumpStrength;
        }

        Velocity = new Vector2(xVelocity, yVelocity);

        MoveAndSlide();
	}
}
