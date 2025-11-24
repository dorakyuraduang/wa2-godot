using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;
using Godot;
public class ContentSegment
{
  public int Begin { get; set; }
  public int End { get; set; }
  public string Text { get; set; }
}
public class SoundSubtitle
{
  public int Id { get; set; }
  public List<ContentSegment> Content { get; set; }
}
public class VoiceSubtitle
{
  public int Id { get; set; }
  public int Scene { get; set; }
  public List<ContentSegment> Content { get; set; }
}
public class SubtitleRoot
{
  public List<SoundSubtitle> SoundSubtitle { get; set; }
  public List<VoiceSubtitle> VoiceSubtitle { get; set; }
}

public partial class SubtitleMgr : Node
{
  public List<SoundSubtitle> SoundSubtitleList;
  public List<VoiceSubtitle> VoiceSubtitleList;
  public Wa2Audio ListenAudio;
  public List<ContentSegment> ListenContent;
  [Export]
  public Label TextLabel;
  public override void _Ready()
  {
    var file = FileAccess.Open("res://assets/sub.yaml", FileAccess.ModeFlags.Read);
    string yamlText = file.GetAsText();
    file.Close();

    var deserializer = new DeserializerBuilder()
     .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();

    var result = deserializer.Deserialize<SubtitleRoot>(yamlText);
    SoundSubtitleList = result.SoundSubtitle;
    VoiceSubtitleList = result.VoiceSubtitle;

    // foreach (var se in VoiceSubtitleList)
    // {
    //   GD.Print($"SoundEffect ID: {se.Id}");
    //   foreach (var segment in se.Content)
    //   {
    //     GD.Print($"  {segment.Begin}-{segment.End}: {segment.Text}");
    //   }
    // }
  }
  public void ListenVoice(int scene, int id, Wa2Audio audio)
  {
    for (int i = 0; i < VoiceSubtitleList.Count; i++)
    {
      if (VoiceSubtitleList[i].Id == id && VoiceSubtitleList[i].Scene == scene)
      {
        ListenContent = VoiceSubtitleList[i].Content;
        ListenAudio = audio;
        return;
      }
    }
    if (ListenAudio == null)
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }
  }
  public void ListenSe(int id, Wa2Audio audio)
  {
    for (int i = 0; i < SoundSubtitleList.Count; i++)
    {
      if (SoundSubtitleList[i].Id == id)
      {
        ListenContent = SoundSubtitleList[i].Content;
        ListenAudio = audio;
        return;
      }
    }
    if (ListenAudio == null)
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }

  }
  public override void _Process(double delta)
  {
    if (ListenAudio != null && ListenAudio.Stream != null && ListenAudio.Playing && ListenContent != null)
    {
      foreach (ContentSegment segment in ListenContent)
      {
        if (ListenAudio.GetPlaybackPosition() * 1000 >= segment.Begin && ListenAudio.GetPlaybackPosition() * 1000 <= segment.End)
        {
          TextLabel.Text = segment.Text;
        }
      }
    }
    else
    {
      TextLabel.Text = "";
      ListenAudio = null;
      ListenContent = null;
    }
  }
}
