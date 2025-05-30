using Godot;
using System;
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
    if (FileAccess.FileExists(string.Format("user://sav{0:D2}.sav", idx)))
    {

      FileAccess file = FileAccess.Open(string.Format("user://sav{0:D2}.sav", idx), FileAccess.ModeFlags.Read);
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
      // GD.Print(idx);
      string scriptName = file.GetBuffer(8).GetStringFromUtf8().Replace("\0", "");
      Category.Show();
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
      else if (scriptName == "5000")
      {
        texture.Region = new Rect2(0, 144, 248, 24);
      }
      else if (scriptName == "5001")
      {
        texture.Region = new Rect2(0, 168, 248, 24);
      }
      else if (scriptName == "5002")
      {
        texture.Region = new Rect2(0, 192, 248, 24);
      }
      else if (scriptName == "5003")
      {
        texture.Region = new Rect2(0, 216, 248, 24);
      }
      else if (scriptName == "5004")
      {
        texture.Region = new Rect2(0, 240, 248, 24);
      }
      else if (scriptName == "5100")
      {
        texture.Region = new Rect2(0, 264, 248, 24);
      }
      else if (scriptName == "5101")
      {
        texture.Region = new Rect2(0, 288, 248, 24);
      }
      else if (scriptName == "5102")
      {
        texture.Region = new Rect2(0, 312, 248, 24);
      }
      else if (scriptName == "5103")
      {
        texture.Region = new Rect2(0, 336, 248, 24);
      }
      else if (scriptName == "5104")
      {
        texture.Region = new Rect2(0, 360, 248, 24);
      }

      if (scriptName[0] == '2' || scriptName[0] == '3')
      {
        Month.Show();
        DayLabel.Show();

        AtlasTexture texture2 = (AtlasTexture)Month.Texture;
        file.Get32();
        int m = (int)file.Get32() - 1;
        texture2.Region = new Rect2(m / 4 * 40, m % 4 * 24, 40, 24);
        DayLabel.Text = string.Format("{0:D2}", file.Get32());
        file.Get32();
      }
      else
      {
        Month.Hide();
        DayLabel.Hide();
        file.Seek(file.GetPosition() + 16);

      }
      string text = Encoding.Unicode.GetString(file.GetBuffer(1024)).Replace("\n", "").Replace("\\n", "").Replace("\0", "");
      if (text.Length >= 14)
      {
        text = text.Substring(0, 13) + "…";
      }
      FirstSentenceLabel.SetText(text);
      NoData.Hide();
      DateLabel.Show();
      ExistData.Show();
      file.Close();

    }
    else
    {
      FirstSentenceLabel.SetText("");
      FirstSentenceLabel.Clear();
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