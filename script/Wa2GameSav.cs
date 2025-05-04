using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public struct Calender
{
	public int Year;
	public int Month;
	public int Day;
	public int DayOfWeek;
}
public struct BgmInfo
{
	public int Id;
	public int Idx;
	public int Loop;
	public int Volume;
}
public struct SelectItem
{
	public string Text;
	public int V1;
	public int V2;
	public int V3;
}
public struct BgInfo
{
	public Vector2 Scale;
	public Vector2 Offset;
	public string Path;
	public int Frame;
	public int V0;
	public int V1;
	public int V2;
	public int V3;
	public int V4;
	public BgInfo()
	{
		Scale = Vector2.One;
		Offset = Vector2.Zero;
		Frame = 0;
	}
}
public struct CharItem
{
	public int pos;
	public int id;
	public int no;
}
public class VoiceInfo
{
	public int Chr;
	public int Id;
	public int Label;
	public int Volume;
}
// public struct FirstCmd{

// }
// public struct SystemTime
// {
// 	public int Year;
// 	public int Month;
// 	public int DayOfWeek;
// 	public int Day;
// 	public int Hour;
// 	public int Minute;
// 	public int Second;
// 	public int Milliseconds;
// }
public class Wa2GameSav
{
	// public string EffectMode="";
	// public int TimeMode;
	// public int Label=-1;
	// public int Weather;
	// public BgmInfo BgmInfo = new();
	// public BgInfo BgInfo = new();
	// public int[] GameFlags = new int[0x1d];
	// public DateTime SystemTime;
	// public byte[] ImageBuffer = new byte[0x9000];
	// public string ScriptName;
	// public string ScriptPak;
	// public uint ScriptPos;
	// public List<SelectItem> SelectItems = new();
	// public Calender Calender = new();
	// public string FirstSentence;
	// public string CharName;
	// public int CurMessageIdx;
	// public List<CharItem> CharItems = new();
	private Wa2EngineMain _engine;


	// public int StartTime;
	// public void Reset()
	// {

	// 	// args.Clear();
	// 	// GameFlags = new int[0x1d];
	// 	// GloFloats = new float[26];
	// 	// GloInts = new int[26];
	// 	// JumpEntrys.Clear();
	// 	CharItems.Clear();
	// 	SelectItems.Clear();
	// 	// BgmInfo = new();
	// 	// BgInfo = new();
	// 	FirstSentence = "";
	// 	CharName = "";
	// 	// EffectMode="";
	// 	// StartTime=0;


	// }
	public Wa2GameSav(Wa2EngineMain e)
	{
		_engine = e;
		// Reset();

		// GD.Print(SystemTime.Year+"年");
		// GD.Print(SystemTime.Month+"月");
		// GD.Print(SystemTime.DayOfWeek+"周");
		// GD.Print(SystemTime.Day+"日");
		// GD.Print(SystemTime.Hour+"小时");
		// GD.Print(SystemTime.Minute+"分钟");
		// GD.Print(SystemTime.Second+"秒");
		// GD.Print(SystemTime.Millisecond+"毫秒");

	}

	public void SaveData(int idx)
	{
		FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", idx), FileAccess.ModeFlags.Write);
		DateTime SystemTime = DateTime.Now;
		Image image = _engine.Viewport.GetTexture().GetImage();
		image.Resize(256, 144);
		file.Store32((uint)SystemTime.Year);
		file.Store32((uint)SystemTime.Month);
		file.Store32((uint)SystemTime.DayOfWeek);
		file.Store32((uint)SystemTime.Day);
		file.Store32((uint)SystemTime.Hour);
		file.Store32((uint)SystemTime.Minute);
		file.Store32((uint)SystemTime.Second);
		file.Store32((uint)SystemTime.Millisecond);
		file.StoreBuffer(image.GetData());
		file.StoreBuffer([.. Encoding.ASCII.GetBytes(_engine.Script.ScriptName).Concat(new byte[8]).Take(8)]);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(_engine.AdvMain.TextLabel.Text).Concat(new byte[256]).Take(256)]);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(_engine.AdvMain.NameLabel.Text).Concat(new byte[16]).Take(16)]);
		file.Store32(_engine.Script.ScriptPos);
		for (int i = 0; i < _engine.GameFlags.Length; i++)
		{
			file.Store32((uint)_engine.GameFlags[i]);
		}
		for (int i = 0; i < _engine.Script.GloInts.Length; i++)
		{
			file.Store32((uint)_engine.Script.GloInts[i]);
		}
		for (int i = 0; i < _engine.Script.GloFloats.Length; i++)
		{
			file.StoreFloat(_engine.Script.GloFloats[i]);
		}
		file.Store32((uint)_engine.Script.JumpEntrys.Count);
		for (int i = 0; i < _engine.Script.JumpEntrys.Count; i++)
		{

			file.Store32(_engine.Script.JumpEntrys[i].Type);
			file.Store32(_engine.Script.JumpEntrys[i].Count);
			file.Store32(_engine.Script.JumpEntrys[i].Pos);
			file.Store32((uint)_engine.Script.JumpEntrys[i].Flag);
			for (int k = 0; k < 64; k++)
			{
				file.Store32(_engine.Script.JumpEntrys[i].PosArr[k]);
			}
			for (int k = 0; k < 64; k++)
			{
				file.Store32(_engine.Script.JumpEntrys[i].FlagArr[k]);
			}
		}
		file.Store32((uint)_engine.Script.Args.Count);
		for (int i = 0; i < _engine.Script.Args.Count; i++)
		{

			file.Store32((uint)_engine.Script.Args[i].CmdType);
			file.Store32((uint)_engine.Script.Args[i].ValType);
			file.Store32((uint)_engine.Script.Args[i].Value0);
			file.Store32((uint)_engine.Script.Args[i].IntValue);
			file.Store32((uint)_engine.Script.Args[i].FloatValue);
		}
		file.Store32((uint)_engine.CharItems.Count);
		for (int i = 0; i < _engine.CharItems.Count; i++)
		{

			file.Store32((uint)_engine.CharItems[i].pos);
			file.Store32((uint)_engine.CharItems[i].id);
			file.Store32((uint)_engine.CharItems[i].no);

		}
		file.Store32((uint)_engine.SelectItems.Count);
		for (int i = 0; i < _engine.SelectItems.Count; i++)
		{
			file.StoreBuffer([.. Encoding.Unicode.GetBytes(_engine.SelectItems[i].Text).Concat(new byte[64]).Take(64)]);
			file.Store32((uint)_engine.SelectItems[i].V1);
			file.Store32((uint)_engine.SelectItems[i].V2);
			file.Store32((uint)_engine.SelectItems[i].V3);
		}
		file.Store32((uint)_engine.Calender.Year);
		file.Store32((uint)_engine.Calender.Month);
		file.Store32((uint)_engine.Calender.Day);
		file.Store32((uint)_engine.Calender.DayOfWeek);
		file.Store32((uint)_engine.TimeMode);
		file.Store32((uint)_engine.Label);
		file.Store32((uint)_engine.Weather);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(_engine.BgInfo.Path).Concat(new byte[32]).Take(32)]);
		file.StoreFloat(_engine.BgInfo.Scale.X);
		file.StoreFloat(_engine.BgInfo.Scale.Y);
		file.StoreFloat(_engine.BgInfo.Offset.X);
		file.StoreFloat(_engine.BgInfo.Offset.Y);
		file.Store32((uint)_engine.BgInfo.Frame);
		file.Store32((uint)_engine.BgInfo.V0);
		file.Store32((uint)_engine.BgInfo.V1);
		file.Store32((uint)_engine.BgInfo.V2);
		file.Store32((uint)_engine.BgInfo.V3);
		file.Store32((uint)_engine.BgInfo.V4);
		file.Store32((uint)_engine.BgmInfo.Id);
		file.Store32((uint)_engine.BgmInfo.Loop);
		file.Store32((uint)_engine.BgmInfo.Volume);
		file.Store32((uint)_engine.CurMessageIdx);
		file.StoreBuffer([.. Encoding.ASCII.GetBytes(_engine.EffectMode).Concat(new byte[16]).Take(16)]);
		file.Close();
	}
	public void LoadData(int idx)
	{
		GD.Print("位置", idx);
		_engine.Reset();
		FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", idx), FileAccess.ModeFlags.Read);
		file.Seek(0x1b000 + 32);
		_engine.JumpScript(file.GetBuffer(8).GetStringFromAscii().Replace("\0", ""));
		_engine.AdvMain.TextLabel.Text = Encoding.Unicode.GetString(file.GetBuffer(256)).Replace("\0", "");
		_engine.AdvMain.NameLabel.Text = Encoding.Unicode.GetString(file.GetBuffer(16)).Replace("\0", "");
		_engine.Script.ScriptPos = file.Get32();
		GD.Print("脚本名:", _engine.Script.ScriptName);
		GD.Print("脚本位置:", _engine.Script.ScriptPos);
		for (int i = 0; i < 0x1d; i++)
		{
			_engine.GameFlags[i] = (int)file.Get32();
		}
		for (int i = 0; i < 26; i++)
		{
			_engine.Script.GloInts[i] = (int)file.Get32();
		}
		for (int i = 0; i < 26; i++)
		{
			_engine.Script.GloFloats[i] = file.GetFloat();
		}

		int JumpEntryCount = (int)file.Get32();
		GD.Print("跳转数量:", JumpEntryCount);
		for (int i = 0; i < JumpEntryCount; i++)
		{

			_engine.Script.JumpEntrys.Add(new());
			_engine.Script.JumpEntrys[i].Type = file.Get32();
			_engine.Script.JumpEntrys[i].Count = file.Get32();
			_engine.Script.JumpEntrys[i].Pos = file.Get32();
			_engine.Script.JumpEntrys[i].Flag = (int)file.Get32();
			for (int k = 0; k < 64; k++)
			{
				_engine.Script.JumpEntrys[i].PosArr[k] = file.Get32();
			}
			for (int k = 0; k < 64; k++)
			{
				_engine.Script.JumpEntrys[i].FlagArr[k] = file.Get32();
			}

		}
		int ArgsCount = (int)file.Get32();
		for (int i = 0; i < ArgsCount; i++)
		{
			_engine.Script.Args.Add(new());
			_engine.Script.Args[i].CmdType = (CmdType)file.Get32();
			_engine.Script.Args[i].ValType = (ValueType)file.Get32();
			_engine.Script.Args[i].Value0 = (int)file.Get32();
			_engine.Script.Args[i].IntValue = (int)file.Get32();
			_engine.Script.Args[i].FloatValue = file.GetFloat();
		}

		int charCount = (int)file.Get32();
		GD.Print("角色数", charCount);
		for (int i = 0; i < charCount; i++)
		{

			_engine.CharItems.Add(new CharItem()
			{
				pos = (int)file.Get32(),
				id = (int)file.Get32(),
				no = (int)file.Get32()
			});
		}
		int selectCount = (int)file.Get32();
		// GD.Print("选项数量:", selectCount);
		for (int i = 0; i < selectCount; i++)
		{

			_engine.SelectItems.Add(new SelectItem()
			{
				Text = Encoding.Unicode.GetString(file.GetBuffer(64)).Replace("\0", ""),
				V1 = (int)file.Get32(),
				V2 = (int)file.Get32(),
				V3 = (int)file.Get32()
			});
		}
		_engine.Calender = new Calender()
		{
			Year = (int)file.Get32(),
			Month = (int)file.Get32(),
			Day = (int)file.Get32(),
			DayOfWeek = (int)file.Get32()
		};
		_engine.TimeMode = (int)file.Get32();
		_engine.Label = (int)file.Get32();
		_engine.Weather = (int)file.Get32();
		_engine.BgInfo.Path = Encoding.Unicode.GetString(file.GetBuffer(32)).Replace("\0", "");


		_engine.BgInfo.Scale.X = file.GetFloat();
		_engine.BgInfo.Scale.Y = file.GetFloat();
		_engine.BgInfo.Offset.X = file.GetFloat();
		_engine.BgInfo.Offset.Y = file.GetFloat();
		_engine.BgInfo.Frame = (int)file.Get32();
		_engine.BgInfo.V0 = (int)file.Get32();
		_engine.BgInfo.V1 = (int)file.Get32();
		_engine.BgInfo.V2 = (int)file.Get32();
		_engine.BgInfo.V3 = (int)file.Get32();
		_engine.BgInfo.V4 = (int)file.Get32();
		_engine.BgmInfo.Id = (int)file.Get32();
		_engine.BgmInfo.Loop = (int)file.Get32();
		_engine.BgmInfo.Volume = (int)file.Get32();
		_engine.CurMessageIdx = (int)file.Get32();
		_engine.EffectMode = file.GetBuffer(16).GetStringFromAscii().Replace("\0", "");
		if (selectCount > 0)
		{
			_engine.ShowSelectMessage();
		}
		_engine.AdvMain.ShowText(false);
		_engine.SoundMgr.PlayBgm(_engine.BgmInfo.Id, _engine.BgmInfo.Loop != 0, _engine.BgmInfo.Volume);
		_engine.BgTexture.SetCurTexture(Wa2Resource.GetTgaImage(_engine.BgInfo.Path));
		_engine.BgTexture.SetCurScale(_engine.BgInfo.Scale);
		_engine.BgTexture.SetCurOffset(_engine.BgInfo.Offset);
		_engine.UpdateChar(0f);
		_engine.HasReadMessage = true;
		_engine.Backlogs.Clear();
		file.Close();
	}
}