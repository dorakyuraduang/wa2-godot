using Godot;
using System;

public partial class SelectMessage : Wa2Button
{
  [Export]
  public Wa2Label TextLabel;
  [Export]
  public TextureRect ReadLabel;
  public override void _Ready()
  {
    MouseEntered += OnMouseEntered;
    MouseExited += OnMouseExited;
  }
  public void OnMouseEntered()
  {
    if (Disabled)
    {
      return;
    }
    Modulate = new Color(2.0f, 2.0f, 2.0f, 1.0f);
  }

  public void OnMouseExited()
  {
    if (Disabled)
    {
      return;
    }
    Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
  }
  public void DeActive()
  {
    TextLabel.Color=new Color(0.5f,0.5f,0.5f,1.0f);
    Disabled = true;
  }
  public void Active()
  {
    TextLabel.Color=new Color(1.0f,1.0f,1.0f,1.0f);
    Disabled = false;
  }
}
