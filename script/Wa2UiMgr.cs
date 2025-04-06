using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Wa2UiMgr : Control
{
	public Stack<Control> UiQueue = new();
	[Export]
	public Wa2AdvMain AdvMain;
	[Export]
	public TitleMenu TitleMenu;
	[Export]
	public LoadSaveMenu LoadSaveMenu;
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
	public void OpenGame()
	{
		_engine.SubViewport.Show();
		_engine.State = Wa2EngineMain.GameState.GAME;
		JumpScene(AdvMain);
	}
	public void ReturnScene(){
		if (UiQueue.Count > 0)
		{
			Control ui = UiQueue.Pop();
			ui.Hide();

		}
	}
	public void OpenSaveMenu()
	{
		LoadSaveMenu.Open(DataMode.Save);
		UiQueue.Push(LoadSaveMenu);
	}
	public void OpenLoadMenu()
	{
		LoadSaveMenu.Open(DataMode.Load);
		UiQueue.Push(LoadSaveMenu);
	}
	public void OpenTitleMenu()
	{
		_engine.SubViewport.Hide();
		_engine.State = Wa2EngineMain.GameState.TITLE;
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
