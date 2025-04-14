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
	public Dictionary<uint, Action<List<Wa2Var>>> FuncDic;
	public Wa2Func(Wa2EngineMain e)
	{
		_engine = e;
		FuncDic = new(){
{0x0,new Action<List<Wa2Var>>(SLoad)},
{0x1,new Action<List<Wa2Var>>(SCall)},
{0x2,new Action<List<Wa2Var>>(call)},
{0x3,new Action<List<Wa2Var>>(run)},
{0x4,new Action<List<Wa2Var>>(print)},
{0x5,new Action<List<Wa2Var>>(Ret)},
{0x6,new Action<List<Wa2Var>>(_int)},
{0x7,new Action<List<Wa2Var>>(_float)},
{0x8,new Action<List<Wa2Var>>(Rand)},
{0x9,new Action<List<Wa2Var>>(Sin)},
{0xa,new Action<List<Wa2Var>>(Cos)},
{0xb,new Action<List<Wa2Var>>(Tan)},
{0xc,new Action<List<Wa2Var>>(Asin)},
{0xd,new Action<List<Wa2Var>>(Acos)},
{0xe,new Action<List<Wa2Var>>(Atan)},
{0xf,new Action<List<Wa2Var>>(Atan2)},
{0x10,new Action<List<Wa2Var>>(Pow)},
{0x11,new Action<List<Wa2Var>>(Sqrt)},
{0x12,new Action<List<Wa2Var>>(TimeGetTime)},
{0x80,new Action<List<Wa2Var>>(printEx)},
{0x81,new Action<List<Wa2Var>>(printEx2)},
{0x82,new Action<List<Wa2Var>>(SetMessage)},
{0x83,new Action<List<Wa2Var>>(SetMessageE)},
{0x84,new Action<List<Wa2Var>>(EndMessage)},
{0x85,new Action<List<Wa2Var>>(SetMessage2)},
{0x86,new Action<List<Wa2Var>>(WaitMessage2)},
{0x87,new Action<List<Wa2Var>>(K)},
{0x88,new Action<List<Wa2Var>>(SetDemoMode)},
{0x89,new Action<List<Wa2Var>>(VI)},
{0x8a,new Action<List<Wa2Var>>(VV)},
{0x8b,new Action<List<Wa2Var>>(VX)},
{0x8c,new Action<List<Wa2Var>>(VW)},
{0x8d,new Action<List<Wa2Var>>(VS)},
{0x8e,new Action<List<Wa2Var>>(W)},
{0x8f,new Action<List<Wa2Var>>(WR)},
{0x90,new Action<List<Wa2Var>>(WN)},
{0x91,new Action<List<Wa2Var>>(WNS)},
{0x92,new Action<List<Wa2Var>>(B)},
{0x93,new Action<List<Wa2Var>>(BC)},
{0x94,new Action<List<Wa2Var>>(V)},
{0x95,new Action<List<Wa2Var>>(H)},
{0x96,new Action<List<Wa2Var>>(SetShake)},
{0x97,new Action<List<Wa2Var>>(StopShake)},
{0x98,new Action<List<Wa2Var>>(F)},
{0x99,new Action<List<Wa2Var>>(FB)},
{0x9a,new Action<List<Wa2Var>>(C)},
{0x9b,new Action<List<Wa2Var>>(CW)},
{0x9c,new Action<List<Wa2Var>>(CR)},
{0x9d,new Action<List<Wa2Var>>(CRW)},
{0x9e,new Action<List<Wa2Var>>(M)},
{0x9f,new Action<List<Wa2Var>>(MS)},
{0xa0,new Action<List<Wa2Var>>(MP)},
{0xa1,new Action<List<Wa2Var>>(MV)},
{0xa2,new Action<List<Wa2Var>>(MW)},
{0xa3,new Action<List<Wa2Var>>(MLW)},
{0xa4,new Action<List<Wa2Var>>(SE)},
{0xa5,new Action<List<Wa2Var>>(SEP)},
{0xa6,new Action<List<Wa2Var>>(SES)},
{0xa7,new Action<List<Wa2Var>>(SEV)},
{0xa8,new Action<List<Wa2Var>>(SEW)},
{0xa9,new Action<List<Wa2Var>>(SEVW)},
{0xaa,new Action<List<Wa2Var>>(SetTimeMode)},
{0xab,new Action<List<Wa2Var>>(SetChromaMode)},
{0xac,new Action<List<Wa2Var>>(SetEffctMode)},
{0xad,new Action<List<Wa2Var>>(SetWeather)},
{0xae,new Action<List<Wa2Var>>(ChangeWeather)},
{0xaf,new Action<List<Wa2Var>>(ResetWeather)},
{0xb0,new Action<List<Wa2Var>>(LoadBmp)},
{0xb1,new Action<List<Wa2Var>>(LoadBmpAnime)},
{0xb2,new Action<List<Wa2Var>>(SetBmpAvi)},
{0xb3,new Action<List<Wa2Var>>(WaitBmpAvi)},
{0xb4,new Action<List<Wa2Var>>(ReleaseBmp)},
{0xb5,new Action<List<Wa2Var>>(WaitBmpAnime)},
{0xb6,new Action<List<Wa2Var>>(SetBmpAnimePlay)},
{0xb7,new Action<List<Wa2Var>>(SetBmpDisp)},
{0xb8,new Action<List<Wa2Var>>(SetBmpLayer)},
{0xb9,new Action<List<Wa2Var>>(SetBmpParam)},
{0xba,new Action<List<Wa2Var>>(SetBmpRevParam)},
{0xbb,new Action<List<Wa2Var>>(SetBmpBright)},
{0xbc,new Action<List<Wa2Var>>(SetBmpMove)},
{0xbd,new Action<List<Wa2Var>>(SetBmpPos)},
{0xbe,new Action<List<Wa2Var>>(SetBmpZoom)},
{0xbf,new Action<List<Wa2Var>>(SetBmpZoom2)},
{0xc0,new Action<List<Wa2Var>>(SetBmpRoll)},
{0xc1,new Action<List<Wa2Var>>(SetMovie)},
{0xc2,new Action<List<Wa2Var>>(Wait)},
{0xc3,new Action<List<Wa2Var>>(StartTimer)},
{0xc4,new Action<List<Wa2Var>>(WaitTimer)},
{0xc5,new Action<List<Wa2Var>>(GoTitle)},
{0xc6,new Action<List<Wa2Var>>(GetGameFlag)},
{0xc7,new Action<List<Wa2Var>>(SetGameFlag)},
{0xc8,new Action<List<Wa2Var>>(LogOut)},
{0xc9,new Action<List<Wa2Var>>(V_Flag)},
{0xca,new Action<List<Wa2Var>>(H_Flag)},
{0xcb,new Action<List<Wa2Var>>(Calender)},
{0xcc,new Action<List<Wa2Var>>(GetTimer)},
{0xcd,new Action<List<Wa2Var>>(GetSkip)},
{0xce,new Action<List<Wa2Var>>(GetClick)},
{0xcf,new Action<List<Wa2Var>>(runEX)},
{0xd0,new Action<List<Wa2Var>>(SetSelectMess)},
{0xd1,new Action<List<Wa2Var>>(SetSelect)},
{0xd2,new Action<List<Wa2Var>>(S)},
{0xd3,new Action<List<Wa2Var>>(Z)},
{0xd4,new Action<List<Wa2Var>>(R)},
{0xd5,new Action<List<Wa2Var>>(WSZ)},
{0xd6,new Action<List<Wa2Var>>(StopSZR)},
{0xd7,new Action<List<Wa2Var>>(VA)},
{0xd8,new Action<List<Wa2Var>>(CS)},
{0xd9,new Action<List<Wa2Var>>(CM)},
{0xda,new Action<List<Wa2Var>>(CRS)},
{0xdb,new Action<List<Wa2Var>>(SkipOFF)},
{0xdc,new Action<List<Wa2Var>>(NevelMode)},
{0xdd,new Action<List<Wa2Var>>(EroMode)},
{0xde,new Action<List<Wa2Var>>(GetReplayMode)},
{0xdf,new Action<List<Wa2Var>>(WN2)},
{0xe0,new Action<List<Wa2Var>>(WNS2)},
{0xe1,new Action<List<Wa2Var>>(B2)},
{0xe2,new Action<List<Wa2Var>>(BC2)},
{0xe3,new Action<List<Wa2Var>>(V2)},
{0xe4,new Action<List<Wa2Var>>(H2)},
{0xe5,new Action<List<Wa2Var>>(SetWeather2)},
{0xe6,new Action<List<Wa2Var>>(ChangeWeather2)},
{0xe7,new Action<List<Wa2Var>>(SetShake2)},
{0xe8,new Action<List<Wa2Var>>(M2)},
{0xe9,new Action<List<Wa2Var>>(NB)},
{0xea,new Action<List<Wa2Var>>(NBR)},
{0xeb,new Action<List<Wa2Var>>(VXV)},
{0xec,new Action<List<Wa2Var>>(Wait2)},
{0xed,new Action<List<Wa2Var>>(IfSkip)},
{0xee,new Action<List<Wa2Var>>(GetSkip2)},
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

	public void printEx(List<Wa2Var> args) { }
	public void printEx2(List<Wa2Var> args) { }
	public void SetMessage(List<Wa2Var> args)
	{
		GD.Print("设置等待点击标签样式");

	}
	public void SetMessageE(List<Wa2Var> args)
	{
		_engine.GameSav.FirstSentence = args[0].Get();

		_engine.AdvMain.ShowText(_engine.GameSav.FirstSentence, _engine.GameSav.CharName);
		_engine.WaitClick = true;
		_engine.GameSav.CurMessageIdx = args[1].Get();



		// _engine.WaitClick=true;


	}
	public void EndMessage(List<Wa2Var> args)
	{
		_engine.WaitClick = false;
		_engine.AdvMain.WaitSprite.Hide();
		// _engine.AdvMain.Clear();
	}
	public void SetMessage2(List<Wa2Var> args) { }
	public void WaitMessage2(List<Wa2Var> args) { }
	public void K(List<Wa2Var> args) { }
	public void SetDemoMode(List<Wa2Var> args) { }
	public void VI(List<Wa2Var> args)
	{
		if (args[0].Get() == 0)
		{
			_engine.GameSav.Label = args[1].Get();
		}
		// int v0 = args[0];
		// int v1 = args[1];
		// _engine.Label = v1;

	}
	public void VV(List<Wa2Var> args)
	{
		if (!_engine.Skipping && !_engine.SkipMode)
		{
			_engine.SoundMgr.PlayVoice(_engine.GameSav.Label, args[4].Get(), args[0].Get());
		}

	}
	public void VX(List<Wa2Var> args)
	{
		GD.Print("插入对话");
	}
	public void VW(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(_engine.SoundMgr.GetVoiceTime());
		_engine.WaitClick = true;
		GD.Print("等待对话结束跳转下一句");
		// _engine.AdvMain.CurName="";
	}
	public void VS(List<Wa2Var> args)
	{
		GD.Print("停止对话");
	}
	public void W(List<Wa2Var> args) { }
	public void WR(List<Wa2Var> args)
	{
		_engine.AdvMain.AdvHide(0.2f);
	}
	public void WN(List<Wa2Var> args)
	{
		if (args[0].CmdType == CmdType.STR_VAR)
		{
			_engine.AdvMain.CurName = args[0].Get();
			// if(args[0].IntValue>0){
			// 	_engine.WirtSysFlag(args[0].IntValue,1);
			// }
		}
		else
		{
			_engine.AdvMain.CurName = "";
		}
		_engine.GameSav.CharName = _engine.AdvMain.CurName;

	}
	public void WNS(List<Wa2Var> args) { }

	public void B(List<Wa2Var> args)
	{
		// GD.Print("设置背景");
		// _e.Viewport.BgDrawFrame = args[3];
		if (args[3].Get() > 0)
		{
			_engine.GameSav.BgInfo.Frame = args[3].Get();
		}
		_engine.AnimatorsFinish();
		Texture2D NextTexture;
		Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());

		if (args[1].Get() >= 0)
		{
			_engine.GameSav.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.GameSav.TimeMode);
			NextTexture = Wa2Resource.GetTgaImage(_engine.GameSav.BgInfo.Path);
		}
		else
		{
			NextTexture = _engine.BgTexture.GetCurTexture();
		}
		// GD.Print("遮罩图", args[0]);
		if (args[0].Get() >= 128)
		{
			_engine.MaskTexture.SetMaskTexture(Wa2Resource.GetMaskImage(args[0].Get() & 0x7f));
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
		_engine.GameSav.BgInfo.Scale = Vector2.One;
		_engine.GameSav.BgInfo.Offset = Vector2.Zero;
		// _engine.MaskTexture.SetMaskTexture(null);
		Wa2ImageAnimator animator1 = new(_engine.MaskTexture);
		Wa2ImageAnimator animator2 = new(_engine.MaskTexture);
		animator1.InitFade(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
		animator2.InitHide(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
		ClearChar(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
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
			Wa2ImageAnimator animator1 = new(_engine.Chars[i]);
			Wa2ImageAnimator animator2 = new(_engine.Chars[i]);
			_engine.Chars[i].SetNextTexture(null);
			animator1.InitFade(time);
			animator2.InitHide(time);
		}
		_engine.GameSav.CharItems.Clear();
	}

	public void BC(List<Wa2Var> args)
	{
		if (args[3].Get() > 0)
		{
			_engine.GameSav.BgInfo.Frame = args[3].Get();
		}

		Texture2D NextTexture;
		Texture2D CeacheTexture = _engine.BgTexture.GetCurTexture();
		if (args[1].Get() >= 0)
		{
			_engine.GameSav.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.GameSav.TimeMode);
			NextTexture = Wa2Resource.GetTgaImage(_engine.GameSav.BgInfo.Path);
		}
		else
		{
			NextTexture = CeacheTexture;
		}
		_engine.BgTexture.SetCurTexture(CeacheTexture);
		_engine.BgTexture.SetNextTexture(NextTexture);
		_engine.BgTexture.SetMaskTexture(null);
		Wa2ImageAnimator animator3 = new(_engine.BgTexture);
		animator3.InitFade(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
		_engine.UpdateChar(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);

		// GD.Print("更新背景和角色");

	}
	public void V(List<Wa2Var> args)
	{
		Texture2D NextTexture;

		if (args[1].Get() >= 0)
		{
			_engine.GameSav.BgInfo.Path = string.Format("v{0:D5}{1:D1}.tga", args[1].Get(), args[2].Get());
			NextTexture = Wa2Resource.GetTgaImage(_engine.GameSav.BgInfo.Path);
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
		Wa2ImageAnimator animator1 = new(_engine.MaskTexture);
		Wa2ImageAnimator animator2 = new(_engine.MaskTexture);
		animator1.InitFade(args[3].Get() * _engine.FrameTime);
		animator2.InitHide(args[3].Get() * _engine.FrameTime);
		ClearChar(args[3].Get() * _engine.FrameTime);
		_engine.BgTexture.SetCurTexture(NextTexture);
	}
	public void H(List<Wa2Var> args)
	{

	}
	public void SetShake(List<Wa2Var> args)
	{

	}
	public void StopShake(List<Wa2Var> args)
	{

	}
	public void F(List<Wa2Var> args)
	{

	}
	public void FB(List<Wa2Var> args)
	{

	}

	public void C(List<Wa2Var> args)
	{
		// _engine.AddChar(args[0], args[1], args[2]);
		// GD.Print("c");
		// if(args[0]>10){

		// }
		_engine.GameSav.AddChar(new CharItem
		{
			id = args[0].Get(),
			no = args[1].Get(),
			pos = args[2].Get(),
		});
		_engine.UpdateChar(args[5].Get() * _engine.FrameTime);
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
	public void CW(List<Wa2Var> args)
	{
		_engine.GameSav.AddChar(new CharItem
		{
			id = args[0].Get(),
			no = args[1].Get(),
			pos = args[2].Get(),
		});
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
	public void CR(List<Wa2Var> args)
	{
		// int pos = _engine.CharDic[args[0]].pos;
		// Wa2Image image = _engine.Chars[pos];
		// image.SetCurTexture(image.GetNextTexture());
		// image.SetNextTexture(null);
		// Wa2Animator animator1 = new(image);
		// // Wa2Animator animator2 = new(image);
		// animator1.InitFade(args[2] * _engine.FrameTime);
		// animator2.InitHide(args[2] * _engine.FrameTime);
		_engine.GameSav.RemoveChar(args[0].Get());
		_engine.UpdateChar(args[2].Get() * _engine.FrameTime);
		// GD.Print(image.GetCurTexture());
		// _engine.RemoveChar(args[0]);
		// _engine.Viewport.Char();
		// _engine.WaitTimer.Start(args[2] * _engine.FrameTime);
	}
	public void CRW(List<Wa2Var> args)
	{
		_engine.GameSav.RemoveChar(args[0].Get());
	}
	public void M(List<Wa2Var> args)
	{
		if (args.Count == 3)
		{
			GD.Print("错误位置", _engine.GameSav.ScriptPos);
		}
		_engine.GameSav.BgmInfo.Id = args[0].Get();
		_engine.GameSav.BgmInfo.Loop = args[2].Get();
		_engine.GameSav.BgmInfo.Volume = args[3].Get();
		_engine.SoundMgr.PlayBgm(args[0].Get(), args[2].Get() != 0, args[3].Get());
	}
	public void MS(List<Wa2Var> args)
	{
		// GD.Print("暂停:", args[0] * _engine.FrameTime);
		_engine.SoundMgr.StopBgm(args[0].Get() * _engine.FrameTime);
		// uint v0 = args[0];
	}
	public void MP(List<Wa2Var> args)
	{

	}
	public void MV(List<Wa2Var> args)
	{
		GD.Print("估计是改变语言音量2个参数");
	}
	public void MW(List<Wa2Var> args)
	{

	}
	public void MLW(List<Wa2Var> args)
	{

	}
	public void SE(List<Wa2Var> args)
	{
		Wa2SoundMgr.Instance.PlaySe(0, args[0].Get(), false, 0, args[1].Get());
	}
	public void SEP(List<Wa2Var> args)
	{
		// GD.Print(args.Count);
		Wa2SoundMgr.Instance.PlaySe(args[0].Get(), args[1].Get(), args[2].Get() != 0, args[3].Get() * _engine.FrameTime, args[4].Get());
	}
	public void SES(List<Wa2Var> args)
	{
		Wa2SoundMgr.Instance.StopSe(args[0].Get(), args[1].Get() * _engine.FrameTime);
	}
	public void SEV(List<Wa2Var> args)
	{
		_engine.SoundMgr.SetSeVolume(args[0].Get(), args[1].Get(), args[2].Get() * _engine.FrameTime);
	}
	public void SEW(List<Wa2Var> args)
	{
		_engine.WaitSe = true;
		_engine.WaitSeChannel = args[0].Get();
		_engine.WaitTimer.Start(_engine.SoundMgr.GetSeTime(args[0].Get()));
	}
	public void SEVW(List<Wa2Var> args)
	{

	}
	public void SetTimeMode(List<Wa2Var> args)
	{
		_engine.GameSav.TimeMode = args[0].Get();
		GD.Print("mode", args[0].Get());
		// uint v0 = args[0];
	}
	public void SetChromaMode(List<Wa2Var> args)
	{
		GD.Print("设置色差");
	}
	public void SetEffctMode(List<Wa2Var> args)
	{
		GD.Print("设置特效模式");
	}
	public void SetWeather(List<Wa2Var> args)
	{
		GD.Print("设置天气");
	}
	public void ChangeWeather(List<Wa2Var> args)
	{
		GD.Print("改变天气");
	}
	public void ResetWeather(List<Wa2Var> args)
	{

	}
	public void LoadBmp(List<Wa2Var> args)
	{
		GD.Print(args[1].Get());
	}
	public void LoadBmpAnime(List<Wa2Var> args)
	{
		GD.Print("加载bmp动画");
	}
	public void SetBmpAvi(List<Wa2Var> args)
	{

	}
	public void WaitBmpAvi(List<Wa2Var> args)
	{

	}
	public void ReleaseBmp(List<Wa2Var> args)
	{
		GD.Print("释放位图");
	}
	public void WaitBmpAnime(List<Wa2Var> args)
	{

	}
	public void SetBmpAnimePlay(List<Wa2Var> args)
	{

	}
	public void SetBmpDisp(List<Wa2Var> args)
	{

	}
	public void SetBmpLayer(List<Wa2Var> args)
	{

	}
	public void SetBmpParam(List<Wa2Var> args)
	{
		GD.Print("设置位图参数");
	}
	public void SetBmpRevParam(List<Wa2Var> args)
	{

	}
	public void SetBmpBright(List<Wa2Var> args)
	{

	}
	public void SetBmpMove(List<Wa2Var> args)
	{
		GD.Print("设置位图移动");
	}
	public void SetBmpPos(List<Wa2Var> args)
	{

	}
	public void SetBmpZoom(List<Wa2Var> args)
	{

	}
	public void SetBmpZoom2(List<Wa2Var> args)
	{
		GD.Print("设置位图缩放");
	}
	public void SetBmpRoll(List<Wa2Var> args)
	{

	}
	public void SetMovie(List<Wa2Var> args)
	{
		GD.Print("视频编号:", args[1].Get());
		if (_engine.ReadSysFlag(args[1].Get())==1)
		{
			_engine.HasPlayMovie=true;
		}
		else
		{
			_engine.HasPlayMovie=false;
			_engine.WirtSysFlag(args[1].Get(), 1);
		}

		GD.Print("视频", Wa2Resource.ResPath + "movie/" + args[0].Get() + "0.mp4");
		_engine.PlayMovie(args[0].Get());

	}
	public void Wait(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(args[0].Get() * _engine.FrameTime);

	}
	public void StartTimer(List<Wa2Var> args)
	{

	}
	public void WaitTimer(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(args[0].Get() * 0.0001f);
		args.Clear();
	}
	public void GoTitle(List<Wa2Var> args)
	{
		_engine.UiMgr.OpenTitleMenu();
	}
	public void GetGameFlag(List<Wa2Var> args)
	{
		args[^1].Set(_engine.ReadSysFlag(args[0].Get()));
	}
	public void SetGameFlag(List<Wa2Var> args)
	{
		GD.Print("flag", args[0].Get());
		_engine.WirtSysFlag(args[0].Get(), args[1].Get());
	}
	public void LogOut(List<Wa2Var> args)
	{

	}
	public void V_Flag(List<Wa2Var> args)
	{
		GD.Print("cg解锁");
	}
	public void H_Flag(List<Wa2Var> args)
	{

	}
	public void Calender(List<Wa2Var> args)
	{
		_engine.GameSav.Calender.Year = args[0].Get();
		_engine.GameSav.Calender.Month = args[1].Get();
		_engine.GameSav.Calender.Day = args[2].Get();
		if (_engine.GameSav.Calender.Year == 2013 && _engine.GameSav.Calender.Month == 2 && _engine.GameSav.Calender.Day == 29)
		{
			_engine.GameSav.Calender.DayOfWeek = 5;
		}
		else
		{
			_engine.GameSav.Calender.DayOfWeek = (int)new DateTime(args[0].Get(), args[1].Get(), args[2].Get()).DayOfWeek;
		}

		// uint hour=args[3];
		// GD.Print("设置日期" + year + "年" + month + "月" + day + "日");
	}
	public void GetTimer(List<Wa2Var> args)
	{

	}
	public void GetSkip(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, 0);
	}
	public void GetClick(List<Wa2Var> args)
	{

	}
	public void runEX(List<Wa2Var> args)
	{

	}
	public void SetSelectMess(List<Wa2Var> args)
	{
		_engine.GameSav.SelectItems.Add(new SelectItem
		{
			Text = args[0].Get(),
			V1 = args[1].Get(),
			V2 = args[2].Get(),
			V3 = args[3].Get(),
		});
	}
	public void SetSelect(List<Wa2Var> args)
	{
		_engine.ShowSelectMessage();
		_engine.SelectVar = args[0];
		// _engine.Script.Wait = true;
	}
	public void S(List<Wa2Var> args)
	{
		Wa2ImageAnimator animator = new(_engine.BgTexture);
		animator.Wait = false;
		animator.InitMove(args[2].Get() * _engine.FrameTime, args[0].Get(), args[1].Get());

	}
	public void Z(List<Wa2Var> args6)
	{

	}
	public void R(List<Wa2Var> args)
	{

	}
	public void WSZ(List<Wa2Var> args)
	{
		GD.Print("等待移动结束");
	}
	public void StopSZR(List<Wa2Var> args)
	{

	}
	public void VA(List<Wa2Var> args)
	{

	}
	public void CS(List<Wa2Var> args)
	{

	}
	public void CM(List<Wa2Var> args)
	{

	}
	public void CRS(List<Wa2Var> args)
	{

	}
	public void SkipOFF(List<Wa2Var> args)
	{

	}
	public void NevelMode(List<Wa2Var> args)
	{

	}
	public void EroMode(List<Wa2Var> args)
	{

	}
	public void GetReplayMode(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.ReplayMode);
	}
	public void WN2(List<Wa2Var> args)
	{

	}
	public void WNS2(List<Wa2Var> args)
	{

	}
	public void B2(List<Wa2Var> args)
	{
		_engine.AnimatorsFinish();
		Texture2D NextTexture;
		Texture2D CeacheTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()); ;
		if (args[3].Get() > 0)
		{
			_engine.GameSav.BgInfo.Frame = args[3].Get();
		}
		if (args[1].Get() >= 0)
		{
			_engine.GameSav.BgInfo.Path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", args[1].Get(), args[2].Get(), _engine.GameSav.TimeMode);
			NextTexture = Wa2Resource.GetTgaImage(_engine.GameSav.BgInfo.Path);
		}
		else
		{
			NextTexture = _engine.BgTexture.GetCurTexture();
		}
		// _engine.MaskTexture.SetCurOffset(_engine.BgTexture.GetCurOffset());
		// _engine.MaskTexture.SetCurScale(_engine.BgTexture.GetCurScale());
		_engine.MaskTexture.SetCurTexture(CeacheTexture);
		_engine.MaskTexture.SetNextTexture(NextTexture);
		_engine.MaskTexture.SetMaskTexture(null);
		Wa2ImageAnimator animator1 = new(_engine.MaskTexture);
		Wa2ImageAnimator animator2 = new(_engine.MaskTexture);
		_engine.MaskTexture.SetCurOffset(Vector2.Zero);
		_engine.MaskTexture.SetCurScale(Vector2.One);
		_engine.GameSav.BgInfo.Offset = new Vector2(args[5].Get() - args[4].Get(), args[6].Get());
		_engine.GameSav.BgInfo.Scale = new Vector2(args[7].Get(), args[8].Get());
		_engine.MaskTexture.SetNextOffset(_engine.GameSav.BgInfo.Offset);
		_engine.MaskTexture.SetNextScale(_engine.GameSav.BgInfo.Scale);

		// _engine.SubViewport.Hide();
		_engine.BgTexture.SetCurOffset(_engine.GameSav.BgInfo.Offset);
		_engine.BgTexture.SetCurScale(_engine.GameSav.BgInfo.Scale);
		_engine.BgTexture.SetCurTexture(NextTexture);
		animator1.InitFade(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
		animator2.InitHide(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
		ClearChar(_engine.GameSav.BgInfo.Frame * _engine.FrameTime);
	}
	public void BC2(List<Wa2Var> args)
	{

	}
	public void V2(List<Wa2Var> args)
	{
		Texture2D NextTexture;

		if (args[1].Get() >= 0)
		{
			_engine.GameSav.BgInfo.Path = string.Format("v{0:D5}{1:D1}.tga", args[1].Get(), args[2].Get());
			NextTexture = Wa2Resource.GetTgaImage(_engine.GameSav.BgInfo.Path);
		}
		else
		{
			NextTexture = ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage());
		}
		_engine.GameSav.BgInfo.Offset = new Vector2(args[5].Get() - args[4].Get(), args[6].Get());
		_engine.GameSav.BgInfo.Scale = new Vector2(args[7].Get(), args[8].Get());
		_engine.MaskTexture.SetNextOffset(_engine.GameSav.BgInfo.Offset);
		_engine.MaskTexture.SetNextScale(_engine.GameSav.BgInfo.Scale);
		_engine.BgTexture.SetCurOffset(_engine.GameSav.BgInfo.Offset);
		_engine.BgTexture.SetCurScale(_engine.GameSav.BgInfo.Scale);
		_engine.MaskTexture.SetCurTexture(ImageTexture.CreateFromImage(_engine.Viewport.GetTexture().GetImage()));
		_engine.MaskTexture.SetNextTexture(NextTexture);
		_engine.MaskTexture.SetMaskTexture(null);
		Wa2ImageAnimator animator1 = new(_engine.MaskTexture);
		Wa2ImageAnimator animator2 = new(_engine.MaskTexture);
		animator1.InitFade(args[3].Get() * _engine.FrameTime);
		animator2.InitHide(args[3].Get() * _engine.FrameTime);
		ClearChar(args[3].Get() * _engine.FrameTime);
		_engine.BgTexture.SetCurTexture(NextTexture);
	}
	public void H2(List<Wa2Var> args)
	{

	}
	public void SetWeather2(List<Wa2Var> args)
	{

	}
	public void ChangeWeather2(List<Wa2Var> args)
	{

	}
	public void SetShake2(List<Wa2Var> args)
	{
		GD.Print("设置震动");
	}
	public void M2(List<Wa2Var> args)
	{

	}
	public void NB(List<Wa2Var> args)
	{

	}
	public void NBR(List<Wa2Var> args)
	{

	}
	public void VXV(List<Wa2Var> args)
	{

	}
	public void Wait2(List<Wa2Var> args)
	{
		_engine.WaitTimer.Start(args[0].Get() * _engine.FrameTime);
		// GD.Print("暂停游戏" + args[0] + "帧");
	}
	public void IfSkip(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.Skipping ? 1 : 0);
	}
	public void GetSkip2(List<Wa2Var> args)
	{
		_engine.Script.PushInt(5, 3, _engine.SkipMode ? 1 : 0);
	}
	public void SLoad(List<Wa2Var> args)
	{
		_engine.Reset();
		_engine.Script.LoadScript(args[0].Get(), (uint)args[1].Get());

	}
	public void SCall(List<Wa2Var> args)
	{
		GD.Print("无用函数");
	}
	public void call(List<Wa2Var> args)
	{
		GD.Print("无用函数");
	}
	public void run(List<Wa2Var> args)
	{
		GD.Print("RUN");
	}
	public void print(List<Wa2Var> args)
	{
		GD.Print("打印函数");
	}
	public void Ret(List<Wa2Var> args)
	{

	}
	public void _int(List<Wa2Var> args)
	{

	}
	public void _float(List<Wa2Var> args)
	{

	}
	public void Rand(List<Wa2Var> args)
	{

	}
	public void Sin(List<Wa2Var> args)
	{
		args[^1].Set(Mathf.Sin(args[0].Get()));
	}
	public void Cos(List<Wa2Var> args)
	{

	}
	public void Tan(List<Wa2Var> args)
	{

	}
	public void Asin(List<Wa2Var> args)
	{

	}
	public void Acos(List<Wa2Var> args)
	{

	}
	public void Atan(List<Wa2Var> args)
	{

	}
	public void Atan2(List<Wa2Var> args)
	{

	}
	public void Pow(List<Wa2Var> args)
	{

	}
	public void Sqrt(List<Wa2Var> args)
	{

	}
	public void TimeGetTime(List<Wa2Var> args)
	{

	}
}
