using Godot;
using System;
namespace Godot.Collections;

public abstract partial class Interactable : Area3D
{
    public abstract void Interact(Interactor interactor);
    public abstract void Focus(Interactor interactor);
    public abstract void Unfocus(Interactor interactor);
}
