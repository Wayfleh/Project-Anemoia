using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class HudManager : Node
{
	[ExportGroup("Stats Bars")]
	[Export] private TextureProgressBar healthBar;
	[Export] private TextureProgressBar bloodBar;
	[Export] private TextureRect[] willPowerBits;
	[ExportSubgroup("Textures")]
	[Export] private Texture2D filledBit, unfilledBit;
	[Export] private Texture2D filledBigBit, unfilledBigBit;

	[ExportGroup("Dialogue Boxes")]
	// [ExportSubgroup("Toggling External Box")]
	[Export] private Control extDialogueBox;
	[ExportSubgroup("Resizing External Footer")]
	[Export] private Control extDialogueBoxFooter;
	[ExportSubgroup("Dialogue Containers")]
	[Export] private ScrollContainer extScrollContainer, intScrollContainer;
	[Export] private VBoxContainer extVBoxContainer, intVBoxContainer;
	//syncing scrolling
	private ScrollBar extScrollBar, intScrollBar;
	private bool _syncing = false;

	[ExportGroup("Dialogue Items")]//creating dialogue text nodes
	[Export] private PackedScene dialogueItemPackedScene; //same dialogue item for everything for now? NOTE:Margin Container
	private MarginContainer lastExtDialogueItemAdded = null;//leading item
	private MarginContainer lastIntDialogueItemAdded = null;

	private bool lastDialogueWasInInternal;//toggle to either side, 
	private int dialogueMarginLeftToMatchIntExt = 0;


	[ExportGroup("Dialogue Choices")]
	[Export] private ScrollContainer choicesScrollContainer;
	[Export] private VBoxContainer choicesVBoxContainer;
	[Export] private float choicesContainerHeight = 200f;
	[Export] private PackedScene buttonPackedScene;

	[ExportGroup("Input Forwarding")]
	[Export] private RootScene rootScene;//used for input forwarding

	[ExportGroup("Testing This Class")]
	[Export] private bool useTestersThatAreInsideThisClass = true;

	public override void _Ready()
	{
		PlayerStatsWrapper.Instance.PlayerHealthChanged += OnPlayerHealthChanged;
		PlayerStatsWrapper.Instance.PlayerBloodChanged += OnPlayerBloodChanged;
		PlayerStatsWrapper.Instance.PlayerWillpowerChanged += OnPlayerWillpowerChanged;

		extScrollBar = extScrollContainer.GetVScrollBar();
		intScrollBar = intScrollContainer.GetVScrollBar();

		extScrollBar.ValueChanged += OnExternalScrollBarChanged;
		intScrollBar.ValueChanged += OnInternalScrollBarChanged;

		choicesVBoxContainer.Resized += OnChoicesScrContResized;
		OnChoicesScrContResized();
	}
	private void OnChoicesScrContResized()
	{
		Vector2 max;
		if (choicesVBoxContainer.GetChildCount() != 0)
			max = new Vector2(-1, choicesContainerHeight);
		else
			max = new Vector2(-1, 0);
		choicesScrollContainer.CustomMaximumSize = max;
		extDialogueBoxFooter.CustomMaximumSize = max;
	}
	private void OnPlayerHealthChanged(int newHealth) => healthBar.Value = newHealth;
	private void OnPlayerBloodChanged(int newBlood) => bloodBar.Value = newBlood;
	private void OnPlayerWillpowerChanged(int newWillpower)
	{
		for (int i = 0; i < willPowerBits.Length; i++)
		{
			if (i >= willPowerBits.Length) break;

			if (i < willPowerBits.Length - 1)
				willPowerBits[i].Texture = i < newWillpower ? filledBit : unfilledBit;
			else
				willPowerBits[i].Texture = i < newWillpower ? filledBigBit : unfilledBigBit;
		}
	}

	int direction = 1;
	public override void _Process(double delta)//a mess
	{
		float deltaFloat = (float)delta;

		if (!useTestersThatAreInsideThisClass) return;
		
		if (Input.IsActionJustPressed("interact"))
		{
			ToggleExternalDialogueBox();
		}

		if (Input.IsActionPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Health -= (int)Math.Ceiling(20 * delta);
			PlayerStatsWrapper.Instance.playerStats.Blood -= (int)Math.Ceiling(20 * delta);
		}
		if (Input.IsActionJustPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Willpower -= 1;
		}
		if (Input.IsActionPressed("jump"))
		{
			choicesContainerHeight += deltaFloat * 30 * direction;
			OnChoicesScrContResized();
		}
		if (Input.IsActionJustPressed("jump"))
		{
			direction *= -1;
			AddButton();
			if (choicesVBoxContainer.GetChildCount() == 20)
				RemoveButtons();
		}
		if (Input.IsActionJustPressed("move_right"))
		{
			AddInternalDialogueItem();
		}
		else if (Input.IsActionJustPressed("move_left"))
		{
			AddExternalDialogueItem();
		}
		else if (Input.IsActionJustPressed("move_backward"))
		{
			RemoveDialogueItems();
		}
	}
	private void AddButton()//will be changed later when implementing stuff
	{
		Button buttonInst = buttonPackedScene.Instantiate<Button>();

		if (buttonInst.GetThemeStylebox("normal").Duplicate() is StyleBoxFlat styleBox)
		{
			styleBox.BgColor = new Color((float)GD.Randf(), (float)GD.Randf(), (float)GD.Randf());
			buttonInst.AddThemeStyleboxOverride("normal", styleBox);
		}
		choicesVBoxContainer.AddChild(buttonInst);
	}
	private void RemoveDialogueItems()
	{
		foreach (Node child in intVBoxContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (Node child in extVBoxContainer.GetChildren())
		{
			child.QueueFree();
		}

		dialogueMarginLeftToMatchIntExt = 0;
	}
	private void RemoveButtons()
	{
		foreach (Node child in choicesVBoxContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
	private void ToggleExternalDialogueBox()
	{
		extDialogueBox.Visible = !extDialogueBox.Visible;
		bool extDBoxActivated = extDialogueBox.Visible;

		Input.MouseMode = extDBoxActivated ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		rootScene.isForwardingInput = !extDBoxActivated;
	}
	private async void AddInternalDialogueItem()
	{
		MarginContainer dialogueItemInst = dialogueItemPackedScene.Instantiate<MarginContainer>();
		intVBoxContainer.AddChild(dialogueItemInst);

		await GetSize(dialogueItemInst);

		if (lastIntDialogueItemAdded != null)
			lastIntDialogueItemAdded.RemoveThemeConstantOverride("margin_bottom");
		int itemSize = (int)dialogueItemInst.Size.Y;//size of dialogueItem(marginContainer) without added margin
		if (lastDialogueWasInInternal == false)//last dialogue was in external
		{
			dialogueItemInst.AddThemeConstantOverride("margin_top", dialogueMarginLeftToMatchIntExt);
			dialogueMarginLeftToMatchIntExt = 0;
		}
		dialogueMarginLeftToMatchIntExt += itemSize;
		if (lastExtDialogueItemAdded != null)
			lastExtDialogueItemAdded.AddThemeConstantOverride("margin_bottom", dialogueMarginLeftToMatchIntExt);


		lastDialogueWasInInternal = true;
		lastIntDialogueItemAdded = dialogueItemInst;
	}
	private async void AddExternalDialogueItem()
	{
		MarginContainer dialogueItemInst = dialogueItemPackedScene.Instantiate<MarginContainer>();
		extVBoxContainer.AddChild(dialogueItemInst);

		await GetSize(dialogueItemInst);

		if (lastExtDialogueItemAdded != null)
			lastExtDialogueItemAdded.RemoveThemeConstantOverride("margin_bottom");
		int itemSize = (int)dialogueItemInst.Size.Y;//size of dialogueItem(marginContainer) without added margin
		if (lastDialogueWasInInternal == true)//last dialogue was in internal
		{
			dialogueItemInst.AddThemeConstantOverride("margin_top", dialogueMarginLeftToMatchIntExt);
			dialogueMarginLeftToMatchIntExt = 0;
		}
		dialogueMarginLeftToMatchIntExt += itemSize;
		if (lastIntDialogueItemAdded != null)
			lastIntDialogueItemAdded.AddThemeConstantOverride("margin_bottom", dialogueMarginLeftToMatchIntExt);

		lastDialogueWasInInternal = false;
		lastExtDialogueItemAdded = dialogueItemInst;
	}

	private async Task GetSize(MarginContainer dialogueItemInst)//ai help
	{
		// Tell the container to queue a layout update normally
		dialogueItemInst.QueueSort();

		// Pause this method and wait for Godot to complete the frame processing loop
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		// The engine has finished sorting, and the size is now fully updated
		GD.Print($"Updated Size after frame: {dialogueItemInst.Size}");
	}

	//## scrolling dialogue boxes
	private void OnInternalScrollBarChanged(double Value)
	{
		if (_syncing)//avoids an infinite cycle
			return;

		_syncing = true;
		extScrollContainer.ScrollVertical = (int)Value;
		_syncing = false;
	}
	private void OnExternalScrollBarChanged(double Value)
	{
		if (_syncing)
			return;

		_syncing = true;
		intScrollContainer.ScrollVertical = (int)Value;
		_syncing = false;

	}
	public override void _ExitTree()
	{
		PlayerStatsWrapper.Instance.PlayerHealthChanged -= OnPlayerHealthChanged;
		PlayerStatsWrapper.Instance.PlayerBloodChanged -= OnPlayerBloodChanged;
		PlayerStatsWrapper.Instance.PlayerWillpowerChanged -= OnPlayerWillpowerChanged;
		extScrollBar.ValueChanged -= OnExternalScrollBarChanged;
		intScrollBar.ValueChanged -= OnInternalScrollBarChanged;
	}

}
