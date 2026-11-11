/*
	DialogueController.cs
	Author: Rafael Caculba
	Description: This script holds dialogue flags, controls dialogue game state, and 
	routes various dialogue actions from Dialogue Manager to the in-game dialogue.
	Ideally, the in-game dialogues don't have to touch Dialogue Manager, only requesting
	info from this singleton.

	Last updated: 10/16/2026
*/
using Godot;
using System;
using System.Collections;
using DialogueManagerRuntime;

public partial class DialogueController : Node
{
	public static DialogueController Instance { get; private set;}
	private BitArray _dialogueFlags = new BitArray(200); //arbitrary size, increase as needed
	[Signal] public delegate void DialogueStartedEventHandler(Resource dialogueResource, String start);

	//aside from test flags, don't change these flags once they're comitted, otherwise everything gets FUCKED
	//to use these flags, replace parameter flagIndex with DialogueController.Flags.[flag name]
	public static class Flags
	{
		public const int test1 = 1;
		public const int test2 = 2;
	}

	public void StartDialogue(Resource dialogueResource)
	{
		var error = EmitSignal(SignalName.DialogueStarted, dialogueResource, "~ start");
		if (error is Error.Unavailable)
		{
			GD.Print("mm you fucked up");
		}
	}

	public void SetFlag(int flagIndex, bool value)
	{
		if (flagIndex >= 0 && flagIndex < _dialogueFlags.Length)
		{
			_dialogueFlags.Set(flagIndex, value);
		}
	}

	public bool CheckFlag(int flagIndex)
	{
		return flagIndex >= 0 && flagIndex < _dialogueFlags.Length && _dialogueFlags[flagIndex];
	}
	public override void _Ready()
	{
		Instance = this;
	}

}
