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
		Frame = 120;
	}
}
public struct CharItem
{
	public int pos;
	public int id;
	public int no;
}
public class VoiceInfo{
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
	public string EffectMode="";
	public int TimeMode;
	public int Label=-1;
	public List<Wa2Var> args = new();
	public List<JumpEntry> JumpEntrys = new();
	public int Weather;
	public BgmInfo BgmInfo = new();
	public BgInfo BgInfo = new();
	public int[] GloInts = new int[26];
	public float[] GloFloats = new float[26];
	public int[] GameFlags = new int[0x1d];
	public DateTime SystemTime;
	public byte[] ImageBuffer = new byte[0x9000];
	public string ScriptName;
	public string ScriptPak;
	public uint ScriptPos;
	public List<SelectItem> SelectItems = new();
	public Calender Calender = new();
	public string FirstSentence;
	public string CharName;
	private Wa2EngineMain _engine;
	public int CurMessageIdx;
	public List<CharItem> CharItems = new();
	// public int StartTime;
	public void Reset()
	{

		args.Clear();
		GameFlags = new int[0x1d];
		GloFloats = new float[26];
		GloInts = new int[26];
		JumpEntrys.Clear();
		CharItems.Clear();
		SelectItems.Clear();
		BgmInfo = new();
		BgInfo = new();
		FirstSentence = "";
		CharName = "";
		EffectMode="";
		// StartTime=0;
		

	}
	public Wa2GameSav(Wa2EngineMain e)
	{
		_engine = e;
		Reset();

		// GD.Print(SystemTime.Year+"年");
		// GD.Print(SystemTime.Month+"月");
		// GD.Print(SystemTime.DayOfWeek+"周");
		// GD.Print(SystemTime.Day+"日");
		// GD.Print(SystemTime.Hour+"小时");
		// GD.Print(SystemTime.Minute+"分钟");
		// GD.Print(SystemTime.Second+"秒");
		// GD.Print(SystemTime.Millisecond+"毫秒");

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
	public void SaveData(int idx)
	{
		FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", idx), FileAccess.ModeFlags.Write);
		SystemTime = DateTime.Now;
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
		file.StoreBuffer([.. Encoding.ASCII.GetBytes(ScriptName).Concat(new byte[8]).Take(8)]);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(FirstSentence).Concat(new byte[256]).Take(256)]);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(CharName).Concat(new byte[16]).Take(16)]);
		file.Store32(ScriptPos);
		for (int i = 0; i < GameFlags.Length; i++)
		{
			file.Store32((uint)GameFlags[i]);
		}
		for (int i = 0; i < GloInts.Length; i++)
		{
			file.Store32((uint)GloInts[i]);
		}
		for (int i = 0; i < GloFloats.Length; i++)
		{
			file.Store32((uint)GloFloats[i]);
		}
		file.Store32((uint)JumpEntrys.Count);
		for (int i = 0; i < JumpEntrys.Count; i++)
		{

			file.Store32(JumpEntrys[i].Type);
			file.Store32(JumpEntrys[i].Count);
			file.Store32(JumpEntrys[i].Pos);
			file.Store32((uint)JumpEntrys[i].Flag);
			for (int k = 0; k < 64; k++)
			{
				file.Store32(JumpEntrys[i].PosArr[k]);
			}
			for (int k = 0; k < 64; k++)
			{
				file.Store32(JumpEntrys[i].FlagArr[k]);
			}
		}
		file.Store32((uint)args.Count);
		for (int i = 0; i < args.Count; i++)
		{

			file.Store32((uint)args[i].CmdType);
			file.Store32((uint)args[i].ValType);
			file.Store32((uint)args[i].Value0);
			file.Store32((uint)args[i].IntValue);
			file.Store32((uint)args[i].FloatValue);
		}
		file.Store32((uint)CharItems.Count);
		for (int i = 0; i < CharItems.Count; i++)
		{

			file.Store32((uint)CharItems[i].pos);
			file.Store32((uint)CharItems[i].id);
			file.Store32((uint)CharItems[i].no);

		}
		file.Store32((uint)SelectItems.Count);
		for (int i = 0; i < SelectItems.Count; i++)
		{
			file.StoreBuffer([.. Encoding.Unicode.GetBytes(SelectItems[i].Text).Concat(new byte[64]).Take(64)]);
			file.Store32((uint)SelectItems[i].V1);
			file.Store32((uint)SelectItems[i].V2);
			file.Store32((uint)SelectItems[i].V3);
		}
		file.Store32((uint)Calender.Year);
		file.Store32((uint)Calender.Month);
		file.Store32((uint)Calender.Day);
		file.Store32((uint)Calender.DayOfWeek);
		file.Store32((uint)TimeMode);
		file.Store32((uint)Label);
		file.Store32((uint)Weather);
		file.StoreBuffer([.. Encoding.Unicode.GetBytes(BgInfo.Path).Concat(new byte[32]).Take(32)]);
		file.StoreFloat(BgInfo.Scale.X);
		file.StoreFloat(BgInfo.Scale.Y);
		file.StoreFloat(BgInfo.Offset.X);
		file.StoreFloat(BgInfo.Offset.Y);
		file.Store32((uint)BgInfo.Frame);
		file.Store32((uint)BgInfo.V0);
		file.Store32((uint)BgInfo.V1);
		file.Store32((uint)BgInfo.V2);
		file.Store32((uint)BgInfo.V3);
		file.Store32((uint)BgInfo.V4);
		file.Store32((uint)BgmInfo.Id);
		file.Store32((uint)BgmInfo.Loop);
		file.Store32((uint)BgmInfo.Volume);
		file.Store32((uint)CurMessageIdx);
		file.StoreBuffer([.. Encoding.ASCII.GetBytes(EffectMode).Concat(new byte[16]).Take(16)]);
		file.Close();
	}
	public void LoadData(int idx)
	{
		GD.Print("位置", idx);
		_engine.Reset();
		Reset();
		FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", idx), FileAccess.ModeFlags.Read);
		file.Seek(0x1b000 + 32);
		ScriptName = file.GetBuffer(8).GetStringFromAscii().Replace("\0", "");
		FirstSentence = Encoding.Unicode.GetString(file.GetBuffer(256)).Replace("\0", "");
		CharName = Encoding.Unicode.GetString(file.GetBuffer(16)).Replace("\0", "");
		_engine.Script.LoadScript(ScriptName);
		ScriptPos = file.Get32();
		GD.Print("脚本名:", ScriptName);
		GD.Print("脚本位置:", ScriptPos);
		GD.Print("第一句话:", FirstSentence);
		GD.Print("角色名:", CharName);
		for (int i = 0; i < 0x1d; i++)
		{
			GameFlags[i] = (int)file.Get32();
		}
		for (int i = 0; i < 26; i++)
		{
			GloInts[i] = (int)file.Get32();
		}
		for (int i = 0; i < 26; i++)
		{
			GloFloats[i] = file.GetFloat();
		}

		int JumpEntryCount = (int)file.Get32();
		GD.Print("跳转数量:", JumpEntryCount);
		for (int i = 0; i < JumpEntryCount; i++)
		{

			JumpEntrys.Add(new());
			JumpEntrys[i].Type = file.Get32();
			JumpEntrys[i].Count = file.Get32();
			JumpEntrys[i].Pos = file.Get32();
			JumpEntrys[i].Flag = (int)file.Get32();
			for (int k = 0; k < 64; k++)
			{
				JumpEntrys[i].PosArr[k] = file.Get32();
			}
			for (int k = 0; k < 64; k++)
			{
				JumpEntrys[i].FlagArr[k] = file.Get32();
			}

		}
		int ArgsCount = (int)file.Get32();
		GD.Print("args数量:", ArgsCount);
		for (int i = 0; i < ArgsCount; i++)
		{
			args.Add(new());
			args[i].CmdType = (CmdType)file.Get32();
			args[i].ValType = (ValueType)file.Get32();
			args[i].Value0 = (int)file.Get32();
			args[i].IntValue = (int)file.Get32();
			args[i].FloatValue = file.GetFloat();
		}

		int charCount = (int)file.Get32();
		GD.Print("角色数", charCount);
		for (int i = 0; i < charCount; i++)
		{

			CharItems.Add(new CharItem()
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

			SelectItems.Add(new SelectItem()
			{
				Text = Encoding.Unicode.GetString(file.GetBuffer(64)).Replace("\0", ""),
				V1 = (int)file.Get32(),
				V2 = (int)file.Get32(),
				V3 = (int)file.Get32()
			});
		}
		Calender = new Calender()
		{
			Year = (int)file.Get32(),
			Month = (int)file.Get32(),
			Day = (int)file.Get32(),
			DayOfWeek = (int)file.Get32()
		};
		TimeMode = (int)file.Get32();
		Label = (int)file.Get32();
		Weather = (int)file.Get32();
		BgInfo.Path = Encoding.Unicode.GetString(file.GetBuffer(32)).Replace("\0", "");


		BgInfo.Scale.X = file.GetFloat();
		BgInfo.Scale.Y =file.GetFloat();
		BgInfo.Offset.X =file.GetFloat();
		BgInfo.Offset.Y =file.GetFloat();
		BgInfo.Frame = (int)file.Get32();
		BgInfo.V0 = (int)file.Get32();
		BgInfo.V1 = (int)file.Get32();
		BgInfo.V2 = (int)file.Get32();
		BgInfo.V3 = (int)file.Get32();
		BgInfo.V4 = (int)file.Get32();
		BgmInfo.Id = (int)file.Get32();
		BgmInfo.Loop = (int)file.Get32();
		BgmInfo.Volume = (int)file.Get32();
		CurMessageIdx= (int)file.Get32();
		EffectMode=file.GetBuffer(16).GetStringFromAscii().Replace("\0", "");
		// StartTime = (int)file.Get32();
		// GD.Print(CurMessageIdx);
		// GD.Print(StartTime);
		if (selectCount > 0)
		{
			_engine.ShowSelectMessage();
			// _engine.SelectVar = args[0];
		}
		else
		{
			_engine.WaitClick = true;

		}
		_engine.AdvMain.ShowText(FirstSentence, CharName, false);
		_engine.SoundMgr.PlayBgm(BgmInfo.Id, BgmInfo.Loop != 0, BgmInfo.Volume);
		_engine.BgTexture.SetCurTexture(Wa2Resource.GetTgaImage(BgInfo.Path));
		_engine.BgTexture.SetCurScale(BgInfo.Scale);
		_engine.BgTexture.SetCurOffset(BgInfo.Offset);
		_engine.UpdateChar(0f);
		_engine.HasReadMessage=true;
		_engine.Backlogs.Clear();
		file.Close();
	}
}