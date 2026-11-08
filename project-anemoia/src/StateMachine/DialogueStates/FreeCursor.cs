using Godot;
using System;
using Godot.Collections;

public partial class FreeCursor : PlayerState
{
    public override void EnterState()
    {
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }

    public override void Update(double delta)
    {
        player.HandleMovement(delta);
        if (Input.IsActionJustReleased("free_cursor"))
            EmitSignal(SignalName.TransitionToState, "LockedCursor");
    }

    public override void ExitState()
    {
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
    }
}