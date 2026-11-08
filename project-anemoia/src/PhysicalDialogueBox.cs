using Godot;
using System;

public partial class PhysicalDialogueBox : PanelContainer
{
	public ScrollContainer scrollBar;
	public VBoxContainer history;
	public override void _Ready()
	{
		scrollBar = (ScrollContainer)GetNode("HistoryScroll");
		history = (VBoxContainer)GetNode("History");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
