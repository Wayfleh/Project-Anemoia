using Godot;
using System;
using Godot.Collections;

public abstract partial class State : Node
{
    protected StateMachine machine;
    protected CharacterBody3D parent;
    [Signal] public delegate void TransitionToStateEventHandler(string state);

    public override async void _Ready()
    {
        machine = (StateMachine)GetParent();
    }

    public abstract void EnterState();
    public abstract void Update(double delta);
    public abstract void ExitState();


}