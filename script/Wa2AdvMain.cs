using Godot;
using System;

public partial class Wa2AdvMain : Control
{
	[Export]
	public Wa2Label NameLabel;
	[Export]
	public Wa2Label TextLabel;
	private Wa2EngineMain _engine;
	public string CurText = "";
	public string CurName = "";
	public enum AdvState
	{
		NONE,
		SHOW_MESSAGE,
		SHOW_TEXT,
		WAIT_CLICK,
		HIDE_MESSAGE
	}
	public AdvState State;
	public bool Active { private set; get; }
	public override void _Ready()
	{
		_engine = Wa2EngineMain.Engine;
		Active = false;
		Modulate = new Color(1, 1, 1, 0);
	}
	public void AdvShow(float time = 0.25f)
	{
		UpdateText();
		State = AdvState.SHOW_MESSAGE;
		_engine.WaitTimer.Start(time);
	}
	public void AdvHide(float time = 0.25f)
	{
		Active = false;
		State = AdvState.HIDE_MESSAGE;
		_engine.WaitTimer.Start(time);
	}
	public override void _GuiInput(InputEvent @event)
	{
		if (!Active)
		{
			return;
		}
		if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			_engine.ClickAdv();
		}
	}
	public void Update()
	{
		if (State == AdvState.SHOW_MESSAGE)
		{

			Modulate = new Color(1, 1, 1, _engine.WaitTimer.GetProgress());
			if (_engine.WaitTimer.GetProgress() >= 1f)
			{
				Modulate = new Color(1, 1, 1, 1);
				Active = true;
				AdvShowText();
			}
		}
		if (State == AdvState.SHOW_TEXT)
		{
			TextLabel.VisibleRatio = _engine.WaitTimer.GetProgress();

			if (_engine.WaitTimer.GetProgress() >= 1f)
			{
				TextLabel.VisibleRatio = 1F;
				State = AdvState.WAIT_CLICK;
				
			}
			UpdateText();
		}
		if (State == AdvState.HIDE_MESSAGE)
		{
			Modulate = new Color(1, 1, 1, 1 - _engine.WaitTimer.GetProgress());
			if (_engine.WaitTimer.GetProgress() >= 1f)
			{
				Modulate = new Color(1, 1, 1, 0);
				State = AdvState.NONE;
			}
		}
		
	}
	public void UpdateText()
	{
		NameLabel.Update();
		TextLabel.Update();
	}
	public void Clear()
	{
		State = AdvState.NONE;
		TextLabel.VisibleRatio = 0f;
		NameLabel.VisibleRatio = 0f;
		NameLabel.Text = "";
		TextLabel.Text = "";
	}
	public void AdvShowText()
	{
		State = AdvState.SHOW_TEXT;
		NameLabel.VisibleRatio = 1f;
		TextLabel.Text = CurText;
		NameLabel.Text = CurName;
		TextLabel.VisibleRatio = 0;
		_engine.WaitTimer.Start((float)CurText.Length / (float)_engine.Prefs.TextSpeed);
		_engine.Script.ParseCmd();
	}
	public void Finish()
	{
		_engine.WaitTimer.Done();
		Update();
	}
}