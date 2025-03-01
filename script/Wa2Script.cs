using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Wa2FuncEntry{
	
}
public class Wa2Script
{
	private static uint _curPos;
	private static Dictionary<uint, uint> _points = new();
	private static string[] _text;
	private static byte[] _bnrbuffer;
	public static void LoadScript(string name, uint pos = 0)
	{
		Wa2Resource.Clear();
		LoadBnr(name);
		_curPos = _points[pos];
		GD.Print(_curPos);
	}
	public static void LoadBnr(string name)
	{
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".bnr");
		if (buffer != null)
		{
			if (BitConverter.ToUInt32(buffer, 0) == 0x5243534c)
			{
				uint pointCount = BitConverter.ToUInt32(buffer, 8);
				GD.Print(pointCount);
				for (int i = 0; i < pointCount; i++)
				{
					_points.Add(BitConverter.ToUInt32(buffer, 12 + i * 8), BitConverter.ToUInt32(buffer, 12 + i * 8 + 4));
				}
			}
			_bnrbuffer = buffer;
			LoadText(name);
		}

	}
	public static void LoadText(string name)
	{
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".txt");
		_text = System.Text.Encoding.GetEncoding("shift-jis").GetString(buffer).Split(',');
		GD.Print(_text.Length);
	}
	public static void ParseCmd()
	{
		if (_curPos < _bnrbuffer.Length)
		{
			switch (ReadU32())
			{
				case 0:
					break;
				case 1:
				case 2:
				case 3:
					break;
				case 4:
				case 5:
				case 6:
					// v5 = parser_func();
					break;

				default:
					break;
			}
		}
	}
	public static uint ReadU32()
	{
		uint r = BitConverter.ToUInt32(_bnrbuffer, (int)_curPos);
		_curPos += 4;
		return r;
	}
	public static void ParseFunc()
	{
		// 	int r = BitConverter.ToInt32(_bnrbuffer, (int)_curPos);
		// 	_curPos += 4;
		// 	return r;
		// }
	}
	public static void LoadFunc(string name)
	{
		byte[] buffer = Wa2Resource.LoadFileBuffer(name);
		if (buffer == null)
		{
			return;
		}
		uint pos = 0;
		pos += 4;
		while (pos < buffer.Length)
		{
		// 	uint v8 = 0;
		// 	uint v9 = 0;
		// 	string funcName = Encoding.GetEncoding("shift_jis").GetString(buffer, (int)pos, 32).Replace("\0", "");
		// 	pos += 120;
		// 	byte v10 = buffer[pos];
		// 	if (v10 == 44)
		// 	{
		// 		if (v8 > 0)
		// 		{

		// 		}
		// 		else
		// 		{
		// 			v9++;
		// 			pos++;
		// 			v8 = 0;
		// 		}
		// 	}
		}
	}
}
