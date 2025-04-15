
using System.Security.Cryptography.X509Certificates;
using Godot;
public partial class BasePage : Control
{
  [Export]
  public Wa2Button ExitBtn;
  [Export]
  public AnimationPlayer AnimationPlayer;
  public Wa2EngineMain _engine;
  public virtual void Open()
  {
    AnimationPlayer.Play("open");

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
  }
  public void OnExitBtnDown()
  {
    Close();

  }
  public virtual void OnAnimationFinished(StringName anime)
  {
    if (anime == "close")
    {
      _engine.UiMgr.ReturnScene();
    }
  }
}