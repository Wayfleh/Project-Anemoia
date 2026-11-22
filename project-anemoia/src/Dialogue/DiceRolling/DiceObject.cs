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
	[Export] private float jitterMagnitude = 7;
	[Export] private Color glowingColor;

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
		if (MarkedFinished || (AngularVelocity.DistanceSquaredTo(Vector3.Zero) > 0.5f) || (LinearVelocity.DistanceSquaredTo(Vector3.Zero) > 1f))
			return;

		diceMesh.RotateZ(-jitterZ);
		diceMesh.RotateY(-jitterY);
		diceMesh.RotateX(-jitterX);

		jitterX = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		jitterY = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		jitterZ = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		diceMesh.RotateX(jitterX);
		diceMesh.RotateY(jitterY);
		diceMesh.RotateZ(jitterZ);
	}

	private void HighlightSide(MeshInstance3D meshInstance3D)
	{
		StandardMaterial3D highlightedVersionMaterial = (StandardMaterial3D)meshInstance3D.Mesh.SurfaceGetMaterial(0).Duplicate();
		highlightedVersionMaterial.AlbedoColor = glowingColor;
		meshInstance3D.MaterialOverride = highlightedVersionMaterial;
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

	public override void _PhysicsProcess(double delta)
	{
		if (Sleeping && !MarkedFinished)
		{
			HandleDiceFinished();
		}
	}
	private void HandleDiceFinished()
	{
		MarkedFinished = true;

		HighlightSide(upwardFacingMeshInst3D);
		EmitSignal(SignalName.DiceFinished);
	}

	public void ShowMesh(String outcome)
	{
		int topFace = GetUpwardFacingSide();
		if (topFace < 0)
		{
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
				throw new Exception("outcome must be success, failure, or crit");
		}
	}
	

	//returns number of upwards facing side, returns -1 if not found
	private int GetUpwardFacingSide()
	{
		foreach (RayCast3D ray in faceNormals.GetChildren())
		{
			if (ray.IsColliding() && ray.GetCollider() is Node collider && collider.IsInGroup("floor"))
			{
				return Math.Abs(ray.GetIndex() - 9);
			}
		}
		return -1;
	}
}
