using Godot;
using System;

public partial class UICalender : Control
{
	[Export]
	public AnimationPlayer AnimationPlayer;
	[Export]
	public TextureRect BgImage;
	[Export]
	public TextureRect WeekDay;
	[Export]
	public TextureRect Num1;
	[Export]
	public TextureRect Num2;
	[Export]
	public TextureRect Num3;
	[Export]
	public TextureRect Num4;
	public Wa2EngineMain _engine;
	public override void _Ready()
	{
		_engine = Wa2EngineMain.Engine;
		AnimationPlayer.AnimationFinished += anime =>
		{
			Close();
		};
	}
  public override void _Process(double delta)
  {
    if(AnimationPlayer.IsPlaying()){
			if(_engine.CanSkip()){
				AnimationPlayer.SpeedScale=8;
			}else{
				AnimationPlayer.SpeedScale=1;
			}
		}
  }

	public void Close()
	{
		Hide();
		_engine.UiMgr.ReturnScene();
	}
	public void Open()
	{
	
		AnimationPlayer.SpeedScale=1;
		if (_engine.GameFlags[0] == 2)
		{
			BgImage.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/calender_01.png");
		}
		else
		{
			BgImage.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/calender_00.png");
		}
		(WeekDay.Texture as AtlasTexture).Region = new Rect2(104 * _engine.Calender.DayOfWeek, 0, 104, 32);
		if (_engine.Calender.Month >= 10)
		{
			Num1.Show();
			(Num1.Texture as AtlasTexture).Region = new Rect2(40 * (_engine.Calender.Month / 10), 0, 40, 48);
			(Num2.Texture as AtlasTexture).Region = new Rect2(40 * (_engine.Calender.Month % 10), 0, 40, 48);
		}
		else
		{
			Num1.Hide();
			(Num2.Texture as AtlasTexture).Region = new Rect2(40 * _engine.Calender.Month, 0, 40, 48);
		}
		if (_engine.Calender.Day >= 10)
		{
			Num4.Show();
			(Num3.Texture as AtlasTexture).Region = new Rect2(40 * (_engine.Calender.Day / 10), 0, 40, 48);
			(Num4.Texture as AtlasTexture).Region = new Rect2(40 * (_engine.Calender.Day % 10), 0, 40, 48);
		}
		else
		{
			Num4.Hide();
			(Num3.Texture as AtlasTexture).Region = new Rect2(40 * _engine.Calender.Day, 0, 40, 48);
		}
		AnimationPlayer.Play("open");
	}
}
