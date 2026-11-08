using Godot;
using System;

public partial class DialogueBox : HBoxContainer
{
	private PhysicalDialogueBox extBox;

	private PhysicalDialogueBox intBox;
	public override void _Ready()
	{
		extBox = (PhysicalDialogueBox) GetNode("ExternalDialogueBox");
		intBox = (PhysicalDialogueBox) GetNode("InternalDialogueBox");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
