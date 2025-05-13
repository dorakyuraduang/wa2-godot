using Godot;
using System;

public partial class Wa2BgmAudio : Wa2Audio
{

	private AudioStream _start_stream;
	private AudioStream _loop_stream;
	public override void _Ready()
	{
		Finished+=_OnFinished;

	}
	public void SetLoopStream(AudioStream stream)
	{
		_loop_stream = stream;
	}
	private void _OnFinished()
	{
		if (Loop)
		{
			PlayStream(_loop_stream, true, 0, _volume);
		}
		else
		{
			Stop();
		}
	}

}
