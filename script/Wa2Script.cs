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
		if (CmdType == CmdType.GLOBAL_VAR)
		{
			// if (IntValue == 6)
			// {
			// 	GD.Print("浮气度位置", Wa2EngineMain.Engine.GameSav.ScriptPos);
			// }
			if (IntValue == 0xe)
			{

			}
			Wa2EngineMain.Engine.GameSav.GameFlags[IntValue] = value;

		}
		if (CmdType == CmdType.LOCAL_VAR)
		{
			// if (!(value is int))
			// {
			// 	GD.Print("浮点");
			// 	GD.Print(Wa2EngineMain.Engine.GameSav.ScriptPos);
			// }
			// else
			// {
			// 	GD.Print("整数");
			// }
			if (IntValue >= 26)
			{
				Wa2EngineMain.Engine.GameSav.GloFloats[IntValue % 26] = value;
			}
			else
			{
				Wa2EngineMain.Engine.GameSav.GloInts[IntValue] = value;
			}
		}
		if (CmdType == CmdType.VAR)
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
			return Wa2EngineMain.Engine.GameSav.GameFlags[IntValue];
		}

		return 0;
	}
}
public class JumpEntry
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

	public bool Wait = false;
	public uint Label;
	private Dictionary<uint, uint> _points = new();
	private Dictionary<uint, uint> _jumpDic = new();
	private List<byte[]> _text = new();
	private byte[] _bnrbuffer;

	private Wa2Func _func;
	public Wa2Script(Wa2Func f)
	{
		_func = f;
		_engine = Wa2EngineMain.Engine;

	}
	public void LoadScript(string name, uint pos = 0)
	{
		_engine.ScriptIdx = Array.IndexOf(Wa2Def.ScriptList, name);
		_points.Clear();
		// _engine.GameSav.Reset();
		_engine.GameSav.JumpEntrys.Clear();
		_engine.GameSav.GloFloats = new float[26];
		_engine.GameSav.GloInts = new int[26];
		_engine.GameSav.ScriptName = name;
		Wa2EngineMain.Engine.Texts.Clear();
		_engine.GameSav.args.Clear();
		_bnrbuffer = null;
		LoadBnr(name);
		_engine.GameSav.ScriptPos = _points[pos];

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
	public static void LoadText(string name)
	{
		byte[] buffer = Wa2Resource.LoadFileBuffer(name + ".txt");
		string strs = Wa2EngineMain.Engine.Wa2Encoding.GetString(buffer);

		// TODO: TTF全局控制
		bool UseTTF = true;
		if (UseTTF)
		{
			strs = Wa2Decode.ReplaceWithJsonMap(strs);
		}

		Wa2EngineMain.Engine.Texts = [.. strs.Split(',')];
	}
	public void ParseGloVar()
	{
		uint flag = ReadU32();
		// GD.Print(_engine.GameSav.ScriptPos);
		// GD.Print(_engine.GameSav.ScriptName);
		// GD.Print("指令:", flag);
		GD.Print("和纱本气度:", _engine.GameSav.GameFlags[5]);
		GD.Print("和纱浮气度:", _engine.GameSav.GameFlags[6]);
		GD.Print("雪菜好意度:", _engine.GameSav.GameFlags[7]);
		GD.Print("FLG_雪菜好意度", _engine.GameSav.GameFlags[7]);
		GD.Print("FLG_小春好意度", _engine.GameSav.GameFlags[8]);
		GD.Print("FLG_千晶好意度", _engine.GameSav.GameFlags[9]);
		GD.Print("FLG_麻理好意度", _engine.GameSav.GameFlags[10]);
		GD.Print("FLG_小春ルート消滅", _engine.GameSav.GameFlags[11]);
		GD.Print("FLG_千晶ルート消滅", _engine.GameSav.GameFlags[12]);
		GD.Print("FLG_麻理ルート消滅", _engine.GameSav.GameFlags[13]);
		switch (flag)
		{
			case 0:
				break;
			case 1:
				break;
			case 2:
				if (_engine.GameSav.JumpEntrys.Count < 15)
				{
					_engine.GameSav.JumpEntrys.Add(new());
				}
				_engine.GameSav.JumpEntrys[^1].Type = 2;
				_engine.GameSav.JumpEntrys[^1].PosArr[0] = ReadU32();
				_engine.GameSav.JumpEntrys[^1].Pos = ReadU32();
				break;
			case 3:
				if (_engine.GameSav.JumpEntrys[^1].Flag == 1)
				{
					_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].Pos;
					_engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);
				}
				else
				{
					_engine.GameSav.JumpEntrys[^1].Type = 3;
					_engine.GameSav.JumpEntrys[^1].PosArr[0] = ReadU32();

				}

				_engine.GameSav.args.Clear();
				break;
			case 4:
				if (_engine.GameSav.JumpEntrys[^1].Flag == 0)
				{
					return;
				}
				_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].Pos;
				_engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);

				_engine.GameSav.args.Clear();

				break;
			case 5:
				if (_engine.GameSav.JumpEntrys.Count < 15)
				{
					_engine.GameSav.JumpEntrys.Add(new());
				}
				_engine.GameSav.JumpEntrys[^1].Type = 5;
				_engine.GameSav.JumpEntrys[^1].Pos = ReadU32();
				_engine.GameSav.JumpEntrys[^1].PosArr[0] = ReadU32();
				_engine.GameSav.JumpEntrys[^1].PosArr[1] = ReadU32();
				_engine.GameSav.JumpEntrys[^1].PosArr[2] = ReadU32();
				break;
			case 6:
				if (_engine.GameSav.JumpEntrys.Count < 15)
				{
					_engine.GameSav.JumpEntrys.Add(new());
				}
				_engine.GameSav.JumpEntrys[^1].Type = 6;
				_engine.GameSav.JumpEntrys[^1].Pos = ReadU32();
				break;
			case 7:

				if (_engine.GameSav.JumpEntrys.Count < 15)
				{
					_engine.GameSav.JumpEntrys.Add(new());
				}
				_engine.GameSav.JumpEntrys[^1].Count = ReadU32();
				for (int i = 0; i < _engine.GameSav.JumpEntrys[^1].Count; i++)
				{
					_engine.GameSav.JumpEntrys[^1].PosArr[i] = ReadU32();
					_engine.GameSav.JumpEntrys[^1].FlagArr[i] = ReadU32();
				}
				_engine.GameSav.JumpEntrys[^1].Pos = ReadU32();
				break;
			case 8:
				//打印
				break;
			//打印
			case 9:
				break;
			case 0xa:
			// if (JumpPos < 0)
			// {
			// }
			// else
			// {

			// }
			// break;
			case 11:
				// if (JumpPos < 0)
				// {

				// }
				// else
				// {

				// }
				break;
			case 12:
				ReadU32();
				break;
			case 13:

				_engine.GameSav.JumpEntrys[^1].Flag = _engine.GameSav.args[^1].Get();
				uint pos1 = _engine.GameSav.JumpEntrys[^1].PosArr[0];
				uint pos2 = _engine.GameSav.JumpEntrys[^1].Pos;
				if (_engine.GameSav.JumpEntrys[^1].Flag == 1)
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
					_engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);

				}
				// _engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);
				// }
				// else if (pos2 > 0)
				// {

				// 	_engine.GameSav.ScriptPos = pos2;
				_engine.GameSav.args.Clear();
				break;
			case 14:
				_engine.GameSav.JumpEntrys[^1].Flag = _engine.GameSav.args[^1].Get();
				if (_engine.GameSav.JumpEntrys[^1].Flag == 0)
				{
					_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].PosArr[0];
				}
				else
				{
					_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].PosArr[2];
					_engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);
				}
				// _engine.GameSav.JumpEntrys.RemoveAt(_engine.GameSav.JumpEntrys.Count - 1);
				break;
			case 15:
				break;
			case 16:
				_engine.GameSav.JumpEntrys[^1].Flag = _engine.GameSav.args[^1].Get();
				if (_engine.GameSav.JumpEntrys[^1].Type != 7)
				{
					return;
				}
				for (int i = 0; i < _engine.GameSav.JumpEntrys[^1].Count; i++)
				{
					if (_engine.GameSav.JumpEntrys[^1].FlagArr[i] == _engine.GameSav.JumpEntrys[^1].Flag)
					{
						_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].PosArr[i];
						_engine.GameSav.args.Clear();
						return;
					}
				}
				_engine.GameSav.ScriptPos = _engine.GameSav.JumpEntrys[^1].Pos;
				_engine.GameSav.args.Clear();
				break;

		}
	}
	public void ParseCmd()
	{
		// GD.Print(_engine.GameSav.ScriptPos);
		if (_engine.GameSav.ScriptPos < _bnrbuffer.Length)
		{
			int cmd = (int)ReadU32();
			// GD.Print("位置",_engine.GameSav.ScriptPos);
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
		// _engine.SysSav.ScriptReadPos[_engine.GameSav.ScriptName] = _engine.GameSav.ScriptPos;
	}
	public void PushInt(int type1, int type2, int v)
	{
		Wa2Var var = new()
		{
			CmdType = (CmdType)type1,
			ValType = (ValueType)type2,
			IntValue = v
		};
		_engine.GameSav.args.Add(var);
	}
	public void PushFloat(int type1, int type2, float v)
	{
		Wa2Var var = new()
		{
			CmdType = (CmdType)type1,
			ValType = (ValueType)type2,
			FloatValue = v
		};
		_engine.GameSav.args.Add(var);
	}
	public void CallFunc()
	{
		uint funcIdx = ReadU32();
		if (_func.FuncDic.TryGetValue(funcIdx, out var func))
		{
			// GD.Print(string.Format("{0:X}", funcIdx));
			func(_engine.GameSav.args);
		}
		// if (funcIdx>=0x80){
		// 	_engine.GameSav.args.Clear();
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
		// GD.Print("脚本名", Wa2EngineMain.Engine.GameSav.ScriptName);
		// GD.Print("位置", Wa2EngineMain.Engine.GameSav.ScriptPos);
		uint v1 = ReadU32();
		if (v1 > 0x1e)
		{
			return;
		}
		Wa2Var a = Wa2Var.CreateEmpty();
		Wa2Var b = Wa2Var.CreateEmpty();
		if (_engine.GameSav.args.Count > 0 && v1 <= 0x1b)
		{
			a = _engine.GameSav.args[^1];
		}
		if ((v1 >= 1 && v1 < 0x17) || v1 == 0x1b || v1 == 0)
		{
			if (_engine.GameSav.args.Count > 1)
			{
				b = _engine.GameSav.args[^2];
				_engine.GameSav.args.RemoveAt(_engine.GameSav.args.Count - 1);
			}
		}
		;
		if (v1 >= 8 && v1 <= 0x16 && _engine.GameSav.args.Count > 0)
		{
			_engine.GameSav.args.RemoveAt(_engine.GameSav.args.Count - 1);
		}
		switch (v1)
		{

			case 0:
				b.Set(a.Get());
				// if (b.CmdType == CmdType.GLOBAL_VAR)
				// {
				// 	_engine.GameSav.GameFlags[b.IntValue] = a.Get();
				// }
				// else if (b.CmdType == CmdType.LOCAL_VAR)
				// {
				// 	GD.Print("局部变量,索引:", b.IntValue, "值:", a.Get());
				// 	if (b.IntValue >= 26)
				// 	{
				// 		_engine.GameSav.GloFloats[b.IntValue % 26] = a.Get();
				// 	}
				// 	else
				// 	{
				// 		_engine.GameSav.GloInts[b.IntValue] = a.Get();
				// 	}
				// }
				break;
			case 1:
				{
					b.Set((int)(a.Get() + b.Get()));
				}
				break;
			case 2:
				{
					b.Set((int)(a.Get() + b.Get()));
					break;
				}

			case 3:
				{
					b.Set((int)(a.Get() + b.Get()));
					break;
				}
			case 4:
				{
					b.Set((int)(a.Get() + b.Get()));
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
					PushInt(5, 3, a.Get() == b.Get() ? 1 : 0);
				}
				break;
			case 9:
				{
					PushInt(5, 3, b.Get() < a.Get() ? 1 : 0);
				}
				break;
			case 0xa:
				{
					PushInt(5, 3, b.Get() > a.Get() ? 1 : 0);
				}
				break;
			case 0xb:
				{
					PushInt(5, 3, b.Get() <= a.Get() ? 1 : 0);
				}
				break;
			case 0xc:
				{
					PushInt(5, 3, b.Get() >= a.Get() ? 1 : 0);
				}
				break;
			case 0xd:
				{
					PushInt(5, 3, (b.Get() == 0 || a.Get() == 0) ? 0 : 1);
				}
				break;
			case 0xe:
				{
					PushInt(5, 3, b.Get() != 0 || a.Get() != 0 ? 1 : 0);
				}
				break;
			case 0xf:
				{
					PushInt(5, 3, b.Get() != a.Get() ? 1 : 0);
				}
				break;
			case 0x10:
				{
					if (a.Get() is int && b.Get() is int)
					{
						PushInt(5, 3, a.Get() + b.Get());
					}
					else
					{
						PushFloat(5, 4, a.Get() + b.Get());
					}
					break;
				}
			case 0x11:
				{
					if (a.Get() is int && b.Get() is int)
					{
						PushInt(5, 3, a.Get() - b.Get());
					}
					else
					{
						PushFloat(5, 4, a.Get() - b.Get());
					}
					break;
				}
			case 0x12:
				{
					if (a.Get() is int && b.Get() is int)
					{
						PushInt(5, 3, b.Get() * a.Get());
					}
					else
					{
						PushFloat(5, 4, b.Get() * a.Get());
					}
					break;
				}
			case 0x13:
				{
					if (a.Get() is int && b.Get() is int)
					{
						PushInt(5, 3, b.Get() / a.Get());
					}
					else
					{
						PushFloat(5, 4, b.Get() / a.Get());
					}
					break;
				}
			case 0x14:
				{

					PushInt(5, 3, b.Get() % a.Get());
					break;
				}
			case 0x15:
				{
					if (a.Get() is int && b.Get() is int)
					{
						PushInt(5, 3, b.Get() & a.Get());
					}
					else
					{
						GD.Print("错误位置", _engine.GameSav.ScriptPos);
					}

					// if (_engine.GameSav.args.Count == 0)
					// {
					// 	GD.Print("错误位置");
					// }
					break;
				}
			case 0x16:
				{
					PushInt(5, 3, b.Get() | a.Get());
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
					// 	_engine.GameSav.args.Add((int)b);
					// }
					// else
					// {
					// 	_engine.GameSav.args.Add((int)b);
					// }

				}
				// GD.Print("字符串",_engine.GameSav.args[^2]);
				// _engine.GameSav.args[^2]=(int)(_engine.GameSav.args[^2]);
				break;
			case 0x1C:
			case 0x1D:
				break;
			case 0x1e:
				_engine.GameSav.args.Clear();

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
	public void FindNextCmd()
	{

		for (int i = (int)_engine.GameSav.ScriptPos; i < _bnrbuffer.Length - 4; i += 4)
		{
			int v1 = BitConverter.ToInt32(_bnrbuffer, i);
			int v2 = BitConverter.ToInt32(_bnrbuffer, i + 4);
			if (v1 == 6 && v2 == 0xe)
			{
				_engine.GameSav.ScriptPos = (uint)i;
				return;
			}
		}
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
