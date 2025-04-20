using Godot;
public partial class OptionsMenu : BasePage
{
  [Export]
  public HBoxContainer MsgWaitBtnList;
  [Export]
  public Wa2Button MsgCutOptinAllBtn;
  [Export]
  public Wa2Button MsgCutOptinReadBtn;
  [Export]
  public Wa2Button WaitFastBtn;
  [Export]
  public Wa2Button WaitNormalBtn;
  [Export]
  public HBoxContainer OptionButtonList;
  [Export]
  public Control PageList;

  public override void Close()
  {
    base.Close();
    _engine.Prefs.Save();
  }
  public override void _Ready()
  {
    base._Ready();
    MsgCutOptinAllBtn.ButtonDown += () => _engine.Prefs.SetConfig("msg_cut_optin", 1);
    MsgCutOptinReadBtn.ButtonDown += () => _engine.Prefs.SetConfig("msg_cut_optin", 0);
    WaitFastBtn.ButtonDown += () => _engine.Prefs.SetConfig("wait", 1);
    WaitNormalBtn.ButtonDown += () => _engine.Prefs.SetConfig("wait", 0);
    for (int i = 0; i < 3; i++)
    {
      int idx = i;
      OptionButtonList.GetChild<Wa2Button>(i).ButtonDown += () =>
      {

        UpdatePage(idx);
      };
    }
  }
  public void UpdatePage(int idx)
  {
    for (int i = 0; i < 3; i++)
    {
      if (i == idx)
      {
        PageList.GetChild<Control>(i).Show();
      }
      else
      {
        PageList.GetChild<Control>(i).Hide();
      }

    }

  }
  public override void Open()
  {
    base.Open();
    OptionButtonList.GetChild<Wa2Button>(0).ButtonPressed = true;
    UpdatePage(0);
    if (_engine.Prefs.GetConfig("msg_cut_optin") == 1)
    {
      MsgCutOptinAllBtn.ButtonPressed = true;
    }
    else
    {
      MsgCutOptinReadBtn.ButtonPressed = true;
    }
    if (_engine.Prefs.GetConfig("wait") == 1)
    {
      WaitFastBtn.ButtonPressed = true;
    }
    else
    {
      WaitNormalBtn.ButtonPressed = true;
    }
  }
}