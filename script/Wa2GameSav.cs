using Godot;
using System;
using System.Collections.Generic;
using System.Text;
public struct Calender{
	public int Year;
	public int Month;
	public int Day;
	public int DayOfWeek;
}
public struct BgmInfo{
	public int Id;
	public int Loop;
	public int Volume;
}
public struct SelectItem
{
	public int StrRef;
	public bool Disable;
	public bool Show;
}
public struct BgInfo{
	public Vector2 Scale;
	public Vector2 Offset;
	public int Id;
	public int No;
	public int Frame;
	public int V0;
	public int V1;
	public int V2;
	public int V3;
	public int V4;
	public BgInfo(){
		Scale=Vector2.One;
		Offset=Vector2.Zero;
		Frame=120;
	}
}
public struct CharItem
{
	public int pos;
	public int id;
	public int no;
}
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
	public int TimeMode;
	public BgmInfo BgmInfo=new();
	public BgInfo BgInfo=new();
	public float BgTime;
	public int[] GloInts = new int[26];
	public float[] GloFloats = new float[26];
	public int[] GameFlags = new int[1024];
	public int GameDate;
	public DateTime SystemTime;
	public int CurBg;
	public byte[] ImageBuffer = new byte[0x9000];
	public string ScriptName;
	public string ScriptPak;
	public uint ScriptPos;
	public Calender Calender=new();
	public string FirstSentence;
	private Wa2EngineMain _engine;
	public List<CharItem> CharItems = new();
	public Wa2GameSav(Wa2EngineMain e){
		_engine=e;
		
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
			if (CharItems[i].id == item.id)
			{
				CharItems[i] = item;
				return;
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
	public void SaveData(int idx){
		FileAccess file=FileAccess.Open(string.Format("user://sav{0:D2}.sav",idx),FileAccess.ModeFlags.Write);
		SystemTime=DateTime.Now;
		Image image=_engine.Viewport.GetTexture().GetImage();
		image.Resize(256,144);
		file.Store32((uint)SystemTime.Year);
		file.Store32((uint)SystemTime.Month);
		file.Store32((uint)SystemTime.DayOfWeek);
		file.Store32((uint)SystemTime.Day);
		file.Store32((uint)SystemTime.Hour);
		file.Store32((uint)SystemTime.Minute);
		file.Store32((uint)SystemTime.Second);
		file.Store32((uint)SystemTime.Millisecond);
		file.StoreBuffer(image.GetData());
	  file.StoreString(ScriptName);
		byte[] buffer=new byte[256];
		byte[] strBuffer=Encoding.Unicode.GetBytes(FirstSentence);
		GD.Print(FirstSentence);
	  Array.Copy(strBuffer,buffer,Math.Min(strBuffer.Length, 256));
		file.StoreBuffer(buffer);
		file.Close();
	}
	public void LoadData(int idx){

	}
}