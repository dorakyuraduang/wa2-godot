using System;
// using FFmpeg.AutoGen;
using Godot;

public partial class BgmPlayer : Control
{
  [Export]
  public TextureRect BgmName;
  [Export]
  public TextureProgressBar BgmVolume;
  public AudioEffectSpectrumAnalyzerInstance _spectrum;
  [Export]
  public int ListenType=0;
  public override void _Ready()
  {
    _spectrum = AudioServer.GetBusEffectInstance(ListenType+1, 0) as AudioEffectSpectrumAnalyzerInstance;
    if (ListenType != 0)
    {
      BgmName.Hide();
    }
  }
  public override void _Process(double delta)
  {
    if (!Visible)
    {
      return;
    }
    float m = _spectrum.GetMagnitudeForFrequencyRange(0, 3200, AudioEffectSpectrumAnalyzerInstance.MagnitudeMode.Average).Length();
    BgmVolume.Value = Math.Clamp((60 + Mathf.LinearToDb(m)) / 60, 0.0f, 1.0f);
    if (ListenType != 0)
    {
      return;
    }
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