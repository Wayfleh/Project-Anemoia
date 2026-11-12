using Godot;
using System;
using System.Diagnostics;

public partial class RootScene : Node
{
	private SubViewport _subViewport;
	public bool isForwardingInput = true;

	public override void _Ready()
	{
		_subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");

		// Ensure the mouse is captured to lock cursor position
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Forward raw mouse motion directly to the inner 3D viewport
		if (@event is InputEventMouseMotion)
		{
			if (isForwardingInput)
			{
				_subViewport.PushInput(@event);
			}
		}
	}
}
