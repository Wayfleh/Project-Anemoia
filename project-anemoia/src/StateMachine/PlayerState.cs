using Godot;
using System;
using Godot.Collections;

// PlayerState.cs
// These states control player behavior during free roam, dialogue, combat, and probably other things
[GlobalClass]
public partial class PlayerState : State
{
    protected PlayerController player;
    public override void _Ready()
    {
        base._Ready();
        player = machine.parent as PlayerController;
    }
    public override void EnterState()
    {
        
    }
    public override void Update(double delta)
    {
        
    }
    public override void ExitState()
    {
        
    }

}