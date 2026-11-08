using Godot;
using System;

public partial class DialogueBox : HBoxContainer
{
	private PhysicalDialogueBox extBox;

	private PhysicalDialogueBox intBox;
	// Choices stuff
	private ScrollContainer choicesScrollContainer;
	private ScrollBar choicesScrollBar;
	private VBoxContainer choices;
	public override void _Ready()
	{
		extBox = GetNode<PhysicalDialogueBox>("ExternalDialogueBox");
		intBox = GetNode<PhysicalDialogueBox>("InternalDialogueBox");

		choicesScrollContainer = GetNode<ScrollContainer>("%ChoicesScrollBar");
		choicesScrollBar = choicesScrollContainer.GetVScrollBar();
		choices = choicesScrollContainer.GetNode<VBoxContainer>("Choices");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
