using Godot;
using System;
using System.Collections.Generic;
using System.Text;
public class Wa2Char
{
	public int pos;
	public int id;
	public int no;
}
public partial class Wa2EngineMain : Node
{
	public enum GameState
	{
		NONE,
		GAME,
		TITLE
	}
	// [Export]
	// public int Mode;
	// [Export]
	// public TextureRect Texture;
	// Called when the node enters the scene tree for the first time.
	public float BgTime;
	public int ReplayMode;
	public Dictionary<int, Wa2Char> CharDic = new();
	public static Wa2EngineMain Engine;
	public int Year;
	public int Month;
	public int Day;
	public int TimeMode;
	public List<Wa2Animator> Animators { private set; get; } = new();
	public bool WaitClick = false;
	public Wa2Prefs Prefs;
	public int Label;
	public bool Skipping = false;
	[Export]
	public TestUi TestUi;
	[Export]
	public Node CharGroup;
	public Wa2Image[] Chars;
	[Export]
	public SubViewportContainer SubViewport;
	[Export]
	public Viewport Viewport;
	[Export]
	public Wa2AdvMain AdvMain;
	[Export]
	public Wa2UiMgr UiMgr;
	[Export]
	public Wa2SoundMgr SoundMgr;
	[Export]
	public Wa2Image BgTexture;
	[Export]
	public Wa2Image MaskTexture;
	public GameState State = GameState.NONE;
	public Wa2Timer WaitTimer = new();
	public float FrameTime { private set; get; } = 1.0f / 60;
	public Wa2Script Script;
	public Wa2Func Func;
	public Wa2EngineMain()
	{
		if (Engine == null)
		{
			Engine = this;
		}
	}
	// public void RemoveChar(int chr)
	// {
	// 	CharDic.Remove(chr);
	// }
	// public void AddChar(int id, int no, int pos, bool show = true)
	// {
	// 	foreach (int k in CharDic.Keys)
	// 	{
	// 		if (CharDic[k].pos == pos && CharDic[k].id != id)
	// 		{
	// 			CharDic.Remove(k);
	// 		}
	// 	}
	// 	if (!CharDic.ContainsKey(id))
	// 	{
	// 		CharDic[id] = new Wa2Char();
	// 	}
	// 	CharDic[id].pos = pos;
	// 	CharDic[id].id = id;
	// 	CharDic[id].no = no;
	// 	CharDic[id].show = show;
	// }
	public override void _Ready()
	{
		
		if (OS.GetName() == "Android")
		{
			
			Wa2Resource.ResPath = "/storage/emulated/0/Wa2Res/";

		}
		else
		{
			Wa2Resource.ResPath = "res://assets/";
		}
		Prefs = new Wa2Prefs();
		Func = new Wa2Func(this);
		Script = new Wa2Script(Func);
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		Wa2Def.LoadFontMap();
		Wa2Resource.LoadPak("BGM.PAK");
		Wa2Resource.LoadPak("/IC/BGM.PAK");
		Wa2Resource.LoadPak("/IC/bak.pak");
		Wa2Resource.LoadPak("/IC/grp.pak");
		Wa2Resource.LoadPak("/IC/char.pak");
		Wa2Resource.LoadPak("/IC/VOICE.PAK");
		Wa2Resource.LoadPak("/IC/SE.PAK");
		Wa2Resource.LoadPak("bak.pak");
		Wa2Resource.LoadPak("ck-gal.pak");
		Wa2Resource.LoadPak("grp.pak");
		Wa2Resource.LoadPak("char.pak");
		Wa2Resource.LoadPak("VOICE.PAK");
		Wa2Resource.LoadPak("SE.PAK");


		Chars = new Wa2Image[Wa2Def.CharPos.Length];
		for (int i = 0; i < Wa2Def.CharPos.Length; i++)
		{
			// Chars[i]=new Wa2Image();
			Chars[i] = CharGroup.GetChild(i) as Wa2Image;
			Chars[i].Size = new Vector2(1280, 720);
			Chars[i].SetCurOffset(new Vector2(Wa2Def.CharPos[i], 0));
			Chars[i].SetNextOffset(new Vector2(Wa2Def.CharPos[i], 0));
			// GD.Print(Chars[i].GetCurOffset());
			// GD.Print(Chars[i].GetNextOffset());
			// CharGroup.AddChild(Chars[i]);
		}

		// GD.Print(Time.GetTicksMsec());
		// GetTree().ChangeSceneToFile("res://scene/as/title_menu.tscn");
	}
	public void ClickAdv()
	{
		if (Skipping)
		{
			return;
		}

		if (AdvMain.State == Wa2AdvMain.AdvState.SHOW_TEXT)
		{
			AdvMain.Finish();
			return;
		}
		if (WaitTimer.IsActive())
		{
			return;
		}
		if (AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK)
		{
			WaitClick = false;
			WaitTimer.Done();
			AdvMain.Clear();
			Script.ParseCmd();
		}


	}
	public void LoadScript(string name, uint pos = 0)
	{
		WaitClick = false;
		WaitTimer.Done();
		AdvMain.Clear();
		Script.LoadScript(name, pos);

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void InputKeyHandling()
	{
		if (Input.IsActionPressed("Skip"))
		{
			Skipping = true;
		}
		else
		{
			Skipping = false;
		}
	}
	public override void _Process(double delta)
	{
		if (State == GameState.NONE)
		{
			UiMgr.OpenTitleMenu();
		}
		else if (State == GameState.GAME)
		{
			InputKeyHandling();
			WaitTimer.Update((float)delta);
			UpdateAnimators((float)delta);
			AdvMain.Update();
			SkipCheck();
			if (!WaitTimer.IsActive() && !WaitClick && !WaitAnimator())
			{
				Script.ParseCmd();
			}
		}
	}
	public void SkipCheck()
	{
		if (Skipping)
		{
			if (WaitTimer.IsActive())
			{
				WaitTimer.Done();
			}
			AdvMain.Finish();
			AnimatorsFinish();
			WaitClick = false;
		}
	}
	public void AnimatorsFinish()
	{
		for (int i = 0; i < Animators.Count; i++)
		{
			Animators[i].Finish();
			Animators.RemoveAt(i);
			i--;
		}
	}
	public void UpdateAnimators(float delta)
	{
		for (int i = 0; i < Animators.Count; i++)
		{
			Animators[i].Timer.Update(delta);
			if (Animators[i].IsActive())
			{
				Animators[i].Update();
			}
			else
			{
				Animators[i].Finish();
				Animators.RemoveAt(i);
				i--;
			}
		}
	}
	public bool WaitAnimator()
	{
		for (int i = 0; i < Animators.Count; i++)
		{
			if (Animators[i].IsActive() && Animators[i].Wait)
			{
				return true;
			}
		}
		return false;
	}
}

