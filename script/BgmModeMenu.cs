using System;
using System.Collections.Generic;
using Godot;
public partial class BgmModeMenu : BasePage
{
  [Export]
  public HBoxContainer BgmBtnContainer;
  [Export]
  public TextureRect BgmName;
  [Export]
  public TextureRect BgmAuthor;
  [Export]
  public Wa2Button PreBtn;
  [Export]
  public Wa2Button NextBtn;
  public int _pageNum;
  public override void _Ready()
  {
    base._Ready();
    PreBtn.ButtonDown += () => ChangePage(-1);
    NextBtn.ButtonDown += () => ChangePage(1);
    for (int i = 0; i < 32; i++)
    {
      Wa2Button btn = BgmBtnContainer.GetChild<VBoxContainer>(i / 13).GetChild<Wa2Button>(i % 13);
      int idx = i;
      btn.ButtonDown += () =>
      {
        int start = _pageNum == 0 ? 0 : 31;
        _engine.SoundMgr.PlayBgm(Wa2Def.BgmSlot[start + idx]);
        int pos = Array.IndexOf(Wa2Def.BgmSlot, Wa2Def.BgmSlot[start + idx]);
        BgmName.Show();
        BgmAuthor.Show();
        (BgmName.Texture as AtlasTexture).Region = new Rect2(0, 24 * pos, 344, 24);
        (BgmAuthor.Texture as AtlasTexture).Region = new Rect2(0, 24 * pos, 512, 24);
      };
    }
  }
  public override void Open()
  {
    base.Open();
    _engine.SoundMgr.StopBgm();
    UpdatePage();
    BgmName.Hide();
    BgmAuthor.Hide();
  }
  public override void OnAnimationFinished(StringName anime)
  {
    base.OnAnimationFinished(anime);
    if (anime == "close")
    {
      _engine.SoundMgr.PlayBgm(31);
    }
  }
  public void ChangePage(int val)
  {
    _pageNum += val;
   
    if (_pageNum < 0)
    {
      _pageNum = 1;
    }
    else if (_pageNum > 1)
    {
      _pageNum = 0;
    }
    //  GD.Print(_pageNum);
    UpdatePage();
  }
  public void UpdatePage()
  {
    int count = _pageNum == 0 ? 31 : 32;
    int start = _pageNum == 0 ? 0 : 31;
    if (_pageNum == 0)
    {
      BgmBtnContainer.GetChild<VBoxContainer>(2).GetChild<Wa2Button>(5).Hide();
    }
    else
    {
      BgmBtnContainer.GetChild<VBoxContainer>(2).GetChild<Wa2Button>(5).Show();
    }
    for (int i = 0; i < count; i++)
    {
      Wa2Button btn = BgmBtnContainer.GetChild<VBoxContainer>(i / 13).GetChild<Wa2Button>(i % 13);
      if (_engine.GetBgmFlag(Wa2Def.BgmSlot[start + i]) == 1)
      {
        btn.Disabled = false;
        int pos = Array.IndexOf(Wa2Def.BgmSlot, Wa2Def.BgmSlot[start + i]);
        btn.GetChild<TextureRect>(0).Show();
        (btn.GetChild<TextureRect>(0).Texture as AtlasTexture).Region = new Rect2(0, 24 * pos, 344, 24);
      }
      else
      {
        btn.GetChild<TextureRect>(0).Hide();
        btn.Disabled = true;
      }
    }
  }
}