using Godot;
using System;
using DialogueManagerRuntime;
using System.Threading.Tasks;

[GlobalClass]
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
	private Resource currentDialogue;
	private bool textIsInternal = true;
	private bool expandedBoxes = false;

	[Export]
	public DiceRollingScene diceRollingScene;
	[Export]
	public SubViewportContainer diceRollSubviewPortContainer;
	[Export]
	public float diceRollLingerTime = 3.0f;
	public struct Check
	{
		public PlayerStats.Attribute attribute;
		public PlayerStats.Skill skill;
		public int degree;
	}

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
		if (nextCue == "~ start")
		{
			StartDialogue();
			currentDialogue = dialogue;
		}
		if (line != null)
			ProgressDialogue(dialogue, line);
		else
		{
			currentDialogue = null;
			EndDialogue();
		}
	}

	
	private void ProgressDialogue(Resource dialogue, DialogueLine line)
	{
		currentLine = line;
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
		// these numbers are kinda arbitrary
		if (intBox.history.Size.Y >= intHistoryChoiceSeparation && !expandedBoxes) // making the external box larger to allow for scrolling
		{
			expandedBoxes = true;
			intBox.history.CustomMinimumSize = new Vector2(0, intBox.history.Size.Y + 200);
			extBox.history.CustomMinimumSize = new Vector2(0, extBox.history.Size.Y + GetViewport().GetVisibleRect().Size.Y);
		}
		extBox.history.CustomMinimumSize = new Vector2(0, extBox.history.CustomMinimumSize.Y + textBox.CustomMinimumSize.Y);
		ScrollHistoryToBottom();

		foreach (DialogueResponse response in line.Responses) // create a choice button for each response
		{
			if (!response.IsAllowed) // if response failed its condition check, don't show it
				continue;
			var diceRoll = line.GetTagValue("check"); //check if choice triggers a dice roll
			var choice = ChoiceButtonScene.Instantiate<Button>();
			choice.Text = response.Text;
			choice.Pressed += () => ChoiceSelected(dialogue, response.Text, response.NextId, diceRoll != "");
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

	private async void ChoiceSelected(Resource dialogue, String message, String nextId, bool diceRoll = false)
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
		foreach (Button choice in choices.GetChildren()) // empty choices
		{
			choices.RemoveChild(choice);
			choice.QueueFree();
		}

		if (diceRoll)
			return;

		QueryDialogue(dialogue, nextId);
	}


	// Called whenever choice is selected to make sure you can see the dialogue
	private async void ScrollHistoryToBottom()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		intBox.scrollContainer.ScrollVertical = (int)intBox.scrollBar.MaxValue;
		extBox.scrollContainer.ScrollVertical = (int)intBox.scrollBar.MaxValue;
	}
	
	private void StartDialogue()
	{

		foreach (Node child in intBox.history.GetChildren())
		{
			intBox.history.RemoveChild(child);
			child.QueueFree();
		}
		foreach (Node child in extBox.history.GetChildren())
		{
			extBox.history.RemoveChild(child);
			child.QueueFree();
		}
	}
	private void EndDialogue()
	{
		int j = 0;
		for (int i = 0; i < intBox.history.GetChildCount(); i++) // put every textbox from external box into internal dialogue box, replace PanelContainers
		{ //every time you see a panel, remove the top of the extbox
			var child = intBox.history.GetChild(i);
			if (child is TextBox)
				continue;
			GD.Print("FUCK");
			var extChild = extBox.history.GetChild(i-j);
			 // extbox index; GetChild() goes back however many nodes were removed
			GD.Print(i);
			GD.Print(extChild);
			extBox.history.RemoveChild(extChild);
			intBox.history.AddChild(extChild);
			intBox.history.MoveChild(extChild, i);
			intBox.history.RemoveChild(child);
			child.QueueFree();
			j++;
		}
		foreach (Node child in extBox.history.GetChildren())
		{
			child.QueueFree();
		}
		extBox.history.CustomMinimumSize = Vector2.Zero;
		intBox.history.CustomMinimumSize = Vector2.Zero;
	}

	// Called in dialogue resource
	public async Task<bool> PerformCheck(string check, bool isAutomatic = false)
	{
		if (ParseCheck(check) is not Check queriedCheck)
		{
			GD.Print("check could not be completed");
			return false;
		}

		int dicePool = PlayerStatsWrapper.Instance.playerStats.attributes_list[queriedCheck.attribute] 
					+ PlayerStatsWrapper.Instance.playerStats.skills_list[queriedCheck.skill];

		if (isAutomatic)
		{
			return dicePool >= queriedCheck.degree;
		}
		else
		{
			GD.Print(PlayerStatsWrapper.Instance.playerStats.attributes_list[queriedCheck.attribute] );
			GD.Print(PlayerStatsWrapper.Instance.playerStats.skills_list[queriedCheck.skill]);
			var result = await DiceRoll(dicePool);
			return result >= queriedCheck.degree;
		}
	}

	// checks have the format "attribute_skill_degree"
	// helper class that parses check string (probably from dialogue manager) into a Check
	// returns null if any elements of the string are invalid
	private Check? ParseCheck(string check)
	{
		string[] elements = check.Split('_');
		Check returnedCheck = new Check();
		if (Enum.TryParse(elements[0], out PlayerStats.Attribute attribute))
			returnedCheck.attribute = attribute;
		else
		{
			GD.Print("ERROR: parsed attribute ({elements[0]}) not a valid member in PlayerStats.Attribute");
			return null;
		}
		
		if (Enum.TryParse(elements[1], out PlayerStats.Skill skill))
			returnedCheck.skill = skill;
		else
		{
			GD.Print("ERROR: parsed skill ({elements[1]}) not a valid member in PlayerStats.Skill");
			return null;
		}

		if (int.TryParse(elements[2], out int degree) && degree > 0)
			returnedCheck.degree = degree;
		else
		{
			GD.Print("ERROR: parsed degree ({elements[2]}) not a valid integer (must be positive nonzero)");
			return null;
		}
		
		return returnedCheck;
		
	}

	private async Task<int> DiceRoll(int dicePool)
	{
		diceRollSubviewPortContainer.Show();
		diceRollingScene.PlayScene(dicePool);
		int diceResult = 0;
		diceRollingScene.DiceSceneFinished += (int successes) => diceResult = successes;
		await ToSignal(diceRollingScene, DiceRollingScene.SignalName.DiceSceneFinished);
		
		//create a continue button when dice roll scene is done, previous query was skipped so this should work fine
		var emptyResponse = ChoiceButtonScene.Instantiate<Button>();
		emptyResponse.Text = "CONTINUE ------>";
		emptyResponse.Pressed += () => ChoiceSelected(currentDialogue, emptyResponse.Text, currentLine.NextId);
		emptyResponse.Pressed += () => DiceRollHide(emptyResponse);
		choices.AddChild(emptyResponse);

		return diceResult;
	}


	private async void DiceRollHide(Button choice)
	{
		diceRollSubviewPortContainer.Hide();
		choice.Pressed -= () => DiceRollHide(choice);
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
