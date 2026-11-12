using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class PlayerStatsWrapper : Node
{
	public static PlayerStatsWrapper Instance;

	[Signal] public delegate void PlayerHealthChangedEventHandler(int health);
	[Signal] public delegate void PlayerBloodChangedEventHandler(int blood);
	[Signal] public delegate void PlayerWillpowerChangedEventHandler(int willpower);

	public PlayerStats playerStats;
	private string psInstancePath = "res://assets/player_test_stats.tres";
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance != null)
			Debug.Print("multiple instances of Player Stats Wrapper yo");
		Instance = this;

		playerStats = GD.Load<PlayerStats>(psInstancePath);
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	int b = 0;
	public void Test()
	{
		playerStats.PrintDebug();
		// if (b >= playerStats.skills_list.Count) return;

		// PlayerStats.Skill[] skills_names = Enum.GetValues<PlayerStats.Skill>();

		// int val = playerStats.ReadSkill(skills_names[b]);
		// if (val == 10)
		// 	b++;
		// else
		// {
		// 	playerStats.IncrementSkill(skills_names[b]);
		// 	GD.Print(skills_names[b].ToString() + ": " + playerStats.ReadSkill(skills_names[b]));
		// }

		// playerStats.IncrementSkill(skills_names[0]);
		// GD.Print(skills_names[0].ToString() + ": " + playerStats.ReadSkill(skills_names[0]));


	}
}
