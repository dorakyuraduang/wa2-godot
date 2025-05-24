using Godot;
using System;

public partial class NovelBackLogMenu : BasePage
{
	[Export]
	public VScrollBar ScrollBar;
	[Export]
	public Wa2Label TextLabel;
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
		BgmPlayer.Show();
		_engine.AdvMain.Hide();
		ScrollBar.MaxValue = Math.Max(0, _engine.Backlogs.Count-1);
		ScrollBar.Value = ScrollBar.MaxValue;
		OnScrollBarValChanged(ScrollBar.Value);
	}
	public override void Close()
	{
		Hide();
		BgmPlayer.Hide();
		_engine.UiMgr.ReturnScene();
		_engine.AdvMain.Show();
	}


	public void OnScrollBarValChanged(double val)
	{
		int pos = (int)val;
		TextLabel.Segment = _engine.Backlogs[pos].Segment;
		TextLabel.SetText(_engine.Backlogs[pos].Text);
		
	}
}
