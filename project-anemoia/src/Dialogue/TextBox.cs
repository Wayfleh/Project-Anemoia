using Godot;
using System;

public partial class TextBox : HBoxContainer
{
    public Label name;
    public Label speech;
    public override void _Ready()
    {
        base._Ready();
        name = GetNode<Label>("Name");
        speech = GetNode<Label>("Speech");
    }

}
