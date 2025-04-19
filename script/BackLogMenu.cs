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
    ScrollBar.ValueChanged += OnScrollBarValChanged;
  }
  public override void Open()
  {
    base.Open();
    ScrollBar.MaxValue = Math.Max(0, _engine.Backlogs.Count - 4);
    ScrollBar.Value = ScrollBar.MaxValue;
    OnScrollBarValChanged(ScrollBar.Value);
  }
  public void OnScrollBarValChanged(double val)
  {
    int start = Math.Max(0, (int)val - 4);
    int count = Math.Min(_engine.Backlogs.Count - start, 4);
    GD.Print(count);
    for (int i = 0; i < 4; i++)
    {
      BackLogItem item = BackLogItems.GetChild<BackLogItem>(i);
      if (i >= count)
      {
        item.Hide();
      }
      else
      {
        item.Show();
        item.SetInfo(_engine.Backlogs[start + i]);
      }

    }
  }
}