using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Godot;
public partial class BackLogMenu : BasePage
{
  [Export]
  public VBoxContainer BackLogItems;
  [Export]
  public VScrollBar ScrollBar;
  public List<VoiceInfo> VoiceInfos;
  public int VoiceIdx = 0;
  public override void _Ready()
  {
    base._Ready();
    Modulate = new Color(1, 1, 1, 1);
    Scale = new Vector2(1, 1);
    ScrollBar.ValueChanged += OnScrollBarValChanged;
    _engine.SoundMgr.GetVoicePlayer(0).Finished += OnVoiceFinished;
    for (int i = 0; i < 4; i++)
    {
      BackLogItem item = BackLogItems.GetChild<BackLogItem>(i);
      item.VoiceBtn.ButtonDown += () =>
      {
        VoiceIdx = 0;
        VoiceInfos = item.VoiceInfos;
      };
    }

  }
  public void OnVoiceFinished()
  {
    if (VoiceInfos!=null && VoiceIdx < (VoiceInfos.Count-1) && _engine.UiMgr.UiQueue.Peek()==this)
    {
      VoiceIdx++;
      _engine.SoundMgr.PlayVoice(VoiceInfos[VoiceIdx].Label, VoiceInfos[VoiceIdx].Id, VoiceInfos[VoiceIdx].Chr, VoiceInfos[VoiceIdx].Volume);
    }
  }
  public override void Open()
  {
    Show();
    BgmPlayer.Show();
    _engine.AdvMain.Hide();
    ScrollBar.MaxValue = Math.Max(0, _engine.Backlogs.Count - 4);
    ScrollBar.Value = ScrollBar.MaxValue;
    OnScrollBarValChanged(ScrollBar.Value);
  }
  public override void Close()
  {
    Hide();
    BgmPlayer.Hide();
    _engine.UiMgr.ReturnScene();
    _engine.AdvMain.Show();
    VoiceInfos=null;
  }


  public void OnScrollBarValChanged(double val)
  {
    int pos = (int)val;
    for (int i = 0; i < 4; i++)
    {
      BackLogItem item = BackLogItems.GetChild<BackLogItem>(i);
      if (pos + i >= _engine.Backlogs.Count)
      {
        item.Hide();
      }
      else
      {
        item.Show();
        item.SetInfo(_engine.Backlogs[pos + i]);
      }

    }
  }
}