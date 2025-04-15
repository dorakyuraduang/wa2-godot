using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public static class Wa2Def
{
	public static int[][] CgSlot = [
[100100],
[100200],
[100300,100301],
[100400],
[100500],
[100600],
[102300],
[102500],
[102700],
[100700],
[102600],
[100800,100801,100802],
[101000],
[101100],
[103300],
[101200,101201,101202,101203,101204,101205,101206,101207],
[101300],
[102400],
[101400],
[101500],
[101600],
[101700,101701],
[101800],
[101900,101901],
[102000,102001],
[102200,102201,102202,102203,102204],
[103200],
[],
[],
[],
[],
[],
[],
[],
[],
[],
[200000,200001,200002],
[200100,200101,200102,200103,200104],
[200200],
[200300],
[211200,211201],
[211400],
[200400],
[201800,201801],
[200500],
[200501],
[200600],
[205600,205601],
[200700,200701],
[200201],
[200302],
[200800],
[200801,200802,200803,200804],
[200900,200901],
[201000,201001],
[201101,201100,201103,201104,201102],
[201200],
[201300,201301],
[201400],
[201500,201501],
[201600,201601],
[201700,201701],
[201900],
[202000],
[202100],
[220000],
[202200,202201,202202],
[202300,202301],
[202400,202401,202402],
[202500],
[202600],
[202700,202701,202702,202703,202704],
[202800,202801,202802],
[202900,202901],
[203000],
[203100],
[203200],
[203300],
[203400,203401,203402,203403,203404,203405,203406],
[203500],
[203600],
[203700,203701],
[203800],
[203900,203901],
[204000,204001],
[204100,204101],
[204200],
[204300],
[204400],
[204500],
[204600,204601],
[204700],
[220400],
[204800],
[211300],
[204900],
[205000,205001,205002,205003],
[205100,205101,205102],
[205200,205201],
[205300,205301,205302],
[205400,205401,205402],
[205500,205501],
[205700],
[205800,205801],
[],
[],
[],
[],
[205900,205901,205902,205903,205904],
[206000],
[206100,206101,206102],
[206200],
[206300],
[206400],
[206500],
[206600,206601],
[206700,206800,206801,206802],
[206900,206901],
[207000,207001,207002],
[207100],
[207200],
[207300],
[207400],
[220500],
[220100],
[207600],
[220600,220601],
[208000],
[208100,208101,208102],
[208200],
[208300],
[208400],
[208500],
[208600,208601,208603,208602],
[208700,208702,208701],
[208800],
[208900],
[209000],
[209100,209101],
[209200],
[209300],
[209400],
[209500],
[207003,207004],
[209700],
[209701],
[209702,209703],
[209800],
[209900,209901,209902],
[210000,210001,210002],
[210100],
[210200],
[210300,210301],
[210400,210401,210402,210403,210404,210405,210406,210407],
[210500,210501,210502,210503],
[210600,210601,210602,210603,210604,210605],
[210700,210701],
[210800,210801],
[210900],
[211000,211001],
[220200,220201],
[211100],
[],
[],
[],
[],
[],
[]
];
	public static string[] ScriptList = [
		"1001",
		"1002",
		"1003",
		"1004",
		"1005",
		"1006",
		"1007",
		"1008",
		"1008_020",
		"1008_030",
		"1008_040",
		"1008_050",
		"1009",
		"1009_020",
		"1009_030",
		"1010",
		"1010_020",
		"1010_030",
		"1010_040",
		"1010_050",
		"1010_060",
		"1010_070",
		"1011",
		"1011_020",
		"1011_030",
		"1012",
		"1012_020",
		"1012_030",
		"1013",
		"catch",
		"catch2",
		"catch3",
		"1012_030_2",
		"2001",
		"2002",
		"2003",
		"2004",
		"2005",
		"2006",
		"2007",
		"2008",
		"2009",
		"2010",
		"2011",
		"2012",
		"2013",
		"2014",
		"2015",
		"2016",
		"2017",
		"2018",
		"2019",
		"2020",
		"2021",
		"2022",
		"2023",
		"2024",
		"2025",
		"2026",
		"2027",
		"2028",
		"2029",
		"2030",
		"2031",
		"2032",
		"2033",
		"2301",
		"2302",
		"2303",
		"2304",
		"2305",
		"2306",
		"2307",
		"2308",
		"2309",
		"2310",
		"2311",
		"2312",
		"2313",
		"2314",
		"2315",
		"2316",
		"2317",
		"2318",
		"2319",
		"2320",
		"2321",
		"2322",
		"2401",
		"2402",
		"2403",
		"2404",
		"2405",
		"2406",
		"2407",
		"2408",
		"2409",
		"2410",
		"2411",
		"2412",
		"2413",
		"2501",
		"2502",
		"2503",
		"2504",
		"2505",
		"2506",
		"2507",
		"2508",
		"2509",
		"2510",
		"2511",
		"2512",
		"2513",
		"2514",
		"2515",
		"2516",
		"2517",
		"3001",
		"3002",
		"3003",
		"3004",
		"3005",
		"3006",
		"3007",
		"3008",
		"3009",
		"3010",
		"3011",
		"3012",
		"3013",
		"3014",
		"3014_2",
		"3014_3",
		"3015",
		"3016",
		"3017",
		"3018",
		"3019",
		"3020",
		"3021",
		"3022",
		"3023",
		"3024",
		"3101",
		"3102",
		"3103",
		"3104",
		"3105",
		"3106",
		"3107",
		"3108",
		"3109",
		"3110",
		"3111",
		"3201",
		"3202",
		"3203",
		"3204",
		"3205",
		"3206",
		"3207",
		"3208",
		"3209",
		"3210",
		"3211",
		"3901",
		"3902",
		"3903",
		"3904",
		"3905",
		"3906",
		"3907",
		"3908",
		"3909",
		"5000",
		"5001",
		"5002",
		"5003",
		"5004",
		"5100",
		"5101",
		"5102",
		"5103",
		"5104"
	];
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
	public static int[] BgmSlot = [
		4,
		1,
		2,
		3,
		5,
		6,
		12,
		7,
		8,
		9,
		0xa,
		0xb,
		0xc,
		0xd,
		0xe,
		0xf,
		0x10,
		0x11,
		0x1b,
		0x21,
		0x20,
		0x1c,
		0x13,
		0x14,
		0x15,
		0x1a,
		0x1d,
		0x17,
		0x18,
		0x16,
		0x22,
		0x3c,
		0x1f,
		0x28,
		0x29,
		0x2a,
		0x2b,
		0x2c,
		0x2d,
		0x2e,
		0x2f,
		0x45,
		0x4a,
		0x34,
		0x35,
		0x3d,
		0x32,
		0x33,
		0x3e,
		0x3f,
		0x43,
		0x44,
		0x46,
		0x47,
		0x36,
		0x37,
		0x40,
		0x3a,
		0x3b,
		0x42,
		0x38,
		0x39,
		0x41,
	];
	public static void LoadFontMap()
	{
		byte[] buffer = FileAccess.GetFileAsBytes("res://assets/font.map");
		//new EncoderReplacementFallback(strUniRepChr), new DecoderReplacementFallback(strUniRepChr)
		// GD.Print("长度",Encoding.GetEncoding("shift_jis").GetString(buffer).Length);
		string str = Wa2EngineMain.Engine.Wa2Encoding.GetString(buffer).Replace("\n", "").Replace("\r", "");
		// GD.Print(str);
		for (int i = 0; i < str.Length; i++)
		{
			FontMap.TryAdd(str[i], i);
		}
	}
}
