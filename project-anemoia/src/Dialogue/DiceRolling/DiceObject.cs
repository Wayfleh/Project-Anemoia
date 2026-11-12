using Godot;
using System;
using System.Diagnostics;
using System.Dynamic;

public partial class DiceObject : RigidBody3D
{
	[Signal] public delegate void DiceFinishedEventHandler();

	[Export] private MeshInstance3D side_1_meshInst;
	[Export] private MeshInstance3D side_2_meshInst;
	[Export] private MeshInstance3D side_3_meshInst;
	[Export] private MeshInstance3D side_4_meshInst;
	[Export] private MeshInstance3D side_5_meshInst;
	[Export] private MeshInstance3D side_6_meshInst;
	[Export] private MeshInstance3D diceBody;
	[Export] private float jitterMagnitude = 7;
	[Export] private Color glowingColor;

	MeshInstance3D upwardFacingMeshInst3D = null;

	private bool startWithRandomSide = true;

	public bool MarkedFinished { get; private set; } = false;//used to check if dice has stopped
	public int ValueResult { get; private set; }

	public override void _Ready()
	{
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

		diceBody.RotateZ(-jitterZ);
		diceBody.RotateY(-jitterY);
		diceBody.RotateX(-jitterX);

		jitterX = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		jitterY = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		jitterZ = (float)GD.RandRange(0, jitterMagnitude) * deltaF;
		diceBody.RotateX(jitterX);
		diceBody.RotateY(jitterY);
		diceBody.RotateZ(jitterZ);
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
		CubeFace facingUp = GetUpwardFacingSide();
		MarkedFinished = true;

		UpdateValues(facingUp);

		HighlightSide(upwardFacingMeshInst3D);
		EmitSignal(SignalName.DiceFinished);
	}

	private void UpdateValues(CubeFace cubeFace)
	{
		switch (cubeFace)
		{
			case CubeFace.Forward:
				upwardFacingMeshInst3D = side_6_meshInst;
				ValueResult = 6;
				break;
			case CubeFace.Back:
				upwardFacingMeshInst3D = side_1_meshInst;
				ValueResult = 1;
				break;
			case CubeFace.Left:
				upwardFacingMeshInst3D = side_2_meshInst;
				ValueResult = 2;
				break;
			case CubeFace.Right:
				upwardFacingMeshInst3D = side_5_meshInst;
				ValueResult = 5;
				break;
			case CubeFace.Top:
				upwardFacingMeshInst3D = side_4_meshInst;
				ValueResult = 4;
				break;
			case CubeFace.Bottom:
				upwardFacingMeshInst3D = side_3_meshInst;
				ValueResult = 3;
				break;
			default:
				Debug.Print("unknown face");
				break;
		}
	}

	//made w ai. works. can be cleaned up ig
	public enum CubeFace { Forward, Back, Right, Left, Top, Bottom, Unknown }

	private CubeFace GetUpwardFacingSide()
	{
		// Convert world UP (0, 1, 0) into the die's local coordinate space
		Vector3 localUp = GlobalTransform.Basis.Inverse() * Vector3.Up;

		// Find which local axis aligns most with localUp
		float maxDot = localUp.X;
		CubeFace bestFace = localUp.X > 0 ? CubeFace.Right : CubeFace.Left;

		if (Mathf.Abs(localUp.Y) > Mathf.Abs(maxDot))
		{
			maxDot = localUp.Y;
			bestFace = localUp.Y > 0 ? CubeFace.Top : CubeFace.Bottom;
		}

		if (Mathf.Abs(localUp.Z) > Mathf.Abs(maxDot))
		{
			bestFace = localUp.Z > 0 ? CubeFace.Back : CubeFace.Forward;
		}

		return bestFace;
	}
}
