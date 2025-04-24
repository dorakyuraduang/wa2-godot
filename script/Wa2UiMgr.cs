using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Wa2UiMgr : Control
{
	public Stack<Control> UiQueue = new();
	[Export]
	public UIConfirm UIConfirm;
	[Export]
	public OptionsMenu OptionsMenu;
	[Export]
	public BgmModeMenu BgmModeMenu;
	[Export]
	public Wa2AdvMain AdvMain;
	[Export]
	public TitleMenu TitleMenu;
	[Export]
	public LoadSaveMenu LoadSaveMenu;
	[Export]
	public CGModeMenu CGModeMenu;
	[Export]
	public BackLogMenu BackLogMenu;
	private Wa2EngineMain _engine;
	public override void _Ready()
	{

		TitleMenu.Visible = false;
		_engine = Wa2EngineMain.Engine;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	public void OpenBackLog()
	{
		BackLogMenu.Open();
		UiQueue.Push(BackLogMenu);
	}
	public void OpenGame()
	{
		_engine.SubViewport.Show();
		_engine.State = Wa2EngineMain.GameState.GAME;
		JumpScene(AdvMain);
	}
	public void ReturnScene()
	{
		if (UiQueue.Count > 0)
		{
			Control ui = UiQueue.Pop();
			ui.Hide();
		}
	}
	public void OpenOptionsMenu(){
		OptionsMenu.Open();
		UiQueue.Push(OptionsMenu);
	}
	public void OpenSaveMenu()
	{
		LoadSaveMenu.Open(DataMode.Save);
		UiQueue.Push(LoadSaveMenu);
	}
	public void OpenCGModeMenu()
	{
		CGModeMenu.Open();
		UiQueue.Push(CGModeMenu);
	}
	public void OpenBgmModeMenu()
	{
		BgmModeMenu.Open();
		UiQueue.Push(BgmModeMenu);
	}
	public void OpenLoadMenu()
	{
		LoadSaveMenu.Open(DataMode.Load);
		UiQueue.Push(LoadSaveMenu);
	}
	public void OpenConfirm(string text1,string text2,bool confirm, Action action){
		UiQueue.Push(UIConfirm);
		UIConfirm.Open(text1,text2,confirm,action);
		
	}
	public void OpenTitleMenu()
	{
		_engine.State = Wa2EngineMain.GameState.TITLE;
		_engine.GameSav.Reset();
		_engine.Reset();
		_engine.SubViewport.Hide();
		
		TitleMenu.Open();
		JumpScene(TitleMenu);
	}
	public void JumpScene(Control scene)
	{
		if (UiQueue.Count > 0)
		{
			Control ui = UiQueue.Pop();
			ui.Hide();

		}
		scene.Show();
		UiQueue.Push(scene);
	}
}
