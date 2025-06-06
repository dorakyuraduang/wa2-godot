using Godot;
using System;
using System.Collections.Generic;


public class Wa2Prefs
{
	public ConfigFile ConfigFile = new();
	public Wa2EngineMain _engine;
	public int[] MsgWaitIdxs = { 0, 4, 2, 1 };
	public void Init(Wa2EngineMain e)
	{
		_engine = e;
		if (!FileAccess.FileExists("user://SYSTEM.ini"))
		{
			SetDefault();

		}
		else
		{
			ConfigFile.Load("user://SYSTEM.ini");
		}
		SetVolume(0, GetConfig("all_vol"));
		SetVolume(1, GetConfig("bgm_vol"));
		SetVolume(2, GetConfig("se_vol"));
		SetVolume(3, GetConfig("voice_vol"));
		
	}
	public void SetWindowAlpha(int alpha)
	{
		SetConfig("win_alpha", alpha);
		_engine.AdvMain.UpdateWindowAlpha();
	}
	public void SetWindowAlphaVis(int alpha)
	{
		SetConfig("win_alpha_vis", alpha);
		_engine.AdvMain.UpdateWindowAlpha();
	}
	public void SetWindowAlphaNovel(int alpha)
	{
		SetConfig("win_alpha_novel", alpha);
		_engine.AdvMain.UpdateWindowAlpha();
	}
	public void SetConfig(string key, int val)
	{
		ConfigFile.SetValue("DEFAULT", key, val);
	}
	public void SetVolume(int idx, int val)
	{
		if (val == 0)
		{
			AudioServer.SetBusVolumeDb(idx, -80);
		}
		else
		{
			AudioServer.SetBusVolumeDb(idx, Mathf.LinearToDb(val / 256.0f));
		}


		switch (idx)
		{
			case 0:
				// GD.Print(val);
				SetConfig("all_vol", val);
				break;
			case 1:
				SetConfig("bgm_vol", val);
				break;
			case 2:
				SetConfig("se_vol", val);
				break;
			case 3:
				SetConfig("voice_vol", val);
				break;
		}

	}
	public bool CanPlayCharVoice(int idx)
	{
		switch (idx)
		{
			case 0:
			case 3:
			case 4:
			case 5:
				return GetConfig("char_voice" + idx) == 1;
			case 2:
				return GetConfig("char_voice1") == 1;
			case 1:
				return GetConfig("char_voice2") == 1;
			case 10:
				return GetConfig("char_voice6") == 1;
			case 11:
				return GetConfig("char_voice7") == 1;
			case 12:
			case 13:
			case 15:
			case 17:
			case 18:
			case 20:
			case 21:
			case 27:
			case 28:
			case 30:
			case 31:
			case 32:
			case 36:
				return GetConfig("char_voice8") == 1;
			case 14:
			case 16:
			case 19:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 29:
			case 33:
			case 34:
			case 35:
				return GetConfig("char_voice9") == 1;
		}
		return true;
	}
	public int GetConfig(string key)
	{
		return (int)ConfigFile.GetValue("DEFAULT", key);
	}
	public int GetMsgWait()
	{
		return GetConfig("msg_wait");
	}
	public void Save()
	{
		ConfigFile.Save("user://SYSTEM.ini");
	}
	public int GetMsgWaitIdx()
	{
		return Array.IndexOf(MsgWaitIdxs, GetMsgWait());
	}
	public int GetMsgWaitValue(int idx)
	{
		return MsgWaitIdxs[idx];
	}

	public void SetDefault()
	{
		string section = "DEFAULT";
		ConfigFile.SetValue(section, "wheel", 0);
		ConfigFile.SetValue(section, "wait", 0);
		ConfigFile.SetValue(section, "msg_wait", 2);
		ConfigFile.SetValue(section, "msg_cut_optin", 1);
		ConfigFile.SetValue(section, "all_sound", 1);
		ConfigFile.SetValue(section, "bgm", 1);
		ConfigFile.SetValue(section, "se", 1);
		ConfigFile.SetValue(section, "voice", 1);
		ConfigFile.SetValue(section, "all_vol", 195);
		ConfigFile.SetValue(section, "bgm_vol", 134);
		ConfigFile.SetValue(section, "se_vol", 195);
		ConfigFile.SetValue(section, "voice_vol", 256);
		ConfigFile.SetValue(section, "mov_lv", 2);
		ConfigFile.SetValue(section, "ero_voice", 0);
		ConfigFile.SetValue(section, "page_voice", 0);
		ConfigFile.SetValue(section, "char_voice0", 1);
		ConfigFile.SetValue(section, "char_voice1", 1);
		ConfigFile.SetValue(section, "char_voice2", 1);
		ConfigFile.SetValue(section, "char_voice3", 1);
		ConfigFile.SetValue(section, "char_voice4", 1);
		ConfigFile.SetValue(section, "char_voice5", 1);
		ConfigFile.SetValue(section, "char_voice6", 1);
		ConfigFile.SetValue(section, "char_voice7", 1);
		ConfigFile.SetValue(section, "char_voice8", 1);
		ConfigFile.SetValue(section, "char_voice9", 1);
		ConfigFile.SetValue(section, "menu", 0);
		ConfigFile.SetValue(section, "auto_max", 137);
		ConfigFile.SetValue(section, "win_color", 0);
		ConfigFile.SetValue(section, "win_alpha", 207);
		ConfigFile.SetValue(section, "win_alpha_vis", 134);
		ConfigFile.SetValue(section, "win_alpha_novel", 134);
		ConfigFile.SetValue(section, "win_alpha_command", 134);
		ConfigFile.SetValue(section, "win_disp", 1);
		ConfigFile.SetValue(section, "yes_no", 1);
		ConfigFile.SetValue(section, "R_click", 0);
		ConfigFile.SetValue(section, "voice_window", 0);
		ConfigFile.SetValue(section, "gvflag_window", 0);
		ConfigFile.SetValue(section, "debug_mouse", -1);
		ConfigFile.SetValue(section, "window_x", 278);
		ConfigFile.SetValue(section, "window_y", 235);
		Save();
	}
}
