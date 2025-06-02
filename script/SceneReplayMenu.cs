using Godot;
public partial class SceneReplayMenu : BasePage
{
  [Export]
  public HBoxContainer Tabs;
  [Export]
  public GridContainer SceneSlots;
  public int _pageNum = 0;
  public override void _Ready()
  {
    base._Ready();
    for (int i = 0; i < 2; i++)
    {
      int idx = i;
      Tabs.GetChild<Wa2Button>(i).ButtonDown += () =>
      {
        _pageNum = idx;

        UpdatePage();
      };
    }
    for (int i = 0; i < 12; i++)
    {
      int idx = i;
      SceneSlots.GetChild<Wa2Button>(i).ButtonDown += () => OnSelectScene(idx + _pageNum*12+1);
    }
  }
  public override void OnCloseAnimationFinished()
  {
    base.OnCloseAnimationFinished();
    _engine.SoundMgr.PlayBgm(31);
  }
  public override void Open()
  {
    _engine.SoundMgr.PlayBgm(15);
    Tabs.GetChild<Wa2Button>(_pageNum).GrabFocus();
    base.Open();
    _engine.ReplayMode = 0;
    UpdatePage();
  }
  public async void OnSelectScene(int idx)
  {
    _engine.ReplayMode = idx;
    FaseClose();
    _engine.UiMgr.TitleMenu.AnimationPlayer.Play("close");
    await ToSignal(_engine.UiMgr.TitleMenu.AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
    _engine.StartScript("9999");
    _engine.UiMgr.OpenGame();
  }
  public void UpdatePage()

  {
    for (int i = 0; i < 12; i++)
    {
      Wa2Button cgBtn = SceneSlots.GetChild<Wa2Button>(i);
      cgBtn.GetChild<TextureRect>(0).Texture = null;
      (int, int, string, string, int) slot = Wa2Def.SceneSlots[i + _pageNum * 12];
      bool flag = _engine.ReadSysFlag(slot.Item5) == 1;
      if (flag)
      {
        cgBtn.Disabled = false;
        cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05903.png");
        cgBtn.GetChild<TextureRect>(0).Texture = Wa2Resource.GetTvImage(slot.Item1);
      }
      else
      {
        GD.Print(slot.Item2);
        cgBtn.Disabled = true;
        if (slot.Item2 == 0)
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05900.png");
        }
        else if (slot.Item2 == 1)
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05901.png");
        }
        else if (slot.Item2 == 2)
        {
          cgBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_05902.png");
        }
      }
    }
  }
}