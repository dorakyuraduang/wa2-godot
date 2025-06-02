using System.Collections.Generic;
using Godot;

public partial class CGViewer : TextureRect
{
  public List<int> CgList = new();
  public int CurIdx;
  public void Open(List<int> list)
  {
    CgList = list;
    CurIdx = 0;
    Show();
   Texture=Wa2Resource.GetCgImage(CgList[CurIdx]);
  }
  public override void _GuiInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed())
    {
      if (CurIdx >= CgList.Count - 1)
      {
        Close();
      }
      else
      {
        CurIdx++;
       Texture=Wa2Resource.GetCgImage(CgList[CurIdx]);
      }
    }
  }
  public void Close()
  {
    Hide();
  }
}