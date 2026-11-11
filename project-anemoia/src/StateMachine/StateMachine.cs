using Godot;
using System;
using Godot.Collections;
using System.Threading.Tasks;

[GlobalClass]
public partial class StateMachine : Node
{
    public State currentState;
    [Export] public CharacterBody3D parent;
    public Dictionary<StringName, State> states;
    [Export] private State initialState;

    public override async void _Ready()
    {
        states = new();
        await ToSignal(parent, CharacterBody3D.SignalName.Ready);
        foreach (State state in this.GetChildren())
        {
            states.Add(state.Name, state);
            state.TransitionToState += ChangeState;
        }
        currentState = initialState;
        currentState.EnterState();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentState != null)
        {
            currentState.Update(delta);
        }
    }
    public void ChangeState(string next)
    {
        State nextState = states[next];
        currentState.ExitState();
        nextState.EnterState();
        currentState = nextState;

        GD.Print(next);
    }


}