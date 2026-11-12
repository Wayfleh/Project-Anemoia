using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class DiceRollingScene : Node
{
	[Signal] public delegate void DiceSceneFinishedEventHandler(int[] diceUpwardFaceResults);

	[Export] public PackedScene dicePackedScene;
	[Export] public Node3D spawnPosition;
	[Export] public float impulseMultiplier;
	
	List<DiceObject> diceObjects;
	int numOfSpawnedDice;

	public void PlayScene(int dice)
	{
		DeleteExistingDice();
		numOfSpawnedDice = 0;

		for (int i = 0; i < dice; i++)
		{
			SpawnDice();
		}
	}
	public override void _Ready()
	{
		diceObjects = new List<DiceObject>();
		//PlayScene(3);
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("interact"))
		{
			PlayScene(GD.RandRange(1,3));
		}
	}
	private void DeleteExistingDice()
	{
		for (int i = 0; i < diceObjects.Count; i++)
		{
			diceObjects[i].DiceFinished -= DiceFinished;
			diceObjects[i].QueueFree();
		}
		diceObjects.Clear();
	}

	private void SpawnDice()
	{
		DiceObject diceInst = dicePackedScene.Instantiate<DiceObject>();

		numOfSpawnedDice++;

		AddChild(diceInst);
		diceObjects.Add(diceInst);
		diceInst.DiceFinished += DiceFinished;

		diceInst.GlobalPosition = spawnPosition.GlobalPosition;

		float x = (float)GD.RandRange(-1.0, 1.0);
		float z = (float)GD.RandRange(-1.0, 1.0);
		Vector3 targetPos = new Vector3(x, 2, z);
		Vector3 impulseDirection = targetPos - diceInst.GlobalPosition;
		diceInst.ApplyImpulse(impulseDirection * impulseMultiplier);
	}

	private void DiceFinished()
	{
		int[] result = new int[diceObjects.Count];
		for (int i = 0; i < diceObjects.Count; i++)
		{
			if (!diceObjects[i].MarkedFinished)
				return;
			result[i] = diceObjects[i].ValueResult;
		}
		string diceVals = "";
		for (int i = 0; i < result.Length; i++)
		{
			diceVals += result[i] + " ";
		}
		Debug.Print("dice scene finished. result: " + diceVals);

		EmitSignal(SignalName.DiceSceneFinished, result);
	}
}
