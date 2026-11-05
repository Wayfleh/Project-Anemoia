using Godot;
using System;
using System.Diagnostics;

public partial class DiceObject : RigidBody3D
{
	[Signal] public delegate void DiceFinishedEventHandler();
	public bool MarkedFinished { get; private set; } = false;
	public int diceInt = -1;

	public override void _Ready()
	{
		SleepingStateChanged += SleepStateCh;
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
		Debug.Print("Dice " + diceInt + " " + facingUp + "Face facing up");
		EmitSignal(SignalName.DiceFinished);
	}

	public enum CubeFace { Forward, Back, Right, Left, Top, Bottom, Unknown }

	public CubeFace GetUpwardFacingSide()
	{
		Vector3 localRight = GlobalTransform.Basis.X;
		Vector3 localLeft = -localRight;
		Vector3 localTop = GlobalTransform.Basis.Y;
		Vector3 localBottom = -localTop;
		Vector3 localBack = GlobalTransform.Basis.Z;
		Vector3 localFront = -localBack;

		Vector3 worldUp = Vector3.Up; // (0, 1, 0)

		float bestDot = float.NegativeInfinity;
		CubeFace bestFace = CubeFace.Unknown;

		// Check each of the 6 faces
		CheckAxis(localTop, CubeFace.Top, ref bestDot, ref bestFace, worldUp);
		CheckAxis(localBottom, CubeFace.Bottom, ref bestDot, ref bestFace, worldUp);
		CheckAxis(localFront, CubeFace.Forward, ref bestDot, ref bestFace, worldUp);
		CheckAxis(localBack, CubeFace.Back, ref bestDot, ref bestFace, worldUp);
		CheckAxis(localRight, CubeFace.Right, ref bestDot, ref bestFace, worldUp);
		CheckAxis(localLeft, CubeFace.Left, ref bestDot, ref bestFace, worldUp);

		return bestFace;
	}

	private void CheckAxis(Vector3 worldDir, CubeFace face, ref float bestDot, ref CubeFace bestFace, Vector3 worldUp)
	{
		// Dot product yields 1 if perfectly aligned, 0 if perpendicular, -1 if completely upside down
		float dot = worldDir.Normalized().Dot(worldUp);

		if (dot > bestDot)
		{
			bestDot = dot;
			bestFace = face;
		}
	}
}
