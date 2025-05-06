using System;
using FFmpeg.AutoGen;
using Godot;

public partial class BgmPlayer : Control
{
  [Export]
  public TextureRect BgmName;
  [Export]
  public TextureProgressBar BgmVolume;
  public AudioEffectSpectrumAnalyzerInstance _spectrum;
  public override void _Ready()
  {
    _spectrum = AudioServer.GetBusEffectInstance(1, 0) as AudioEffectSpectrumAnalyzerInstance;
  }
  public override void _Process(double delta)
  {
    if (!Visible)
    {
      return;
    }
    float m = _spectrum.GetMagnitudeForFrequencyRange(0, 3200, AudioEffectSpectrumAnalyzerInstance.MagnitudeMode.Average).Length();
    // GD.Print(_spectrum.GetMagnitudeForFrequencyRange(20,20000).Length());
    BgmVolume.Value = Math.Clamp((60 + Mathf.LinearToDb(m)) / 60, 0.0f, 1.0f);
    int pos = Array.IndexOf(Wa2Def.BgmSlot, Wa2EngineMain.Engine.SoundMgr.BgmId);
    if (pos >= 0)
    {
      BgmName.Show();
      (BgmName.Texture as AtlasTexture).Region = new Rect2(0, 16 * pos, 264, 16);
    }
    else
    {
      BgmName.Hide();
    }


  }
}