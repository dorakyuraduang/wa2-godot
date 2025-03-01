using Godot;
using System;

public partial class Wa2Main : Control
{
	public override void _Process(double delta)
	{
		Wa2Script.ParseCmd();
	}
}
