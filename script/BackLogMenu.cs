using System;
using Godot;
public partial class BackLogMenu : BasePage
{
  [Export]
  public VBoxContainer BackLogItems;
  [Export]
  public VScrollBar ScrollBar;
  public override void _Ready()
  {
    base._Ready();
    Modulate = new Color(1, 1, 1, 1);
    Scale = new Vector2(1, 1);
    ScrollBar.ValueChanged += OnScrollBarValChanged;
  }
  public override void Open()
  {
    Show();
    _engine.AdvMain.Hide();
    ScrollBar.MaxValue = Math.Max(0, _engine.Backlogs.Count - 4);
    ScrollBar.Value = ScrollBar.MaxValue;
    OnScrollBarValChanged(ScrollBar.Value);
  }
  public override void Close()
  {
    Hide();
    _engine.UiMgr.ReturnScene();
    _engine.AdvMain.Show();
  }


  public void OnScrollBarValChanged(double val)
  {
    int pos=(int)val;
    for (int i = 0; i < 4; i++)
    {
      BackLogItem item = BackLogItems.GetChild<BackLogItem>(i);
      if (pos+i >= _engine.Backlogs.Count)
      {
        item.Hide();
      }
      else
      {
        item.Show();
        item.SetInfo(_engine.Backlogs[pos + i]);
      }

    }
  }
}