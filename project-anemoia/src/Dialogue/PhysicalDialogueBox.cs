using Godot;
using System;

public partial class PhysicalDialogueBox : PanelContainer
{
	[Export]
	public ScrollContainer scrollContainer;
	public VBoxContainer history;
	public ScrollBar scrollBar;
	public override void _Ready()
	{
		history = (VBoxContainer)scrollContainer.GetNode("History");
		scrollBar = scrollContainer.GetVScrollBar();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
