using Godot;
using System;
namespace Godot.Collections;

public partial class Interactor : Area3D
{
    private PlayerController player;
    private Interactable cachedClosest;
    public override void _Ready()
    {
        player = (PlayerController)Owner; // Only the player needs an interactor
        AreaExited += (Area3D area) => // I love these lambda things!
        {
            if (cachedClosest == area && area is Interactable interactable)
                interactable.Unfocus(this);
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Interactable newClosest = getClosestInteractable();
        if (newClosest != cachedClosest)
        {
            if (cachedClosest is GodotObject godotObj && IsInstanceValid(godotObj)) // if old closest hasn't been deleted
            {
                cachedClosest.Unfocus(this);
            }
            if (newClosest != null) // new closest can't possibly be deleted probably maybe
            {
                newClosest.Focus(this);
            }
            cachedClosest = newClosest;
        }

    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            if (cachedClosest != null)
            {
                cachedClosest.Interact(this);
            }
        }
    }



    // Returns closest overlapping Interactable or null if there isn't one
    public Interactable getClosestInteractable()
    {
        Array<Area3D> list = this.GetOverlappingAreas();
        float distance;
        float closestDistance = Mathf.Inf;
        Interactable closest = null;

        foreach (Area3D area in list)
        {
            distance = area.GlobalPosition.DistanceTo(this.GlobalPosition);

            // Sets the first interactable in the list as closest
            if (distance < closestDistance && area is Interactable interactable)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }
        return closest;
    }
}
