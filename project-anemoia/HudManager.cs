using Godot;
using System;
using System.Diagnostics;

public partial class HudManager : Node
{
	[Export] private TextureProgressBar healthBar;
	[Export] private TextureProgressBar bloodBar;
	[Export] private TextureRect[] willPowerBits;
	[Export] private Texture2D filledBit, unfilledBit;
	[Export] private Texture2D filledBigBit, unfilledBigBit;

	[Export] private Control externalDialogueBox;
	[Export] private Control externalDialogueBoxFooter;
	[Export] private ScrollContainer externalScrollContainer, internalScrollContainer;
	[Export] private VBoxContainer externalVBoxContainer, internalVBoxContainer;
	private ScrollBar externalScrollBar, internalScrollBar;
	private bool _syncing = false;
	[Export] private PackedScene dialogueItemPackedScene; //same dialogue item for everything for now? NOTE:Margin Container

	private int dialogueHeight = 0;


	[Export] private ScrollContainer choicesScrollContainer;
	[Export] private VBoxContainer choicesVBoxContainer;
	[Export] private float choicesScrContHeightConstant = 200f;
	[Export] private PackedScene buttonPackedScene;

	[Export] private RootScene rootScene;//used for input forwarding

	[Export] private bool DebuggingInput = true;

	public override void _Ready()
	{
		PlayerStatsWrapper.Instance.PlayerHealthChanged += OnPlayerHealthChanged;
		PlayerStatsWrapper.Instance.PlayerBloodChanged += OnPlayerBloodChanged;
		PlayerStatsWrapper.Instance.PlayerWillpowerChanged += OnPlayerWillpowerChanged;

		externalScrollBar = externalScrollContainer.GetVScrollBar();
		internalScrollBar = internalScrollContainer.GetVScrollBar();

		externalScrollBar.ValueChanged += OnExternalScrollBarChanged;
		internalScrollBar.ValueChanged += OnInternalScrollBarChanged;

		choicesVBoxContainer.Resized += OnChoicesScrContResized;
		OnChoicesScrContResized();
		randomNumberGenerator = new RandomNumberGenerator();
	}
	private void OnChoicesScrContResized()
	{
		Vector2 max;
		if (choicesVBoxContainer.GetChildCount() != 0)
			max = new Vector2(-1, choicesScrContHeightConstant);
		else
			max = new Vector2(-1, 0);
		choicesScrollContainer.CustomMaximumSize = max;
		externalDialogueBoxFooter.CustomMaximumSize = max;
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
	RandomNumberGenerator randomNumberGenerator;
	public override void _Process(double delta)//a mess
	{
		float deltaFloat = (float)delta;

		if (Input.IsActionJustPressed("interact"))
		{
			ToggleExternalDialogueBox();
		}

		if (!DebuggingInput) return;

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
			choicesScrContHeightConstant += deltaFloat * 30 * direction;
			OnChoicesScrContResized();
		}
		if (Input.IsActionJustPressed("jump"))
		{
			AddInternalDialogueItem();
			direction *= -1;
			AddButton();
			if (choicesVBoxContainer.GetChildCount() == 20)
				RemoveButtons();
		}
	}
	private void AddButton()//will be changed later when implementing stuff?
	{
		Button buttonInst = buttonPackedScene.Instantiate<Button>();

		if (buttonInst.GetThemeStylebox("normal").Duplicate() is StyleBoxFlat styleBox)
		{
			styleBox.BgColor = new Color((float)GD.Randf(), (float)GD.Randf(), (float)GD.Randf());
			buttonInst.AddThemeStyleboxOverride("normal", styleBox);
		}
		choicesVBoxContainer.AddChild(buttonInst);
	}
	private void RemoveButtons()
	{
		int childCount = choicesVBoxContainer.GetChildCount();
		if (childCount == 0) return;

		foreach (Node child in choicesVBoxContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
	private void ToggleExternalDialogueBox()
	{
		externalDialogueBox.Visible = !externalDialogueBox.Visible;
		bool extDBoxActivated = externalDialogueBox.Visible;

		Input.MouseMode = extDBoxActivated ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		rootScene.isForwardingInput = !extDBoxActivated;
	}
	private void AddInternalDialogueItem()
	{
		MarginContainer dialogueItemInst = dialogueItemPackedScene.Instantiate<MarginContainer>();
		dialogueItemInst.AddThemeConstantOverride("margin_top", dialogueHeight);
		dialogueHeight += (int)dialogueItemInst.Size.Y;/////////////////////////////////////UUUUUUH. gonna continue work here 
		internalVBoxContainer.AddChild(dialogueItemInst);
	}

	//## scrolling dialogue boxes
	private void OnInternalScrollBarChanged(double Value)
	{
		if (_syncing)//avoids an infinite cycle
			return;

		_syncing = true;
		externalScrollContainer.ScrollVertical = (int)Value;
		_syncing = false;
	}
	private void OnExternalScrollBarChanged(double Value)
	{
		if (_syncing)
			return;

		_syncing = true;
		internalScrollContainer.ScrollVertical = (int)Value;
		_syncing = false;

	}
	public override void _ExitTree()
	{
		PlayerStatsWrapper.Instance.PlayerHealthChanged -= OnPlayerHealthChanged;
		PlayerStatsWrapper.Instance.PlayerBloodChanged -= OnPlayerBloodChanged;
		PlayerStatsWrapper.Instance.PlayerWillpowerChanged -= OnPlayerWillpowerChanged;
		externalScrollBar.ValueChanged -= OnExternalScrollBarChanged;
		internalScrollBar.ValueChanged -= OnInternalScrollBarChanged;
	}

}
