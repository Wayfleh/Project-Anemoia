using Godot;
using System;

[GlobalClass]
public partial class PhysicalDialogueBox : PanelContainer
{
	[Export]
	public ScrollContainer scrollContainer;
	public VBoxContainer history;
	public ScrollBar scrollBar;
	public override void _Ready()
	{
		foreach (Node child in scrollContainer.GetChildren())
		{
			GD.Print("Found child name: " + child.Name);
		}
		history = scrollContainer.GetNode<VBoxContainer>("History");
		scrollBar = scrollContainer.GetVScrollBar();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
