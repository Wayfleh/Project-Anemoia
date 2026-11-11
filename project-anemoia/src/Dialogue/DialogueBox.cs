using Godot;
using System;
using DialogueManagerRuntime;
using System.Threading.Tasks;

public partial class DialogueBox : HBoxContainer
{
	[Export]
	public int MAX_NUMBER_OF_TEXTBOXES;
	[Export]
	public int intHistoryChoiceSeparation = 700;
	private static readonly PackedScene ChoiceButtonScene = GD.Load<PackedScene>("res://src/Dialogue/ChoiceButton.tscn");
	private static readonly PackedScene TextBoxScene = GD.Load<PackedScene>("res://src/Dialogue/TextBox.tscn");
	private PhysicalDialogueBox extBox;

	private PhysicalDialogueBox intBox;
	// Choices stuff
	private ScrollContainer choicesScrollContainer;
	private ScrollBar choicesScrollBar;
	private VBoxContainer choices;

	private DialogueLine currentLine;
	private bool textIsInternal = true;
	private bool expandedBoxes = false;

	public override void _Ready()
	{
		base._Ready();
		extBox = GetNode<PhysicalDialogueBox>("ExternalDialogueBox");
		intBox = GetNode<PhysicalDialogueBox>("InternalDialogueBox");

		choicesScrollContainer = GetNode<ScrollContainer>("%ChoiceScroll");
		choicesScrollBar = choicesScrollContainer.GetVScrollBar();
		choices = choicesScrollContainer.GetNode<VBoxContainer>("Choices");
		DialogueController.Instance.DialogueStarted += QueryDialogue;

		extBox.scrollBar.ValueChanged += SyncIntWithExt;
		intBox.scrollBar.ValueChanged += SyncExtWithInt;
	}

	private async void QueryDialogue(Resource dialogue, String nextCue)
	{
		var line = await DialogueManager.GetNextDialogueLine(dialogue, nextCue);
		ProgressDialogue(dialogue, line);
	}

	
	private void ProgressDialogue(Resource dialogue, DialogueLine line)
	{
		var textBox = TextBoxScene.Instantiate<TextBox>();
		textBox.SetDialogue(line.Character, line.Text);
		var area = line.GetTagValue("area"); //it's either "internal" or "external"
		if (area != "")
			textIsInternal = (area == "external") ? false : true; //I'll just default it to true idc

		if (textIsInternal) // putting textbox in internal box
		{
			intBox.history.AddChild(textBox);
			var spacer = new PanelContainer();
			spacer.CustomMinimumSize = new Vector2(textBox.CustomMinimumSize.X, textBox.CustomMinimumSize.Y);
			spacer.MouseFilter = MouseFilterEnum.Pass;
			extBox.history.AddChild(spacer);
			// these numbers are kinda arbitrary
			if (intBox.history.Size.Y >= intHistoryChoiceSeparation - 150 || !expandedBoxes) // making the external box larger to allow for scrolling
			{
				expandedBoxes = true;
				intBox.history.CustomMinimumSize = new Vector2(0, intBox.history.Size.Y + 200);
				extBox.history.CustomMinimumSize = new Vector2(0, extBox.history.Size.Y + GetViewport().GetVisibleRect().Size.Y / 2);
			}
		}
		else // putting textbox in external box
		{
			if (extBox.Visible == false)
				extBox.Show();
			extBox.history.AddChild(textBox);
			var spacer = new PanelContainer();
			spacer.CustomMinimumSize = new Vector2(textBox.CustomMinimumSize.X, textBox.CustomMinimumSize.Y);
			spacer.MouseFilter = MouseFilterEnum.Pass;
			intBox.history.AddChild(spacer);
		}
		extBox.history.CustomMinimumSize = new Vector2(0, extBox.history.CustomMinimumSize.Y + textBox.CustomMinimumSize.Y);
		ScrollHistoryToBottom();
		
		foreach (DialogueResponse response in line.Responses) // create a choice button for each response
		{
			if (!response.IsAllowed) // if response failed its condition check, don't show it
				continue;
			var choice = ChoiceButtonScene.Instantiate<Button>();
			choice.Text = response.Text;
			choice.Pressed += () => ChoiceSelected(dialogue, response.Text, response.NextId);
			choices.AddChild(choice);
			
		}

		if (choices.GetChildren().Count == 0) // if there are no responses or you don't meet any of the requirements
		{
			var emptyResponse = ChoiceButtonScene.Instantiate<Button>();
			emptyResponse.Text = "CONTINUE ------>";
			emptyResponse.Pressed += () => ChoiceSelected(dialogue, emptyResponse.Text, line.NextId);
			choices.AddChild(emptyResponse);
		}
	}

	private async void ScrollHistoryToBottom()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		intBox.scrollContainer.ScrollVertical = (int)intBox.scrollBar.MaxValue;
		extBox.scrollContainer.ScrollVertical = (int)intBox.scrollBar.MaxValue;
	}
	

	private void EndDialogue()
	{
		
	}
	private void ChoiceSelected(Resource dialogue, String message, String nextId)
	{
		if (message != "CONTINUE ------>")
		{
			// save choice in internal textbox
			var textBox = TextBoxScene.Instantiate<TextBox>();
			textBox.SetDialogue("You", message);
			intBox.history.AddChild(textBox);

			// spacer in external textbox
			var spacer = new PanelContainer();
			spacer.CustomMinimumSize = new Vector2(textBox.CustomMinimumSize.X, textBox.CustomMinimumSize.Y);
			spacer.MouseFilter = MouseFilterEnum.Pass;
			extBox.history.AddChild(spacer);
		}

		QueryDialogue(dialogue, nextId);
		foreach (Button choice in choices.GetChildren()) // empty choices
		{
			choices.RemoveChild(choice);
			choice.QueueFree();
		}
	}

	// moves external box when internal box is scrolled (with mouse)
	private void SyncIntWithExt(double value)
	{
		if (!Mathf.IsEqualApprox((float)intBox.scrollBar.Value, (float)value))
		{
			if (value >= intBox.scrollBar.MaxValue - intBox.scrollBar.Page) //clamp scroll bar
			{
				extBox.scrollContainer.ScrollVertical = (int) (intBox.scrollBar.MaxValue - intBox.scrollBar.Page);
				return;
			}
			intBox.scrollBar.Value = value;
		}
	}
	// moves internal box when external box is scrolled (with mouse)
	private void SyncExtWithInt(double value)
	{
		if (!Mathf.IsEqualApprox((float)extBox.scrollBar.Value, (float)value))
		{
			if (value >= extBox.scrollBar.MaxValue - extBox.scrollBar.Page) //clamp scroll bar
			{
				intBox.scrollContainer.ScrollVertical = (int) (extBox.scrollBar.MaxValue - extBox.scrollBar.Page);
				return;
			}
			extBox.scrollBar.Value = value;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		DialogueController.Instance.DialogueStarted -= QueryDialogue;
		extBox.scrollBar.ValueChanged -= SyncIntWithExt;
		intBox.scrollBar.ValueChanged -= SyncExtWithInt;
    }

}
