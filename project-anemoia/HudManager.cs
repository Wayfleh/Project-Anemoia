using Godot;
using System;
using System.Diagnostics;

public partial class HudManager : Node
{
	public override void _Ready()
	{
		SignalBus.Instance.PlayerHealthChanged += OnPlayerHealthChanged;
	}
	private void OnPlayerHealthChanged(int health)
	{
		// Note: Godot passes ints as longs across the C# signal boundary
		// Value = (double)currentHealth / maxHealth * 100.0;
		Debug.Print("new health" + health);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
