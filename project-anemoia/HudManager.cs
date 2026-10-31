using Godot;
using System;
using System.Diagnostics;

public partial class HudManager : Node
{
	[Export] private TextureProgressBar healthBar;
	[Export] private TextureProgressBar bloodBar;
	[Export] private TextureRect[] willPowerBits;
	[Export] private Texture2D filledBit, unfilledBit;
	[Export] private Texture2D filledBigBit, unfilledBigBit;

	public override void _Ready()
	{
		PlayerStatsWrapper.Instance.PlayerHealthChanged += OnPlayerHealthChanged;
		PlayerStatsWrapper.Instance.PlayerBloodChanged += OnPlayerBloodChanged;
		PlayerStatsWrapper.Instance.PlayerWillpowerChanged += OnPlayerWillpowerChanged;
	}
	private void OnPlayerHealthChanged(int newHealth) => healthBar.Value = newHealth;
	private void OnPlayerBloodChanged(int newBlood) => bloodBar.Value = newBlood;
	private void OnPlayerWillpowerChanged(int newWillpower)
	{
		for (int i = 0; i < willPowerBits.Length; i++)
		{
			if (i >= willPowerBits.Length) break;

			if (i < willPowerBits.Length - 1)
				willPowerBits[i].Texture = i < newWillpower ? filledBit : unfilledBit;
			else
				willPowerBits[i].Texture = i < newWillpower ? filledBigBit : unfilledBigBit;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Health -= (int)Math.Ceiling(20 * delta);
			PlayerStatsWrapper.Instance.playerStats.Blood -= (int)Math.Ceiling(20 * delta);
		}
		if (Input.IsActionJustPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Willpower -= 1;
			Debug.Print("Willpower: " + PlayerStatsWrapper.Instance.playerStats.Willpower);
		}
	}
}
