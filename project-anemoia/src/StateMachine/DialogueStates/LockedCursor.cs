using Godot;
using System;
using Godot.Collections;

public partial class LockedCursor : PlayerState
{
    public override void EnterState()
    {
		Input.SetMouseMode(Input.MouseModeEnum.Captured);
    }

    public override void Update(double delta)
    {
        player.HandleMovement(delta);
        if (Input.IsActionJustPressed("free_cursor"))
            EmitSignal(SignalName.TransitionToState, "FreeCursor");
    }

    public override void ExitState()
    {
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (machine.currentState != this)
            return;
        player.HandleCamera(@event);
    }
}