using Godot;
using System;
using System.Diagnostics;

public partial class SignalBus : Node
{
	public static SignalBus Instance { get; private set; }
	//[Export] public HudManager HudManager;
	[Signal]
    public delegate void PlayerHealthChangedEventHandler(int health);


	public override void _Ready()
	{
		if (Instance != null)
			Debug.Print("multiple instances of signal bus yo");
		Instance = this;
	}

}
