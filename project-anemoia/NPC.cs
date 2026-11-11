using Godot;
using System;

public partial class NPC : CharacterBody3D
{
	[Export]
	public NPCStat Stats { get; set; }
	
}
