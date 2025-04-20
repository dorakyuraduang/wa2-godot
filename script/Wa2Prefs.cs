using Godot;
using System;
using System.Collections.Generic;


public class Wa2Prefs
{
	public ConfigFile ConfigFile = new();
	public float TextSpeed = 40;
	public void Init()
	{
		if (!FileAccess.FileExists("user://SYSTEM.ini"))
		{
			SetDefault();
			Save();
		}else{
			ConfigFile.Load("user://SYSTEM.ini");
		}
	}
	public void SetConfig(string key,int val){
		ConfigFile.SetValue("DEFAULT", key,val);
	}
	public 	int GetConfig(string key){
		return (int)ConfigFile.GetValue("DEFAULT", key);
	}
	public void Save()
	{
		ConfigFile.Save("user://SYSTEM.ini");
	}
	public void SetDefault()
	{
		string section = "DEFAULT";
		ConfigFile.SetValue(section, "wheel", 0);
		ConfigFile.SetValue(section, "wait", 0);
		ConfigFile.SetValue(section, "msg_wait", 4);
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
	}
}
