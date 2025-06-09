
using System.Collections.Generic;
using Godot;
public partial class BasePage : Control
{
  [Export]
  public BgmPlayer BgmPlayer;
  [Export]
  public Wa2Button ExitBtn;
  [Export]
  public AnimationPlayer AnimationPlayer;
  public Wa2EngineMain _engine;
  private List<Wa2Button> _buttons;

  public virtual void Open()
  {
    DisabledAllButton();
    AnimationPlayer.Play("open");
    BgmPlayer.Show();

  }
  public override void _Ready()
  {
    _buttons = GetAllButtons(this);
    _engine = Wa2EngineMain.Engine;
    ExitBtn.ButtonDown += OnExitBtnDown;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }
  public List<Wa2Button> GetAllButtons(Node node, List<Wa2Button> list = null)
  {
    list ??= new List<Wa2Button>();
    foreach (Node child in node.GetChildren())
    {
      if (child is Wa2Button btn)
        list.Add(btn);
      GetAllButtons(child, list);
    }
    return list;
  }
  public virtual void Close()
  {
    DisabledAllButton();
    AnimationPlayer.Play("close");
    BgmPlayer.Hide();
    // _engine.UiMgr.UiQueue.Pop();
  }
  public virtual void FaseClose()
  {

    _engine.UiMgr.ReturnScene();
  }
  public void OnExitBtnDown()
  {
    Close();

  }
  public virtual void OnAnimationFinished(StringName anime)
  {
    if (anime == "close")
    {
      OnCloseAnimationFinished();
    }
    if (anime == "open")
    {
      OnOpenAnimationFinished();
    }
  }
  public virtual void OnCloseAnimationFinished()
  {
    _engine.UiMgr.ReturnScene();
  }
  public virtual void OnOpenAnimationFinished()
  {
    EnabledAllButton();
  }
  public void DisabledAllButton()
  {
    foreach (Wa2Button btn in _buttons)
    {
      btn.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

  }
  public void EnabledAllButton()
  {
    foreach (Wa2Button btn in _buttons)
    {
      btn.MouseFilter = Control.MouseFilterEnum.Stop;
    }
  }
}