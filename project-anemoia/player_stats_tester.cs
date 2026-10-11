using Godot;
using System;

public partial class player_stats_tester : Node
{
	public static player_stats_tester instance;
	[Export]
	public PlayerStats playerStats;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (instance == null)
			instance = this;
		else
		{
			GD.Print("two player stats auto loader instances");
			return;
		}
		playerStats = new PlayerStats();
	}

	int b = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (b >= playerStats.skills_list.Count) return;

		PlayerStats.Skill[] skills_names = Enum.GetValues<PlayerStats.Skill>();

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
