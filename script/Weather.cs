using Godot;
using System;
[Tool]

public partial class Weather : Node2D
{
	[Export]
	public int Fps = 60;
	[Export]
	public int SpeedX;
	[Export]
	public int SpeedY;
	[Export]
	public int Count;
	[Export]
	public int Type;
	[Export]
	public int Mask;
	public double FrameTimer;
	public bool percent80Count;
	public override void _Ready()
	{
		Reset();
	}
	public override void _Process(double delta)
	{
		FrameTimer -= delta;
		if (FrameTimer <= 0)
		{
			FrameTimer += delta;
		}
		else
		{
			return;
		}
		switch (Type)
		{
			case 3:
				break;
		}
	}
	public void Reset()
	{
		FrameTimer = 1.0 / Fps;
		int spawnCount = 0;
		if (percent80Count)
		{
			for (int i = 0; i < (int)(0.8 * Count); i++)
			{
				
			}
		}
	}
}
