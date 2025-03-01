using Godot;
using System;
public partial class Wa2SoundMgr : Node
{
	private Wa2BgmAudio _bgmAudio=new ();
	private AudioStreamPlayer _sysSeAudio=new();
	public void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	{
		_bgmAudio.PlayStream(Wa2Resource.GetBgmStream(id, false), loopFlag, 0, volume / 255.0f);
		_bgmAudio.SetLoopStream(Wa2Resource.GetBgmStream(id, true));
	}
	public void StopBgm(int frame = 0)
	{
		_bgmAudio.StopStream(frame);
	}

	public static Wa2SoundMgr Instance
	{
		private set; get;

	}
	public override void _Ready()
	{
		Instance = this;
		_sysSeAudio.Stream=new AudioStreamPolyphonic();
		
		AddChild(_bgmAudio);
		AddChild(_sysSeAudio);
		_sysSeAudio.Play();

	}
	public void PlaySysSe(AudioStream stream)
	{
		AudioStreamPlaybackPolyphonic playBack=(AudioStreamPlaybackPolyphonic)_sysSeAudio.GetStreamPlayback();
		if (stream != null)
		{
			playBack.PlayStream(stream);
		}
	}
}
