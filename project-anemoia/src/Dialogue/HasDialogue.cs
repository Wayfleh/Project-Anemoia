using Godot;
using Godot.Collections;
using System;
using DialogueManagerRuntime;
using System.Diagnostics;

public partial class HasDialogue : Interactable
{
	[Export] 
	public Resource dialogue;

    public override void Focus(Interactor interactor)
	{
		GD.Print("Ready to play dialogue");
	}

    public override void Interact(Interactor interactor)
	{
		DialogueController.Instance.StartDialogue(dialogue);
	}

    public override void Unfocus(Interactor interactor)
	{
		GD.Print("UNFOCUSED");
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
