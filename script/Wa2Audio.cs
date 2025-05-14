using Godot;
using System;

public partial class Wa2Audio : AudioStreamPlayer
{
	protected float _duration;
	protected float _counter;
	protected int _state;
	protected float _volume;
	public string StreamPath;
	public int Volume { private set; get; }
	public bool Loop { get; protected set; }
	public void StopStream(float time)
	{
		Loop=false;
		_duration = time;
		_counter = 0;
		if (_duration > 0.0f)
		{
			_state = 2;
		}
		else
		{
			Stop();
			Stream=null;
			
		}
	}
	public void SetVolume(int volume, float frame)
	{
		Volume = volume;
		_volume = volume / 256.0f;
		_duration = frame;
		_counter = 0;
		if (_duration > 0)
		{
			_state = 1;
		}
		else
		{
			_state = 0;
			VolumeDb = Mathf.LinearToDb(_volume);
		}
	}
	// public void PlayStream(AudioStream stream, bool loop, float time, int volume)
	// {
	// 	_duration = time;
	// 	Seek(0);
	// 	_counter = 0;
	// 	_volume = volume;
	// 	Loop = loop;
	// 	Stream = stream;
	// 	Play();
	// 	if (_duration > 0)
	// 	{
	// 		_state = 1;
	// 	}
	// 	else
	// 	{
	// 		_state = 0;
	// 		VolumeDb = Mathf.LinearToDb(volume);
	// 	}

	// }
	public override void _Process(double delta)
	{
		switch (_state)
		{
			case 1:
				_counter += (float)delta;

				if (_counter >= _duration)
				{
					_state = 0;
					VolumeDb = Mathf.LinearToDb(_volume);
				}
				else
				{
					VolumeDb = Mathf.LinearToDb((float)_counter / _duration * _volume);
				}
				break;
			case 2:
				_counter += (float)delta;

				if (_counter >= _duration)
				{
					_state = 0;
					Stop();
					Stream=null;
					
				}
				else
				{
					VolumeDb = Mathf.LinearToDb((1.0f-(float)_counter / _duration) * _volume);
				}
				break;
		}
	}
	// 	private void _OnFinished()
	// {
	// 	if (Loop)
	// 	{
	// 		PlayStream(Stream, true, 0, Volume);
	// 	}
	// 	else
	// 	{
	// 		Stop();
	// 	}
	// }
	// public override void _Ready()
	// {
	// 	Finished+=_OnFinished;
	// }

}
