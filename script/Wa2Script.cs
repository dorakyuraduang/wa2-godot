using Godot;
using System;
using System.Collections.Generic;
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
			Wa2EngineMain.Engine.GameFlags[IntValue] = value;

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
				Wa2EngineMain.Engine.Script.GloFloats[IntValue % 26] = value;
			}
			else
			{
				Wa2EngineMain.Engine.Script.GloInts[IntValue] = (int)value;
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
			// GD.Print(IntValue);
			if (IntValue == 0)
			{
				return "春希";
			}
			else if (IntValue > 0)
			{
				return Wa2EngineMain.Engine.Script.Texts[IntValue];
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
				return Wa2EngineMain.Engine.Script.GloFloats[IntValue % 26];
			}
			else
			{
				return Wa2EngineMain.Engine.Script.GloInts[IntValue];
			}

		}
		if (CmdType == CmdType.GLOBAL_VAR)
		{
			return Wa2EngineMain.Engine.GameFlags[IntValue];
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
	public Dictionary<int, uint> Points = new();
	// private Dictionary<uint, uint> _jumpDic = new();
	private byte[] _bnrbuffer;
	public float[] GloFloats = new float[26];
	public int[] GloInts = new int[26];
	public string ScriptName;
	public uint ScriptPos;
	public bool Exit=false;
	public List<string> Texts = new();
	public List<JumpEntry> JumpEntrys = new();
	public List<Wa2Var> Args = new();
	public int ScriptIdx;

	public Wa2Script(string name, int pos = 0)
	{
		_engine = Wa2EngineMain.Engine;
		ScriptIdx = Array.IndexOf(Wa2Def.ScriptList, name);
		ScriptName = name;
		_bnrbuffer = null;
		LoadBnr(name);
		ScriptPos = Points[pos];
		_engine.HasReadMessage = _engine.GetReadMessage(0);
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
					Points.Add(BitConverter.ToInt32(buffer, 12 + i * 8), BitConverter.ToUInt32(buffer, 12 + i * 8 + 4));
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

		// TODO: TTF全局控制
		// bool UseTTF = true;
		// if (UseTTF)
		// {
		// 	strs = Wa2Decode.ReplaceWithJsonMap(strs);
		// }

		Texts = [.. strs.Split(',')];
	}
	public bool ParseJumpFlag()
	{
		uint flag = ReadU32();
		// GD.Print(ScriptPos);
		// GD.Print(_engine.GameSav.ScriptName);
		// GD.Print("指令:", flag);
		// GD.Print("和纱本气度:", _engine.GameSav.GameFlags[5]);
		// GD.Print("和纱浮气度:", _engine.GameSav.GameFlags[6]);
		// GD.Print("雪菜好意度:", _engine.GameSav.GameFlags[7]);
		// GD.Print("FLG_雪菜好意度", _engine.GameSav.GameFlags[7]);
		// GD.Print("FLG_小春好意度", _engine.GameSav.GameFlags[8]);
		// GD.Print("FLG_千晶好意度", _engine.GameSav.GameFlags[9]);
		// GD.Print("FLG_麻理好意度", _engine.GameSav.GameFlags[10]);
		// GD.Print("FLG_小春ルート消滅", _engine.GameSav.GameFlags[11]);
		// GD.Print("FLG_千晶ルート消滅", _engine.GameSav.GameFlags[12]);
		// GD.Print("FLG_麻理ルート消滅", _engine.GameSav.GameFlags[13]);
		// GD.Print("jump_flag:", flag);
		switch (flag)
		{
			case 0:
				return false;
			case 1:
			  Exit=true;
				break;
			case 2:
				if (JumpEntrys.Count < 15)
				{
					JumpEntrys.Add(new());
				}
				JumpEntrys[^1].Type = 2;
				JumpEntrys[^1].PosArr[0] = ReadU32();
				JumpEntrys[^1].Pos = ReadU32();
				break;
			case 3:
				if (JumpEntrys[^1].Flag == 1)
				{
					ScriptPos = JumpEntrys[^1].Pos;
					// JumpEntrys.RemoveAt(JumpEntrys.Count - 1);
				}
				else
				{
					JumpEntrys[^1].Type = 3;
					JumpEntrys[^1].PosArr[0] = ReadU32();

				}
				// Args.Clear();
				break;
			case 4:
				if (JumpEntrys[^1].Flag == 0)
				{
					break;
				}
				ScriptPos = JumpEntrys[^1].Pos;
				// JumpEntrys.RemoveAt(JumpEntrys.Count - 1);
				// Args.Clear();
				break;
			case 5:
				if (JumpEntrys.Count < 15)
				{
					JumpEntrys.Add(new());
				}
				JumpEntrys[^1].Type = 5;
				JumpEntrys[^1].Pos = ReadU32();
				JumpEntrys[^1].PosArr[0] = ReadU32();
				JumpEntrys[^1].PosArr[1] = ReadU32();
				JumpEntrys[^1].PosArr[2] = ReadU32();
				break;
			case 6:
				if (JumpEntrys.Count < 15)
				{
					JumpEntrys.Add(new());
				}
				JumpEntrys[^1].Type = 6;
				JumpEntrys[^1].Pos = ReadU32();
				break;
			case 7:

				if (JumpEntrys.Count < 15)
				{
					JumpEntrys.Add(new());
				}
				JumpEntrys[^1].Type = 7;
				JumpEntrys[^1].Count = ReadU32();
				for (int i = 0; i < JumpEntrys[^1].Count; i++)
				{
					JumpEntrys[^1].PosArr[i] = ReadU32();
					JumpEntrys[^1].FlagArr[i] = ReadU32();
				}
				JumpEntrys[^1].Pos = ReadU32();
				break;
			case 8:
			case 9:
				break;
			case 0xa:
				for (int i = JumpEntrys.Count - 1; i >= 0; i--)
				{
					uint type = JumpEntrys[i].Type;

					if (type == 5 || type == 6 || type == 7)
					{
						break;
					}
					JumpEntrys.RemoveAt(i);
				}
				break;
			case 11:
				for (int i = JumpEntrys.Count - 1; i >= 0; i--)
				{
					uint type = JumpEntrys[i].Type;

					if (type == 5)
					{
						ScriptPos = JumpEntrys[i].PosArr[2];
						break;
					}
					else if (type == 6)
					{
						ScriptPos = JumpEntrys[i].PosArr[0];
						break;

					}
					else
					{
						JumpEntrys.RemoveAt(i);
					}

				}
				break;
			case 12:
				ScriptPos = ReadU32();
				break;
			case 13:

				JumpEntrys[^1].Flag = Args[^1].Get();
				uint pos1 = JumpEntrys[^1].PosArr[0];
				uint pos2 = JumpEntrys[^1].Pos;
				if (JumpEntrys[^1].Flag == 1)
				{
					break;
				}
				if (pos1 != 0)
				{
					ScriptPos = pos1;
				}
				else
				{
					ScriptPos = pos2;
				}
				// Args.Clear();
				break;
			case 14:
				//debug
				// if (_engine.TestMode)
				// {
				// 	ScriptPos = JumpEntrys[^1].Pos;
				// 	break;
				// }

				JumpEntrys[^1].Flag = Args[^1].Get();
				if (JumpEntrys[^1].Flag == 0)
				{
					ScriptPos = JumpEntrys[^1].Pos;
				}
				else
				{
					ScriptPos = JumpEntrys[^1].PosArr[2];
				}
				// Args.Clear();
				break;
			case 15:
				JumpEntrys[^1].Flag = Args[^1].Get();
				if (JumpEntrys[^1].Flag != 0)
				{
					break;
				}
				else
				{
					ScriptPos = JumpEntrys[^1].Pos;
				}

				break;
			case 16:
				JumpEntrys[^1].Flag = Args[^1].Get();
				if (JumpEntrys[^1].Type != 7)
				{
					break;
				}
				for (int i = 0; i < JumpEntrys[^1].Count; i++)
				{
					if (JumpEntrys[^1].FlagArr[i] == JumpEntrys[^1].Flag)
					{
						ScriptPos = JumpEntrys[^1].PosArr[i];
						// Args.Clear();
						break;
					}
				}
				ScriptPos = JumpEntrys[^1].Pos;
				// Args.Clear();
				break;
		}
		Args.Clear();
		return true;
	}
	public void ParseCmd()
	{
		bool flag = true;
		while (flag && ScriptPos < _bnrbuffer.Length && _engine.State == Wa2EngineMain.GameState.GAME && _engine.ScriptStack.Count > 0 && _engine.Script != null && _engine.Script == this)
		{
			int cmd = (int)ReadU32();
			switch (cmd)
			{
				case 0:
					flag = ParseJumpFlag();
					break;
				case 1:
				case 2:
				case 3:
					PushInt(cmd, -1, (int)ReadU32());
					flag = true;
					break;
				case 4:
					flag = CallFunc();
					// GD.Print(flag);
					// flag = true;
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
					flag = true;
					break;
				case 6:
					ParseCalc();
					flag = true;
					break;

				default:
					break;
			}
			if (JumpEntrys.Count > 0 && JumpEntrys[^1].Type - 2 <= 5)
			{
				for (int i = JumpEntrys.Count - 1; i >= 0; i--)
				{
					var entry = JumpEntrys[i];
					if (entry.Pos != ScriptPos)
					{
						break;
					}
					JumpEntrys.RemoveAt(i);
				}
			}
			if (ScriptPos >= _bnrbuffer.Length ||Exit)
			{
				if (_engine.ScriptStack.Count > 1)
				{
					_engine.ScriptStack.Pop();
				}
				_engine.Script = _engine.ScriptStack.Peek();
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
		Args.Add(var);
	}
	public void PushFloat(int type1, int type2, float v)
	{
		Wa2Var var = new()
		{
			CmdType = (CmdType)type1,
			ValType = (ValueType)type2,
			FloatValue = v
		};
		Args.Add(var);
	}
	public bool CallFunc()
	{
		uint funcIdx = ReadU32();
		GD.Print(string.Format("调用函数{0:X}", funcIdx));
		if (_engine.Func.FuncDic.TryGetValue(funcIdx, out var func))
		{
			return func(Args);
		}
		return true;
	}
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
		if (Args.Count > 0 && v1 <= 0x1b)
		{
			a = Args[^1];
		}
		if ((v1 >= 1 && v1 < 0x17) || v1 == 0x1b || v1 == 0)
		{
			if (Args.Count > 1)
			{
				b = Args[^2];
				Args.RemoveAt(Args.Count - 1);
			}
		}
		;
		if (v1 >= 8 && v1 <= 0x16 && Args.Count > 0)
		{
			Args.RemoveAt(Args.Count - 1);
		}
		// GD.Print(a.Get());
		// GD.Print(b.Get());
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
					b.Set((int)(b.Get() - a.Get()));
					break;
				}

			case 3:
				{
					b.Set((int)(a.Get() * b.Get()));
					break;
				}
			case 4:
				{
					b.Set((int)(b.Get() / a.Get()));
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
						PushInt(5, 3, b.Get() - a.Get());
					}
					else
					{
						PushFloat(5, 4, b.Get() - a.Get());
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
						PushInt(5, 3, 0);
						GD.Print("错误位置", ScriptPos);
					}

					// if (Args.Count == 0)
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
				// GD.Print(a.Get());
				break;
			case 0x1A:
				a.Set(a.Get() - 1);
				break;
			case 0x1B:
				{
					b.ValType = (ValueType)a.Get();
					// if (a.GetType() == typeof(int))
					// {
					// 	Args.Add((int)b);
					// }
					// else
					// {
					// 	Args.Add((int)b);
					// }

				}
				// GD.Print("字符串",Args[^2]);
				// Args[^2]=(int)(Args[^2]);
				break;
			case 0x1C:
			case 0x1D:
				break;
			case 0x1e:
				Args.Clear();

				break;
			default:
				break;
		}
	}
	public uint ReadU32()
	{
		uint r = BitConverter.ToUInt32(_bnrbuffer, (int)ScriptPos);
		ScriptPos += 4;
		return r;
	}
	public void FindNextCmd()
	{

		for (int i = (int)ScriptPos; i < _bnrbuffer.Length - 4; i += 4)
		{
			int v1 = BitConverter.ToInt32(_bnrbuffer, i);
			int v2 = BitConverter.ToInt32(_bnrbuffer, i + 4);
			if (v1 == 6 && v2 == 0xe)
			{
				ScriptPos = (uint)i;
				return;
			}
		}
	}
	public float ReadF32()
	{
		float r = BitConverter.ToSingle(_bnrbuffer, (int)ScriptPos);
		ScriptPos += 4;
		return r;
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
