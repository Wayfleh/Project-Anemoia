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
	}
	private void OnChoicesScrContResized()
	{
		choicesScrollContainer.CustomMaximumSize = new Vector2(0, Mathf.Min(choicesVBoxContainer.Size.Y, choicesScrContMaxHeight));
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
			Control n = (Control)choicesVBoxContainer.GetChild(0).Duplicate();
			choicesVBoxContainer.AddChild(n);
		}
		if (Input.IsActionPressed("jump"))
		{
			externalDialogueBoxFooter.CustomMinimumSize += Vector2.Up * deltaFloat * 10 * direction;
			externalDialogueBoxFooter.CustomMaximumSize += Vector2.Up * deltaFloat * 10 * direction;
		}
		if (Input.IsActionJustPressed("interact"))
		{
			PlayerStatsWrapper.Instance.playerStats.Willpower -= 1;
			Debug.Print("Willpower: " + PlayerStatsWrapper.Instance.playerStats.Willpower);
			externalDialogueBox.Visible = !externalDialogueBox.Visible;

			Input.MouseMode = externalDialogueBox.Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
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
