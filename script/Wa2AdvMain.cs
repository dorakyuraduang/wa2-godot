using Godot;
using System;

public partial class Wa2AdvMain : Control
{

	[Export]
	public Wa2Button BackLogButton;
	[Export]
	public TextureRect IsReadTexture;
	[Export]
	public TextureRect AutoModeTexture;
	[Export]
	public TextureRect SkipModeTexture;
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
	[Export]
	public TextureRect Window;
	// [Export]
	// public Control DebugUi;
	// 	[Export]
	// public bool Debug;
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
		OptionButton.ButtonDown += OnOptionButtonDown;
		BackLogButton.ButtonDown += OnBackLogButtonDown;
		for (int i = 0; i < SelectMessageContainer.GetChildCount(); i++)
		{
			int idx = i;
			SelectMessageContainer.GetChild<SelectMessage>(i).ButtonDown += () => OnSelectMessageButtonDown(idx);
		}
	}
	public void OnOptionButtonDown()
	{
		_engine.UiMgr.OpenOptionsMenu();
	}
	public void OnAutoButtonDown()
	{
		_engine.AutoMode = !_engine.AutoMode;
		if (_engine.AutoMode)
		{
			_engine.AutoModeStart();
		}
	}
	public void OnBackLogButtonDown()
	{
		_engine.UiMgr.OpenBackLog();
	}
	public void OnSkipButtonDown()
	{
		_engine.StopAutoMode();
		if (!_engine.HasReadMessage && _engine.Prefs.GetConfig("msg_cut_optin") == 0)
		{
			_engine.SkipMode = false;
		}
		else
		{
			_engine.SkipMode = !_engine.SkipMode;
		}
	}
	public void OnSelectMessageButtonDown(int idx)
	{

		for (int i = 0; i < SelectMessageContainer.GetChildCount(); i++)
		{
			SelectMessageContainer.GetChild<SelectMessage>(i).Hide();
		}
		// GD.Print("索引",_engine.Script.args[^2].IntValue);
		_engine.Script.Args[^1].Set(idx);
		// _engine.GameSav.args.RemoveAt(_engine.GameSav.args.Count-1);
		_engine.SelectItems.Clear();
		SelectMessageContainer.Hide();
		_engine.WaitClick = false;
		// GD.Print("和纱本气度:",_engine.GameSav.GameFlags[5]);
		// GD.Print("和纱浮气度:", _engine.GameSav.GameFlags[6]);
		// GD.Print("雪菜好意度:", _engine.GameSav.GameFlags[7]);
		// _engine.Script.Wait = false;
	}

	public void OnOffButtonDown()
	{
		if (_engine.WaitClick)
		{
			_engine.StopSkip();
			_engine.StopAutoMode();
			_engine.AdvMain.Hide();
		}

	}
	public void SetDemoMode(bool b)
	{
		if (b)
		{
			LoadButton.Visible = false;
			SaveButton.Visible = false;
			AutoButton.Visible = false;
			OffButton.Visible = false;
			BackLogButton.Visible = false;
			OptionButton.Visible = false;
		}
		else
		{
			LoadButton.Visible = true;
			SaveButton.Visible = true;
			AutoButton.Visible = true;
			OffButton.Visible = true;
			BackLogButton.Visible = true;
			OptionButton.Visible = true;

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
		if (!SelectMessageContainer.Visible && _engine.WaitClick && !_engine.TextTimer.IsActive() && !_engine.CanSkip() && !_engine.AutoMode && !_engine.DemoMode && !_engine.WaitTimer.IsActive() && _engine.ScriptStack.Count==1)
		{
			WaitSprite.Show();
		}
		else
		{
			WaitSprite.Hide();
		}
		AutoModeTexture.Visible = _engine.AutoMode;
		SkipModeTexture.Visible = _engine.SkipMode;
		IsReadTexture.Visible = _engine.HasReadMessage;
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
		Modulate = new Color(1, 1, 1, 0);
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

		// Wa2AdvAnimator animator = new(this);
		// animator.InitFade(time, fadein);
		_engine.AnimatorMgr.AddAdvFeadAnimation(this,time, fadein);

	}
	public void TextStart(float delay = 0f)
	{

		if (TextLabel.Text != "")
		{
			// GD.Print(TextLabel.Text.Length / _engine.Prefs.TextSpeed);
			_engine.TextTimer.Start(TextLabel.Text.Length / _engine.Prefs.TextSpeed, delay);
		}

	}

	public void ShowText(bool fade = true)
	{

		// NameLabel.Text = name;
		AdvShow(fade);
		if (!_engine.CanSkip() && fade)
		{
			TextStart(0.2f);
		}
		else
		{
			TextLabel.VisibleRatio = 1f;
			NameLabel.VisibleRatio = 1f;
			WaitSprite.Position = TextLabel.GetEndPosition();
			// WaitSprite.Show();
		}
		if (_engine.GetReadMessage(_engine.CurMessageIdx))
		{
			_engine.HasReadMessage = true;
		}
		else
		{
			_engine.HasReadMessage = false;
			_engine.SetReadMessage(_engine.CurMessageIdx);
			if (_engine.Prefs.GetConfig("msg_cut_optin") == 0)
			{
				_engine.StopSkip();
			}
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
		if ((!_engine.WaitClick && !_engine.AdvMain.SelectMessageContainer.Visible) || _engine.WaitTimer.IsActive() || _engine.ScriptStack.Count > 1)
		{
			return;
		}
		_engine.UiMgr.OpenSaveMenu();
	}
	public void OnLoadButtonDown()
	{
		if ((!_engine.WaitClick && !_engine.AdvMain.SelectMessageContainer.Visible) || _engine.WaitTimer.IsActive() || _engine.ScriptStack.Count > 1)
		{
			return;
		}
		_engine.UiMgr.OpenLoadMenu();
	}
	public void SetWindowAlpha(int alpha)
	{
		Window.Modulate = new Color(1, 1, 1, alpha / 256f);
	}
	// public override void _Process(double delta)
	// {
	// 	if (Debug && _engine.State==Wa2EngineMain.GameState.GAME && _engine.GameSav.GameFlags!=null)
	// 	{
	// 		for (int i = 0; i < 9; i++)
	// 		{
	// 			DebugUi.GetChild<HBoxContainer>(i).GetChild<Label>(1).Text = _engine.GameSav.GameFlags[5 + i].ToString();
	// 		}

	// 	}

	// }
	// public override void _Ready()
	// {
	// 	DebugUi.Visible = Debug;
	// }

}