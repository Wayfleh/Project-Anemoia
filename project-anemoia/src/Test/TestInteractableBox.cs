using Godot;
using Godot.Collections;
using System;

public partial class TestInteractableBox : Interactable
{
    public override void Interact(Interactor interactor)
    {
        GD.Print("INTERACTED");
    }
    public override void Focus(Interactor interactor)
    {
        GD.Print("FOCUSED");
    }
    public override void Unfocus(Interactor interactor)
    {
        GD.Print("UNFOCUSED");
    }
}
