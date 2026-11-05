using Godot;
using System;

public partial class player_stats_test_script : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("jump"))
		{
			PlayerStatsWrapper.Instance.playerStats.Health -= 5;
			PlayerStatsWrapper.Instance.playerStats.Blood -= 5;
			PlayerStatsWrapper.Instance.playerStats.Willpower -= 5;
			PlayerStatsWrapper.Instance.Test();
		}
	}
}
