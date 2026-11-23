using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class DiceRollingScene : Node
{
	[Signal] public delegate void DiceSceneFinishedEventHandler(int successes);

	[Export] public PackedScene dicePackedScene;
	[Export] public Node3D spawnPosition;
	[Export] public float impulseMultiplier;
	
	List<DiceObject> diceObjects;
	private int[] resultArray;
	int numOfSpawnedDice;

	public async void PlayScene(int dice)
	{
		DeleteExistingDice();
		numOfSpawnedDice = dice;
		resultArray = CalculateResults();
		for (int i = 0; i < dice; i++)
		{
			SpawnDice();
			await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		}
	}
	public override void _Ready()
	{
		diceObjects = new List<DiceObject>();
		//PlayScene(3);
	}
	public override void _Process(double delta)
	{
		
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

	private int[] CalculateResults()
	{
		int[] results = new int[numOfSpawnedDice];
		for (int i = 0; i < numOfSpawnedDice; i++)
		{
			results[i] = Random.Shared.Next(10);
		}
		return results;
	}

	private void SpawnDice()
	{
		DiceObject diceInst = dicePackedScene.Instantiate<DiceObject>();


		AddChild(diceInst);
		diceObjects.Add(diceInst);
		diceInst.DiceFinished += DiceFinished;
		float spawnX = GD.RandRange(-20, 20);
		float spawnZ = GD.RandRange(-20, 20);
		Vector3 randSpawn = new Vector3(spawnX, 0, spawnZ);
		diceInst.GlobalPosition = spawnPosition.GlobalPosition + randSpawn;
		ThrowDice(diceInst);
	}

	private void ThrowDice(DiceObject die)
	{
		float x = (float)GD.RandRange(-50.0, 50.0);
		float z = (float)GD.RandRange(-50.0, 50.0);
		Vector3 targetPos = new Vector3(x, 10, z);
		Vector3 impulseDirection = targetPos - die.GlobalPosition;
		die.ApplyImpulse(impulseDirection * impulseMultiplier);
	}

	private void DiceFinished()
	{
		int successes = 0;
		for (int i = 0; i < diceObjects.Count; i++)
		{ //Make sure all dice are done first
			if (!diceObjects[i].MarkedFinished)
				return;
			else if (diceObjects[i].IsOnTopOfDie())
			{
				ThrowDice(diceObjects[i]);
				return;
			}
		}
		for (int i = 0; i < diceObjects.Count; i++)
		{
			switch (resultArray[i])
			{
				case 0:
					diceObjects[i].ShowMesh("crit");
					successes += 2;
					break;
				case < 6:
					diceObjects[i].ShowMesh("failure");
					break;
				case >= 6:
					diceObjects[i].ShowMesh("success");
					successes += 1;
					break;
				default:
					throw new Exception("dice got fucked up somehow");
			}
		}
		string diceVals = "";
		for (int i = 0; i < resultArray.Length; i++)
		{
			diceVals += resultArray[i] + " ";
		}
		Debug.Print("dice scene finished. result: " + diceVals);

		EmitSignal(SignalName.DiceSceneFinished, successes);
	}
}
