
using System.Threading.Tasks;
using Godot;
public enum DataMode
{
  Save,
  Load
}

public partial class LoadSaveMenu : Control
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
  public AnimationPlayer AnimationPlayer;
  [Export]
  public ColorRect Mask;
  [Export]
  public Label TipLabel;
  public TextureRect PageBottom;
  [Export]
  public TextureRect PageTop;
  [Export]
  public Wa2Button ExitBtn;
  [Export]
  public GridContainer DataSlots;
  [Export]
  public HBoxContainer Tabs;

  public int _pageNum = 0;
  public Wa2EngineMain _engine;
  private DataMode _mode;
  private int _selectIdx;
  public void OnCancelBtnDown()
  {
    Mask.Hide();
    ConfirmMessage.Hide();
  }
  public void OnConfirmBtnDown()
  {
    if (_mode == DataMode.Save)
    {
      _engine.GameSav.SaveData(_selectIdx);

    }
    if (_mode == DataMode.Load && _engine.State == Wa2EngineMain.GameState.TITLE)
    {
      _engine.SoundMgr.StopBgm();
    }
    ShowTipMessage();
    ConfirmMessage.Hide();
  }
  public override void _Ready()
  {
    CancelBtn.ButtonDown += OnCancelBtnDown;
    ConfirmBtn.ButtonDown += OnConfirmBtnDown;
    _engine = Wa2EngineMain.Engine;
    ExitBtn.ButtonDown += OnExitBtnDown;
    for (int i = 0; i < 10; i++)
    {
      int idx = i;
      DataSlots.GetChild<Wa2Button>(i).ButtonDown += () => OnDataSlotDown(idx);
    }
    for (int i = 0; i < 10; i++)
    {
      int idx = i;
      Tabs.GetChild<Wa2Button>(i).ButtonDown += () =>
      {
        _pageNum = idx;

        UpdatePage();
      };
    }
  }
  public void OnDataSlotDown(int idx)
  {
    _selectIdx = _pageNum * 10 + idx;
    if (_mode == DataMode.Save)
    {
      if (FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", _selectIdx)) && _engine.Prefs.GetConfig("yes_no") == 1)
      {
        ShowConfirmMessage();
      }
      else
      {
        _engine.GameSav.SaveData(_selectIdx);
        ShowTipMessage();
      }
    }
    else if (_mode == DataMode.Load && FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", _selectIdx)) && _engine.Prefs.GetConfig("yes_no") == 1)
    {
      ShowConfirmMessage();
    }
    else
    {
      ShowTipMessage();
    }

  }
  public async void ShowTipMessage()
  {
    Mask.Show();
    TipMessage.Show();
    if (_mode == DataMode.Save)
    {
      TipLabel.Text = "存档已保存";
      await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
      Close();
    }
    else if (_mode == DataMode.Load)
    {
      TipLabel.Text = "存档读取成功";

      if (_engine.State == Wa2EngineMain.GameState.TITLE)
      {
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        Close();
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _engine.UiMgr.TitleMenu.AnimationPlayer.Play("close");
        await ToSignal(_engine.UiMgr.TitleMenu.AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _engine.UiMgr.OpenGame();
        _engine.GameSav.LoadData(_selectIdx);
      }
      else if (_engine.State == Wa2EngineMain.GameState.GAME)
      {
        _engine.GameSav.LoadData(_selectIdx);
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        Close();

      }



    }

  }
  public void ShowConfirmMessage()
  {
    Mask.Show();
    ConfirmMessage.Show();
    if (_mode == DataMode.Load)
    {
      ConfirmLabel.Text = "读取存档。\n确定吗？";
    }
    else if (_mode == DataMode.Save)
    {
      ConfirmLabel.Text = "存档将被覆盖。\n确定吗？";
    }
  }


  public void Open(DataMode mode)
  {
    _mode = mode;
    Tabs.GetChild<Wa2Button>(_pageNum).GrabFocus();
    if (_mode == DataMode.Save)
    {
      PageTop.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_01000.png");
    }
    else
    {
      PageTop.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_02000.png");
    }

    UpdatePage();
    AnimationPlayer.Play("open");
  }
  public async void Close()
  {
    AnimationPlayer.Play("close");
    await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
    _engine.UiMgr.ReturnScene();
  }
  public void OnExitBtnDown()
  {
    Close();
  }
  public void UpdatePage()
  {
    for (int i = 0; i < 10; i++)
    {
      DataSlots.GetChild<DataSlot>(i).Update(_pageNum * 10 + i);
    }
  }
}