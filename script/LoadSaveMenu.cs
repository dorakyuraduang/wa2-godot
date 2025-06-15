
using System.Threading.Tasks;
using Godot;
public enum DataMode
{
  Save,
  Load
}

public partial class LoadSaveMenu : BasePage
{
  // [Export]
  // public Wa2Button ConfirmBtn;
  // [Export]
  // public Wa2Button CancelBtn;
  // [Export]

  // public Label ConfirmLabel;
  // [Export]
  // public Control ConfirmMessage;
  // [Export]
  // public Control TipMessage;

  // [Export]
  // public ColorRect Mask;
  // [Export]
  // public Label TipLabel;
  public TextureRect PageBottom;
  [Export]
  public TextureRect PageTop;
  [Export]
  public GridContainer DataSlots;
  [Export]
  public HBoxContainer Tabs;
  [Export]
  public TextureRect NewDataTexture;

  public int _pageNum = 0;

  private DataMode _mode;
  private int _selectIdx;
  private int _newDataIdx;
  // public void OnCancelBtnDown()
  // {
  //   Mask.Hide();
  //   ConfirmMessage.Hide();
  // }
  // public void OnConfirmBtnDown()
  // {
  //   if (_mode == DataMode.Save)
  //   {
  //     _engine.GameSav.SaveData(_selectIdx);

  //   }
  //   if (_mode == DataMode.Load && _engine.State == Wa2EngineMain.GameState.TITLE)
  //   {
  //     _engine.SoundMgr.StopBgm();
  //   }
  //   ShowTipMessage();
  //   ConfirmMessage.Hide();
  // }
  public override void _Ready()
  {
    // CancelBtn.ButtonDown += OnCancelBtnDown;
    // ConfirmBtn.ButtonDown += OnConfirmBtnDown;
    base._Ready();
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
  public async void SaveData()
  {

    if (_engine.Script == null)
    {
      return;
    }
    _engine.GameSav.SaveData(_selectIdx);
    await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
    _engine.UiMgr.UIConfirm.Close();
    Close();
  }
  public void OnDataSlotDown(int idx)
  {
    if (AnimationPlayer.IsPlaying())
    {
      return;
    }
    _selectIdx = _pageNum * 10 + idx;
    if (_mode == DataMode.Save)
    {

      _engine.UiMgr.OpenConfirm("存档将被覆盖。\n确定吗？", "存档保存成功", FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", _selectIdx)) && _engine.Prefs.GetConfig("yes_no") == 1, SaveData);

    }
    else if (_mode == DataMode.Load && FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", _selectIdx)))
    {
      _engine.UiMgr.OpenConfirm("读取存档。\n确定吗？", "存档读取成功", _engine.Prefs.GetConfig("yes_no") == 1, LoadData);
    }

  }
  public async void LoadData()
  {
    if (_engine.State == Wa2EngineMain.GameState.TITLE)
    {
      _engine.SoundMgr.StopBgm();
      await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
      Close();
      _engine.UiMgr.UIConfirm.Close();
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
      _engine.UiMgr.UIConfirm.Close();
      Close();

    }
  }

  public void Open(DataMode mode)
  {
    base.Open();
    _mode = mode;
    Tabs.GetChild<Wa2Button>(_pageNum).ButtonPressed = true;
    if (_mode == DataMode.Save)
    {
      PageTop.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_01000.png");
    }
    else
    {
      PageTop.Texture = ResourceLoader.Load<Texture2D>("res://assets/grp/sys_02000.png");
    }
    _newDataIdx = GetNewDataIdx();
    UpdatePage();
  }

  public void UpdatePage()
  {
    for (int i = 0; i < 10; i++)
    {
      DataSlots.GetChild<DataSlot>(i).Update(_pageNum * 10 + i);
    }
    if (_newDataIdx >= 0 && _newDataIdx / 10 == _pageNum)
    {
      NewDataTexture.Show();
      int posIdx = _newDataIdx % 10;
      if (posIdx %2==1)
      {
        NewDataTexture.Position = new Vector2(190+632, 174+96*(posIdx/2));
      }
      else
      {
        NewDataTexture.Position = new Vector2(190, 174+96*(posIdx/2));
      }
    }
    else
    {
      NewDataTexture.Hide();
    }
  }
  public int GetNewDataIdx()
  {
    int num = 0;
    int idx = -1;
    for (int i = 0; i < 100; i++)
    {
      if (FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", i)))
      {

        FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", i), FileAccess.ModeFlags.Read);
        int year = (int)file.Get32();
        int month = (int)file.Get32();
        int dayOfWeek = (int)file.Get32();
        int day = (int)file.Get32();
        int hour = (int)file.Get32();
        int minute = (int)file.Get32();
        int second = (int)file.Get32();
        int millisecond = (int)file.Get32();
        int num2 = second + 60 * (minute + 60 * (hour + 24 * (day + 31 * (month + 12 * (year % 100)))));
        if (num2 > num)
        {
          idx = i;
          num = num2;
        }
      }
    }
    return idx;
  }
}