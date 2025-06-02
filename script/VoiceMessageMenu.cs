using Godot;
using System;

public partial class VoiceMessageMenu : BasePage
{
  [Export]
  public HBoxContainer VoiceButtonContainer;
  [Export]
  public TextureRect VoiceTexture;
  [Export]
  public Wa2Button BackButton;
  public override void Open()
  {
    base.Open();
    _engine.SoundMgr.StopBgm();
    _engine.SoundMgr.GetVoicePlayer(0).Finished += OnBackButtonDown;
  }
  public override void Close()
  {
    if (VoiceTexture.Visible)
    {
      OnBackButtonDown();
    }
    else
    {
      base.Close();
      _engine.SoundMgr.GetVoicePlayer(0).Finished -= OnBackButtonDown;
    }

  }

  public override void _Ready()
  {
    base._Ready();
    VoiceButtonContainer.GetChild<Wa2Button>(0).ButtonDown += () => PlayVoiceMessage(3);
    VoiceButtonContainer.GetChild<Wa2Button>(1).ButtonDown += () => PlayVoiceMessage(1);
    VoiceButtonContainer.GetChild<Wa2Button>(2).ButtonDown += () => PlayVoiceMessage(0);
    VoiceButtonContainer.GetChild<Wa2Button>(3).ButtonDown += () => PlayVoiceMessage(2);
    VoiceButtonContainer.GetChild<Wa2Button>(4).ButtonDown += () => PlayVoiceMessage(4);
    BackButton.ButtonDown += OnBackButtonDown;
  }
  public void PlayVoiceMessage(int idx)
  {
    VoiceTexture.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_0720" + idx + ".png");
    _engine.SoundMgr.PlayVoiceMessage(idx);
    AnimationPlayer.Play("select_char");
  }
  public override void OnCloseAnimationFinished()
  {
    base.OnCloseAnimationFinished();
    _engine.SoundMgr.StopVoice(0);
    _engine.SoundMgr.PlayBgm(31);


  }
  public void OnBackButtonDown()
  {
    _engine.SoundMgr.StopVoice(0);
    VoiceTexture.Hide();
  }
}
