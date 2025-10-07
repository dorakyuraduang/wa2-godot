using Godot;
using System;
using System.Collections.Generic;

public partial class ErrorMessage : Control
{
	[Export]
	public Label label;
	[Export]
	public TextureButton btn;
	public List<string> _errorMessageList = new();

	public override void _Ready()
	{
		btn.ButtonDown += () =>
		{
			_errorMessageList.RemoveAt(0);
			if (_errorMessageList.Count == 0)
			{
				Visible = false;
			}
			else
			{
				label.Text = _errorMessageList[0];
			}


		};
	}
	public void Open(string message)
	{
		_errorMessageList.Add(message);
		if (!Visible)
		{
			label.Text = _errorMessageList[0];
			Visible = true;
		}

	}
}
