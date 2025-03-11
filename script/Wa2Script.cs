using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
public struct JumpEntry
{
	public uint Type;
	public uint Count;
	public uint Pos;
	public int Flag;
	public uint[] PosArr;
	public uint[] FlagArr;

	public JumpEntry()
	{
		PosArr = new uint[64];
		FlagArr = new uint[64];
	}

}
public class Wa2Script
{
	private JumpEntry[] _jumpEntrys = new JumpEntry[16];
	public bool Wait = false;
	public int JumpPos;
	public uint CurPos { private set; get; }
	public uint Label;
	private Dictionary<uint, uint> _points = new();
	private Dictionary<uint, uint> _jumpDic = new();
	private string[] _text;
	private byte[] _bnrbuffer;
	private List<dynamic> args = new();
	private Wa2Func _func;
	public Wa2Script(Wa2Func f)
	{
		_func = f;

	}
	public void LoadScript(string name, uint pos = 0)
	{
		GD.Print("脚本名:", name);
		Wa2Resource.Clear();
		_points.Clear();
		_text = null;
		args.Clear();
		_bnrbuffer = null;
		LoadBnr(name);
		CurPos = _points[pos];
		_jumpEntrys = new JumpEntry[15];
		// GD.Print(CurPos);
	}
	public void LoadBnr(string name)
	{
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".bnr");
		if (buffer != null)
		{
			if (BitConverter.ToUInt32(buffer, 0) == 0x5243534c)
			{
				uint pointCount = BitConverter.ToUInt32(buffer, 8);
				for (int i = 0; i < pointCount; i++)
				{
					_points.Add(BitConverter.ToUInt32(buffer, 12 + i * 8), BitConverter.ToUInt32(buffer, 12 + i * 8 + 4));
				}
			}
			_bnrbuffer = buffer;
			LoadText(name);
		}

	}
	public void LoadText(string name)
	{
		GD.Print("加载文本", name);
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".txt");
		GD.Print(buffer);
		_text = Encoding.GetEncoding("shift-jis").GetString(buffer).Split(',');
		// GD.Print(_text.Length);
	}
	public void ParseGalVar()
	{
		uint flag = ReadU32();
		switch (flag)
		{
			case 0:
				break;
			case 1:
				break;
			case 2:
				if (JumpPos < 15)
				{
					JumpPos++;
					_jumpEntrys[JumpPos] = new();
				}
				_jumpEntrys[JumpPos].Type = 2;
				_jumpEntrys[JumpPos].PosArr[0] = ReadU32();
				_jumpEntrys[JumpPos].Pos = ReadU32();
				break;
			case 3:
				break;
			case 4:
				if (_jumpEntrys[JumpPos].Flag == 0)
				{
					CurPos = _jumpEntrys[JumpPos].Pos;
					args.Clear();
				}
				break;
			case 5:
				if (JumpPos < 15)
				{
					JumpPos++;
				}
				else
				{

				}
				ReadU32();
				ReadU32();
				ReadU32();
				ReadU32();
				break;
			case 6:
				if (JumpPos < 15)
				{
					JumpPos++;
				}
				else
				{

				}
				ReadU32();
				break;
			case 7:

				if (JumpPos < 15)
				{
					JumpPos++;
					_jumpEntrys[JumpPos] = new();
				}
				_jumpEntrys[JumpPos].Count = ReadU32();
				for (int i = 0; i < _jumpEntrys[JumpPos].Count; i++)
				{
					_jumpEntrys[JumpPos].PosArr[i] = ReadU32();
					_jumpEntrys[JumpPos].FlagArr[i] = ReadU32();
				}
				_jumpEntrys[JumpPos].Pos = ReadU32();
				break;
			case 8:
				break;
			case 9:
				break;
			case 0xa:
				if (JumpPos < 0)
				{
				}
				else
				{

				}
				break;
			case 11:
				if (JumpPos < 0)
				{

				}
				else
				{

				}
				break;
			case 12:
				ReadU32();
				break;
			case 13:
				_jumpEntrys[JumpPos].Flag = args[^1];
				if (_jumpEntrys[JumpPos].Flag == 0)
				{
					CurPos = _jumpEntrys[JumpPos].Pos;
					args.Clear();
				}
				break;
			case 14:
				break;
			case 15:
				break;
			case 16:
				_jumpEntrys[JumpPos].Flag = args[^1];
				if (_jumpEntrys[JumpPos].Type != 7)
				{
					return;
				}
				for (int i = 0; i < _jumpEntrys[JumpPos].Count; i++)
				{
					if (_jumpEntrys[JumpPos].FlagArr[i] == _jumpEntrys[JumpPos].Flag)
					{
						CurPos = _jumpEntrys[JumpPos].PosArr[i];
						args.Clear();
						return;
					}
				}
				CurPos = _jumpEntrys[JumpPos].Pos;
				args.Clear();
				break;

		}
	}
	public void ParseCmd()
	{
		if (CurPos < _bnrbuffer.Length && !Wait)
		{
			switch (ReadU32())
			{
				case 0:
					ParseGalVar();
					ParseCmd();
					break;
				case 1:
				case 2:
				case 3:
					args.Add((int)ReadU32());
					ParseCmd();
					break;
				case 4:

					CallFunc();
					break;
				case 5:
					if (ReadU32() == 4)
					{
						args.Add(ReadF32());
					}
					else
					{
						args.Add((int)ReadU32());
					}
					ParseCmd();
					break;
				case 6:
					ParseCalc();
					ParseCmd();
					break;

				default:
					break;
			}
		}
	}
	public void PushInt(int v)
	{
		args.Add((int)v);
	}
	public void PushFloat(float v)
	{
		args.Add(v);
	}
	public void CallFunc()
	{
		uint funcIdx = ReadU32();
		if (_func.FuncDic.TryGetValue(funcIdx, out var func))
		{
			GD.Print(string.Format("{0:X}", funcIdx));
			func(args);
		}
		// if (funcIdx>=0x80){
		// 	args.Clear();
		// }	
	}
	public string ParseStr(int pos)
	{
		// if (pos == 77260)
		// {
		// 	GD.Print(CurPos);
		// }

		return _text[pos];

	}
	public void ParseCalc()
	{
		uint v1 = ReadU32();
		if (v1 > 0x1e)
		{
			return;
		}
		dynamic a = 0;
		dynamic b = 0;
		if (args.Count >= 1 && v1<=0x1b)
		{
			a = args[^1];
			args.RemoveAt(args.Count - 1);
		}
		if (v1 >= 1 && v1 < 0x17)
		{
			if (args.Count >= 1)
			{
				b = args[^1];
				args.RemoveAt(args.Count - 1);
			}
		}
		// if (v1 >= 0x17)
		// {
		// 	args.Clear();
		// }
		;
		switch (v1)
		{

			case 0:
				break;
			case 1:
				{

					args.Add(a + b);
					
				}
			break;
			case 2:
				{
					args.Add(b - a);
					break;
				}

			case 3:
				{
					args.Add(b * a);
					break;
				}
			case 4:
				{
					args.Add(b / a);
					break;
				}
			case 5:
				{
					args.Add(b % a);
					break;
				}
			case 6:
				{
					args.Add(b & a);
					break;
				}
			case 7:
				{
					args.Add(b | a);
					break;
				}
			case 8:
				{
					args.Add(b == a ? 1 : 0);
				}
				break;
			case 9:
				{
					args.Add(a < b ? 1 : 0);
				}
				break;
			case 0xa:
				{
					args.Add(a > b ? 1 : 0);
				}
				break;
			case 0xb:
				{
					args.Add(a <= b ? 1 : 0);
				}
				break;
			case 0xc:
				{
					args.Add(a >= b ? 1 : 0);
				}
				break;
			case 0xd:
				{
					args.Add((b == 0 || a == 0) ? 0 : 1);
				}
				break;
			case 0xe:
				{
					args.Add(b || a ? 1 : 0);
				}
				break;
			case 0xf:
				{
					args.Add(b != a ? 1 : 0);
				}
				break;
			case 0x10:
				{
					args.Add((int)(b + a));
					break;
				}
			case 0x11:
				{

					args.Add((int)(b - a));
					break;
				}
			case 0x12:
				{

					args.Add((int)(b * a));
					break;
				}
			case 0x13:
				{

					args.Add((int)b / a);
					break;
				}
			case 0x14:
				{

					args.Add((int)b % a);
					break;
				}
			case 0x15:
				{
					args.Add((int)b & (int)a);
					if (args.Count == 0)
					{
						GD.Print("错误位置");
					}
					break;
				}
			case 0x16:
				{
					args.Add((int)b | (int)a);
					break;
				}
			case 0x17:
				args.Add(a * -1);
				break;
			case 0x18:
				args.Add(a == 0 ? 1 : 0);
				break;
			case 0x19:
				args.Add(a++);
				break;
			case 0x1A:
				args.Add(a--);
				break;
			case 0x1B:
				{
					if (a.GetType() == typeof(int))
					{
						args.Add((int)b);
					}
					else
					{
						args.Add((int)b);
					}

				}
				// GD.Print("字符串",args[^2]);
				// args[^2]=(int)(args[^2]);
				break;
			case 0x1C:
			case 0x1D:
				break;
			case 0x1e:
				args.Clear();

				break;
			default:
				break;
		}
	}
	public uint ReadU32()
	{
		uint r = BitConverter.ToUInt32(_bnrbuffer, (int)CurPos);
		CurPos += 4;
		return r;
	}

	public float ReadF32()
	{
		float r = BitConverter.ToSingle(_bnrbuffer, (int)CurPos);
		CurPos += 4;
		return r;
	}
	public static void ParseFunc()
	{
		// 	int r = BitConverter.ToInt32(_bnrbuffer, (int)CurPos);
		// 	CurPos += 4;
		// 	return r;
		// }
	}
	// public static void LoadFunc(string name)
	// {
	// 	byte[] buffer = Wa2Resource.LoadFileBuffer(name);
	// 	if (buffer == null)
	// 	{
	// 		return;
	// 	}
	// 	uint pos = 0;
	// 	pos += 4;
	// 	while (pos < buffer.Length)
	// 	{
	// 		uint v8 = 0;
	// 		uint v9 = 0;
	// 		string funcName = Encoding.GetEncoding("shift_jis").GetString(buffer, (int)pos, 32).Replace("\0", "");
	// 		pos += 120;
	// 		byte v10 = buffer[pos];
	// 		if (v10 == 44)
	// 		{
	// 			if (v8 > 0)
	// 			{

	// 			}
	// 			else
	// 			{
	// 				v9++;
	// 				pos++;
	// 				v8 = 0;
	// 			}
	// 		}
	// 	}
	// }
}
