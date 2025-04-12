using Godot;

public partial class Wa2SelectMessage : Wa2Button
{
  [Export]
  public Wa2Label TextLabel;
  [Export]
  public TextureRect LabelTexture;
  public override void _Ready(){
    FocusEntered+=OnFocusEntered;
    FocusExited+=OnFocusExited;
  }
  public void OnFocusEntered(){
    TextLabel.Color=new Color(1,1,1,1);
    // TextLabel.Update();

  }
  public void OnFocusExited(){
    TextLabel.Color=new Color(1,0.968f,0.6f,1);
    // TextLabel.Update();

  }
  public void SetText(string text)
  {
    TextLabel.Text = text;
    // TextLabel.Update();
  }
}