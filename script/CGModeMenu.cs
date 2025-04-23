using System.Collections.Generic;
using System.Linq;
using Godot;
public partial class CGModeMenu : BasePage
{
  [Export]
  public HBoxContainer Tabs;
  [Export]
  public GridContainer CGSlots;
  [Export]
  public CGViewer CGViewer;

  public List<int> CurCgList = new();
  public int _pageNum;
  public override void Open()
  {
    _engine.SoundMgr.PlayBgm(41);
    base.Open();
    Tabs.GetChild<Wa2Button>(_pageNum).GrabFocus();
    UpdatePage();
  }
  public override void Close()
  {
    AnimationPlayer.Play("close");
    CGViewer.Close();
    _engine.SoundMgr.PlayBgm(31);

  }
  public override void _Ready()
  {
    base._Ready();
    CurCgList.Clear();
    for (int i = 0; i < 12; i++)
    {
      int idx = i;
      CGSlots.GetChild<Wa2Button>(i).ButtonDown += () =>
      {

        bool flag = false;
        for (int k = 0; k < Wa2Def.CgSlot[idx + _pageNum * 12].Length; k++)
        {
          // GD.Print(_engine.GetCgFlag(Wa2Def.CgSlot[idx + _pageNum * 12][k]));
          if (_engine.GetCgFlag(Wa2Def.CgSlot[idx + _pageNum * 12][k]) == 1)
          {
            flag = true;
            CurCgList = Wa2Def.CgSlot[idx + _pageNum * 12].ToList();
            break;
          }
        }
        if (flag)
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