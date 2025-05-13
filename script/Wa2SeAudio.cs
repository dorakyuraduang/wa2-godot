using System.Runtime.Serialization.Formatters;
using Godot;
public partial class Wa2SeAudio : Wa2Audio
{
  public int Id = -1;
  public void StopSound(float time)
  {
    Loop = false;
    Id = -1;
    StopStream(time);

  }
  public void PlaySound(int id, bool loop, float time, int volume)
  {


    Loop = loop;
    if (id != Id)
    {
      Id = id;
      Seek(0);
      SetVolume(0, 0);
      Stream = Wa2Resource.GetSeStream(id);
    }
    SetVolume(volume, time);
    Play();
  }
  public override void _Ready()
  {
    Finished += _OnFinished;

  }
  private void _OnFinished()
  {
    if (Loop)
    {
      Seek(0);
      PlaySound(Id, Loop, 0, Volume);
    }
    else
    {
      StopSound(0);
    }
  }
}