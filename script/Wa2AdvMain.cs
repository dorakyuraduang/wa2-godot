using Godot;
using System;

public partial class Wa2AdvMain : Control
{
	[Export]
	public Wa2Button SaveButton;
	[Export]
	public Wa2Button LoadButton;
	[Export]
	public Wa2Button OptionButton;
	[Export]
	public Wa2Button AutoButton;
	[Export]
	public Wa2Button SkipButton;
	[Export]
	public Wa2Button OffButton;
	[Export]
	public VBoxContainer SelectMessageContainer;
	[Export]
	public Wa2Label NameLabel;
	[Export]
	public Wa2Label TextLabel;
	[Export]
	public AnimatedSprite2D WaitSprite;

	private Wa2EngineMain _engine;
	public string CurText = "";
	public string CurName = "";
	public enum AdvState
	{
		NONE,
		SHOW_ADV,
		HIDE_ADV
	}
	public AdvState State;
	// public bool Active;
	public void Init(Wa2EngineMain e)
	{
		_engine = e;
		Visible = false;
		Modulate = new Color(1, 1, 1, 0);
		LoadButton.ButtonDown += OnLoadButtonDown;
		SaveButton.ButtonDown += OnSaveButtonDown;
		AutoButton.ButtonDown += OnAutoButtonDown;
		SkipButton.ButtonDown += OnSkipButtonDown;
		OffButton.ButtonDown += OnOffButtonDown;
		for (int i = 0; i < SelectMessageContainer.GetChildCount(); i++)
		{
			int idx = i;
			SelectMessageContainer.GetChild<SelectMessage>(i).ButtonDown += () => OnSelectMessageButtonDown(idx);
		}
	}
	public void OnAutoButtonDown()
	{
		_engine.AutoMode = !_engine.AutoMode;
		if (_engine.AutoMode)
		{
			_engine.AutoModeStart();
		}
	}
	public void OnSkipButtonDown()
	{
		_engine.SkipMode = !_engine.SkipMode;
	}
	public void OnSelectMessageButtonDown(int idx)
	{

		for (int i = 0; i < SelectMessageContainer.GetChildCount(); i++)
		{
			SelectMessageContainer.GetChild<SelectMessage>(i).Hide();
		}
		// GD.Print("索引",_engine.Script.args[^2].IntValue);
		_engine.GameSav.args[^1].Set(idx);
		_engine.GameSav.SelectItems.Clear();
		SelectMessageContainer.Hide();
		// _engine.Script.Wait = false;
	}
	// public void AdvShow(float time = 0.25f)
	// {
	// 	// if(Active){
	// 	// 	return;
	// 	// }
	// 	// UpdateText();
	// 	State = AdvState.SHOW_ADV;
	// 	_engine.WaitTimer.Start(time);
	// }
	// public void AdvHide(float time = 0.25f)
	// {
	// 	Active = false;
	// 	State = AdvState.HIDE_ADV;
	// 	_engine.WaitTimer.Start(time);
	// }
	// public override void _GuiInput(InputEvent @event)
	// {

	// }
	public void OnOffButtonDown()
	{
		if (_engine.WaitClick)
		{
			_engine.AdvMain.Hide();
		}

	}
	public void Update(float delta)
	{
		if (_engine.TextTimer.IsActive())
		{
			_engine.TextTimer.Update(delta);
			if (_engine.TextTimer.IsStart())
			{
				NameLabel.VisibleRatio = 1f;
			}
			TextLabel.VisibleRatio = _engine.TextTimer.GetProgress();
			if (_engine.TextTimer.IsDone())
			{
				TextLabel.VisibleRatio = 1F;
				WaitSprite.Position = TextLabel.GetEndPosition();

			}
		}
		if (_engine.WaitClick && !_engine.TextTimer.IsActive() && !_engine.Skipping && !_engine.SkipMode && !_engine.AutoMode)
		{
			WaitSprite.Show();
		}
		else
		{
			WaitSprite.Hide();
		}
		// if (_engine.WaitClick && !_engine.TextTimer.IsActive())
		// {
		// 	WaitSprite.Show();
		// }
		// else
		// {
		// 	WaitSprite.Hide();
		// }
		// UpdateText();
	}
	// public void UpdateText()
	// {
	// 	NameLabel.Update();
	// 	TextLabel.Update();
	// }
	public void Clear()
	{
		TextLabel.VisibleRatio = 0f;
		NameLabel.VisibleRatio = 0f;
		NameLabel.Text = "";
		TextLabel.Text = "";
		// WaitSprite.Hide();
	}
	public void AdvShow(bool fade = true)
	{
		TextLabel.VisibleRatio = 0f;
		NameLabel.VisibleRatio = 0f;
		if (!Visible || Modulate.A < 1)
		{
			if (fade)
			{
				AdvFade(0.2f, true);
			}
			else
			{
				Visible = true;
				Modulate = new Color(1, 1, 1, 1);
			}
		}
		// AdvFade(time, true);
		// _engine.WaitTimer.Start(time);
	}
	public void AdvHide(float time = 0.2f)
	{
		AdvFade(time, false);
		// _engine.WaitTimer.Start(time);
	}
	public void AdvFade(float time, bool fadein)
	{

		Wa2AdvAnimator animator = new(this);
		animator.InitFade(time, fadein);

	}
	public void TextStart(float delay = 0f)
	{
		
		if (TextLabel.Text != "")
		{
			// GD.Print(TextLabel.Text.Length / _engine.Prefs.TextSpeed);
			_engine.TextTimer.Start(TextLabel.Text.Length / _engine.Prefs.TextSpeed, delay);
		}

	}
	public void ShowText(string text, string name, bool fade = true)
	{

		TextLabel.Text = text;
		NameLabel.Text = name;
		AdvShow(fade);
		if (!_engine.Skipping && fade)
		{
			TextStart(0.2f);
		}
		else
		{
			TextLabel.VisibleRatio = 1f;
			NameLabel.VisibleRatio = 1f;
			WaitSprite.Position = TextLabel.GetEndPosition();
			WaitSprite.Show();
		}

		// _engine.Script.ParseCmd();
		// GD.Print("AdvShowText");
	}
	public void ClearText()
	{
		TextLabel.Text = "";
		NameLabel.Text = "";
		TextLabel.VisibleRatio = 0;
		// WaitSprite.Hide();
	}
	// public void Finish()
	// {
	// 	_engine.WaitTimer.Done();
	// }
	public void OnSaveButtonDown()
	{
		if (_engine.WaitTimer.IsActive() || _engine.WaitAnimator() || _engine.VideoPlayer.IsPlaying() || _engine.WaitTimer.IsActive())
		{

			// GD.Print("等待中");
			return;
		}
		_engine.UiMgr.OpenSaveMenu();
	}
	public void OnLoadButtonDown()
	{
		_engine.UiMgr.OpenLoadMenu();
	}
}