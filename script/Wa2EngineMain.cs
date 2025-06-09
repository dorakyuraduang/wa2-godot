using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
public class BacklogEntry
{

	public string Name;
	public string Text;
	public int Segment;
	public List<VoiceInfo> VoiceInfos = new();
}
public partial class Wa2EngineMain : Control
{
	public enum GameState
	{
		NONE,
		LOGO,
		OP,
		TITLE,
		GAME,

	}
	[Export]
	public Control BmpContainer;
	[Export]
	public AnimatorMgr AnimatorMgr;
	// [Export]
	// public int Mode;
	// [Export]
	// public TextureRect Texture;
	// Called when the node enters the scene tree for the first time.
	public Wa2GameSav GameSav;
	[Export]
	public SubtitleMgr SubtitleMgr;
	// public int CurSelect=0;

	// public int[] GameFlags = new int[1024];
	// public int _frame;
	// public List<string> Texts = new();
	public List<BacklogEntry> Backlogs = new();
	public bool EroMode = false;
	// public int BgType = 0;
	public bool TestMode = true;
	public bool SkipMode = false;
	public bool SkipDisable = false;
	public int ReplayMode = 0;
	public bool AutoMode = false;
	public bool NovelMode = false;
	public bool ClickedInWait;
	public int CurMessageIdx;
	public static Wa2EngineMain Engine;
	public Wa2Var SelectVar;
	public double PressedTime = 0.0f;
	public bool IsPressed = false;
	public int SelectIdx = -1;
	public int ScriptIdx;
	public int Year;
	public int Month;
	public int Day;
	public int TimeMode;
	public int StartTime;
	public bool WaitSe = false;
	public Wa2Prefs Prefs;
	public int Label;
	public bool Skipping = false;
	public bool DemoMode = false;


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
	public bool HasPlayMovie = false;
	public GameState State = GameState.NONE;
	public Wa2WaitTimer WaitTimer = new();
	public Wa2Timer AutoTimer = new();
	public List<VoiceInfo> VoiceInfos = new();
	public bool HasReadMessage = false;
	public List<SelectItem> SelectItems = new();
	public Calender Calender = new();
	// public string FirstSentence;
	// public string CharName;
	public List<CharItem> CharItems = new();
	// public bool MessageHasRead = true;
	// public Wa2Timer SeWaitTimer = new();
	public float FrameTime { private set; get; } = 1.0f / 60;
	public float ScriptFrameTime { private set; get; } = 1.0f / 30;
	public Wa2Script Script;
	public Stack<Wa2Script> ScriptStack = new();
	public Wa2Func Func;
	public Wa2Encoding Wa2Encoding;
	public FileAccess SysSav;
	public Dictionary<int, Sprite2D> BmpDict = new();
	public string EffectMode = "";
	// public int TimeMode;
	// public int Label=-1;
	public int Weather;
	public BgmInfo BgmInfo = new();
	public BgInfo BgInfo = new();
	public Color FBColor = new(0.5f, 0.5f, 0.5f, 1);
	public bool IsClick;
	public int[] GameFlags = new int[0x1d];
	public double ScriptDelta = 0.0f;
	public double FrameDelta = 0.0f;


	public Wa2EngineMain()
	{
		if (Engine == null)
		{
			Engine = this;
			// Wa2Def.LoadSliceData("res://assets/fonts/cn/本体80.png",Wa2Def.FontSliceData);
			// Wa2Def.LoadSliceData("res://assets/fonts/cn/袋影80.png",Wa2Def.FontShadowSliceData);
		}
	}
	public void SetFBColor(Color color)
	{
		FBColor = color;
		RenderingServer.GlobalShaderParameterSet("fb", FBColor);
	}
	public Color GetFBColor()
	{
		return FBColor;
	}
	// public void JumpScript(string name)
	// {
	// 	GameFlags = new int[0x1d];
	// 	ScriptStack.Clear();
	// 	Script = new Wa2Script(name);
	// 	ScriptStack.Push(Script);
	// }
	public void StopSkip()
	{
		Skipping = false;
		SkipMode = false;
	}
	public void AddChar(CharItem item)
	{
		for (int i = 0; i < CharItems.Count; i++)
		{
			if (CharItems[i].id == item.id || CharItems[i].pos == item.pos)
			{
				CharItems.RemoveAt(i);
				break;
			}
		}
		CharItems.Add(item);

	}
	public void AddSeInfp(SeInfo seInfo)
	{

	}
	public void RemoveChar(int id)
	{
		for (int i = 0; i < CharItems.Count; i++)
		{
			if (CharItems[i].id == id)
			{
				CharItems.RemoveAt(i);
				return;
			}
		}
	}
	public void ShowSelectMessage()
	{
		// GD.Print("和纱本气度:", GameSav.GameFlags[5]);
		// GD.Print("和纱浮气度:", GameSav.GameFlags[6]);
		// GD.Print("雪菜好意度:", GameSav.GameFlags[7]);
		int idx = int.Parse(Script.ScriptName);
		SelectIdx = Script.Args[^1].Get() + 4 * Array.IndexOf(Wa2Def.SelectScript, idx) + 900;
		AdvMain.SelectMessageContainer.Show();
		for (int i = 0; i < 3; i++)
		{
			SelectMessage btn = AdvMain.SelectMessageContainer.GetChild<SelectMessage>(i);
			if (i < SelectItems.Count)
			{

				btn.TextLabel.SetText(SelectItems[i].Text);
				if ((ReadSysFlag(SelectIdx) & (1 << i)) > 0)
				{
					btn.ReadLabel.Show();
				}
				else
				{
					btn.ReadLabel.Hide();
				}
				if (SelectItems[i].V2 == ReadSysFlag(SelectItems[i].V1))
				{
					btn.Active();
				}
				else
				{

					btn.DeActive();
				}
				// btn.TextLabel.Update();
				btn.Show();
			}
			else
			{
				btn.Hide();
			}
		}
	}
	public void SetBgmFlag(int id)
	{
		WirtSysFlag(100 + id, 1);
	}
	public int GetBgmFlag(int id)
	{
		return ReadSysFlag(100 + id);
	}
	public void UpdateChar(float time)
	{
		List<int> posList = new();
		foreach (CharItem value in CharItems)
		{
			Wa2Image image = Chars[value.pos];
			image.Show();
			if (time > 0)
			{
				AnimatorMgr.AddCharFeadAnimation(image, Wa2Resource.GetChrImage(value.id, value.no), time);

			}
			else
			{
				image.SetCurTexture(Wa2Resource.GetChrImage(value.id, value.no));
			}
			posList.Add(value.pos);

		}
		for (int i = 0; i < Chars.Length; i++)
		{
			if (posList.Contains(i))
			{
				continue;
			}
			Wa2Image image = Chars[i];
			if (time > 0)
			{
				AnimatorMgr.AddCharFeadAnimation(image, null, time);
			}
			else
			{
				image.SetCurTexture(null);
				image.SetNextTexture(null);
				image.Hide();
			}

		}
	}
	public byte GetCgFlag(int idx)
	{
		SysSav.Seek((ulong)idx + 0x80000);
		return SysSav.Get8();
	}
	public void SetCgFlag(int idx, byte value)
	{
		SysSav.Seek((ulong)idx + 0x80000);
		SysSav.Store8(value);
	}
	public int ReadSysFlag(int idx)
	{

		SysSav.Seek((ulong)idx * 4 + 0x268480);
		return (int)SysSav.Get32();
	}
	public void WirtSysFlag(int idx, int value)
	{
		SysSav.Seek((ulong)idx * 4 + 0x268480);
		SysSav.Store32((uint)value);
	}
	public void SetReadMessage(int idx)
	{
		if (idx >= 4096)
		{
			return;
		}
		int byteIndex = idx / 8;
		if (byteIndex > 512)
		{
			return;
		}
		SysSav.Seek((ulong)(ScriptIdx * 512 + byteIndex));
		byte r = SysSav.Get8();
		int bitOffset = idx % 8;
		byte mask = (byte)(0xFF >> (7));
		r &= (byte)~(mask << (8 - bitOffset - 1));
		r |= (byte)((1 & mask) << (8 - bitOffset - 1));
		SysSav.Seek((ulong)(ScriptIdx * 512 + byteIndex));
		SysSav.Store8(r);
	}
	public bool GetReadMessage(int idx)
	{
		if (idx >= 4096)
		{
			return false;
		}
		int byteIndex = idx / 8;
		if (byteIndex > 512)
		{
			return false;
		}
		int bitOffset = idx % 8;
		byte mask = (byte)(0xFF >> 7);
		SysSav.Seek((ulong)(ScriptIdx * 512 + byteIndex));
		byte value = (byte)((SysSav.Get8() >> (8 - bitOffset - 1)) & mask);
		return value == 1;
	}
	public override void _ExitTree()
	{
		SysSav.Close();
	}
	public override void _Notification(int what)
	{
		if (what == 1007)
		{
			Back();
		}
	}
	public void Back()
	{
		if (UiMgr.UiQueue.Peek() != null)
		{
			var ui = UiMgr.UiQueue.Peek();
			if (ui is BasePage)
			{
				if (!(ui as BasePage).AnimationPlayer.IsPlaying())
				{
					SoundMgr.PlaySysSe(ResourceLoader.Load<AudioStream>("res://assets/se/SE_9213.wav"));
					(ui as BasePage).Close();
				}

			}
			else if (ui == UiMgr.AdvMain && State == GameState.GAME && !AnimatorMgr.WaitAnimation() && !VideoPlayer.IsPlaying() && AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK)
			{
				UiMgr.OpenConfirm("返回主菜单\n确认吗", "", true, () =>
				{
					UiMgr.UIConfirm.Close();
					UiMgr.OpenTitleMenu();
				});
				SoundMgr.PlaySysSe(ResourceLoader.Load<AudioStream>("res://assets/se/SE_9213.wav"));
			}
		}
	}

	public override void _Ready()
	{
		// GD.Print(FrameTime);
		GetTree().SetQuitOnGoBack(false);
		GameSav = new(this);
		if (OS.GetName() == "Android")
		{

			for (int i = 0; i < 100; i++)
			{
				if (System.IO.Directory.Exists(string.Format("/storage/emulated/{0}/Wa2Res/", i)))
				{
					Wa2Resource.ResPath = string.Format("/storage/emulated/{0}/Wa2Res/", i);
					break;
				}
			}
			// if (Wa2Resource.ResPath == "")
			// {
			// Wa2Resource.ResPath = OS.GetSystemDir(OS.SystemDir.Documents)+"Wa2Res/";
			// }
			OS.RequestPermissions();
			while (!OS.GetGrantedPermissions().Contains("android.permission.MANAGE_EXTERNAL_STORAGE") && (!OS.GetGrantedPermissions().Contains("android.permission.READ_EXTERNAL_STORAGE"))) ;
			// await ToSignal(GetTree(), SceneTree.SignalName.OnRequestPermissionsResult);
			// }
		}
		else
		{
			Wa2Resource.ResPath = "res://assets/";
		}
		Prefs = new Wa2Prefs();
		Prefs.Init(this);
		if (!FileAccess.FileExists("user://sys.sav"))
		{
			SysSav = FileAccess.Open("user://sys.sav", FileAccess.ModeFlags.Write);
			SysSav.StoreBuffer(new byte[0x26A000]);
			SysSav.Close();
		}
		SysSav = FileAccess.Open("user://sys.sav", FileAccess.ModeFlags.ReadWrite);
		SoundMgr.Init(this);
		// if (SysSav.GetLength() < 0x26A000)
		// {
		// 	SysSav.Seek(SysSav.GetLength() - 1);
		// 	SysSav.StoreBuffer(new byte[0x26A000 - SysSav.GetLength()]);
		// }


		Func = new Wa2Func(this);
		// Script = new Wa2Script(Func);
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		Wa2Encoding = new();
		Wa2Def.LoadFontMap();
		Wa2Resource.LoadPak("BGM.PAK");
		Wa2Resource.LoadPak("IC/BGM.PAK");
		Wa2Resource.LoadPak("IC/bak.pak");
		Wa2Resource.LoadPak("IC/grp.pak");
		Wa2Resource.LoadPak("IC/char.pak");
		Wa2Resource.LoadPak("IC/VOICE.PAK");
		Wa2Resource.LoadPak("IC/SE.PAK");
		Wa2Resource.LoadPak("bak.pak");
		Wa2Resource.LoadPak("ck-gal.pak");
		Wa2Resource.LoadPak("grp.pak");
		Wa2Resource.LoadPak("char.pak");
		Wa2Resource.LoadPak("VOICE.PAK");
		Wa2Resource.LoadPak("SE.PAK");
		// VideoPlayer.Finished += OnVideoFinished;
		AdvMain.Init(this);
		Chars = new Wa2Image[Wa2Def.CharPos.Length];
		for (int i = 0; i < Wa2Def.CharPos.Length; i++)
		{
			// Chars[i]=new Wa2Image();
			Chars[i] = CharGroup.GetChild(i) as Wa2Image;
			Chars[i].Size = new Vector2(1280, 720);
			Chars[i].SetCurOffset(new Vector2(-Wa2Def.CharPos[i], 0));
			Chars[i].SetNextOffset(new Vector2(-Wa2Def.CharPos[i], 0));
			Chars[i].ZIndex = -Wa2Def.CharPos[i] + 720;
			Chars[i].SetCenter(true);
			Chars[i].Hide();
			// GD.Print(Chars[i].GetCurOffset());
			// GD.Print(Chars[i].GetNextOffset());
			// CharGroup.AddChild(Chars[i]);
		}
		VideoPlayer.Finished += OnVideoFinished;
		State = GameState.LOGO;
		// GD.Print(Time.GetTicksMsec());
		// GetTree().ChangeSceneToFile("res://scene/as/title_menu.tscn");
	}
	public void ClickAdv(bool click = false)
	{
		// if(WaitTime){
		// 	return;
		// }
		if (WaitTimer.IsActive() || AdvMain.State == Wa2AdvMain.AdvState.PARSE_TEXT)
		{
			if (!ClickedInWait)
			{
				ClickedInWait = true;
			}
			if (CanSkip() || ClickedInWait)
			{
				if (VideoPlayer.IsPlaying())
				{
					if (!HasPlayMovie || !click)
					{
						ClickedInWait = false;
						return;
					}
					HideVideo();
				}
				AdvMain.Update();
				if (CanSkip())
				{
					AnimatorMgr.FinishAll();
					if (!WaitTimer.IsDone())
					{
						WaitTimer.Done();
						switch (WaitTimer.Type)
						{
							case Wa2WaitTimer.WaitType.WAIT_VOICE:
								// GD.Print("等待语音结束");
								SoundMgr.StopVoice(WaitTimer.Value);
								break;
							case Wa2WaitTimer.WaitType.WAIT_SE:
								// GD.Print("等待音效结束");
								SoundMgr.StopSe(WaitTimer.Value);
								break;
							case Wa2WaitTimer.WaitType.WAIT_TIMER:
								StartTime = (int)Time.GetTicksMsec() - WaitTimer.Value;
								break;
						}
					}
				}

				ClickedInWait = false;
			}
			return;
		}
		if (State == GameState.GAME && UiMgr.UiQueue.Peek() == UiMgr.AdvMain && !AdvMain.SelectMessageContainer.Visible && (AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK || CanSkip()))
		{
			bool WaitAnime = AnimatorMgr.WaitAnimation();
			if (WaitAnime && !CanSkip())
			{
				return;
			}
			if (CanSkip())
			{
				AnimatorMgr.FinishAll();
			}
			if (AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK)
			{
				if (!AdvMain.WaitKey)
				{
					AdvMain.State = Wa2AdvMain.AdvState.END;
				}
				else
				{
					AdvMain.State = Wa2AdvMain.AdvState.PARSE_TEXT;
				}
				if (AutoTimer.IsActive() && !AutoTimer.IsDone())
				{
					AutoTimer.DeActive();
				}
			}
			if (!AdvMain.WaitKey)
			{
				ScriptParse();
			}

		}
	}

	public void Reset(bool stop = true)
	{

		CharItems.Clear();
		SelectItems.Clear();
		VoiceInfos.Clear();
		BgmInfo = new();
		BgInfo = new();
		AdvMain.SetNovelMode(false);
		EffectMode = "";
		DemoMode = false;
		AdvMain.SetDemoMode(false);
		StartTime = (int)Time.GetTicksMsec();
		SetFBColor(new Color(0.5f, 0.5f, 0.5f, 1));
		ClickedInWait = false;
		SkipDisable = false;
		WaitTimer.DeActive();
		AdvMain.Clear();
		UpdateChar(0);
		EroMode = false;
		// CgMode = false;
		// ReplayMode = 0;
		AnimatorMgr.FinishAll(true);
		BgTexture.Reset();
		MaskTexture.Reset();
		AdvMain.WaitKey = false;
		AdvMain.State = Wa2AdvMain.AdvState.END;
		AdvMain.TextLabel.Modulate = new Color(1, 1, 1, 1);
		AdvMain.Mask.Modulate = new Color(1, 1, 1, 1);
		ScriptDelta = 0.0f;
		FrameDelta = 0.0f;
		if (stop)
		{
			StopAutoMode();
			StopSkip();
		}
		SoundMgr.StopAll();
		AdvMain.SelectMessageContainer.Hide();
	}
	public void OnVideoFinished()
	{
		HideVideo();
		if (GameState.LOGO == State)
		{
			State = GameState.OP;
		}
		else if (GameState.OP == State)
		{
			UiMgr.OpenTitleMenu();
		}
	}
	public void HideVideo()
	{
		VideoPlayer.Stream = null;
		VideoPlayer.Hide();
		WaitTimer.DeActive();
	}
	public void AddBackLog(BacklogEntry e)
	{
		// GD.Print(Backlogs.Count);
		if (Backlogs.Count > 50)
		{
			Backlogs.RemoveAt(0);
		}
		Backlogs.Add(e);
	}
	public void StartScript(string name, int pos = 0)
	{
		SoundMgr.StopBgm();
		Reset(true);
		Calender = new();
		GameFlags = new int[0x1d];
		Backlogs.Clear();
		ScriptStack.Clear();
		Script = new(name, pos);
		ScriptStack.Push(Script);
		SetScriptIdx(Script.ScriptName);
		// HasReadMessage = GetReadMessage(CurMessageIdx);
	}
	public void ScriptParse()
	{
		if (Script != null)
		{
			Script.ParseCmd();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void InputKeyHandling()
	{
		if (UiMgr.UiQueue.Peek() != UiMgr.AdvMain && UiMgr.UiQueue.Peek() != UiMgr.UICalender && State != GameState.GAME)
		{
			StopSkip();
			return;
		}
		// GD.Print("pressed:",AdvMain.IsPressed);
		if (IsPressed)
		{
			PressedTime += GetProcessDeltaTime();
		}
		if (Input.IsActionPressed("Skip") || PressedTime >= 0.5)
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
		if (State == GameState.LOGO)
		{
			if (!WaitTimer.IsActive())
			{
				PlayMovie("mv00");
			}
		}
		else if (State == GameState.OP)
		{
			if (!WaitTimer.IsActive())
			{
				if (ReadSysFlag(220) == 1)
				{
					PlayMovie("mv20");
				}
				else if (ReadSysFlag(210) == 1)
				{
					PlayMovie("mv10");
				}
				else if (ReadSysFlag(202) == 1)
				{
					PlayMovie("mv02");
				}
				else
				{
					UiMgr.OpenTitleMenu();
				}

			}
		}
		else if (State == GameState.GAME)
		{

			InputKeyHandling();
			UpdateTimer(delta);
			UpdateFrame(delta);
			CheckScript(delta);
		}
	}
	public void CheckScript(double delta)
	{
		ScriptDelta += delta;
		if (ScriptDelta >= ScriptFrameTime)
		{
			ScriptDelta -= ScriptFrameTime;

		}
		else
		{
			return;
		}
		if (AdvMain.State != Wa2AdvMain.AdvState.PARSE_TEXT && !AutoTimer.IsActive() && !WaitTimer.IsActive() && !CanSkip() && !AdvMain.SelectMessageContainer.Visible && (AdvMain.State == Wa2AdvMain.AdvState.END || AdvMain.State == Wa2AdvMain.AdvState.FADE_OUT || (DemoMode && AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK)) && UiMgr.UiQueue.Peek() == UiMgr.AdvMain)
		{

			bool flag = !AnimatorMgr.WaitAnimation();
			if (flag)
			{
				ScriptParse();
			}
		}
	}
	public bool CanSkip()
	{
		return (SkipMode || Skipping) && (HasReadMessage || (int)Prefs.GetConfig("msg_cut_optin") == 1) && !SkipDisable;
	}
	public void AutoModeStart()
	{

		float autoTime = DemoMode ? 137 * FrameTime : Prefs.GetConfig("auto_max") * FrameTime;
		if (SoundMgr.GetVoiceRemainingTime(0) > 0)
		{
			AutoTimer.Start(SoundMgr.GetVoiceRemainingTime(0) + autoTime);
		}
		else
		{
			AutoTimer.Start(autoTime);
		}
	}
	public void UpdateFrame(double delta)
	{
		FrameDelta += delta;
		if (CanSkip())
		{
			ClickAdv();
		}
		if (FrameDelta >= FrameTime)
		{
			FrameDelta -= FrameTime;
		}
		else
		{
			return;
		}
		AdvMain.Update();
		if (AdvMain.State == Wa2AdvMain.AdvState.WAIT_CLICK && !AutoTimer.IsActive())
		{
			if (AutoMode || DemoMode)
			{
				AutoModeStart();
			}
		}
	}
	public void UpdateTimer(double delta)
	{
		// GD.Print(EndTime - StartTime);

		if (WaitTimer.IsActive())
		{
			if (!WaitTimer.IsDone())
			{
				WaitTimer.Update((float)delta);
			}
			if (WaitTimer.IsDone())
			{
				WaitTimer.DeActive();
				ScriptParse();
			}
		}

		if (AutoTimer.IsActive() && UiMgr.UiQueue.Peek() == UiMgr.AdvMain && AdvMain.Visible)
		{
			if (!AutoTimer.IsDone() && (AutoMode || DemoMode))
			{
				AutoTimer.Update((float)delta);
			}
			else
			{
				AutoTimer.DeActive();
				if (AutoMode || DemoMode)
				{
					ClickAdv();
				}
			}
		}
		// UpdateAnimators((float)delta);
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Keycode == Key.Escape && keyEvent.Pressed && !keyEvent.IsEcho())
			{
				Back();
			}
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		switch (State)
		{
			case GameState.TITLE:
				break;
			case GameState.LOGO:
			case GameState.OP:
				if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
				{
					if (VideoPlayer.IsPlaying() && VideoPlayer.StreamPosition > 0)
					{
						HideVideo();
						// WaitTimer.DeActive();
						if (GameState.LOGO == State)
						{
							State = GameState.OP;
						}
						else if (GameState.OP == State)
						{
							UiMgr.OpenTitleMenu();
						}
					}
				}
				break;
			case GameState.GAME:
				if (@event is InputEventScreenTouch && @event.IsPressed())
				{
					IsPressed = true;
				}
				else
				{
					IsPressed = false;
				}
				if (!IsPressed)
				{
					PressedTime = 0.0;
				}
				if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
				{
					bool flag = true;
					IsClick = true;
					if (SkipMode && AdvMain.Visible)
					{
						StopSkip();
						flag = false;
					}
					if (!AdvMain.Visible && !VideoPlayer.IsPlaying() && UiMgr.UiQueue.Peek() == UiMgr.AdvMain && AdvMain.State == Wa2AdvMain.AdvState.HIDE)
					{
						AdvMain.Show();
						AdvMain.State = Wa2AdvMain.AdvState.WAIT_CLICK;
						flag = false;
					}
					if (flag)
					{
						ClickAdv(true);
					}
				}
				else
				{
					IsClick = false;
				}
				break;
		}
	}
	public void StopAutoMode()
	{

		AutoMode = false;
		AutoTimer.DeActive();

	}
	public void PlayMovie(string name)
	{
		if (!FileAccess.FileExists(Wa2Resource.ResPath + "movie/" + name + "0.mp4"))
		{
			OnVideoFinished();
		}
		else
		{
			VideoPlayer.Call("set_movie", Wa2Resource.ResPath + "movie/" + name + "0.mp4");
			WaitTimer.Start((float)VideoPlayer.GetStreamLength());
			VideoPlayer.Play();
			VideoPlayer.Show();
		}

	}
	// public void Load
	public void RenderImage(int id, int efc, bool updateChar, int type, int frame, int offset, int x, int y, float scaleX, float ScaleY)
	{
		// BgType = type;
		BgInfo.Type = type;
		AnimatorMgr.FinishAll(true);
		Texture2D NextTexture;
		Texture2D CeacheTexture = ImageTexture.CreateFromImage(Viewport.GetTexture().GetImage());
		Wa2Image targetTexture;
		if (updateChar)
		{
			targetTexture = BgTexture;
		}
		else
		{
			targetTexture = MaskTexture;
		}
		if (id >= 0)
		{
			if (type == 1)
			{
				BgInfo.Path = string.Format("v{0:D6}.tga", id, TimeMode);
				SetCgFlag(id, 1);
			}
			else if (type == 2)
			{
				BgInfo.Path = string.Format("h{0:D6}.tga", id, TimeMode);
			}
			else
			{
				BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", id / 10, id % 10, TimeMode);
			}
			NextTexture = Wa2Resource.GetTgaImage(BgInfo.Path);
		}
		else
		{
			NextTexture = BgTexture.GetCurTexture();

		}
		if (efc >= 128)
		{
			targetTexture.SetMaskTexture(Wa2Resource.GetMaskImage(efc & 0x7f));
		}
		else
		{
			targetTexture.SetMaskTexture(null);
		}
		BgInfo.Offset = new Vector2(x - offset, y);
		BgInfo.Scale = new Vector2(scaleX, ScaleY);
		targetTexture.SetNextTexture(NextTexture);

		if (!updateChar)
		{
			MaskTexture.SetCurTexture(CeacheTexture);
			MaskTexture.SetCurOffset(Vector2.Zero);
			MaskTexture.SetCurScale(Vector2.One);
			MaskTexture.SetNextOffset(BgInfo.Offset);
			MaskTexture.SetNextScale(BgInfo.Scale);
			BgTexture.SetCurOffset(BgInfo.Offset);
			BgTexture.SetCurScale(BgInfo.Scale);
			BgTexture.SetCurTexture(NextTexture);
			AnimatorMgr.AddMaskFeadAnimation(MaskTexture, frame * FrameTime, true);
		}
		else
		{
			AnimatorMgr.AddBgFeadAnimation(BgTexture, frame * FrameTime, BgInfo.Offset, BgInfo.Scale);
		}
		if (updateChar)
		{
			UpdateChar(frame * FrameTime);
		}
		else
		{
			ClearChar(frame * FrameTime);
		}

	}
	public bool ClearChar(float time)
	{
		for (int i = 0; i < Chars.Length; i++)
		{
			if (Chars[i].GetCurTexture() == null)
			{
				continue;
			}
			AnimatorMgr.AddCharFeadAnimation(Chars[i], null, time);
		}
		CharItems.Clear();
		return false;
	}
	public void SetScriptIdx(string name)
	{
		int idx = Array.IndexOf(Wa2Def.ScriptList, name.ToLower());
		if (idx >= 0)
		{
			if (idx != ScriptIdx)
			{
				if (idx == 29 || idx == 30 || idx == 31)
				{
					SetReadMessage(CurMessageIdx + 1);
				}
				else
				{
					CurMessageIdx = 0;
					ScriptIdx = idx;
				}
			}
			HasReadMessage = GetReadMessage(CurMessageIdx);
		}
	}
}

