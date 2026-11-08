using Godot;
using System;

[GlobalClass]
public partial class NPCStat : Resource
{
	[Export]
	public int Health { get; set; } = 100;

	[Export]
	public int Blood { get; set; } = 100;
	
	public NPCStat()
	{
	}
}
