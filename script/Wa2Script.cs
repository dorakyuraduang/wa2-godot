using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
public enum CmdType
{
	NONE = 0,
	GLOBAL_VAR = 1,
	LOCAL_VAR = 2,
	STR_VAR = 3,
	FUNC = 4,
	VAR = 5,
	CALC = 6
}
public enum ValueType
{
	REF = 0,
	FLOAT = 4,
	INT = 3
}

public class Wa2Var
{
	public CmdType CmdType;
	public ValueType ValType;
	public int Value0;
	public int IntValue;
	public float FloatValue;
	static public Wa2Var CreateEmpty()
	{
		Wa2Var var = new()
		{
			CmdType = CmdType.VAR,
			ValType = ValueType.INT,
			IntValue = 0
		};
		return var;
	}
	public void Set(dynamic value)
	{
		if (value is int)
		{
			ValType = ValueType.INT;
			IntValue = value;
		}
		else if (value is float)
		{
			ValType = ValueType.FLOAT;
			FloatValue = value;
		}
	}
	public dynamic Get()
	{
		if (ValType == ValueType.FLOAT)
		{
			return FloatValue;
		}
		if (ValType == ValueType.INT)
		{
			return IntValue;
		}
		if (CmdType == CmdType.STR_VAR)
		{
			if (IntValue == 0)
			{
				return "春希";
			}
			else if (IntValue > 0)
			{
				return Wa2EngineMain.Engine.Texts[IntValue].Replace("\\n", "\n"); ;
			}
			else
			{
				return "";
			}
		}
		if (CmdType == CmdType.LOCAL_VAR)
		{
			if (IntValue >= 26)
			{
				return Wa2EngineMain.Engine.GameSav.GloFloats[IntValue % 26];
			}
			else
			{
				return Wa2EngineMain.Engine.GameSav.GloInts[IntValue];
			}

		}
		if (CmdType == CmdType.GLOBAL_VAR)
		{
			return Wa2EngineMain.Engine.GloFlags[IntValue];
		}

		return 0;
	}
}
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
	public Wa2EngineMain _engine;
	private JumpEntry[] _jumpEntrys = new JumpEntry[16];
	public bool Wait = false;
	public int JumpPos;
	public uint Label;
	private Dictionary<uint, uint> _points = new();
	private Dictionary<uint, uint> _jumpDic = new();
	private List<byte[]> _text = new();
	private byte[] _bnrbuffer;
	public List<Wa2Var> args = new();
	private Wa2Func _func;
	public Wa2Script(Wa2Func f)
	{
		_func = f;
		_engine = Wa2EngineMain.Engine;

	}
	public void LoadScript(string name, uint pos = 0)
	{

		_engine.GameSav.ScriptName = name;

		_points.Clear();
		Wa2EngineMain.Engine.Texts.Clear();
		args.Clear();
		_bnrbuffer = null;
		JumpPos = 0;
		LoadBnr(name);
		_engine.GameSav.ScriptPos = _points[pos];
		_jumpEntrys = new JumpEntry[16];
		// GD.Print(_engine.GameSav.ScriptPos);
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
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".txt");
		string strs = Wa2EngineMain.Engine.Wa2Encoding.GetString(buffer);
		Wa2EngineMain.Engine.Texts = [.. strs.Split(',')];
	}
	public void ParseGloVar()
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
				if (_jumpEntrys[JumpPos].Flag != 0)
				{
					_engine.GameSav.ScriptPos = _jumpEntrys[JumpPos].Pos;
					args.Clear();
				}
				break;
			case 5:
				GD.Print(_engine.GameSav.ScriptPos);
				if (JumpPos < 15)
				{
					JumpPos++;
					_jumpEntrys[JumpPos] = new();
				}
				_jumpEntrys[JumpPos].Type = 5;
				_jumpEntrys[JumpPos].Pos = ReadU32();
				_jumpEntrys[JumpPos].PosArr[0] = ReadU32();
				_jumpEntrys[JumpPos].PosArr[1] = ReadU32();
				_jumpEntrys[JumpPos].PosArr[2] = ReadU32();
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
				GD.Print("jump:", args[^1].Get());
				_jumpEntrys[JumpPos].Flag = args[^1].Get();
				uint pos1 = _jumpEntrys[JumpPos].PosArr[0];
				uint pos2 = _jumpEntrys[JumpPos].Pos;
				if (_jumpEntrys[JumpPos].Flag == 1)
				{
					return;
				}
				if (pos1 != 0)
				{
					_engine.GameSav.ScriptPos = pos1;
				}
				else
				{
					_engine.GameSav.ScriptPos = pos2;
				}

				// }
				// else if (pos2 > 0)
				// {

				// 	_engine.GameSav.ScriptPos = pos2;
		GD.Print(_engine.GameSav.ScriptPos);
		args.Clear();
		break;
			case 14:
			_jumpEntrys[JumpPos].Flag = args[^1].Get();
			if (_jumpEntrys[JumpPos].Flag == 0)
			{
				_engine.GameSav.ScriptPos = _jumpEntrys[JumpPos].PosArr[0];
			}
			else
			{
				_engine.GameSav.ScriptPos = _jumpEntrys[JumpPos].PosArr[2];
			}
			break;
		case 15:
			break;
		case 16:
			_jumpEntrys[JumpPos].Flag = args[^1].Get();
			if (_jumpEntrys[JumpPos].Type != 7)
			{
				return;
			}
			for (int i = 0; i < _jumpEntrys[JumpPos].Count; i++)
			{
				if (_jumpEntrys[JumpPos].FlagArr[i] == _jumpEntrys[JumpPos].Flag)
				{
					_engine.GameSav.ScriptPos = _jumpEntrys[JumpPos].PosArr[i];
					args.Clear();
					return;
				}
			}
			_engine.GameSav.ScriptPos = _jumpEntrys[JumpPos].Pos;
			args.Clear();
			break;

		}
	}
	public void ParseCmd()
	{
		if (_engine.GameSav.ScriptPos < _bnrbuffer.Length && !Wait)
		{
			int cmd = (int)ReadU32();
			switch (cmd)
			{
				case 0:
					ParseGloVar();
					ParseCmd();
					break;
				case 1:
				case 2:
				case 3:
					PushInt(cmd, -1, (int)ReadU32());
					ParseCmd();
					break;
				case 4:
					CallFunc();
					break;
				case 5:
					int type = (int)ReadU32();
					if (type == 4)
					{
						PushFloat(cmd, type, ReadF32());
					}
					else
					{
						PushInt(cmd, type, (int)ReadU32());
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
	public void PushInt(int type1, int type2, int v)
	{
		Wa2Var var = new()
		{
			CmdType = (CmdType)type1,
			ValType = (ValueType)type2,
			IntValue = v
		};
		args.Add(var);
	}
	public void PushFloat(int type1, int type2, float v)
	{
		Wa2Var var = new()
		{
			CmdType = (CmdType)type1,
			ValType = (ValueType)type2,
			FloatValue = v
		};
		args.Add(var);
	}
	public void CallFunc()
	{
		uint funcIdx = ReadU32();
		if (_func.FuncDic.TryGetValue(funcIdx, out var func))
		{
			// GD.Print(string.Format("{0:X}", funcIdx));
			func(args);
		}
		// if (funcIdx>=0x80){
		// 	args.Clear();
		// }	
	}
	// public string ParseStr(int pos)
	// {
	// 	// if (pos == 77260)
	// 	// {
	// 	// 	GD.Print(_engine.GameSav.ScriptPos);
	// 	// }
	// 	// StringBuilder binary = new StringBuilder();
	// 	// byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(_text[pos]);

	// 	// foreach (byte b in bytes)
	// 	// {
	// 	// 	binary.Append(b.ToString("X2"));
	// 	// }

	// 	// GD.Print(binary);
	// 	// GD.Print(_text[pos]);
	// 	return Wa2EngineMain.Engine.Wa2Encoding.GetString(_text[pos]);

	// }
	public void ParseCalc()
	{
		uint v1 = ReadU32();
		if (v1 > 0x1e)
		{
			return;
		}
		Wa2Var a = Wa2Var.CreateEmpty();
		Wa2Var b = Wa2Var.CreateEmpty();
		if (args.Count > 0 && v1 <= 0x1b)
		{
			a = args[^1];
		}
		if ((v1 >= 1 && v1 < 0x17) || v1 == 0x1b || v1 == 0)
		{
			if (args.Count > 1)
			{
				b = args[^2];
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
				if (b.CmdType == CmdType.GLOBAL_VAR)
				{
					GD.Print("全局变量,索引:", b.IntValue, "值:", a.Get());
					_engine.GloFlags[b.IntValue] = a.Get();
				}
				else if (b.CmdType == CmdType.LOCAL_VAR)
				{
					GD.Print("局部变量,索引:", b.IntValue, "值:", a.Get());
					if (b.IntValue >= 26)
					{
						_engine.GameSav.GloFloats[b.IntValue % 26] = a.Get();
					}
					else
					{
						_engine.GameSav.GloInts[b.IntValue] = a.Get();
					}
				}
				break;
			case 1:
				{

					b.Set(a.Get() + b.Get());

				}
				break;
			case 2:
				{
					b.Set(b.Get() - a.Get());
					break;
				}

			case 3:
				{
					b.Set(b.Get() * a.Get());
					break;
				}
			case 4:
				{
					b.Set(b.Get() / a.Get());
					break;
				}
			case 5:
				{
					b.Set(b.Get() % a.Get());
					break;
				}
			case 6:
				{
					b.Set(b.Get() & a.Get());
					break;
				}
			case 7:
				{
					b.Set(b.Get() | a.Get());
					break;
				}
			case 8:
				{
					b.Set(a.Get() == b.Get() ? 1 : 0);
					GD.Print(a.Get());
					GD.Print(b.Get());
				}
				break;
			case 9:
				{
					b.Set(a.Get() < b.Get() ? 1 : 0);
				}
				break;
			case 0xa:
				{
					b.Set(b.Get() > a.Get() ? 1 : 0);
				}
				break;
			case 0xb:
				{
					b.Set(b.Get() <= a.Get() ? 1 : 0);
				}
				break;
			case 0xc:
				{
					b.Set(b.Get() >= a.Get() ? 1 : 0);
				}
				break;
			case 0xd:
				{
					b.Set((b.Get() == 0 || a.Get() == 0) ? 0 : 1);
				}
				break;
			case 0xe:
				{
					b.Set(b.Get() || a.Get() ? 1 : 0);
				}
				break;
			case 0xf:
				{
					b.Set(b.Get() != a.Get() ? 1 : 0);
				}
				break;
			case 0x10:
				{
					b.Set(a.Get() + b.Get());
					break;
				}
			case 0x11:
				{

					b.Set(b.Get() - a.Get());
					break;
				}
			case 0x12:
				{

					b.Set(b.Get() * a.Get());
					break;
				}
			case 0x13:
				{
					b.Set(b.Get() / a.Get());
					break;
				}
			case 0x14:
				{

					b.Set(b.Get() % a.Get());
					break;
				}
			case 0x15:
				{
					b.Set(b.Get() & a.Get());
					if (args.Count == 0)
					{
						GD.Print("错误位置");
					}
					break;
				}
			case 0x16:
				{
					b.Set(b.Get() | a.Get());
					break;
				}
			case 0x17:
				a.Set(a.Get() * -1);
				break;
			case 0x18:
				a.Set(a.Get() == 0 ? 1 : 0);
				break;
			case 0x19:
				a.Set(a.Get() + 1);
				break;
			case 0x1A:
				a.Set(a.Get() - 1);
				break;
			case 0x1B:
				{
					b.ValType = (ValueType)a.Get();
					// if (a.GetType() == typeof(int))
					// {
					// 	args.Add((int)b);
					// }
					// else
					// {
					// 	args.Add((int)b);
					// }

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
		uint r = BitConverter.ToUInt32(_bnrbuffer, (int)_engine.GameSav.ScriptPos);
		_engine.GameSav.ScriptPos += 4;
		return r;
	}

	public float ReadF32()
	{
		float r = BitConverter.ToSingle(_bnrbuffer, (int)_engine.GameSav.ScriptPos);
		_engine.GameSav.ScriptPos += 4;
		return r;
	}
	public static void ParseFunc()
	{
		// 	int r = BitConverter.ToInt32(_bnrbuffer, (int)_engine.GameSav.ScriptPos);
		// 	_engine.GameSav.ScriptPos += 4;
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
