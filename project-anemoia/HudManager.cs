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
	private ScrollBar externalScrollBar, internalScrollBar;
	private bool _syncing = false;
	[Export] private ScrollContainer choicesScrollContainer;
	[Export] private VBoxContainer choicesVBoxContainer;
	[Export] private float choicesScrContMaxHeight = 200f;

	[Export] private RootScene rootScene;//used for input forwarding

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
		Vector2 m = new Vector2(-1, Mathf.Min(choicesVBoxContainer.Size.Y, choicesScrContMaxHeight));
		choicesScrollContainer.CustomMaximumSize = m;
		externalDialogueBoxFooter.CustomMaximumSize = m;
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
	public override void _Process(double delta)
	{
		float deltaFloat = (float)delta;
		if (Input.IsActionPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Health -= (int)Math.Ceiling(20 * delta);
			PlayerStatsWrapper.Instance.playerStats.Blood -= (int)Math.Ceiling(20 * delta);
		}

		if (Input.IsActionJustPressed("jump"))
		{
			direction *= -1;
			Button n = (Button)choicesVBoxContainer.GetChild(0).Duplicate();

			// 1. Fetch and duplicate the existing StyleBox to make it unique
			if (n.GetThemeStylebox("normal").Duplicate() is StyleBoxFlat styleBox)
			{
				// 2. Change your decoration colors
				styleBox.BgColor = new Color((float)GD.Randf(), (float)GD.Randf(), (float)GD.Randf());

				// 3. Apply the modified StyleBox back to the button
				n.AddThemeStyleboxOverride("normal", styleBox);
			}
			choicesVBoxContainer.AddChild(n);
		}
		if (Input.IsActionPressed("jump"))
		{
			choicesScrContMaxHeight += deltaFloat * 30 * direction;
			choicesScrContMaxHeight += deltaFloat * 30 * direction;
		}
		if (Input.IsActionJustPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Willpower -= 1;
			externalDialogueBox.Visible = !externalDialogueBox.Visible;

			Input.MouseMode = externalDialogueBox.Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			rootScene.isForwardingInput = !externalDialogueBox.Visible;
		}
	}
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
