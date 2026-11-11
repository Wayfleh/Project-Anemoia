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
	public struct Check
	{
		public PlayerStats.Attribute attribute;
		public PlayerStats.Skill skill;
		public int degree;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance != null)
			Debug.Print("multiple instances of Player Stats Wrapper yo");
		Instance = this;

		playerStats = new PlayerStats();
	}

	public bool PerformCheck(string check, bool isAutomatic)
	{
		if (ParseCheck(check) is not Check queriedCheck)
		{
			GD.Print("check could not be completed");
			return false;
		}

		int dicePool = Instance.playerStats.attributes_list[queriedCheck.attribute] 
					+ Instance.playerStats.skills_list[queriedCheck.skill];

		if (isAutomatic)
		{
			return dicePool > queriedCheck.degree;
		}
		else
			return DiceRoll(dicePool) > queriedCheck.degree;
	}

	//change this to use Ulugbek's dice rolling scene
	public int DiceRoll(int dicePool)
	{
		GD.Print("DICE ROLLING NOT IMPLEMENTED YET");
		return dicePool;
	}
	// checks have the format "attribute_skill_degree"
	// helper class that parses check string (probably from dialogue manager) into a Check
	// returns null if any elements of the string are invalid
	public Check? ParseCheck(string check)
	{
		string[] elements = check.Split('_');
		Check returnedCheck = new Check();
		if (Enum.TryParse(elements[0], out PlayerStats.Attribute attribute))
			returnedCheck.attribute = attribute;
		else
		{
			GD.Print("ERROR: parsed attribute ({elements[0]}) not a valid member in PlayerStats.Attribute");
			return null;
		}
		
		if (Enum.TryParse(elements[1], out PlayerStats.Skill skill))
			returnedCheck.skill = skill;
		else
		{
			GD.Print("ERROR: parsed skill ({elements[1]}) not a valid member in PlayerStats.Skill");
			return null;
		}

		if (int.TryParse(elements[3], out int degree) && degree > 0)
			returnedCheck.degree = degree;
		else
		{
			GD.Print("ERROR: parsed degree ({elements[2]}) not a valid integer (must be positive nonzero)");
			return null;
		}
		
		return returnedCheck;
		
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
