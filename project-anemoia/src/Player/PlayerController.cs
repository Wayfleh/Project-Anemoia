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
	private float tBob = 0.0f; // growing time variable; tells player where during the headbob they're in
	private float head_tilt = Mathf.DegToRad(1.5f);
	private Boolean is_rested = true;

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
		if(inputDir != Vector2.Zero && is_rested)
		{
			float onFloor = IsOnFloor() ? 1f : 0f;
			tBob += (float)delta * velocity.Length() * onFloor;

			bobTransform.Origin.X = Mathf.Lerp(bobTransform.Origin.X, headbob(tBob).X, (float)delta * 10);
			bobTransform.Origin.Y = Mathf.Lerp(bobTransform.Origin.Y, headbob(tBob).Y, (float)delta * 10);
		}
		else //resets transform origin and time so it looks more natural
		{
			bobTransform.Origin.X = Mathf.Lerp(bobTransform.Origin.X, 0, (float)delta * 4);
			bobTransform.Origin.Y = Mathf.Lerp(bobTransform.Origin.Y, 0, (float)delta * 4);
		}

		if (inputDir == Vector2.Zero && Mathf.IsEqualApprox(bobTransform.Origin.X, 0))
		{
			is_rested = true;
			tBob = 0.0f;
		}
		camera.Transform = bobTransform;

		//head tilt
		Vector3 headRot = head.Rotation;
		if (inputDir.X < 0) // Left tilt
			headRot.Z = Mathf.Lerp(headRot.Z, head_tilt, (float) delta * 4);
		else if (inputDir.X > 0) // Right tilt
			headRot.Z = Mathf.Lerp(headRot.Z, -head_tilt, (float) delta * 4);
		else
			headRot.Z = Mathf.Lerp(headRot.Z, 0.0f, (float) delta * 7);
		
		if (Input.IsActionPressed("move_forward"))
			head_tilt = Mathf.DegToRad(1);
		else
			head_tilt = Mathf.DegToRad(1.5f);
		head.Rotation = headRot;

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
		pos.Y = Mathf.Sin(time * BOB_FREQUENCY) * BOB_AMPLITUDE;
		pos.X = Mathf.Cos(1 + time * BOB_FREQUENCY * 0.5f) * BOB_AMPLITUDE * 2;
		return pos;
	}
}
