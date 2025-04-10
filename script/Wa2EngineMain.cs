using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// using System.IO;

using System.Text;
using System.Threading.Tasks;
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
	public Wa2GameSav GameSav;
	// public int CurSelect=0;

	// public int[] GameFlags = new int[1024];
	public List<String> Texts = new();
	public bool SkipMode =false;
	public int ReplayMode;
	public bool AutoMode=false;
	public int WaitSeChannel;
	public static Wa2EngineMain Engine;
	public Wa2Var SelectVar;
	public int Year;
	public int Month;
	public int Day;
	public int TimeMode;
	public List<Wa2Animator> Animators { private set; get; } = new();
	public bool WaitClick = false;
	public bool WaitSe = false;
	public Wa2Prefs Prefs;
	public int Label;
	public bool Skipping = false;
	[Export]
	public VideoStreamPlayer VideoPlayer;
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
	public Wa2Timer SeWaitTimer = new();
	public float FrameTime { private set; get; } = 1.0f / 60;
	public Wa2Script Script;
	public Wa2Func Func;
	public Wa2Encoding Wa2Encoding;
	public FileAccess SysSav;
	public Wa2EngineMain()
	{
		if (Engine == null)
		{
			Engine = this;

		}
	}
	public void StopSkip()
	{
		Skipping = false;
		SkipMode = false;
	}
	public void ShowSelectMessage()
	{
		for (int i = 0; i < 3; i++)
		{
			SelectMessage btn = AdvMain.SelectMessageContainer.GetChild<SelectMessage>(i);
			if (i < GameSav.SelectItems.Count)
			{

				btn.TextLabel.Text = GameSav.SelectItems[i].Text;
				btn.TextLabel.Update();
				btn.Show();
			}
			else
			{
				btn.Hide();
			}
		}
	}
	public void UpdateChar(float time)
	{
		List<int> posList = new();
		foreach (CharItem value in GameSav.CharItems)
		{
			// GD.Print("id:", value.id, "pos:", value.pos);
			Wa2Image image = Chars[value.pos];
			Wa2Animator animator1 = new(image);
			image.SetNextTexture(Wa2Resource.GetChrImage(value.id, value.no));
			animator1.InitFade(time);
			posList.Add(value.pos);
		}
		for (int i = 0; i < Chars.Length; i++)
		{
			if (posList.Contains(i))
			{
				continue;
			}
			Wa2Image image = Chars[i];
			Wa2Animator animator2 = new(image);
			// image.SetCurTexture(image.GetNextTexture());
			image.SetNextTexture(null);
			animator2.InitFade(time);
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
	public int ReadSysFlag(int idx)
	{
		SysSav.Seek((ulong)idx * 4);
		return (int)SysSav.Get32();
	}
	public void WirtSysFlag(int idx, int value)
	{
		SysSav.Seek((ulong)idx * 4);
		SysSav.Store32((uint)value);
	}
	public override void _ExitTree()
	{
		SysSav.Close();
	}

	public override void _Ready()
	{
		GameSav = new(this);

		if (OS.GetName() == "Android")
		{
			Wa2Resource.ResPath = "/storage/emulated/0/Wa2Res/";
			// if (!OS.HasFeature("android.permission.MANAGE_EXTERNAL_STORAGE"))
			// {
			OS.RequestPermissions();
			while (!OS.GetGrantedPermissions().Contains("android.permission.MANAGE_EXTERNAL_STORAGE")) ;
			// await ToSignal(GetTree(), SceneTree.SignalName.OnRequestPermissionsResult);
			// }
		}
		else
		{
			Wa2Resource.ResPath = "res://assets/";
		}
		if (!FileAccess.FileExists("user://sys.sav"))
		{
			SysSav = FileAccess.Open("user://sys.sav", FileAccess.ModeFlags.WriteRead);
			SysSav.StoreBuffer(new byte[0x4000]);
		}
		else
		{
			SysSav = FileAccess.Open("user://sys.sav", FileAccess.ModeFlags.WriteRead);
		}
		Prefs = new Wa2Prefs();
		Func = new Wa2Func(this);
		Script = new Wa2Script(Func);
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		Wa2Encoding = new();
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
		VideoPlayer.Finished += OnVideoFinished;
		AdvMain.Init(this);
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
		if(SkipMode){
			StopSkip();
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
			// AdvMain.Clear();
			Script.ParseCmd();
			GD.Print("点击");
		}
		if (VideoPlayer.IsPlaying())
		{
			VideoPlayer.StreamPosition = VideoPlayer.GetStreamLength();
		}

	}
	public void Reset()
	{
		Script.Wait = false;
		WaitClick = false;
		WaitTimer.Done();
		AdvMain.Clear();
		WaitSeFinish();
		Skipping = false;
		// GameSav.Reset();
	}
	public void OnVideoFinished()
	{
		VideoPlayer.Stream = null;
		VideoPlayer.Hide();
	}
	public void StartScript(string name, uint pos = 0)
	{
		Reset();
		GameSav.Reset();

		Script.LoadScript(name, pos);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void InputKeyHandling()
	{
		if (UiMgr.UiQueue.Peek() != UiMgr.AdvMain)
		{
			StopSkip();
			return;
		}
		// GD.Print("pressed:",AdvMain.IsPressed);
		if(AdvMain.IsPressed){
			AdvMain.PressedTime+=GetProcessDeltaTime();
		}
		if (Input.IsActionPressed("Skip") || AdvMain.PressedTime >= 0.5)
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
			if (!WaitTimer.IsActive() && !WaitClick && !WaitAnimator() && !VideoPlayer.IsPlaying())
			{
				// AdvMain.Clear();
				WaitSeFinish();
				Script.ParseCmd();
				
			}
		}
	}
	public void WaitSeFinish()
	{
		if (WaitSe)
		{
			WaitSe = false;
			if (WaitSeChannel >= 0)
			{

				SoundMgr.StopSe(WaitSeChannel);
				WaitSeChannel = -1;
			}
		}
	}
	public void SkipCheck()
	{
		if (Skipping || SkipMode)
		{
			if (WaitTimer.IsActive())
			{
				WaitTimer.Done();
			}
			AdvMain.Finish();
			AnimatorsFinish();
			WaitSeFinish();
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
	public void LoadSav(int idx)
	{
		FileAccess file = FileAccess.Open(string.Format("user://{0:D2}.sav", idx), FileAccess.ModeFlags.Read);
		if (file == null)
		{
			return;
		}
		file.Seek(0x110a0);
		byte[] buffer = file.GetBuffer(2317 * 4);
		for (int i = 0; i < 8; i++)
		{
			int charShow = BitConverter.ToInt32(buffer, 48 + i * 4);
			if (charShow > 0)
			{
				int u1 = BitConverter.ToInt32(buffer, 100 + i * 4);
				int u2 = BitConverter.ToInt32(buffer, 72 + i * 4);
				if (u2 > 0)
				{
					int no = BitConverter.ToInt32(buffer, 64 + i * 4);
					int pos = BitConverter.ToInt32(buffer, 84 + i * 4);
					int u3 = BitConverter.ToInt32(buffer, 108 + i * 4);
					int u4 = BitConverter.ToInt32(buffer, 92 + i * 4);
					int chr = BitConverter.ToInt32(buffer, 56 + i * 4);
				}
				else
				{
					int pos = BitConverter.ToInt32(buffer, 76 + i * 4);
					int no = BitConverter.ToInt32(buffer, 64 + i * 4);
					int u3 = BitConverter.ToInt32(buffer, 108 + i * 4);
					int u4 = BitConverter.ToInt32(buffer, 92 + i * 4);
					int chr = BitConverter.ToInt32(buffer, 56 + i * 4);
				}
			}
		}
	}
	public void UpdateBg()
	{

	}
}

