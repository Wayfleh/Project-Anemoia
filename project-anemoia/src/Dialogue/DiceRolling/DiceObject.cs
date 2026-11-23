using Godot;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Text.RegularExpressions;

public partial class DiceObject : RigidBody3D
{
	[Signal] public delegate void DiceFinishedEventHandler();

	[Export] Material critMaterial;
	[Export] Material succMaterial; //shut it
	[Export] Material failMaterial;
	private MeshInstance3D diceMesh;
	[Export] float minAngularSpeed = 0.5f;
	[Export] float minLinearSpeed = 1.0f;
	[Export] float minRollingTimeS = 2.0f;
	private float currentRollingTime = 0.0f;
	MeshInstance3D upwardFacingMeshInst3D = null;
	private Node3D faceNormals;

	private bool startWithRandomSide = true;

	public bool MarkedFinished { get; private set; } = false;//used to check if dice has stopped
	public int ValueResult { get; private set; }

	public override void _Ready()
	{
		diceMesh = GetNode<MeshInstance3D>("DiceMesh");
		faceNormals = GetNode<Node3D>("FaceNormals");
		SleepingStateChanged += OnSleepStateChanged;
		if (startWithRandomSide)
		{
			float x = (float)GD.RandRange(0, 360f);
			float y = (float)GD.RandRange(0, 360f);
			float z = (float)GD.RandRange(0, 360f);
			RotateX(x);
			RotateX(y);
			RotateX(z);
		}
	}

	float jitterX, jitterY, jitterZ;
	public override void _Process(double delta)
	{
		float deltaF = (float)delta;
		if (MarkedFinished)
		{
			return;
		}
		else if (currentRollingTime >= minRollingTimeS && ( (AngularVelocity.DistanceSquaredTo(Vector3.Zero) < minAngularSpeed) || (LinearVelocity.DistanceSquaredTo(Vector3.Zero) < minLinearSpeed) ) )
		{
			HandleDiceFinished();
		}
		else
			currentRollingTime += deltaF;

	}

    public override void _PhysicsProcess(double delta)
    {
		if (MarkedFinished || ( (AngularVelocity.DistanceSquaredTo(Vector3.Zero) < minAngularSpeed) && (LinearVelocity.DistanceSquaredTo(Vector3.Zero) < minLinearSpeed) ) )
		{
			AngularVelocity.Lerp(Vector3.Zero, (float)delta * 50);
			LinearVelocity.Lerp(Vector3.Zero, (float)delta * 5);
		}
		if (currentRollingTime >= minRollingTimeS && !Sleeping && AngularVelocity.DistanceSquaredTo(Vector3.Zero) < 0.05 && LinearVelocity.DistanceSquaredTo(Vector3.Zero) < 0.1)
		{
			Freeze = true;
			Sleeping = true;
		}
    }

	private void OnSleepStateChanged()
	{
		//in case we have new cool features that moves the dice again after they stop
		if (!Sleeping)
		{
			MarkedFinished = false;
			Debug.Print("not sleeping");
		}
	}

	private void HandleDiceFinished()
	{
		MarkedFinished = true;
		currentRollingTime = 0.0f;
		EmitSignal(SignalName.DiceFinished);
	}

	public void ShowMesh(String outcome)
	{

		int topFace = GetUpwardFacingSide();
		if (topFace < 0)
		{
			GD.Print("Top face not found");
			throw new Exception("Upward facing side not determinible for dice: " + this);
		}
		switch(outcome){
			case "success":
				diceMesh.SetSurfaceOverrideMaterial(topFace, succMaterial);
				break;
			case "failure":
				diceMesh.SetSurfaceOverrideMaterial(topFace, failMaterial);
				break;
			case "crit":
				diceMesh.SetSurfaceOverrideMaterial(topFace, critMaterial);
				break;
			default:
				System.Diagnostics.Debugger.Break();
				throw new Exception("outcome must be success, failure, or crit");
		}
	}
	

	//returns number of upwards facing side, returns -1 if not found
	private int GetUpwardFacingSide()
	{
		int upFace = -1;
		float bestDotProd = float.NegativeInfinity;
		foreach (RayCast3D ray in faceNormals.GetChildren())
		{ //convert ray to global space, compare with up vector; what's up vector? go fuck yourself
			var globalRay = (ray.ToGlobal(ray.TargetPosition) - ray.GlobalPosition).Normalized();
			var upDot = globalRay.Dot(Vector3.Up);
			if (upDot > bestDotProd)
			{
				bestDotProd = upDot;
				upFace = ray.GetIndex();
			}
		}
		return upFace;
	}

	public bool IsOnTopOfDie()
	{
		RayCast3D downRay = null;
		float bestDotProd = float.NegativeInfinity;
		foreach (RayCast3D ray in faceNormals.GetChildren())
		{ 
			var globalRay = (ray.ToGlobal(ray.TargetPosition) - ray.GlobalPosition).Normalized();
			var upDot = globalRay.Dot(Vector3.Down);
			if (upDot > bestDotProd)
			{
				bestDotProd = upDot;
				downRay = ray;
			}
		}
		if (downRay.GetCollider() is DiceObject die)
		{
			MarkedFinished = false;
			Freeze = false;
			return true;
		}
		return false;
	}
}
