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
		OptionsButton.ButtonDown+=OnOptionsButtonDown;
		BgmModeButton.ButtonDown+=OnBgmModeButtonDown;
		CodeaButton.ButtonDown += OnCodeaButtonDown;
		LoadtButton.ButtonDown += OnLoadButtonDown;
		CgModeButton.ButtonDown+=OnCgModeButtonDown;

	}
	public void OnOptionsButtonDown(){
		_engine.UiMgr.OpenOptionsMenu();
	}
	public void OnCgModeButtonDown(){
		_engine.UiMgr.OpenCGModeMenu();
	}
	public void OnBgmModeButtonDown(){
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
		_engine.SoundMgr.StopBgm();
		Show();
		AnimationPlayer.Play("RESET");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		// AnimationPlayer.Play("logo");
		// await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		AnimationPlayer.Play("open");
		await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		_engine.SoundMgr.PlayBgm(31);
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
