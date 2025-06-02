
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

  public virtual void Open()
  {
    AnimationPlayer.Play("open");
    BgmPlayer.Show();

  }
  public override void _Ready()
  {
    _engine = Wa2EngineMain.Engine;
    ExitBtn.ButtonDown += OnExitBtnDown;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }
  public virtual void Close()
  {
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
  }
  public  virtual  void OnCloseAnimationFinished()
  {
    _engine.UiMgr.ReturnScene();
  }
}