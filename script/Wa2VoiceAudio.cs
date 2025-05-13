using Godot;
public partial class Wa2VoiceAudio : Wa2Audio
{
  public int Id;
  public int Chr;
  public int Label;
  public void PlaySound(AudioStream stream, bool loop, int volume)
  {
    SetVolume(0,0);
    Seek(0);
    _counter = 0;
    SetVolume(volume,0);
    Loop = loop;
    Stream = stream;
    Play();
    _state = 0;
  }
}