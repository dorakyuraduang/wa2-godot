using System.Collections.Generic;
using Godot;
public partial class BackLogItem : Control
{
  [Export]
  public Wa2Button VoiceBtn;
  [Export]
  public Wa2Label NmaeLabel;
  [Export]
  public Wa2Label TextLabel;
  public List<VoiceInfo> VoiceInfos;
  public Wa2EngineMain _engine;
  public override void _Ready()
  {
    _engine = Wa2EngineMain.Engine;
    VoiceBtn.ButtonDown += OnViceoBtnDown;
  }
  public void SetInfo(BacklogEntry e)
  {
    // GD.Print(e.Name);
    // GD.Print(e.Text);
    TextLabel.Segment = e.Segment;
    NmaeLabel.SetText(e.Name);
    TextLabel.SetText(e.Text);
    
    VoiceInfos = e.VoiceInfos;
    if (VoiceInfos != null && VoiceInfos.Count > 0)
    {
      VoiceBtn.Show();
      if (!_engine.Prefs.CanPlayCharVoice(VoiceInfos[0].Chr))
      {
        VoiceBtn.Disabled = true;
      }
      else
      {
        VoiceBtn.Disabled = false;
      }
    }
    else
    {
      VoiceBtn.Hide();
    }
  }
  public void OnViceoBtnDown()
  {
    if (VoiceInfos != null && VoiceInfos.Count > 0)
    {
      _engine.SoundMgr.PlayVoice(VoiceInfos[0].Label, VoiceInfos[0].Id, VoiceInfos[0].Chr, VoiceInfos[0].Volume);
    }
  }

}