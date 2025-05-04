using Godot;
public partial class BackLogItem : Control
{
  [Export]
  public Wa2Button VoiceBtn;
  [Export]
  public Wa2Label NmaeLabel;
  [Export]
  public Wa2Label TextLabel;
  public VoiceInfo VoiceInfo;
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
    NmaeLabel.Text = e.Name;
    TextLabel.Text = e.Text;
    NmaeLabel.Update(-1);
    TextLabel.Update(-1);
    VoiceInfo = e.VoiceInfo;
    if (VoiceInfo != null)
    {
      VoiceBtn.Show();
      if (!_engine.Prefs.CanPlayCharVoice(VoiceInfo.Chr))
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
    if (VoiceInfo != null)
    {
      _engine.SoundMgr.PlayVoice(VoiceInfo.Label, VoiceInfo.Id, VoiceInfo.Chr, VoiceInfo.Volume);
    }
  }
}