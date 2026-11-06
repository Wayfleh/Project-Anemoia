using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public abstract partial class State : Node
{
    private StateMachine machine;
    private CharacterBody2D parent;
    [Signal] public delegate void TransitionToStateEventHandler(string state);

    public override async void _Ready()
    {
        machine = (StateMachine)GetParent();
        await ToSignal(machine, StateMachine.SignalName.Ready);
        parent = machine.parent;
    }

    public abstract void EnterState();
    public abstract void Update();
    public abstract void ExitState();


}