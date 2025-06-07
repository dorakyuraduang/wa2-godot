using System;
using Godot;
public partial class OptionsMenu : BasePage
{
  [Export]
  public HBoxContainer YesNoBtnList;
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
  [Export]
  public Control CharVoiceList;
  [Export]
  public TextureProgressBar AllVolumeBar;
  [Export]
  public TextureProgressBar BgmVolumeBar;
  [Export]
  public TextureProgressBar SeVolumeBar;
  [Export]
  public TextureProgressBar VoiceVolumeBar;
  [Export]
  public VBoxContainer VolumeBtnList;
  [Export]
  public TextureProgressBar WindowAlphaBar;
  [Export]
  public TextureProgressBar WindowAlphaVisBar;
  [Export]
  public TextureProgressBar WindowAlphaNovelBar;
  [Export]
  public TextureProgressBar AutoMaxBar;
  [Export]
  public HBoxContainer WindowAlphaBtnList;
  [Export]
  public HBoxContainer WindowAlphaVisBtnList;
  [Export]
  public HBoxContainer WindowAlphaNovelBtnList;
  [Export]
  public HBoxContainer AutoMaxBtnList;
  [Export]
  public Wa2Button DefaultBtn;
  [Export]
  public Wa2Button PageVoiceYesBtn;
  [Export]
  public Wa2Button PageVoiceNoBtn;
  [Export]
  public Wa2Button EroVoiceYesBtn;
  [Export]
  public Wa2Button EroVoiceNoBtn;

  public override void Close()
  {
    base.Close();
    _engine.Prefs.Save();
  }
  public override void _Ready()
  {
    base._Ready();
    DefaultBtn.ButtonDown += OnDefaultBtnDown;
    YesNoBtnList.GetChild<Wa2Button>(0).ButtonDown += () => _engine.Prefs.SetConfig("yes_no", 1);
    YesNoBtnList.GetChild<Wa2Button>(1).ButtonDown += () => _engine.Prefs.SetConfig("yes_no", 0);
    PageVoiceYesBtn.ButtonDown += () => _engine.Prefs.SetConfig("page_voice", 1);
    PageVoiceNoBtn.ButtonDown += () => _engine.Prefs.SetConfig("page_voice", 0);
    EroVoiceYesBtn.ButtonDown += () => _engine.Prefs.SetConfig("ero_voice", 1);
    EroVoiceYesBtn.ButtonDown += () => _engine.Prefs.SetConfig("ero_voice", 0);
    WindowAlphaBtnList.GetChild<Wa2Button>(0).ButtonDown += () => WindowAlphaBar.Value--;
    WindowAlphaBtnList.GetChild<Wa2Button>(1).ButtonDown += () => WindowAlphaBar.Value++;
    WindowAlphaVisBtnList.GetChild<Wa2Button>(0).ButtonDown += () => WindowAlphaVisBar.Value--;
    WindowAlphaVisBtnList.GetChild<Wa2Button>(1).ButtonDown += () => WindowAlphaVisBar.Value++;
    WindowAlphaNovelBtnList.GetChild<Wa2Button>(0).ButtonDown += () => WindowAlphaNovelBar.Value--;
    WindowAlphaNovelBtnList.GetChild<Wa2Button>(1).ButtonDown += () => WindowAlphaNovelBar.Value++;
    AutoMaxBtnList.GetChild<Wa2Button>(0).ButtonDown += () => AutoMaxBar.Value--;
    AutoMaxBtnList.GetChild<Wa2Button>(1).ButtonDown += () => AutoMaxBar.Value++;
    WindowAlphaBar.ValueChanged += (double val) => _engine.Prefs.SetWindowAlpha((int)(val * 256 / 21));
    WindowAlphaVisBar.ValueChanged += (double val) => _engine.Prefs.SetWindowAlphaVis((int)(val * 256 / 21));
    WindowAlphaNovelBar.ValueChanged += (double val) => _engine.Prefs.SetWindowAlphaNovel((int)(val * 256 / 21));
    AutoMaxBar.ValueChanged += (double val) => _engine.Prefs.SetConfig("auto_max", 60 + (int)(val * 540 / 21));
    MsgCutOptinAllBtn.ButtonDown += () => _engine.Prefs.SetConfig("msg_cut_optin", 1);
    MsgCutOptinReadBtn.ButtonDown += () => _engine.Prefs.SetConfig("msg_cut_optin", 0);
    WaitFastBtn.ButtonDown += () => _engine.Prefs.SetConfig("wait", 1);
    WaitNormalBtn.ButtonDown += () => _engine.Prefs.SetConfig("wait", 0);
    for (int i = 0; i < 4; i++)
    {
      int idx = i;
      MsgWaitBtnList.GetChild<Wa2Button>(i).ButtonDown += () =>
      {
        _engine.Prefs.SetConfig("msg_wait", _engine.Prefs.GetMsgWaitValue(idx));
      };
    }
    VolumeBtnList.GetChild<HBoxContainer>(0).GetChild<Wa2Button>(0).ButtonDown += () =>
    {
      AllVolumeBar.Value--;
    };
    VolumeBtnList.GetChild<HBoxContainer>(1).GetChild<Wa2Button>(0).ButtonDown += () =>
    {
      BgmVolumeBar.Value--;
    };
    VolumeBtnList.GetChild<HBoxContainer>(2).GetChild<Wa2Button>(0).ButtonDown += () =>
    {
      SeVolumeBar.Value--;
    };
    VolumeBtnList.GetChild<HBoxContainer>(3).GetChild<Wa2Button>(0).ButtonDown += () =>
    {
      VoiceVolumeBar.Value--;
    };
    VolumeBtnList.GetChild<HBoxContainer>(0).GetChild<Wa2Button>(1).ButtonDown += () =>
    {
      AllVolumeBar.Value++;
    };
    VolumeBtnList.GetChild<HBoxContainer>(1).GetChild<Wa2Button>(1).ButtonDown += () =>
    {
      BgmVolumeBar.Value++;
    };
    VolumeBtnList.GetChild<HBoxContainer>(2).GetChild<Wa2Button>(1).ButtonDown += () =>
    {
      SeVolumeBar.Value++;
    };
    VolumeBtnList.GetChild<HBoxContainer>(3).GetChild<Wa2Button>(1).ButtonDown += () =>
    {
      VoiceVolumeBar.Value++;
    };
    AllVolumeBar.ValueChanged += (double val) => _engine.Prefs.SetVolume(0, (int)(val * 256 / 21));
    BgmVolumeBar.ValueChanged += (double val) => _engine.Prefs.SetVolume(1, (int)(val * 256 / 21));
    SeVolumeBar.ValueChanged += (double val) => _engine.Prefs.SetVolume(2, (int)(val * 256 / 21));
    VoiceVolumeBar.ValueChanged += (double val) => _engine.Prefs.SetVolume(3, (int)(val * 256 / 21));

    for (int i = 0; i < 10; i++)
    {
      int idx = i;
      CharVoiceList.GetChild<HBoxContainer>(i).GetChild<Wa2Button>(0).ButtonDown += () =>
      {
        _engine.Prefs.SetConfig("char_voice" + idx, 1);
      };
      CharVoiceList.GetChild<HBoxContainer>(i).GetChild<Wa2Button>(1).ButtonDown += () =>
    {
      _engine.Prefs.SetConfig("char_voice" + idx, 0);
    };
    }
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
  public void OnDefaultBtnDown()
  {
    _engine.UiMgr.OpenConfirm("所有设置返回默认设置。\n确定吗？", "", _engine.Prefs.GetConfig("yes_no") == 1, SetDefault);

  }
  public void SetDefault()
  {
    _engine.Prefs.SetDefault();
    UpdateOption();
    _engine.AdvMain.UpdateWindowAlpha();
    _engine.UiMgr.UIConfirm.Close();
  }
  public void UpdateOption()
  {
    YesNoBtnList.GetChild<Wa2Button>(0).ButtonPressed = _engine.Prefs.GetConfig("yes_no") == 1;
    YesNoBtnList.GetChild<Wa2Button>(1).ButtonPressed = _engine.Prefs.GetConfig("yes_no") == 0;
    AllVolumeBar.Value = (int)(_engine.Prefs.GetConfig("all_vol") / (256 / 21));
    BgmVolumeBar.Value = (int)(_engine.Prefs.GetConfig("bgm_vol") / (256 / 21));
    SeVolumeBar.Value = (int)(_engine.Prefs.GetConfig("se_vol") / (256 / 21));
    VoiceVolumeBar.Value = (int)(_engine.Prefs.GetConfig("voice_vol") / (256 / 21));
    AutoMaxBar.Value = (int)((_engine.Prefs.GetConfig("auto_max") - 60) / (540 / 21));
    WindowAlphaBar.Value = (int)(_engine.Prefs.GetConfig("win_alpha") / (256 / 21));
    WindowAlphaVisBar.Value = (int)(_engine.Prefs.GetConfig("win_alpha_vis") / (256 / 21));
    WindowAlphaNovelBar.Value = (int)(_engine.Prefs.GetConfig("win_alpha_novel") / (256 / 21));
    MsgWaitBtnList.GetChild<Wa2Button>(_engine.Prefs.GetMsgWaitIdx()).ButtonPressed = true;
    for (int i = 0; i < 10; i++)
    {
      if (_engine.Prefs.GetConfig("char_voice" + i) == 1)
      {
        CharVoiceList.GetChild<HBoxContainer>(i).GetChild<Wa2Button>(0).ButtonPressed = true;
      }
      else
      {
        CharVoiceList.GetChild<HBoxContainer>(i).GetChild<Wa2Button>(1).ButtonPressed = true;
      }
    }
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
    if (_engine.Prefs.GetConfig("page_voice") == 1)
    {
      PageVoiceYesBtn.ButtonPressed = true;
    }
    else
    {
      PageVoiceNoBtn.ButtonPressed = true;

    }
    if (_engine.Prefs.GetConfig("ero_voice") == 1)
    {
       EroVoiceYesBtn.ButtonPressed = true;
    }
    else
    {
      EroVoiceNoBtn.ButtonPressed =  true;
    }
  }

  public override void Open()
  {
    base.Open();
    OptionButtonList.GetChild<Wa2Button>(0).ButtonPressed = true;
    UpdateOption();
    UpdatePage(0);
  }
}