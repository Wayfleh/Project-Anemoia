using Godot;
using System;
using System.Diagnostics;

public partial class DiceObject : RigidBody3D
{
	[Signal] public delegate void DiceFinishedEventHandler();

	[Export] private MeshInstance3D side_1_meshInst;
	[Export] private MeshInstance3D side_2_meshInst;
	[Export] private MeshInstance3D side_3_meshInst;
	[Export] private MeshInstance3D side_4_meshInst;
	[Export] private MeshInstance3D side_5_meshInst;
	[Export] private MeshInstance3D side_6_meshInst;
	[Export] private MeshInstance3D ARTGHWUEHFHJAK;
	[Export] private Color highlightedColor;

	private bool startWithRandomSide = true;

	public bool MarkedFinished { get; private set; } = false;
	public int diceInt = -1;

	public override void _Ready()
	{
		SleepingStateChanged += SleepStateCh;
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
	float x, y, z;
	float ra = 5;
	public override void _Process(double delta)
	{
		if (!MarkedFinished)
			return;
		ARTGHWUEHFHJAK.RotateZ(-z);
		ARTGHWUEHFHJAK.RotateY(-y);
		ARTGHWUEHFHJAK.RotateX(-x);
		x = (float)(GD.RandRange(0, ra) * delta);
		y = (float)(GD.RandRange(0, ra) * delta);
		z = (float)(GD.RandRange(0, ra) * delta);
		ARTGHWUEHFHJAK.RotateX(x);
		ARTGHWUEHFHJAK.RotateY(y);
		ARTGHWUEHFHJAK.RotateZ(z);
	}

	private void HighlightSide(CubeFace cubeFace)
	{
		MeshInstance3D meshInstance3D = null;
		switch (cubeFace)
		{
			case CubeFace.Forward:
				meshInstance3D = side_6_meshInst;
				break;
			case CubeFace.Back:
				meshInstance3D = side_1_meshInst;
				break;
			case CubeFace.Left:
				meshInstance3D = side_2_meshInst;
				break;
			case CubeFace.Right:
				meshInstance3D = side_5_meshInst;
				break;
			case CubeFace.Top:
				meshInstance3D = side_4_meshInst;
				break;
			case CubeFace.Bottom:
				meshInstance3D = side_3_meshInst;
				break;
			default:
				Debug.Print("unknown face to highlgigjt");
				break;
		}

		StandardMaterial3D highlightedVersionMaterial = (StandardMaterial3D)meshInstance3D.Mesh.SurfaceGetMaterial(0).Duplicate();
		highlightedVersionMaterial.AlbedoColor = highlightedColor;
		meshInstance3D.MaterialOverride = highlightedVersionMaterial;
	}
	private void SleepStateCh()
	{
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
			MarkFinished();
		}
	}
	private void MarkFinished()
	{
		CubeFace facingUp = GetUpwardFacingSide();
		MarkedFinished = true;

		HighlightSide(facingUp);
		EmitSignal(SignalName.DiceFinished);
	}

	//made w ai. works 
	public enum CubeFace { Forward, Back, Right, Left, Top, Bottom, Unknown }

	public CubeFace GetUpwardFacingSide()
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
