using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
public class Wa2Func
{
	private Wa2EngineMain _engine;
	public int idx = 0;
	public Dictionary<uint, Action<List<dynamic>>> FuncDic;
	public Wa2Func(Wa2EngineMain e)
	{
		_engine = e;
		FuncDic = new(){
{0x0,new Action<List<dynamic>>(SLoad)},
{0x1,new Action<List<dynamic>>(SCall)},
{0x2,new Action<List<dynamic>>(call)},
{0x3,new Action<List<dynamic>>(run)},
{0x4,new Action<List<dynamic>>(print)},
{0x5,new Action<List<dynamic>>(Ret)},
{0x6,new Action<List<dynamic>>(_int)},
{0x7,new Action<List<dynamic>>(_float)},
{0x8,new Action<List<dynamic>>(Rand)},
{0x9,new Action<List<dynamic>>(Sin)},
{0xa,new Action<List<dynamic>>(Cos)},
{0xb,new Action<List<dynamic>>(Tan)},
{0xc,new Action<List<dynamic>>(Asin)},
{0xd,new Action<List<dynamic>>(Acos)},
{0xe,new Action<List<dynamic>>(Atan)},
{0xf,new Action<List<dynamic>>(Atan2)},
{0x10,new Action<List<dynamic>>(Pow)},
{0x11,new Action<List<dynamic>>(Sqrt)},
{0x12,new Action<List<dynamic>>(TimeGetTime)},
{0x80,new Action<List<dynamic>>(printEx)},
{0x81,new Action<List<dynamic>>(printEx2)},
{0x82,new Action<List<dynamic>>(SetMessage)},
{0x83,new Action<List<dynamic>>(SetMessageE)},
{0x84,new Action<List<dynamic>>(EndMessage)},
{0x85,new Action<List<dynamic>>(SetMessage2)},
{0x86,new Action<List<dynamic>>(WaitMessage2)},
{0x87,new Action<List<dynamic>>(K)},
{0x88,new Action<List<dynamic>>(SetDemoMode)},
{0x89,new Action<List<dynamic>>(VI)},
{0x8a,new Action<List<dynamic>>(VV)},
{0x8b,new Action<List<dynamic>>(VX)},
{0x8c,new Action<List<dynamic>>(VW)},
{0x8d,new Action<List<dynamic>>(VS)},
{0x8e,new Action<List<dynamic>>(W)},
{0x8f,new Action<List<dynamic>>(WR)},
{0x90,new Action<List<dynamic>>(WN)},
{0x91,new Action<List<dynamic>>(WNS)},
{0x92,new Action<List<dynamic>>(B)},
{0x93,new Action<List<dynamic>>(BC)},
{0x94,new Action<List<dynamic>>(V)},
{0x95,new Action<List<dynamic>>(H)},
{0x96,new Action<List<dynamic>>(SetShake)},
{0x97,new Action<List<dynamic>>(StopShake)},
{0x98,new Action<List<dynamic>>(F)},
{0x99,new Action<List<dynamic>>(FB)},
{0x9a,new Action<List<dynamic>>(C)},
{0x9b,new Action<List<dynamic>>(CW)},
{0x9c,new Action<List<dynamic>>(CR)},
{0x9d,new Action<List<dynamic>>(CRW)},
{0x9e,new Action<List<dynamic>>(M)},
{0x9f,new Action<List<dynamic>>(MS)},
{0xa0,new Action<List<dynamic>>(MP)},
{0xa1,new Action<List<dynamic>>(MV)},
{0xa2,new Action<List<dynamic>>(MW)},
{0xa3,new Action<List<dynamic>>(MLW)},
{0xa4,new Action<List<dynamic>>(SE)},
{0xa5,new Action<List<dynamic>>(SEP)},
{0xa6,new Action<List<dynamic>>(SES)},
{0xa7,new Action<List<dynamic>>(SEV)},
{0xa8,new Action<List<dynamic>>(SEW)},
{0xa9,new Action<List<dynamic>>(SEVW)},
{0xaa,new Action<List<dynamic>>(SetTimeMode)},
{0xab,new Action<List<dynamic>>(SetChromaMode)},
{0xac,new Action<List<dynamic>>(SetEffctMode)},
{0xad,new Action<List<dynamic>>(SetWeather)},
{0xae,new Action<List<dynamic>>(ChangeWeather)},
{0xaf,new Action<List<dynamic>>(ResetWeather)},
{0xb0,new Action<List<dynamic>>(LoadBmp)},
{0xb1,new Action<List<dynamic>>(LoadBmpAnime)},
{0xb2,new Action<List<dynamic>>(SetBmpAvi)},
{0xb3,new Action<List<dynamic>>(WaitBmpAvi)},
{0xb4,new Action<List<dynamic>>(ReleaseBmp)},
{0xb5,new Action<List<dynamic>>(WaitBmpAnime)},
{0xb6,new Action<List<dynamic>>(SetBmpAnimePlay)},
{0xb7,new Action<List<dynamic>>(SetBmpDisp)},
{0xb8,new Action<List<dynamic>>(SetBmpLayer)},
{0xb9,new Action<List<dynamic>>(SetBmpParam)},
{0xba,new Action<List<dynamic>>(SetBmpRevParam)},
{0xbb,new Action<List<dynamic>>(SetBmpBright)},
{0xbc,new Action<List<dynamic>>(SetBmpMove)},
{0xbd,new Action<List<dynamic>>(SetBmpPos)},
{0xbe,new Action<List<dynamic>>(SetBmpZoom)},
{0xbf,new Action<List<dynamic>>(SetBmpZoom2)},
{0xc0,new Action<List<dynamic>>(SetBmpRoll)},
{0xc1,new Action<List<dynamic>>(SetMovie)},
{0xc2,new Action<List<dynamic>>(Wait)},
{0xc3,new Action<List<dynamic>>(StartTimer)},
{0xc4,new Action<List<dynamic>>(WaitTimer)},
{0xc5,new Action<List<dynamic>>(GoTitle)},
{0xc6,new Action<List<dynamic>>(GetGameFlag)},
{0xc7,new Action<List<dynamic>>(SetGameFlag)},
{0xc8,new Action<List<dynamic>>(LogOut)},
{0xc9,new Action<List<dynamic>>(V_Flag)},
{0xca,new Action<List<dynamic>>(H_Flag)},
{0xcb,new Action<List<dynamic>>(Calender)},
{0xcc,new Action<List<dynamic>>(GetTimer)},
{0xcd,new Action<List<dynamic>>(GetSkip)},
{0xce,new Action<List<dynamic>>(GetClick)},
{0xcf,new Action<List<dynamic>>(runEX)},
{0xd0,new Action<List<dynamic>>(SetSelectMess)},
{0xd1,new Action<List<dynamic>>(SetSelect)},
{0xd2,new Action<List<dynamic>>(S)},
{0xd3,new Action<List<dynamic>>(Z)},
{0xd4,new Action<List<dynamic>>(R)},
{0xd5,new Action<List<dynamic>>(WSZ)},
{0xd6,new Action<List<dynamic>>(StopSZR)},
{0xd7,new Action<List<dynamic>>(VA)},
{0xd8,new Action<List<dynamic>>(CS)},
{0xd9,new Action<List<dynamic>>(CM)},
{0xda,new Action<List<dynamic>>(CRS)},
{0xdb,new Action<List<dynamic>>(SkipOFF)},
{0xdc,new Action<List<dynamic>>(NevelMode)},
{0xdd,new Action<List<dynamic>>(EroMode)},
{0xde,new Action<List<dynamic>>(GetReplayMode)},
{0xdf,new Action<List<dynamic>>(WN2)},
{0xe0,new Action<List<dynamic>>(WNS2)},
{0xe1,new Action<List<dynamic>>(B2)},
{0xe2,new Action<List<dynamic>>(BC2)},
{0xe3,new Action<List<dynamic>>(V2)},
{0xe4,new Action<List<dynamic>>(H2)},
{0xe5,new Action<List<dynamic>>(SetWeather2)},
{0xe6,new Action<List<dynamic>>(ChangeWeather2)},
{0xe7,new Action<List<dynamic>>(SetShake2)},
{0xe8,new Action<List<dynamic>>(M2)},
{0xe9,new Action<List<dynamic>>(NB)},
{0xea,new Action<List<dynamic>>(NBR)},
{0xeb,new Action<List<dynamic>>(VXV)},
{0xec,new Action<List<dynamic>>(Wait2)},
{0xed,new Action<List<dynamic>>(IfSkip)},
{0xee,new Action<List<dynamic>>(GetSkip2)},
	};

	}

	public void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	{
		Wa2SoundMgr.Instance.PlayBgm(id, loopFlag, volume);
	}
	public void StopBgm(int frame = 0)
	{
		Wa2SoundMgr.Instance.StopBgm(frame * _engine.FrameTime);
	}
	public void PlaySysSe(AudioStream stream)
	{
		Wa2SoundMgr.Instance.PlaySysSe(stream);
	}
	public void LoadScript(string name, uint pos = 0)
	{
		_engine.LoadScript(name, pos);
	}
	public void printEx(List<dynamic> args) { }
	public void printEx2(List<dynamic> args) { }
	public void SetMessage(List<dynamic> args)
	{
		GD.Print("设置等待点击标签样式");

	}
	public void SetMessageE(List<dynamic> args)
	{
		_engine.AdvMain.CurText = _engine.Script.ParseStr(args[0]).Replace("\\n", "\n");
		if (!_engine.AdvMain.Active)
		{
			_engine.AdvMain.AdvShow();
		}
		else
		{
			_engine.AdvMain.AdvShowText();
		}


	}
	public void EndMessage(List<dynamic> args)
	{
		_engine.WaitClick = true;
	}
	public void SetMessage2(List<dynamic> args) { }
	public void WaitMessage2(List<dynamic> args) { }
	public void K(List<dynamic> args) { }
	public void SetDemoMode(List<dynamic> args) { }
	public void VI(List<dynamic> args)
	{
		if (args[0] == 0)
		{
			_engine.Label = args[1];
		}
		// int v0 = args[0];
		// int v1 = args[1];
		// _engine.Label = v1;

	}
	public void VV(List<dynamic> args)
	{
		if (!_engine.Skipping)
		{
			_engine.SoundMgr.PlayViceo(_engine.Label, args[4], args[0]);
		}

	}
	public void VX(List<dynamic> args)
	{
		GD.Print("插入对话");
	}
	public void VW(List<dynamic> args)
	{
		_engine.WaitTimer.Start(_engine.SoundMgr.GetViceoTime());
		_engine.WaitClick=true;
		GD.Print("等待对话结束跳转下一句");
		// _engine.AdvMain.CurName="";
	}
	public void VS(List<dynamic> args)
	{
		GD.Print("停止对话");
	}
	public void W(List<dynamic> args) { }
	public void WR(List<dynamic> args)
	{
		_engine.AdvMain.AdvHide();
	}
	public void WN(List<dynamic> args)
	{
		if (args[0] >= 0)
		{
			_engine.AdvMain.CurName = _engine.Script.ParseStr(args[0]);
		}
		else
		{
			_engine.AdvMain.CurName = "";
		}


	}
	public void WNS(List<dynamic> args) { }

	public void B(List<dynamic> args)
	{
		// GD.Print("设置背景");
		// _e.Viewport.BgDrawFrame = args[3];
		if (args[3] > 0)
		{
			_engine.BgTime = args[3] * _engine.FrameTime;
		}
		_engine.AnimatorsFinish();
		Texture2D NextTexture;
		Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		if (args[1] >= 0)
		{
			NextTexture = Wa2Resource.GetBgImage(args[1], _engine.TimeMode, args[2]);
		}
		else
		{
			NextTexture = CeacheTexture;
		}
		GD.Print("遮罩图", args[0]);
		if (args[0] >= 128)
		{
			_engine.MaskTexture.SetMaskTexture(Wa2Resource.GetMaskImage(args[0] & 0x7f));
		}
		else
		{
			_engine.MaskTexture.SetMaskTexture(null);
		}
		_engine.MaskTexture.SetCurOffset(Vector2.Zero);
		_engine.MaskTexture.SetCurScale(Vector2.One);
		_engine.MaskTexture.SetCurTexture(CeacheTexture);
		_engine.MaskTexture.SetNextScale(new Vector2(1, 1));
		_engine.MaskTexture.SetNextOffset(new Vector2(0, 0));
		_engine.MaskTexture.SetNextTexture(NextTexture);
		_engine.BgTexture.SetCurOffset(new Vector2(0, 0));
		_engine.BgTexture.SetCurScale(new Vector2(1, 1));
		// _engine.MaskTexture.SetMaskTexture(null);
		Wa2Animator animator1 = new(_engine.MaskTexture);
		Wa2Animator animator2 = new(_engine.MaskTexture);
		animator1.InitFade(_engine.BgTime);
		animator2.InitHide(_engine.BgTime);
		ClearChar(_engine.BgTime);
		// _engine.SubViewport.Hide();
		_engine.BgTexture.SetCurTexture(NextTexture);
		// _engine.BgTexture.SetCurScale(NextTexture);
		// _engine.BgTexture.SetCurOffset(NextTexture);
		// _engine.WaitTimer.Start(args[3] * _engine.FrameTime);
		// GD.Print("时间:",args[3]*_engine.FrameTime);
		// Viewport.Clear();

		// int v0, int v1, int v2, int v3, int v4, int v5, int v6, float v7, float v8

	}
	public void ClearChar(float time)
	{
		for (int i = 0; i < _engine.Chars.Length; i++)
		{
			if (_engine.Chars[i].GetCurTexture() == null)
			{
				continue;
			}
			Wa2Animator animator1 = new(_engine.Chars[i]);
			Wa2Animator animator2 = new(_engine.Chars[i]);
			_engine.Chars[i].SetNextTexture(null);
			animator1.InitFade(time);
			animator2.InitHide(time);
		}
		_engine.CharDic.Clear();
	}
	public void UpdateChar(float time)
	{
		List<int> posList = new();
		foreach (Wa2Char value in _engine.CharDic.Values)
		{
			GD.Print("id:",value.id,"pos:",value.pos);
			Wa2Image image = _engine.Chars[value.pos];
			Wa2Animator animator1 = new(image);
			image.SetNextTexture(Wa2Resource.GetChrImage(value.id, value.no));
			animator1.InitFade(time);
			posList.Add(value.pos);
		}
		for (int i = 0; i < _engine.Chars.Length; i++)
		{
			if (posList.Contains(i))
			{
				continue;
			}
			Wa2Image image = _engine.Chars[i];
			Wa2Animator animator2 = new(image);
			// image.SetCurTexture(image.GetNextTexture());
			image.SetNextTexture(null);
			animator2.InitFade(time);
		}
		GD.Print("角色数量:",posList.Count);
	}
	public void BC(List<dynamic> args)
	{
		if (args[3] > 0)
		{
			_engine.BgTime = args[3] * _engine.FrameTime;
		}

		Texture2D NextTexture;
		Texture2D CeacheTexture = _engine.BgTexture.GetCurTexture();
		if (args[1] >= 0)
		{
			NextTexture = Wa2Resource.GetBgImage(args[1], _engine.TimeMode, args[2]);
		}
		else
		{
			NextTexture = CeacheTexture;
		}
		// _engine.BgTexture.SetCurOffset(new Vector2(0, 0));
		// _engine.BgTexture.SetCurScale(new Vector2(1, 1));
		_engine.BgTexture.SetCurTexture(CeacheTexture);
		_engine.BgTexture.SetNextTexture(NextTexture);
		_engine.BgTexture.SetMaskTexture(null);
		Wa2Animator animator3 = new(_engine.BgTexture);
		animator3.InitFade(_engine.BgTime);
		UpdateChar(_engine.BgTime);
		// _engine.SubViewport.Hide();
		GD.Print("更新背景和角色");
		// _engine.CharDic.Clear();
		// _engine.Viewport.Char();

	}
	public void V(List<dynamic> args)
	{
		Texture2D NextTexture;

		if (args[1] >= 0)
		{
			NextTexture = Wa2Resource.GetCgImage(args[1], args[2]);
		}
		else
		{
			NextTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		}
		_engine.MaskTexture.SetCurOffset(_engine.BgTexture.GetCurOffset());
		_engine.MaskTexture.SetCurScale(_engine.BgTexture.GetCurScale());
		_engine.MaskTexture.SetCurTexture(ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()));
		_engine.MaskTexture.SetNextTexture(NextTexture);
		_engine.MaskTexture.SetMaskTexture(null);
		Wa2Animator animator1 = new(_engine.MaskTexture);
		Wa2Animator animator2 = new(_engine.MaskTexture);
		animator1.InitFade(args[3] * _engine.FrameTime);
		animator2.InitHide(args[3] * _engine.FrameTime);
		ClearChar(args[3] * _engine.FrameTime);
		_engine.BgTexture.SetCurTexture(NextTexture);
	}
	public void H(List<dynamic> args)
	{

	}
	public void SetShake(List<dynamic> args)
	{

	}
	public void StopShake(List<dynamic> args)
	{

	}
	public void F(List<dynamic> args)
	{

	}
	public void FB(List<dynamic> args)
	{

	}

	public void C(List<dynamic> args)
	{
		// _engine.AddChar(args[0], args[1], args[2]);
		// GD.Print("c");
		// if(args[0]>10){

		// }
		_engine.CharDic[args[0]] = new Wa2Char
		{
			id = args[0],
			no = args[1],
			pos = args[2],
		};
		int test = args[5];
		UpdateChar(args[5] * _engine.FrameTime);
		// int pos = Array.FindIndex(_engine.CharIdxs, x => x == args[0]);
		// if (pos != args[2])
		// {

		// 	RemovChar(pos, args[5] * _engine.FrameTime);
		// }
		// Wa2Image image = _engine.Chars[args[2]];
		// // image.SetCurTexture(image.GetNextTexture());
		// image.SetNextTexture(Wa2Resource.GetChrImage(args[0], args[1]));
		// Wa2Animator animator = new(image);
		// animator.InitFade(args[5] * _engine.FrameTime);
		// _engine.Viewport.Char();
		// _engine.WaitTimer.Start(args[5] * _engine.FrameTime);
	}
	// public void RemovChar(int pos)
	// {
	// 	Wa2Image image = _engine.Chars[pos];
	// 	image.SetCurTexture(image.GetNextTexture());
	// 	image.SetNextTexture(null);
	// 	Wa2Animator animator1 = new(image);
	// 	Wa2Animator animator2 = new(image);
	// 	// animator1.InitFade(time, (int)image.GetNextOffset().X, (int)image.GetNextOffset().Y);
	// 	// animator2.InitHide(time);

	// }
	public void CW(List<dynamic> args)
	{
		_engine.CharDic[args[0]] = new Wa2Char
		{
			id = args[0],
			no = args[1],
			pos = args[2],
		};
		// int pos = _engine.CharDic[args[0]].pos;
		// if (pos!=-1 && pos != args[2])
		// {

		// 	RemovChar(pos);
		// }
		// _engine.CharIdxs[args[2]] = args[0];
		// Wa2Image image = _engine.Chars[args[2]];
		// image.SetCurTexture(image.GetNextTexture());
		// image.SetNextTexture(Wa2Resource.GetChrImage(args[0], args[1]));
		// Wa2Animator animator = new(image);
		// animator.InitFade(args[5] * _engine.FrameTime, (int)image.GetNextOffset().X, (int)image.GetNextOffset().Y);
		// Wa2Image image = _engine.Chars[args[2]];
		// image.SetCurTexture(image.GetNextTexture());
		// image.SetNextTexture(null);
		// Wa2Animator animator = new(image);
		// animator.InitFade(args[5] * _engine.FrameTime);
		// animator.InitHide(args[5] * _engine.FrameTime);
		// _engine.Viewport.CharEx();
		// _engine.WaitTimer.Start(args[5] * _engine.FrameTime);
	}
	public void CR(List<dynamic> args)
	{
		// int pos = _engine.CharDic[args[0]].pos;
		// Wa2Image image = _engine.Chars[pos];
		// image.SetCurTexture(image.GetNextTexture());
		// image.SetNextTexture(null);
		// Wa2Animator animator1 = new(image);
		// // Wa2Animator animator2 = new(image);
		// animator1.InitFade(args[2] * _engine.FrameTime);
		// animator2.InitHide(args[2] * _engine.FrameTime);
		_engine.CharDic.Remove(args[0]);
		UpdateChar(args[2] * _engine.FrameTime);
		// GD.Print(image.GetCurTexture());
		// _engine.RemoveChar(args[0]);
		// _engine.Viewport.Char();
		// _engine.WaitTimer.Start(args[2] * _engine.FrameTime);
	}
	public void CRW(List<dynamic> args)
	{
		_engine.CharDic.Remove(args[0]);
	}
	public void M(List<dynamic> args)
	{
		if (args.Count == 3)
		{
			GD.Print("错误位置", _engine.Script.CurPos);
		}
		_engine.SoundMgr.PlayBgm(args[0], args[2] != 0, args[3]);
	}
	public void MS(List<dynamic> args)
	{
		// GD.Print("暂停:", args[0] * _engine.FrameTime);
		_engine.SoundMgr.StopBgm(args[0] * _engine.FrameTime);
		// uint v0 = args[0];
	}
	public void MP(List<dynamic> args)
	{

	}
	public void MV(List<dynamic> args)
	{
		GD.Print("估计是改变语言音量2个参数");
	}
	public void MW(List<dynamic> args)
	{

	}
	public void MLW(List<dynamic> args)
	{

	}
	public void SE(List<dynamic> args)
	{
		Wa2SoundMgr.Instance.PlaySe(0, args[0], false, 0, args[1]);
	}
	public void SEP(List<dynamic> args)
	{
		// GD.Print(args.Count);
		Wa2SoundMgr.Instance.PlaySe(args[0], args[1], args[2] != 0, args[3] * _engine.FrameTime, args[4]);
	}
	public void SES(List<dynamic> args)
	{
		Wa2SoundMgr.Instance.StopSe(args[0], args[1] * _engine.FrameTime);
	}
	public void SEV(List<dynamic> args)
	{
		_engine.SoundMgr.SetSeVolume(args[0], args[1], args[2] * _engine.FrameTime);
	}
	public void SEW(List<dynamic> args)
	{
		_engine.WaitSe = true;
		_engine.WaitSeChannel = args[0];
		_engine.WaitTimer.Start(_engine.SoundMgr.GetSeTime(args[0]));
	}
	public void SEVW(List<dynamic> args)
	{

	}
	public void SetTimeMode(List<dynamic> args)
	{
		_engine.TimeMode = args[0];
		// uint v0 = args[0];
	}
	public void SetChromaMode(List<dynamic> args)
	{
		GD.Print("设置色差");
	}
	public void SetEffctMode(List<dynamic> args)
	{
		GD.Print("设置特效模式");
	}
	public void SetWeather(List<dynamic> args)
	{
		GD.Print("设置天气");
	}
	public void ChangeWeather(List<dynamic> args)
	{
		GD.Print("改变天气");
	}
	public void ResetWeather(List<dynamic> args)
	{

	}
	public void LoadBmp(List<dynamic> args)
	{
		GD.Print(_engine.Script.ParseStr(args[1]));
	}
	public void LoadBmpAnime(List<dynamic> args)
	{
		GD.Print("加载bmp动画");
	}
	public void SetBmpAvi(List<dynamic> args)
	{

	}
	public void WaitBmpAvi(List<dynamic> args)
	{

	}
	public void ReleaseBmp(List<dynamic> args)
	{
		GD.Print("释放位图");
	}
	public void WaitBmpAnime(List<dynamic> args)
	{

	}
	public void SetBmpAnimePlay(List<dynamic> args)
	{

	}
	public void SetBmpDisp(List<dynamic> args)
	{

	}
	public void SetBmpLayer(List<dynamic> args)
	{

	}
	public void SetBmpParam(List<dynamic> args)
	{
		GD.Print("设置位图参数");
	}
	public void SetBmpRevParam(List<dynamic> args)
	{

	}
	public void SetBmpBright(List<dynamic> args)
	{

	}
	public void SetBmpMove(List<dynamic> args)
	{
		GD.Print("设置位图移动");
	}
	public void SetBmpPos(List<dynamic> args)
	{

	}
	public void SetBmpZoom(List<dynamic> args)
	{

	}
	public void SetBmpZoom2(List<dynamic> args)
	{
		GD.Print("设置位图缩放");
	}
	public void SetBmpRoll(List<dynamic> args)
	{

	}
	public void SetMovie(List<dynamic> args)
	{

	}
	public void Wait(List<dynamic> args)
	{
		_engine.WaitTimer.Start(args[0] * _engine.FrameTime);

	}
	public void StartTimer(List<dynamic> args)
	{

	}
	public void WaitTimer(List<dynamic> args)
	{
		_engine.WaitTimer.Start(args[0] * 0.0001f);
		args.Clear();
	}
	public void GoTitle(List<dynamic> args)
	{
		_engine.UiMgr.OpenTitleMenu();
	}
	public void GetGameFlag(List<dynamic> args)
	{
		_engine.Script.PushInt(_engine.GameFlags[args[0]]);
	}
	public void SetGameFlag(List<dynamic> args)
	{
		_engine.GameFlags[args[0]]=args[1];
	}
	public void LogOut(List<dynamic> args)
	{

	}
	public void V_Flag(List<dynamic> args)
	{

	}
	public void H_Flag(List<dynamic> args)
	{

	}
	public void Calender(List<dynamic> args)
	{
		_engine.Year = args[0];
		_engine.Month = args[1];
		_engine.Day = args[2];
		// uint hour=args[3];
		// GD.Print("设置日期" + year + "年" + month + "月" + day + "日");
	}
	public void GetTimer(List<dynamic> args)
	{

	}
	public void GetSkip(List<dynamic> args)
	{
		_engine.Script.PushInt(0);
	}
	public void GetClick(List<dynamic> args)
	{

	}
	public void runEX(List<dynamic> args)
	{

	}
	public void SetSelectMess(List<dynamic> args)
	{

	}
	public void SetSelect(List<dynamic> args)
	{

	}
	public void S(List<dynamic> args)
	{
		Wa2Animator animator = new(_engine.BgTexture);
		animator.Wait = false;
		animator.InitMove(args[2] * _engine.FrameTime, args[0], args[1]);

	}
	public void Z(List<dynamic> args6)
	{

	}
	public void R(List<dynamic> args)
	{

	}
	public void WSZ(List<dynamic> args)
	{

	}
	public void StopSZR(List<dynamic> args)
	{

	}
	public void VA(List<dynamic> args)
	{

	}
	public void CS(List<dynamic> args)
	{

	}
	public void CM(List<dynamic> args)
	{

	}
	public void CRS(List<dynamic> args)
	{

	}
	public void SkipOFF(List<dynamic> args)
	{

	}
	public void NevelMode(List<dynamic> args)
	{

	}
	public void EroMode(List<dynamic> args)
	{

	}
	public void GetReplayMode(List<dynamic> args)
	{
		_engine.Script.PushInt(_engine.ReplayMode);
	}
	public void WN2(List<dynamic> args)
	{

	}
	public void WNS2(List<dynamic> args)
	{

	}
	public void B2(List<dynamic> args)
	{
		_engine.AnimatorsFinish();
		Texture2D NextTexture;
		Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		if (args[3] > 0)
		{
			_engine.BgTime = args[3] * _engine.FrameTime;
		}
		if (args[1] >= 0)
		{
			NextTexture = Wa2Resource.GetBgImage(args[1], _engine.TimeMode, args[2]);
		}
		else
		{
			NextTexture = CeacheTexture;
		}
		// _engine.MaskTexture.SetCurOffset(_engine.BgTexture.GetCurOffset());
		// _engine.MaskTexture.SetCurScale(_engine.BgTexture.GetCurScale());
		_engine.MaskTexture.SetCurTexture(CeacheTexture);
		_engine.MaskTexture.SetNextTexture(NextTexture);
		_engine.MaskTexture.SetMaskTexture(null);
		Wa2Animator animator1 = new(_engine.MaskTexture);
		Wa2Animator animator2 = new(_engine.MaskTexture);
		_engine.MaskTexture.SetCurOffset(Vector2.Zero);
		_engine.MaskTexture.SetCurScale(Vector2.One);
		_engine.MaskTexture.SetNextOffset(new Vector2(args[5] - args[4], args[6]));
		_engine.MaskTexture.SetNextScale(new Vector2(args[7], args[8]));

		// _engine.SubViewport.Hide();
		_engine.BgTexture.SetCurOffset(new Vector2(args[5] - args[4], args[6]));
		_engine.BgTexture.SetCurScale(new Vector2(args[7], args[8]));
		_engine.BgTexture.SetCurTexture(NextTexture);
		animator1.InitFade(_engine.BgTime);
		animator2.InitHide(_engine.BgTime);
		ClearChar(_engine.BgTime);
	}
	public void BC2(List<dynamic> args)
	{

	}
	public void V2(List<dynamic> args)
	{

	}
	public void H2(List<dynamic> args)
	{

	}
	public void SetWeather2(List<dynamic> args)
	{

	}
	public void ChangeWeather2(List<dynamic> args)
	{

	}
	public void SetShake2(List<dynamic> args)
	{
		GD.Print("设置震动");
	}
	public void M2(List<dynamic> args)
	{

	}
	public void NB(List<dynamic> args)
	{

	}
	public void NBR(List<dynamic> args)
	{

	}
	public void VXV(List<dynamic> args)
	{

	}
	public void Wait2(List<dynamic> args)
	{
		_engine.WaitTimer.Start(args[0] * _engine.FrameTime);
		// GD.Print("暂停游戏" + args[0] + "帧");
	}
	public void IfSkip(List<dynamic> args)
	{

	}
	public void GetSkip2(List<dynamic> args)
	{

	}
	public void SLoad(List<dynamic> args)
	{

		LoadScript(_engine.Script.ParseStr(args[0]));
	}
	public void SCall(List<dynamic> args)
	{
		GD.Print("无用函数");
	}
	public void call(List<dynamic> args)
	{
		GD.Print("无用函数");
	}
	public void run(List<dynamic> args)
	{
		GD.Print("RUN");
	}
	public void print(List<dynamic> args)
	{
		GD.Print("打印函数");
	}
	public void Ret(List<dynamic> args)
	{

	}
	public void _int(List<dynamic> args)
	{

	}
	public void _float(List<dynamic> args)
	{

	}
	public void Rand(List<dynamic> args)
	{

	}
	public void Sin(List<dynamic> args)
	{
		_engine.Script.PushFloat(Mathf.Sin(args[0]));
	}
	public void Cos(List<dynamic> args)
	{

	}
	public void Tan(List<dynamic> args)
	{

	}
	public void Asin(List<dynamic> args)
	{

	}
	public void Acos(List<dynamic> args)
	{

	}
	public void Atan(List<dynamic> args)
	{

	}
	public void Atan2(List<dynamic> args)
	{

	}
	public void Pow(List<dynamic> args)
	{

	}
	public void Sqrt(List<dynamic> args)
	{

	}
	public void TimeGetTime(List<dynamic> args)
	{

	}
}
