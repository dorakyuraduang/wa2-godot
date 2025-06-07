using System.Collections.Generic;
using System.Text;
using Godot;

public partial class SavLoadTest : Node
{
  [Export(PropertyHint.File)]
  public string Path;
  public Wa2Encoding e;
  public void PrintSavInfo(FileAccess file)
  {
    GD.Print("存档类型", file.Get32());
    GD.Print("日历", file.Get32());
    GD.Print("年", file.Get16());
    GD.Print("月", file.Get16());
    GD.Print("星期", file.Get16());
    GD.Print("日", file.Get16());
    GD.Print("小时", file.Get16());
    GD.Print("分", file.Get16());
    GD.Print("秒", file.Get16());
    GD.Print("毫秒", file.Get16());
    GD.Print("第一句话", e.GetString(file.GetBuffer(32)));
  }
  public void PrintGameState(FileAccess file)
  {
    ulong start = 0x110a0;
    file.Seek(0x110a0);
    file.GetBuffer(24);
    GD.Print("图片id", file.Get32());
    GD.Print("图片x", file.Get32());
    GD.Print("图片y", file.Get32());
    GD.Print("图片缩放x", file.GetFloat());
    GD.Print("图片缩放y", file.GetFloat());
    GD.Print("图片类型", file.Get32());
    GD.Print("v12", file.Get32());
    GD.Print("图片显示", file.Get32());
    GD.Print("v14", file.Get32());
    GD.Print("v15", file.Get32());
    GD.Print("v16", file.Get32());
    GD.Print("v17", file.Get32());
    GD.Print("v18", file.Get32());
    GD.Print("TimeMode", file.Get32());
    GD.Print("v20", file.Get32());
    GD.Print("ChromaMode1", file.Get32());
    GD.Print("ChromaMode2", file.Get32());
    GD.Print("Amp1", e.GetString(file.GetBuffer(32)).Replace("\0", ""));
    GD.Print("Amp2", e.GetString(file.GetBuffer(32)).Replace("\0", ""));
    GD.Print("Amp3", e.GetString(file.GetBuffer(32)).Replace("\0", ""));
    GD.Print("v47", file.Get32());
    for (int i = 0; i < 8; i++)
    {
      file.Seek(0x11160 + (ulong)i * 4);
      int show = (int)file.Get32();
      if (show == 1)
      {

        file.Seek(0x11160 + 32 + (ulong)i * 4);
        GD.Print("角色名", Wa2Def.CharDict.GetValueOrDefault((int)file.Get32()));
        // GD.Print("位置", Wa2Def.CharPos[i]);
        file.Seek(0x11160 + 64 + (ulong)i * 4);
        GD.Print("立绘id", file.Get32());
        file.Seek(0x11160 + 96 + (ulong)i * 4);
        int usePos = file.Get16();
        GD.Print("使用位置", usePos);
        if (usePos == 1)
        {
          file.Seek(0x11160 + 144 + (ulong)i * 4);
          GD.Print("位置", file.GetFloat());
        }
        else
        {
          file.Seek(0x11160 + 112 + (ulong)i * 4);
          GD.Print("位置", Wa2Def.CharPos[file.Get32()]);
        }
        file.Seek(0x11160 + 176 + (ulong)i * 4);
        GD.Print("offset", file.GetFloat());
        file.Seek(0x11160 + 208 + (ulong)i * 4);
        GD.Print("fb", file.Get32());
        file.Seek(0x11160 + 240 + (ulong)i * 4);
        GD.Print("alpha", file.Get32());
      }

    }
    //end116
    //949-955 effect
    //956
    file.Seek(start + 956 * 4);
    GD.Print("EroMode", file.Get32());
    GD.Print("游戏时间", file.Get32());
    //959
    file.Seek(start + 959 * 4);
    for (int i = 0; i < 4; i++)
    {
      file.Seek(start + 959 * 4 + (ulong)i * 4);
      GD.Print("BGM", (int)file.Get32());
      file.Seek(start + 963 * 4 + (ulong)i * 4);
      GD.Print("播放", (int)file.Get32());
      file.Seek(start + 967 * 4 + (ulong)i * 4);
      GD.Print("循环", file.Get32());
      file.Seek(start + 971 * 4 + (ulong)i * 4);
      GD.Print("音量", file.Get32());
    }
    for (int i = 0; i < 16; i++)
    {
      file.Seek(start + 975 * 4 + (ulong)i * 4);
      GD.Print("音效播放", file.Get32());
      file.Seek(start + 991 * 4 + (ulong)i * 4);
      GD.Print("音效id", file.Get32());
      file.Seek(start + 1007 * 4 + (ulong)i * 4);
      GD.Print("音效循环", file.Get32());
      file.Seek(start + 1023 * 4 + (ulong)i * 4);
      GD.Print("音效音量", file.Get32());
    }
    for (int i = 0; i < 4; i++)
    {
      file.Seek(start + 1055 * 4 + (ulong)i * 4);
      GD.Print("语音播放", file.Get32());
      file.Seek(start + 1059 * 4 + (ulong)i * 4);
      GD.Print("语音id", file.Get32());
      file.Seek(start + 1063 * 4 + (ulong)i * 4);
      GD.Print("语音label", file.Get32());
      file.Seek(start + 1067 * 4 + (ulong)i * 4);
      GD.Print("语音角色", file.Get32());
      file.Seek(start + 1071 * 4 + (ulong)i * 4);
      GD.Print("语音音量", file.Get32());
    }
    file.Seek(start + 1091 * 4);
    GD.Print("语音标签", file.Get32());
    file.Seek(start + 1093 * 4);
    GD.Print("va参数", file.Get32());

    // public int[] CharsShow;
    // public int[] CharsIdx;
    // //72-75 ?
    // public int V72;
    // public int V73;
    // public int V74;
    // public int V75;
    // public int[] Pos1;
    // public int[] Pos2;
    // public int[] U1;
    // public int[] U2;
    // public int[] CharAlpha;

  }
  public override void _Ready()
  {
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    e = new();
    FileAccess file = FileAccess.Open(Path, FileAccess.ModeFlags.Read);
    PrintSavInfo(file);
    PrintGameState(file);

    //   	public int Flag;
    // public int Calendar;
    // public short Year;
    // public short Month;
    // public short WeekOfDay;
    // public short Day;
    // public short Hout;
    // public short Minute;
    // public short Second;
    // public short MillisSecond;
    // public byte[] First;
    // public byte[] Image;

    file.Close();
  }
}