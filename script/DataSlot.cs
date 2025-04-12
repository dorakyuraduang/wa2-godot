using Godot;
using System.Text;
public partial class DataSlot : Wa2Button
{
  [Export]
  public TextureRect NoData;
  [Export]
  public TextureRect ExistData;
  [Export]
  public TextureRect SaveTexture;
  [Export]
  public Label IdxLabel;
  [Export]
  public Label DateLabel;
  [Export]
  public TextureRect Category;
  [Export]
  public Label DayLabel;
  [Export]
  public TextureRect Month;
  [Export]
  public Wa2Label FirstSentenceLabel;
  public void Update(int idx)
  {
    IdxLabel.Text = string.Format("{0:D2}", idx + 1);
    if (FileAccess.FileExists(string.Format("user://sav{0:D2}.sav",idx)))
    {

      FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav",idx), FileAccess.ModeFlags.Read);
      int year = (int)file.Get32();
      int month = (int)file.Get32();
      int dayOfWeek = (int)file.Get32();
      int day = (int)file.Get32();
      int hour = (int)file.Get32();
      int minute = (int)file.Get32();
      int second = (int)file.Get32();
      int millisecond = (int)file.Get32();
      DateLabel.Text = string.Format("{0:D4} {1:D2}/{2:D2} {3:D2}:{4:D2}", year, month, day, hour, minute);
      SaveTexture.Texture = ImageTexture.CreateFromImage(Image.CreateFromData(256, 144, false, Image.Format.Rgb8, file.GetBuffer(0x1b000)));
      string scriptName = file.GetBuffer(8).GetStringFromUtf8();
      Category.Show();
      if (scriptName[0]=='2' || scriptName[0]=='3'){
        Month.Show();
        DayLabel.Show();
        DayLabel.Text=string.Format("{0:D2}", day);
        AtlasTexture texture2 = (AtlasTexture)Category.Texture;
        texture2.Region = new Rect2(month/4*40, month%4*24, 40, 24);
      }else{
        Month.Hide();
        DayLabel.Hide();
      }
      AtlasTexture texture = (AtlasTexture)Category.Texture;
      if (scriptName[0] == '1')
      {
        texture.Region = new Rect2(0, 0, 248, 24);
      }
      else if (scriptName[0] == '2')
      {
        texture.Region = new Rect2(0, 24, 248, 24);
      }
      else if (scriptName[0] == '3')
      {
        texture.Region = new Rect2(0, 48, 248, 24);
      }
      FirstSentenceLabel.Text=Encoding.Unicode.GetString(file.GetBuffer(256)).Replace("\n","").Replace("\0","");
      NoData.Hide();
      DateLabel.Show();
      ExistData.Show();
      file.Close();

    }
    else
    {
      FirstSentenceLabel.Text="";
      Category.Hide();
      SaveTexture.Texture = null;
      NoData.Show();
      ExistData.Hide();
      DateLabel.Hide();
      Month.Hide();
      DayLabel.Hide();
    }
  }
}