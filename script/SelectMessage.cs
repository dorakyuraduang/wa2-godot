using Godot;
using System;

public partial class SelectMessage : Wa2Button
{
	[Export]
	public Wa2Label TextLabel;
  public override void _Ready()
  {
    MouseEntered += OnMouseEntered;
		MouseExited+=OnMouseExited;
  }
	public void OnMouseEntered(){
		Modulate=new Color(2.0f,2.0f,2.0f,1.0f);
	}

	public void OnMouseExited(){
		Modulate=new Color(1.0f,1.0f,1.0f,1.0f);
	}
}
