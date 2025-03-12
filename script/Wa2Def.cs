using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public static class Wa2Def
{
	public static Dictionary<char, int> FontMap = new();
	public static readonly int[] CharPos = new int[]{
		-290,
		0,
		290,
		-380,
		380,
		-320,
		320,
		-480,
		480,
		-160,
		160
	};
	public static Dictionary<int, string> CharDict = new(){
		{0, "har"},
		{1, "kaz"},
		{2, "set"},
		{3, "koh"},
		{4, "izu"},
		{5, "mar"},
		{10, "tak"},
		{11, "ioo"},
		{12, "chi"},
		{13, "pap"},
		{14, "mam"},
		{15, "oto"},
		{16, "you"},
		{17, "tan"},
		{18, "shi"},
		{19, "tom"},
		{20,"sat"},
		{21,"hon"},
		{22,"nak"},
		{23,"say"},
		{24,"aco"},
		{25,"mih"},
		{26,"mhh"},
		{27,"ueh"},
		{28,"yos"},
		{30,"ham"},
		{31,"mat"},
		{32,"kiz"},
		{33,"suz"},
		{34,"saw"},
		{35,"miy"},
		{36,"yan"},
		{37,"mas"},
	};
	public static void LoadFontMap()
	{
		byte[] buffer = FileAccess.GetFileAsBytes("res://assets/font.map");
//new EncoderReplacementFallback(strUniRepChr), new DecoderReplacementFallback(strUniRepChr)
		// GD.Print("长度",Encoding.GetEncoding("shift_jis").GetString(buffer).Length);
		string str = Wa2EngineMain.Engine.Wa2Encoding.GetString(buffer).Replace("\n", "").Replace("\r", "");
		GD.Print(str);
		for (int i = 0; i < str.Length; i++)
		{
			FontMap.TryAdd(str[i], i);
		}
	}
}
