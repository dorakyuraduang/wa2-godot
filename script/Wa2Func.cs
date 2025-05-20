using FFmpeg.AutoGen;
using Godot;
using System;
using System.Collections.Generic;
public class Wa2Func
{
	private Wa2EngineMain _engine;
	public int idx = 0;
	public Dictionary<uint, Func<List<Wa2Var>, bool>> FuncDic;
	public Wa2Func(Wa2EngineMain e)
	{
		_engine = e;
		FuncDic = new(){
			{0x0,SLoad},
			{0x1,SCall},
			{0x2,call},
			{0x3,run},
			{0x4,print},
			{0x5,Ret},
			{0x6,_int},
			{0x7,_float},
			{0x8,Rand},
			{0x9,Sin},
			{0xa,Cos},
			{0xb,Tan},
			{0xc,Asin},
			{0xd,Acos},
			{0xe,Atan},
			{0xf,Atan2},
			{0x10,Pow},
			{0x11,Sqrt},
			{0x12,TimeGetTime},
			{0x80,printEx},
			{0x81,printEx2},
			{0x82,SetMessage},
			{0x83,SetMessageE},
			{0x84,EndMessage},
			{0x85,SetMessage2},
			{0x86,WaitMessage2},
			{0x87,K},
			{0x88,SetDemoMode},
			{0x89,VI},
			{0x8a,VV},
			{0x8b,VX},
			{0x8c,VW},
			{0x8d,VS},
			{0x8e,W},
			{0x8f,WR},
			{0x90,WN},
			{0x91,WNS},
			{0x92,B},
			{0x93,BC},
			{0x94,V},
			{0x95,H},
			{0x96,SetShake},
			{0x97,StopShake},
			{0x98,F},
			{0x99,FB},
			{0x9a,C},
			{0x9b,CW},
			{0x9c,CR},
			{0x9d,CRW},
			{0x9e,M},
			{0x9f,MS},
			{0xa0,MP},
			{0xa1,MV},
			{0xa2,MW},
			{0xa3,MLW},
			{0xa4,SE},
			{0xa5,SEP},
			{0xa6,SES},
			{0xa7,SEV},
			{0xa8,SEW},
			{0xa9,SEVW},
			{0xaa,SetTimeMode},
			{0xab,SetChromaMode},
			{0xac,SetEffctMode},
			{0xad,SetWeather},
			{0xae,ChangeWeather},
			{0xaf,ResetWeather},
			{0xb0,LoadBmp},
			{0xb1,LoadBmpAnime},
			{0xb2,SetBmpAvi},
			{0xb3,WaitBmpAvi},
			{0xb4,ReleaseBmp},
			{0xb5,WaitBmpAnime},
			{0xb6,SetBmpAnimePlay},
			{0xb7,SetBmpDisp},
			{0xb8,SetBmpLayer},
			{0xb9,SetBmpParam},
			{0xba,SetBmpRevParam},
			{0xbb,SetBmpBright},
			{0xbc,SetBmpMove},
			{0xbd,SetBmpPos},
			{0xbe,SetBmpZoom},
			{0xbf,SetBmpZoom2},
			{0xc0,SetBmpRoll},
			{0xc1,SetMovie},
			{0xc2,Wait},
			{0xc3,StartTimer},
			{0xc4,WaitTimer},
			{0xc5,GoTitle},
			{0xc6,GetGameFlag},
			{0xc7,SetGameFlag},
			{0xc8,LogOut},
			{0xc9,V_Flag},
			{0xca,H_Flag},
			{0xcb,Calender},
			{0xcc,GetTimer},
			{0xcd,GetSkip},
			{0xce,GetClick},
			{0xcf,runEX},
			{0xd0,SetSelectMess},
			{0xd1,SetSelect},
			{0xd2,S},
			{0xd3,Z},
			{0xd4,R},
			{0xd5,WSZ},
			{0xd6,StopSZR},
			{0xd7,VA},
			{0xd8,CS},
			{0xd9,CM},
			{0xda,CRS},
			{0xdb,SkipOFF},
			{0xdc,NovelMode},
			{0xdd,EroMode},
			{0xde,GetReplayMode},
			{0xdf,WN2},
			{0xe0,WNS2},
			{0xe1,B2},
			{0xe2,BC2},
			{0xe3,V2},
			{0xe4,H2},
			{0xe5,SetWeather2},
			{0xe6,ChangeWeather2},
			{0xe7,SetShake2},
			{0xe8,M2},
			{0xe9,NB},
			{0xea,NBR},
			{0xeb,VXV},
			{0xec,Wait2},
			{0xed,IfSkip},
			{0xee,GetSkip2},
	};

	}

	// public void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	// {
	// 	GD.Print("播放bgm");
	// 	Wa2SoundMgr.Instance.PlayBgm(id, loopFlag, volume);
	// }
	public void StopBgm(int frame = 0)
	{
		Wa2SoundMgr.Instance.StopBgm(frame * _engine.FrameTime);
	}
	public void PlaySysSe(AudioStream stream)
	{
		Wa2SoundMgr.Instance.PlaySysSe(stream);
	}

	public bool printEx(List<Wa2Var> args)
	{
		return true;
	}
	public bool printEx2(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetMessage(List<Wa2Var> args)
	{
		SetMessageEx(args[0].Get(), args[1].Get(), args[2].Get(), 1);
		return false;

	}
	public bool SetMessageE(List<Wa2Var> args)
	{
		SetMessageEx(args[0].Get(), args[1].Get(), args[2].Get(), 2);
		return false;
	}
	public void SetMessageEx(string text, int idx, int v3, int v4)
	{
		_engine.AdvMain.WaitKey = false;
		if (v3 == 0)
		{
			_engine.AdvMain.TextLabel.Text += "\\k" + text;
		}
		else
		{
			// _engine.AdvMain.ClearText();
			_engine.AdvMain.TextLabel.Clear();
			_engine.AdvMain.NameLabel.Clear();
			_engine.AdvMain.TextLabel.Segment = 0;
			_engine.AdvMain.TextLabel.Text = text;
		}

		_engine.AdvMain.TextProgress = 0;
		_engine.AdvMain.ParseMode = v4;
		_engine.CurMessageIdx = idx;
		_engine.AdvMain.ShowText();


	}
	public bool EndMessage(List<Wa2Var> args)
	{
		// _engine.AdvMain.TextLabel.Segment = 0;
		_engine.AddhBackLog(new BacklogEntry()
		{
			Name = _engine.AdvMain.NameLabel.Text,
			Text = _engine.AdvMain.TextLabel.Text,
			VoiceInfos = [.. _engine.VoiceInfos]
		});
		_engine.VoiceInfos.Clear();
		return true;
	}
	public bool SetMessage2(List<Wa2Var> args)
	{
		SetMessageEx(args[0].Get(), args[1].Get(), args[2].Get(), 2);
		_engine.SkipDisable = true;
		return false;
	}
	public bool WaitMessage2(List<Wa2Var> args)
	{
		_engine.SkipDisable = false;
		// _engine.AdvMain.TextLabel.Segment = 0;
		return true;

	}
	public bool K(List<Wa2Var> args)
	{
		return false;
	}
	public bool SetDemoMode(List<Wa2Var> args)
	{
		_engine.DemoMode = args[0].Get() > 0;
		_engine.AdvMain.SetDemoMode(_engine.DemoMode);
		if (_engine.DemoMode)
		{
			_engine.StopAutoMode();
		}
		return true;
	}
	public bool VI(List<Wa2Var> args)
	{
		// if (args[0].Get() == 0)
		// {
		if (args[0].Get() is int && args[0].Get() == 0)
		{
			_engine.Label = args[1].Get();
		}
		return true;
		// }
		// int v0 = args[0];
		// int v1 = args[1];
		// _engine.Label = v1;

	}
	public bool VV(List<Wa2Var> args)
	{
		//2循环
		//3通道？
		// GD.Print("vv轨道", args[3].Get());

		_engine.SoundMgr.PlayVoice(_engine.Label, args[4].Get(), args[0].Get(), args[1].Get(), args[2].Get() == 1, args[3].Get());

		return true;
	}
	public bool VX(List<Wa2Var> args)
	{
		//5通道？
		//2标签
		//4循环
		// GD.Print("轨道:", args[5].Get());

		_engine.SoundMgr.PlayVoice(args[2].Get(), args[1].Get(), args[0].Get(), args[3].Get(), args[4].Get() == 1, args[5].Get());
		return true;
	}
	public bool VW(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(_engine.SoundMgr.GetVoiceTime(), Wa2WaitTimer.WaitType.WAIT_VOICE, args[1].Get());

		// _engine.WaitClick = true;
		//1音轨
		return false;
		// _engine.AdvMain.CurName="";
	}
	public bool VS(List<Wa2Var> args)
	{
		//0时间
		//1音轨
		// GD.Print("停止音轨", args[1].Get());
		_engine.SoundMgr.StopVoice(args[1].Get(), args[0].Get() * _engine.FrameTime);
		return true;
	}
	public bool W(List<Wa2Var> args)
	{
		return false;
	}
	public bool WR(List<Wa2Var> args)
	{
		_engine.AdvMain.AdvHide(0.2f);
		return false;
	}
	public bool WN(List<Wa2Var> args)
	{
		if (args[0].CmdType == CmdType.STR_VAR)
		{
			_engine.AdvMain.NameLabel.Text = args[0].Get();
		}
		else
		{
			_engine.AdvMain.NameLabel.Text = "";
		}
		return true;
	}
	public bool WNS(List<Wa2Var> args)
	{
		return false;
	}

	public bool B(List<Wa2Var> args)
	{

		// _engine.AnimatorMgr.FinishAll(true);
		// Texture2D NextTexture;
		// Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		// if (args[1].Get() >= 0)
		// {
		// 	_engine.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.TimeMode);
		// 	NextTexture = Wa2Resource.GetTgaImage(_engine.BgInfo.Path);
		// }
		// else
		// {
		// 	NextTexture = _engine.BgTexture.GetCurTexture();
		// }
		// if (args[0].Get() >= 128)
		// {
		// 	_engine.MaskTexture.SetMaskTexture(Wa2Resource.GetMaskImage(args[0].Get() & 0x7f));
		// }
		// else
		// {
		// 	_engine.MaskTexture.SetMaskTexture(null);
		// }
		// _engine.BgTexture.SetCurOffset(new Vector2(0, 0));
		// _engine.BgTexture.SetCurScale(new Vector2(1, 1));
		// _engine.BgInfo.Scale = Vector2.One;
		// _engine.BgInfo.Offset = Vector2.Zero;
		// _engine.ClearChar(args[3].Get() * _engine.FrameTime);
		// _engine.BgTexture.SetCurTexture(NextTexture);
		// if (args[3].Get() == 0)
		// {
		// 	return true;
		// }
		// _engine.MaskTexture.SetCurOffset(Vector2.Zero);
		// _engine.MaskTexture.SetCurScale(Vector2.One);
		// _engine.MaskTexture.SetCurTexture(CeacheTexture);
		// _engine.MaskTexture.SetNextScale(new Vector2(1, 1));
		// _engine.MaskTexture.SetNextOffset(new Vector2(0, 0));
		// _engine.MaskTexture.SetNextTexture(NextTexture);
		// _engine.AnimatorMgr.AddMaskFeadAnimation(_engine.MaskTexture, args[3].Get() * _engine.FrameTime);
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), false, 0, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get() / 1280f, args[8].Get() / 720f);
		return false;
	}
	public bool BC(List<Wa2Var> args)
	{
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), true, 0, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get() / 1280f, args[8].Get() / 720f);
		return false;
		// _engine.AnimatorMgr.FinishAll(true);
		// Texture2D NextTexture;
		// Texture2D CeacheTexture = _engine.BgTexture.GetCurTexture();
		// if (args[1].Get() >= 0)
		// {
		// 	_engine.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.TimeMode);
		// 	NextTexture = Wa2Resource.GetTgaImage(_engine.BgInfo.Path);
		// }
		// else
		// {
		// 	NextTexture = CeacheTexture;
		// }
		// _engine.BgTexture.SetCurTexture(CeacheTexture);
		// _engine.BgTexture.SetNextTexture(NextTexture);
		// _engine.BgTexture.SetMaskTexture(null);
		// // Wa2ImageAnimator animator3 = new(_engine.BgTexture);
		// // animator3.InitFade(_engine.BgInfo.Frame * _engine.FrameTime);
		// _engine.AnimatorMgr.AddMaskFeadAnimation(_engine.BgTexture, args[3].Get() * _engine.FrameTime, false);
		// _engine.UpdateChar(args[3].Get() * _engine.FrameTime);

		// GD.Print("更新背景和角色");

	}
	public bool V(List<Wa2Var> args)
	{
		// Texture2D NextTexture;
		// _engine.AnimatorMgr.FinishAll(true);
		// if (args[1].Get() >= 0)
		// {
		// 	_engine.BgInfo.Path = string.Format("v{0:D5}{1:D1}.tga", args[1].Get(), args[2].Get());
		// 	_engine.SetCgFlag(args[1].Get() * 10 + args[2].Get(), 1);
		// 	NextTexture = Wa2Resource.GetTgaImage(_engine.BgInfo.Path);
		// }
		// else
		// {
		// 	NextTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		// }
		// _engine.MaskTexture.SetCurOffset(_engine.BgTexture.GetCurOffset());
		// _engine.MaskTexture.SetCurScale(_engine.BgTexture.GetCurScale());
		// _engine.MaskTexture.SetCurTexture(ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()));
		// _engine.MaskTexture.SetNextTexture(NextTexture);
		// _engine.MaskTexture.SetMaskTexture(null);
		// _engine.AnimatorMgr.AddMaskFeadAnimation(_engine.MaskTexture, args[3].Get() * _engine.FrameTime);
		// _engine.ClearChar(args[3].Get() * _engine.FrameTime);
		// _engine.BgTexture.SetCurTexture(NextTexture);
		// return false;
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), false, 1, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get() / 1280f, args[8].Get() / 720f);
		return false;
	}
	public bool H(List<Wa2Var> args)
	{
		return false;
	}
	public bool SetShake(List<Wa2Var> args)
	{
		return false;
	}
	public bool StopShake(List<Wa2Var> args)
	{
		return false;
	}
	public bool F(List<Wa2Var> args)
	{
		float r = (int)args[2].Get() / 255f;
		float g = (int)args[3].Get() / 255f;
		float b = (int)args[4].Get() / 255f;
		if (args[1].Get() > 0)
		{
			_engine.AnimatorMgr.AddFAnimation(new Color(r, g, b), args[1].Get() * _engine.FrameTime);
		}
		else
		{
			_engine.SetFBColor(new Color(r, g, b));
		}
		return false;
	}
	public bool FB(List<Wa2Var> args)
	{
		float r = (int)args[1].Get() / 255f;
		float g = (int)args[2].Get() / 255f;
		float b = (int)args[3].Get() / 255f;

		if (args[0].Get() > 0)
		{
			_engine.AnimatorMgr.AddFBAnimation(new Color(r, g, b), args[0].Get() * _engine.FrameTime);
		}
		else
		{
			_engine.SetFBColor(new Color(r, g, b));
		}
		return false;
	}

	public bool C(List<Wa2Var> args)
	{

		_engine.AddChar(new CharItem
		{
			id = args[0].Get(),
			no = args[1].Get(),
			pos = args[2].Get(),
		});
		_engine.UpdateChar(args[5].Get() * _engine.FrameTime);
		return false;

	}
	public bool CW(List<Wa2Var> args)
	{
		_engine.AddChar(new CharItem
		{
			id = args[0].Get(),
			no = args[1].Get(),
			pos = args[2].Get(),
		});
		return true;
	}
	public bool CR(List<Wa2Var> args)
	{
		_engine.RemoveChar(args[0].Get());
		_engine.UpdateChar(args[2].Get() * _engine.FrameTime);
		return false;
	}
	public bool CRW(List<Wa2Var> args)
	{
		_engine.RemoveChar(args[0].Get());
		return true;
	}
	public bool M(List<Wa2Var> args)
	{
		if (args.Count == 3)
		{
			GD.Print("错误位置", _engine.Script.ScriptPos);
		}
		_engine.BgmInfo.Id = args[0].Get();
		_engine.BgmInfo.Loop = args[2].Get();
		_engine.BgmInfo.Volume = args[3].Get();
		GD.Print("播放bgm");
		// GD.Print( args[1].Get());
		_engine.SoundMgr.PlayBgm(args[0].Get(), args[2].Get() != 0, args[3].Get());
		return true;
	}
	public bool MS(List<Wa2Var> args)
	{
		// GD.Print("暂停:", args[0] * _engine.FrameTime);
		_engine.SoundMgr.StopBgm(args[0].Get() * _engine.FrameTime);
		return true;
		// uint v0 = args[0];
	}
	public bool MP(List<Wa2Var> args)
	{
		GD.Print("函数:Mp");
		return false;
	}
	public bool MV(List<Wa2Var> args)
	{
		GD.Print("估计是改变语言音量2个参数");
		return true;
	}
	public bool MW(List<Wa2Var> args)
	{
		GD.Print("函数:mw");
		return false;
	}
	public bool MLW(List<Wa2Var> args)
	{
		GD.Print("函数:mlw");
		return false;
	}
	public bool SE(List<Wa2Var> args)
	{
		Wa2SoundMgr.Instance.PlaySe(0, args[0].Get(), false, 0, args[1].Get());
		return true;
	}
	public bool SEP(List<Wa2Var> args)
	{
		// GD.Print(args.Count);
		GD.Print("循环播放:", args[3].Get());
		GD.Print("id:", args[1].Get());
		Wa2SoundMgr.Instance.PlaySe(args[0].Get(), args[1].Get(), args[3].Get() != 0, args[2].Get() * _engine.FrameTime, args[4].Get());
		args.Clear();
		return true;
	}
	public bool SES(List<Wa2Var> args)
	{
		if (args[0].Get() > 10)
		{
			GD.Print("错误");
		}
		Wa2SoundMgr.Instance.StopSe(args[0].Get(), args[1].Get() * _engine.FrameTime);
		return true;
	}
	public bool SEV(List<Wa2Var> args)
	{
		_engine.SoundMgr.SetSeVolume(args[0].Get(), args[1].Get(), args[2].Get() * _engine.FrameTime);
		return true;
	}
	public bool SEW(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(_engine.SoundMgr.GetSeTime(args[0].Get()), Wa2WaitTimer.WaitType.WAIT_SE, args[0].Get());
		return false;
	}
	public bool SEVW(List<Wa2Var> args)
	{
		GD.Print("函数:sevm");
		return false;
	}
	public bool SetTimeMode(List<Wa2Var> args)
	{
		_engine.TimeMode = args[0].Get();
		GD.Print("mode", args[0].Get());
		return true;
		// uint v0 = args[0];
	}
	public bool SetChromaMode(List<Wa2Var> args)
	{
		GD.Print("设置色差");
		return true;
	}
	public bool SetEffctMode(List<Wa2Var> args)
	{
		_engine.EffectMode = args[0].Get();
		return true;
	}
	public bool SetWeather(List<Wa2Var> args)
	{
		GD.Print("设置天气");
		return true;
	}
	public bool ChangeWeather(List<Wa2Var> args)
	{
		GD.Print("改变天气");
		return true;
	}
	public bool ResetWeather(List<Wa2Var> args)
	{
		return true;
	}
	public bool LoadBmp(List<Wa2Var> args)
	{
		Sprite2D texture = new();
		// // AtlasTexture a=new();
		// a.Atlas= 
		if ((args[1].Get() as string).EndsWith(".tga"))
		{
			texture.Texture = Wa2Resource.LoadTgaImage(args[1].Get());
		}
		else
		{
			texture.Texture = Wa2Resource.LoadBmpImage(args[1].Get());
		}
		ShaderMaterial material = new();
		texture.Material = material;
		texture.Centered = false;
		texture.ZIndex = args[2].Get();
		_engine.BmpDict[args[0].Get()] = texture;
		_engine.BmpContainer.CallDeferred("add_child", texture);
		for (int i = 0; i < args.Count; i++)
		{
			GD.Print("loadbmp" + i + ":", args[i].Get());
		}
		return true;
	}
	public bool LoadBmpAnime(List<Wa2Var> args)
	{
		for (int i = 0; i < args.Count; i++)
		{
			GD.Print("bmpani" + i + ":", args[i].Get());
		}
		GD.Print("加载bmp动画");
		return true;
	}
	public bool SetBmpAvi(List<Wa2Var> args)
	{
		return true;
	}
	public bool WaitBmpAvi(List<Wa2Var> args)
	{
		return false;
	}
	public bool ReleaseBmp(List<Wa2Var> args)
	{
		if (_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			_engine.BmpDict[args[0].Get()].Hide();
			_engine.BmpDict[args[0].Get()].QueueFree();
			_engine.BmpDict.Remove(args[0].Get());
		}

		return true;
	}
	public bool WaitBmpAnime(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			return false;
		}
		return false;
	}
	public bool SetBmpAnimePlay(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{

		}
		return true;
	}
	public bool SetBmpDisp(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{

		}
		return true;
	}
	public bool SetBmpLayer(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{

		}
		return true;

	}
	public bool SetBmpParam(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			return true;
		}
		Sprite2D tex = _engine.BmpDict[args[0].Get()];
		int mode = args[1].Get();
		switch (mode)
		{
			case 1:
				break;
			case 3:
				(tex.Material as ShaderMaterial).Shader = ResourceLoader.Load<Shader>("res://shader/bmp_add.gdshader");
				break;
			case 4:
				(tex.Material as ShaderMaterial).Shader = ResourceLoader.Load<Shader>("res://shader/add.gdshader");
				break;
		}

		float d = args[3].Get() * _engine.FrameTime;
		float a = args[2].Get() / 255f;
		if (args[3].Get() > 0)
		{
			_engine.AnimatorMgr.AddFeadAnimation(tex, d, a);
		}
		else
		{
			(_engine.BmpDict[args[0].Get()] as Sprite2D).Modulate = new Color(1, 1, 1, a);
		}
		return true;
	}
	public bool SetBmpRevParam(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetBmpBright(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetBmpMove(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			return true;
		}
		Sprite2D tex = _engine.BmpDict[args[0].Get()];
		tex.Position = new Vector2(args[1].Get(), args[2].Get()) / tex.Scale;
		return true;

		// GD.Print(tex.Position);
	}
	public bool SetBmpPos(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetBmpZoom(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			return true;
		}
		Sprite2D tex = _engine.BmpDict[args[0].Get()];
		tex.Centered = false;
		tex.Scale = new Vector2(args[3].Get() / tex.Texture.GetSize().X, args[4].Get() / tex.Texture.GetSize().Y);
		tex.Position = new Vector2(args[1].Get(), args[2].Get());
		return true;
	}
	public bool SetBmpZoom2(List<Wa2Var> args)
	{
		if (!_engine.BmpDict.ContainsKey(args[0].Get()))
		{
			return true;
		}
		Sprite2D tex = _engine.BmpDict[args[0].Get()];
		tex.Offset = -new Vector2(args[1].Get(), args[2].Get()) / 2;
		tex.Scale = new Vector2(tex.Texture.GetSize().X / args[1].Get(), tex.Texture.GetSize().Y / args[2].Get());
		return true;
	}
	public bool SetBmpRoll(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetMovie(List<Wa2Var> args)
	{
		_engine.StopSkip();
		_engine.SoundMgr.StopAll();
		if (_engine.ReadSysFlag(args[1].Get()) == 1)
		{
			_engine.HasPlayMovie = true;
		}
		else
		{
			_engine.HasPlayMovie = false;
			_engine.WirtSysFlag(args[1].Get(), 1);
		}

		_engine.PlayMovie(args[0].Get());
		return false;
	}
	public bool Wait(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(args[0].Get() * _engine.FrameTime);
		return false;

	}
	public bool StartTimer(List<Wa2Var> args)
	{
		_engine.StartTime = (int)Time.GetTicksMsec();
		return true;
	}
	public bool WaitTimer(List<Wa2Var> args)
	{

		// _engine.EndTime = _engine.StartTime + (int)args[0].Get();
		// if (_engine.Skipping && _engine.SkipMode)
		// {
		// 	_engine.StartTime = _engine.EndTime;
		// }
		if (Time.GetTicksMsec() < (ulong)(_engine.StartTime + args[0].Get()))
		{
			_engine.WaitTimer.Start((_engine.StartTime + args[0].Get() - (int)Time.GetTicksMsec()) * 0.001f, Wa2WaitTimer.WaitType.WAIT_TIMER, (int)args[0].Get());
		}
		args.Clear();
		return false;
	}
	public bool GoTitle(List<Wa2Var> args)
	{
		_engine.UiMgr.OpenTitleMenu();
		return true;
	}
	public bool GetGameFlag(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.ReadSysFlag(args[0].Get()));
		return true;
	}
	public bool SetGameFlag(List<Wa2Var> args)
	{
		_engine.WirtSysFlag(args[0].Get(), args[1].Get());
		return true;
	}
	public bool LogOut(List<Wa2Var> args)
	{
		return false;
	}
	public bool V_Flag(List<Wa2Var> args)
	{
		_engine.SetCgFlag(args[0].Get() * 10 + args[1].Get(), (byte)args[2].Get());
		return true;
	}
	public bool H_Flag(List<Wa2Var> args)
	{
		GD.Print("事件解锁");
		return true;
	}
	public bool Calender(List<Wa2Var> args)
	{
		_engine.Calender.Year = args[0].Get();
		_engine.Calender.Month = args[1].Get();
		_engine.Calender.Day = args[2].Get();
		if (_engine.Calender.Year == 2013 && _engine.Calender.Month == 2 && _engine.Calender.Day == 29)
		{
			_engine.Calender.DayOfWeek = 5;
		}
		else
		{
			_engine.Calender.DayOfWeek = (int)new DateTime(args[0].Get(), args[1].Get(), args[2].Get()).DayOfWeek;
		}
		_engine.UiMgr.OpenUICalender();
		// uint hour=args[3];
		// GD.Print("设置日期" + year + "年" + month + "月" + day + "日");
		return false;
	}
	public bool GetTimer(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, (int)Time.GetTicksMsec() - _engine.StartTime);
		return true;
	}
	public bool GetSkip(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.CanSkip() ? 1 : 0);
		return true;
	}
	public bool GetClick(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.IsClick ? 1 : 0);
		return true;
	}
	public bool runEX(List<Wa2Var> args)
	{
		return false;
	}
	public bool SetSelectMess(List<Wa2Var> args)
	{
		//text
		//alpha
		//disable
		_engine.SelectItems.Add(new SelectItem
		{
			Text = args[0].Get(),
			V1 = args[1].Get(),
			V2 = args[2].Get(),
			V3 = args[3].Get(),
		});
		return true;
	}
	public bool SetSelect(List<Wa2Var> args)
	{
		_engine.ShowSelectMessage();
		return false;

		// _engine.SelectVar = args[0];
		// _engine.Script.Wait = true;
	}
	public bool S(List<Wa2Var> args)
	{
		_engine.AnimatorMgr.AddBgMoveAnimation(_engine.BgTexture, args[2].Get() * _engine.FrameTime, args[0].Get(), args[1].Get());
		// Wa2ImageAnimator animator = new(_engine.BgTexture);
		// animator.Wait = false;
		// animator.InitMove(args[2].Get() * _engine.FrameTime, args[0].Get(), args[1].Get());
		return true;
	}
	public bool Z(List<Wa2Var> args6)
	{
		return false;
	}
	public bool R(List<Wa2Var> args)
	{
		return false;
	}
	public bool WSZ(List<Wa2Var> args)
	{
		_engine.AnimatorMgr.AllWait();
		return false;
	}
	public bool StopSZR(List<Wa2Var> args)
	{
		GD.Print("stop_szr");
		return false;
	}
	public bool VA(List<Wa2Var> args)
	{
		GD.Print("va");
		return false;
	}
	public bool CS(List<Wa2Var> args)
	{
		GD.Print("cs");
		return false;
	}
	public bool CM(List<Wa2Var> args)
	{
		GD.Print("cm");
		return false;
	}
	public bool CRS(List<Wa2Var> args)
	{
		GD.Print("crs");
		return false;
	}
	public bool SkipOFF(List<Wa2Var> args)
	{
		_engine.StopSkip();
		return false;
	}
	public bool NovelMode(List<Wa2Var> args)
	{
		_engine.AdvMain.SetNovelMode(args[0].Get() != 0);
		return false;
	}
	public bool EroMode(List<Wa2Var> args)
	{
		_engine.EroMode = true;
		return true;
	}
	public bool GetReplayMode(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.ReplayMode ? 1 : 0);
		return true;
	}
	public bool WN2(List<Wa2Var> args)
	{
		if (args[0].CmdType == CmdType.STR_VAR)
		{
			_engine.AdvMain.NameLabel.Text = args[0].Get();
		}
		else
		{
			_engine.AdvMain.NameLabel.Text = "";
		}
		return true;

	}
	public bool WNS2(List<Wa2Var> args)
	{
		GD.Print("wns2");
		return false;
	}
	public bool B2(List<Wa2Var> args)
	{
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), false, 0, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get(), args[8].Get());
		return false;
		// _engine.AnimatorMgr.FinishAll(true);
		// Texture2D NextTexture;
		// Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()); ;
		// if (args[1].Get() >= 0)
		// {
		// 	_engine.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.TimeMode);
		// 	NextTexture = Wa2Resource.GetTgaImage(_engine.BgInfo.Path);
		// }
		// else
		// {
		// 	NextTexture = _engine.BgTexture.GetCurTexture();
		// }
		// // _engine.MaskTexture.SetCurOffset(_engine.BgTexture.GetCurOffset());
		// // _engine.MaskTexture.SetCurScale(_engine.BgTexture.GetCurScale());
		// _engine.MaskTexture.SetCurTexture(CeacheTexture);
		// _engine.MaskTexture.SetNextTexture(NextTexture);
		// _engine.MaskTexture.SetMaskTexture(null);
		// // Wa2ImageAnimator animator1 = new(_engine.MaskTexture);
		// // Wa2ImageAnimator animator2 = new(_engine.MaskTexture);
		// _engine.MaskTexture.SetCurOffset(Vector2.Zero);
		// _engine.MaskTexture.SetCurScale(Vector2.One);
		// _engine.BgInfo.Offset = new Vector2(args[5].Get() - args[4].Get(), args[6].Get());
		// _engine.BgInfo.Scale = new Vector2(args[7].Get(), args[8].Get());
		// _engine.MaskTexture.SetNextOffset(_engine.BgInfo.Offset);
		// _engine.MaskTexture.SetNextScale(_engine.BgInfo.Scale);

		// // _engine.SubViewport.Hide();
		// _engine.BgTexture.SetCurOffset(_engine.BgInfo.Offset);
		// _engine.BgTexture.SetCurScale(_engine.BgInfo.Scale);
		// _engine.BgTexture.SetCurTexture(NextTexture);
		// // animator1.InitFade(_engine.BgInfo.Frame * _engine.FrameTime);
		// // animator2.InitHide(_engine.BgInfo.Frame * _engine.FrameTime);
		// _engine.AnimatorMgr.AddMaskFeadAnimation(_engine.MaskTexture, args[3].Get() * _engine.FrameTime, true);
		// _engine.ClearChar(args[3].Get() * _engine.FrameTime);
		// return false;
	}
	public bool BC2(List<Wa2Var> args)
	{
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), true, 0, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get(), args[8].Get());
		return false;
	}
	public bool V2(List<Wa2Var> args)
	{
		// Texture2D NextTexture;

		// if (args[1].Get() >= 0)
		// {
		// 	_engine.BgInfo.Path = string.Format("v{0:D5}{1:D1}.tga", args[1].Get(), args[2].Get());
		// 	_engine.SetCgFlag(args[1].Get() * 10 + args[2].Get(), 1);
		// 	NextTexture = Wa2Resource.GetTgaImage(_engine.BgInfo.Path);
		// }
		// else
		// {
		// 	NextTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		// }
		// _engine.BgInfo.Offset = new Vector2(args[5].Get() - args[4].Get(), args[6].Get());
		// _engine.BgInfo.Scale = new Vector2(args[7].Get(), args[8].Get());
		// _engine.MaskTexture.SetNextOffset(_engine.BgInfo.Offset);
		// _engine.MaskTexture.SetNextScale(_engine.BgInfo.Scale);
		// _engine.BgTexture.SetCurOffset(_engine.BgInfo.Offset);
		// _engine.BgTexture.SetCurScale(_engine.BgInfo.Scale);
		// _engine.MaskTexture.SetCurTexture(ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()));
		// _engine.MaskTexture.SetNextTexture(NextTexture);
		// _engine.MaskTexture.SetMaskTexture(null);
		// _engine.AnimatorMgr.AddMaskFeadAnimation(_engine.MaskTexture, args[3].Get() * _engine.FrameTime);
		// _engine.ClearChar(args[3].Get() * _engine.FrameTime);
		// _engine.BgTexture.SetCurTexture(NextTexture);
		// return false;
		int id = args[2].Get() + 10 * args[1].Get();
		_engine.RenderImage(id, args[0].Get(), false, 1, args[3].Get(), args[4].Get(), args[5].Get(), args[6].Get(), args[7].Get(), args[8].Get());
		return false;
	}
	public bool H2(List<Wa2Var> args)
	{
		return false;
	}
	public bool SetWeather2(List<Wa2Var> args)
	{
		return true;
	}
	public bool ChangeWeather2(List<Wa2Var> args)
	{
		return true;
	}
	public bool SetShake2(List<Wa2Var> args)
	{
		return true;
	}
	public bool M2(List<Wa2Var> args)
	{
		return false;
	}
	public bool NB(List<Wa2Var> args)
	{
		_engine.AdvMain.NovelHide(args[0].Get() * _engine.FrameTime);
		return false;
	}
	public bool NBR(List<Wa2Var> args)
	{
		_engine.AdvMain.NovelShow(args[0].Get() * _engine.FrameTime);
		return false;
	}
	public bool VXV(List<Wa2Var> args)
	{
		_engine.SoundMgr.SetVoiceVolume(args[2].Get(), args[0].Get(), args[1].Get());
		return true;
	}
	public bool Wait2(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(args[0].Get() * _engine.FrameTime);
		return false;
		// GD.Print("暂停游戏" + args[0] + "帧");
	}
	public bool IfSkip(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.CanSkip() ? 1 : 0);
		return true;
	}
	public bool GetSkip2(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.CanSkip() ? 1 : 0);
		return true;
	}
	public bool SLoad(List<Wa2Var> args)
	{
		_engine.Reset(false);
		_engine.ScriptStack.Clear();
		_engine.Script = new(args[0].Get(), args[1].Get());
		_engine.ScriptStack.Push(_engine.Script);
		return false;

	}
	public bool SCall(List<Wa2Var> args)
	{
		_engine.Reset(false);
		_engine.ScriptStack.Push(new(args[0].Get(), args[1].Get()));
		_engine.Script = _engine.ScriptStack.Peek();
		// GD.Print("scall:", args[0].Get());
		return false;
		// GD.Print("无用函数");
	}
	public bool call(List<Wa2Var> args)
	{
		_engine.ScriptStack.Push(new(_engine.Script.ScriptName, args[0].Get()));
		_engine.Script = _engine.ScriptStack.Peek();
		return false;
	}
	public bool run(List<Wa2Var> args)
	{
		GD.Print("RUN");
		return false;
	}
	public bool print(List<Wa2Var> args)
	{
		GD.Print("打印函数");
		return true;
	}
	public bool Ret(List<Wa2Var> args)
	{
		// _engine.ScriptStack.Pop();
		// _engine.Script=_engine.ScriptStack.Peek();
		return false;
	}
	public bool _int(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushInt(5, 3, (int)v);
		return true;

	}
	public bool _float(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, (float)v);

		return true;
	}
	public bool Rand(List<Wa2Var> args)
	{
		return true;
	}
	public bool Sin(List<Wa2Var> args)
	{
		var v = args[^1].Get() * (3.1415926f / 180.0f);
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Sin(v));
		return true;
	}
	public bool Cos(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Cos(v));
		return true;
	}
	public bool Tan(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Tan(v));
		return true;
	}
	public bool Asin(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Asin(v));
		return true;
	}
	public bool Acos(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Acos(v));
		return true;
	}
	public bool Atan(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Atan(v));
		return true;
	}
	public bool Atan2(List<Wa2Var> args)
	{
		return true;
	}
	public bool Pow(List<Wa2Var> args)
	{
		// var v=args[^1].Get();
		// args.RemoveAt(args.Count-1);
		// _engine.Script.PushFloat(5, 4,MathF.Pow(v));
		return true;
	}
	public bool Sqrt(List<Wa2Var> args)
	{
		var v = args[^1].Get();
		args.RemoveAt(args.Count - 1);
		_engine.Script.PushFloat(5, 4, MathF.Sqrt(v));
		return true;
	}
	public bool TimeGetTime(List<Wa2Var> args)
	{
		return true;
	}
}
