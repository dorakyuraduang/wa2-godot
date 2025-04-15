using System.Collections.Generic;
using Godot;
public partial class CGModeMenu : Control
{
  [Export]
  public AnimationPlayer AnimationPlayer;
  [Export]
  public Wa2Button ExitBtn;
  public Wa2EngineMain _engine;
  [Export]
  public HBoxContainer Tabs;
  [Export]
  public GridContainer CGSlots;
  [Export]
  public CGViewer CGViewer;

  public List<int> CurCgList = new();
  public int _pageNum;
  public void Open()
  {
    _engine.SoundMgr.PlayBgm(41);
    AnimationPlayer.Play("open");
    Tabs.GetChild<Wa2Button>(_pageNum).GrabFocus();
    UpdatePage();
  }
  public async void Close()
  {
    AnimationPlayer.Play("close");
    _engine.SoundMgr.PlayBgm(31);
    await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
    _engine.UiMgr.ReturnScene();
    
  }
  public override void _Ready()
  {
    
    _engine = Wa2EngineMain.Engine;
    ExitBtn.ButtonDown += OnExitBtnDown;
    for (int i = 0; i < 12; i++)
    {
      int idx = i;
      CGSlots.GetChild<Wa2Button>(i).ButtonDown += () =>
      {

        List<int> CurCgList = new();
        for (int k = 0; k < Wa2Def.CgSlot[idx + _pageNum * 12].Length; k++)
        {
          GD.Print(_engine.GetCgFlag(Wa2Def.CgSlot[idx + _pageNum * 12][k]));
          if (_engine.GetCgFlag(Wa2Def.CgSlot[idx + _pageNum * 12][k]) == 1)
          {
            CurCgList.Add(Wa2Def.CgSlot[idx + _pageNum * 12][k]);
          }
        }
        GD.Print(idx + _pageNum * 12);
        if (CurCgList.Count > 0)
        {
          CGViewer.Open(CurCgList);
        }
      };
    }
    for (int i = 0; i < 14; i++)
    {
      int idx = i;
      Tabs.GetChild<Wa2Button>(i).ButtonDown += () =>
      {
        _pageNum = idx;

        UpdatePage();
      };
    }
  }
  public void OnExitBtnDown()
  {
    Close();
  }
  public void UpdatePage()

  {
    for (int i = 0; i < 12; i++)
    {
      bool flag = false;
      Wa2Button cgBtn = CGSlots.GetChild<Wa2Button>(i);
      cgBtn.GetChild<TextureRect>(0).Texture = null;
      int[] slots = Wa2Def.CgSlot[i + _pageNum * 12];
      if (slots.Length == 0)
      {
        cgBtn.Disabled = true;
        cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05903.png");
        continue;
      }
      for (int k = 0; k < slots.Length; k++)
      {
        if (_engine.GetCgFlag(slots[k]) == 1)
        {
          flag = true;
          break;
        }
      }
      if (flag)
      {
        cgBtn.Disabled = false;
        cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05903.png");
        cgBtn.GetChild<TextureRect>(0).Texture = Wa2Resource.GetTvImage(slots[0]);
      }
      else
      {
        cgBtn.Disabled = true;
        if (_pageNum < 3)
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05900.png");
        }
        else if (_pageNum < 10)
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05901.png");
        }
        else
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05902.png");
        }
      }
    }
  }
}