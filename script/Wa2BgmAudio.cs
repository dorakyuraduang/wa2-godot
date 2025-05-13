using Godot;
using System;

public partial class Wa2BgmAudio : Wa2Audio
{

	private AudioStream _start_stream;
	private AudioStream _loop_stream;
	public override void _Ready()
	{
		Finished += _OnFinished;

	}
	public void SetLoopStream(AudioStream stream)
	{
		_loop_stream = stream;
	}
	private void _OnFinished()
	{
		if (Loop)
		{
			PlaySound(_loop_stream, true, 0, Volume);
		}
		else
		{
			Stop();
		}
	}
	public void PlaySound(AudioStream stream, bool loop, float time, int volume)
	{
		
		Seek(0);
		Loop = loop;
		Stream = stream;
		SetVolume(0,0);
		SetVolume(volume,time);
		Play();

	}
}
