using Godot;
using System;
using DialogueManagerRuntime;
using System.Threading.Tasks;

public partial class DialogueBox : HBoxContainer
{
	private static readonly PackedScene ChoiceButtonScene = GD.Load<PackedScene>("res://src/Dialogue/ChoiceButton.tscn");
	private PhysicalDialogueBox extBox;

	private PhysicalDialogueBox intBox;
	// Choices stuff
	private ScrollContainer choicesScrollContainer;
	private ScrollBar choicesScrollBar;
	private VBoxContainer choices;

	private DialogueLine currentLine;

	public override void _Ready()
	{
		extBox = GetNode<PhysicalDialogueBox>("ExternalDialogueBox");
		intBox = GetNode<PhysicalDialogueBox>("InternalDialogueBox");

		choicesScrollContainer = GetNode<ScrollContainer>("%ChoicesScrollBar");
		choicesScrollBar = choicesScrollContainer.GetVScrollBar();
		choices = choicesScrollContainer.GetNode<VBoxContainer>("Choices");
	}

	private async void QueryDialogue(Resource dialogue, String nextCue)
	{
		var line = await DialogueManager.GetNextDialogueLine(dialogue, nextCue);
		ProgressDialogue(dialogue, line);
	}

	private void ProgressDialogue(Resource dialogue, DialogueLine line)
	{
		foreach (DialogueResponse response in line.Responses)
		{
			if (!response.IsAllowed) //if response failed its condition check, don't show it
				continue;
			var choice = ChoiceButtonScene.Instantiate<ChoiceButton>();
			choice.Text = response.Text;

		}
		if (choices.GetChildren().Count == 0) // if there are no responses or you don't meet any of the requirements
		{
			var emptyResponse = ChoiceButtonScene.Instantiate<ChoiceButton>();
			emptyResponse.Text = "CONTINUE ------>";
			emptyResponse.Pressed += () => ChoiceSelected(dialogue, emptyResponse.response.NextId);
		}
	}

	private void ChoiceSelected(Resource dialogue, String nextCue)
	{
		QueryDialogue(dialogue, nextCue);
		foreach (ChoiceButton choice in choices.GetChildren())
		{
			choice.Pressed -= () => ChoiceSelected(dialogue, nextCue);
			choice.QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
