using System;
using System.Security.Cryptography.X509Certificates;
using Godot;
public partial class UIConfirm : Control
{
  [Export]
  public Wa2Button ConfirmBtn;
  [Export]
  public Wa2Button CancelBtn;
  [Export]

  public Label ConfirmLabel;
  [Export]
  public Control ConfirmMessage;
  [Export]
  public Control TipMessage;
  [Export]
  public Label TipLabel;
  public Action ConfirmAction;
  public Wa2EngineMain _engine;
  public override void _Ready()
  {
    _engine = Wa2EngineMain.Engine;
    ConfirmBtn.ButtonDown += OnConfirmBtnDown;
    CancelBtn.ButtonDown += OnCancelBtnDown;
  }
  public void Open(string text1, string text2, bool confirm, Action action)
  {
    Show();
    ConfirmLabel.Text = text1;
    TipLabel.Text = text2;
    ConfirmAction = action;
    if (confirm)
    {
      ConfirmMessage.Show();
    }
    else
    {
      TipMessage.Show();
      ConfirmAction();
    }

  }
  public void OnConfirmBtnDown()
  {
    ConfirmAction();
    if (TipLabel.Text != "")
    {
      TipMessage.Show();
    }

    ConfirmMessage.Hide();
  }
  public void OnCancelBtnDown()
  {
    Close();
  }
  public void Close()
  {
    ConfirmMessage.Hide();
    TipMessage.Hide();
    Hide();
    _engine.UiMgr.ReturnScene();
  }
}