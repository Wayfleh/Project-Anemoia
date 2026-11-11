using Godot;
using System;
using System.Threading.Tasks;

public partial class TextBox : Control
{
    public RichTextLabel name;
    public RichTextLabel message;
    public override void _Ready()
    {
        base._Ready();
        name = GetNode<RichTextLabel>("Name");
        message = GetNode<RichTextLabel>("Message");
    }

    public async void SetDialogue(String _name, String _message, Color nameColor = default)
    {
        if (!IsNodeReady())
        {
            await ToSignal(this, SignalName.Ready);
        }
        if (nameColor == default)
        {
            nameColor = Colors.Red; // default color is red
        }

        
        name.Clear();
        message.Clear();

	    // add the name line
        name.PushColor(nameColor);
        name.AppendText(_name);
        name.AppendText(" - ");
        // calculate the position of the first line
        var firstLinePosition = name.GetContentWidth() - message.OffsetLeft;
        message.AppendText($"[p tab_stops={firstLinePosition}]\t{_message}[/p]");
	    // Set minimum size based on message content height
        CustomMinimumSize = new Vector2(CustomMinimumSize.X, message.GetContentHeight() );
    }
}
