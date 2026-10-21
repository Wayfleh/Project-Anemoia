using Godot;
using System;
namespace Godot.Collections;

public partial class Door : Interactable
{
    private Boolean doorClosed = true;

    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("../../AnimationPlayer");
    }


    public override void Interact(Interactor interactor)
    {
        if (animPlayer == null)
            GD.Print("Animation Player not found");
        float reverseAnimation = doorClosed ? 1 : -1; 
        //Opens door, starts reversing from current pos if door is already opening, reverses from end if door is already opened
        animPlayer.Play("Open", customSpeed: reverseAnimation, fromEnd: !doorClosed && !animPlayer.IsPlaying());
        doorClosed = !doorClosed;
    }
    public override void Focus(Interactor interactor)
    {
    }
    public override void Unfocus(Interactor interactor)
    {
        
    }
}
