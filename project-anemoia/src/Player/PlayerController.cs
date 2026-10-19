using Godot;
using System;
using System.Numerics;
namespace Godot;

public partial class PlayerController : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float SENSITIVITY = 0.003f;
	public Camera3D camera;
	public Node3D head;

	//Head movement variables
	private const float BOB_FREQUENCY = 2.0f;
	private const float BOB_AMPLITUDE = 0.08f;
	private float tBob = 1.0f; // growing time variable; tells player where during the headbob they're in
	private float head_tilt = Mathf.DegToRad(3);

    public override void _Ready()
    {
        base._Ready();
		camera = GetNode<Camera3D>("Head/Camera3D");
		head = GetNode<Node3D>("Head");
    }


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		// Head Bob

		Transform3D bobTransform = camera.Transform;
		if(inputDir != Vector2.Zero)
		{
			float onFloor = IsOnFloor() ? 1f : 0f;
			tBob += (float)delta * velocity.Length() * onFloor;
			bobTransform.Origin = headbob(tBob);
		}
		else
		{
			bobTransform.Origin.X = Mathf.Lerp(bobTransform.Origin.X, 0, (float)delta * 4);
			bobTransform.Origin.Y = Mathf.Lerp(bobTransform.Origin.Y, 0, (float)delta * 4);
			tBob = 1.0f;
		}
		camera.Transform = bobTransform;

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
		if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			Input.SetMouseMode(Input.MouseModeEnum.Captured);
			head.RotateY(-mouseMotionEvent.Relative.X * SENSITIVITY);
			camera.RotateX(-mouseMotionEvent.Relative.Y * SENSITIVITY);
			Vector3 camRot = camera.Rotation;
			camRot.X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-70), Mathf.DegToRad(60));

			camera.Rotation = camRot;
		}
    }

	private Vector3 headbob(float time)
	{
		Vector3 pos = Vector3.Zero;
		pos.Y = Mathf.Sin(1 + time * BOB_FREQUENCY) * BOB_AMPLITUDE;
		pos.X = Mathf.Cos(time * BOB_FREQUENCY * 0.5f) * BOB_AMPLITUDE * 2;
		return pos;


	}
}
