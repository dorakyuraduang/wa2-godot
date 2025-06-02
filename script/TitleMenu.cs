using Godot;



public partial class TitleMenu : Control
{
	[Export]
	public Wa2Button CgModeButton;
	[Export]
	public Wa2Button BgmModeButton;
	[Export]
	public Control MenuBttons;
	[Export]
	public Control InitalStart;
	[Export]
	public AnimationPlayer AnimationPlayer;
	[Export]
	public Wa2Button StartButton;
	[Export]
	public Wa2Button LoadtButton;
	[Export]
	public Wa2Button OptionsButton;
	[Export]
	public Wa2Button QuitButton;
	[Export]
	public Wa2Button ICButton;
	[Export]
	public Wa2Button CcButton;
	[Export]
	public Wa2Button CodeaButton;
	[Export]
	public Wa2Button StartBackButton;
	[Export]
	public Wa2Button SpecialBackButton;
	[Export]
	public Wa2Button SpecialButton;
	[Export]
	public Control Special;
	[Export]
	public Control DigitalNovel;
	[Export]
	public Wa2Button DigitalNovelButton;
	[Export]
	public Wa2Button DigitalNovelBackButton;
	[Export]
	public Wa2Button DigitalNovel1Button;
	[Export]
	public Wa2Button DigitalNovel2Button;
	[Export]
	public Wa2Button SceneReplayButton;
	[Export]
	public Wa2Button VoiceMessageButton;
	private Wa2EngineMain _engine;


	public override void _Ready()
	{
		_engine = Wa2EngineMain.Engine;
		StartButton.ButtonDown += OnStartButtonDown;
		StartBackButton.ButtonDown += OnStartBackButtonDown;
		QuitButton.ButtonDown += OnQuitButtonDown;
		ICButton.ButtonDown += OnIcButtonDown;
		CcButton.ButtonDown += OnCCButtonDown;
		SpecialButton.ButtonDown += OnSpecialButtonDown;
		SpecialBackButton.ButtonDown += OnSpecialBackButtonDown;
		// As1Button.ButtonDown += OnAs1ButtonDown;
		// As2Button.ButtonDown += OnAs2ButtonDown;
		DigitalNovel1Button.ButtonDown += OnDigitalNovel1ButtonDown;
		DigitalNovel2Button.ButtonDown += OnDigitalNovel2ButtonDown;
		DigitalNovelButton.ButtonDown += OnDigitalNovelButtonDown;
		DigitalNovelBackButton.ButtonDown += OnDigitalNovelBackButtonDown;
		OptionsButton.ButtonDown += OnOptionsButtonDown;
		BgmModeButton.ButtonDown += OnBgmModeButtonDown;
		CodeaButton.ButtonDown += OnCodeaButtonDown;
		LoadtButton.ButtonDown += OnLoadButtonDown;
		CgModeButton.ButtonDown += OnCgModeButtonDown;
		SceneReplayButton.ButtonDown += OnSceneReplayButtonDown;
		VoiceMessageButton.ButtonDown+=OnVoiceMessageButtonDown;
	}
	public void OnSceneReplayButtonDown()
	{
		_engine.UiMgr.OpenSceneReplayMenu();
	}
	public void OnVoiceMessageButtonDown()
	{
		_engine.UiMgr.OpenVoiceMessageMenu();
	}
	public void OnDigitalNovelButtonDown()
	{
		Special.Hide();
		DigitalNovel.Show();
	}
	public void OnDigitalNovelBackButtonDown()
	{
		DigitalNovel.Hide();
		Special.Show();

	}
	public void OnOptionsButtonDown()
	{
		_engine.UiMgr.OpenOptionsMenu();
	}
	public void OnCgModeButtonDown()
	{
		_engine.UiMgr.OpenCGModeMenu();
	}
	public void OnBgmModeButtonDown()
	{
		_engine.UiMgr.OpenBgmModeMenu();
	}
	public void OnLoadButtonDown()
	{
		_engine.UiMgr.OpenLoadMenu();
	}
	public void OnSpecialButtonDown()
	{
		MenuBttons.Hide();
		Special.Show();
	}
	public async void OnCodeaButtonDown()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("3001");
		_engine.UiMgr.OpenGame();
	}
	public async void OnCCButtonDown()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("2001");
		_engine.UiMgr.OpenGame();
	}
	public async void OnDigitalNovel1ButtonDown()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("5000");
		_engine.UiMgr.OpenGame();

	}
	public async void OnDigitalNovel2ButtonDown()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("5100");
		_engine.UiMgr.OpenGame();

	}
	public async void OnIcButtonDown()
	{
		_engine.SoundMgr.StopBgm();
		AnimationPlayer.Play("close");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.StartScript("1001");
		_engine.UiMgr.OpenGame();
	}
	// public async void OnAs1ButtonDown()
	// {
	// 	_engine.SoundMgr.StopBgm();
	// 	AnimationPlayer.Play("close");
	// 	await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
	// 	_engine.StartScript("6001");
	// 	_engine.UiMgr.OpenGame();

	// }
	// public async void OnAs2ButtonDown()
	// {
	// 	_engine.SoundMgr.StopBgm();
	// 	AnimationPlayer.Play("close");
	// 	await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
	// 	_engine.StartScript("6101");
	// 	_engine.UiMgr.OpenGame();

	// }
	public void OnQuitButtonDown()
	{
		GetTree().Quit();
	}
	public void OnStartBackButtonDown()
	{
		MenuBttons.Show();
		InitalStart.Hide();
	}
	public void OnSpecialBackButtonDown()
	{
		MenuBttons.Show();
		Special.Hide();
	}
	public void OnStartButtonDown()
	{
		MenuBttons.Hide();
		InitalStart.Show();
	}
	public async void Open()
	{
		Show();
		AnimationPlayer.Play("RESET");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.SoundMgr.StopBgm();
		if (_engine.ReplayMode > 0)
		{
			AnimationPlayer.Play("open");
			AnimationPlayer.Advance(AnimationPlayer.CurrentAnimation.Length);
			_engine.UiMgr.OpenSceneReplayMenu();
		}
		else
		{
			// AnimationPlayer.Play("logo");
			// await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			AnimationPlayer.Play("open");
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			_engine.SoundMgr.PlayBgm(31);
		}
		
		_engine.ReplayMode = 0;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
	public override void _GuiInput(InputEvent @event)
	{

		if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			if (AnimationPlayer.CurrentAnimation != "close")
			{
				AnimationPlayer.Advance(AnimationPlayer.CurrentAnimation.Length);
			}

		}

	}
}
