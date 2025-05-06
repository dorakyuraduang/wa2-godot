using Godot;
[GlobalClass]
public partial class Wa2Ani : Node{
  [Export(PropertyHint.File)]
  public string path;
  [Export]
  public Texture2D Texture;
  public override void _Ready()
  {
    LoadAni();
  }
  public void LoadAni()
  {
    FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
    string fileName=path.GetFile();
    if (file != null)
    {
      SpriteFrames s=new();
      file.Seek(24);
      int frames = (int)file.Get32();
       s.AddAnimation("start");
       s.SetAnimationSpeed("start", 60);
      for (int i = 0; i < frames; i++)
      {
        AtlasTexture atlas = new();
       
        atlas.Atlas = Texture;
        file.Seek((ulong)(64 + i * 64 + 20));
        int x = file.Get16();
        int y = file.Get16();
        int w = file.Get16();
        int h = file.Get16();
        GD.Print("idx:",i,"x:",x, "y:",y,"w:",w,"h:",h);
        atlas.Region = new Rect2(x, y, w, h);
        // file.Seek(file.GetPosition() + 1);
        // int ox = file.Get8();
        // file.Seek(file.GetPosition() + 1);
        // int oy = file.Get8();
        // atlas.Margin = new Rect2(ox, oy, 0, 0);
        s.AddFrame("start",atlas,1,i);
      }
      ResourceSaver.Save(s,"res://assets/ani/"+fileName+".tres");
    }
  }
}