using Godot;
public enum DataMode
{
  Save,
  Load
}

public partial class LoadSaveMenu : Control
{
  [Export]
  public TextureRect PageTop;
  [Export]
  public Wa2Button ExitBtn;
  [Export]
  public GridContainer DataSlots;
  [Export]
  public HBoxContainer Tabs;
  public int _pageNum=0;
  public Wa2EngineMain _engine;
  private DataMode _mode;
  public override void _Ready()
  {
    _engine=Wa2EngineMain.Engine;
    ExitBtn.ButtonDown += OnExitBtnDown;
    for(int i=0;i<10;i++)
    {
      int idx=i;
      DataSlots.GetChild<Wa2Button>(i).ButtonDown +=()=>OnDataSlotDown(idx);
    }
    for (int i=0;i<10;i++){
      int idx=i;
      Tabs.GetChild<Wa2Button>(i).ButtonDown +=()=>{
        _pageNum=idx;
        UpdatePage();
      };
    }
  }
  public void OnDataSlotDown(int idx)
  {
    if (_mode == DataMode.Save)
    {
      _engine.GameSav.SaveData(_pageNum*10+idx);
    }
    else if (_mode == DataMode.Load)
    {

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

    Show();
    UpdatePage();
  }
  public void Close()
  {
    Hide();
  }
  public void OnExitBtnDown()
  {
    Close();
  }
  public void UpdatePage(){
    for (int i=0;i<10;i++){
      DataSlots.GetChild<DataSlot>(i).Update(_pageNum*10+i);
    }
  }
}