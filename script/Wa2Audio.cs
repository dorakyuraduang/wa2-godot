using Godot;
using System;

public partial class Wa2Audio : AudioStreamPlayer
{
	protected float _duration;
	protected float _counter;
	protected int _state;
	protected float _volume;
	public bool Loop{ get;private set;}
	public void StopStream(float time)
	{

		_duration = time;
		_counter = 0;
		if (_duration > 0.0f)
		{
			_state = 2;
		}
		else
		{
			Stop();
		}
	}
	public void SetVolume(float volume, float frame)
	{
		_duration = frame;
		_counter = 0;
		_volume = volume;
		if (_duration > 0)
		{
			_state = 1;
		}
		VolumeDb = Mathf.LinearToDb(volume);
	}
	public void PlayStream(AudioStream stream, bool loop, float time, float volume)
	{
		_duration = time;
		Seek(0);
		_counter = 0;
		_volume = volume;
		Loop = loop;
		Stream = stream;
		Play();
		if (_duration > 0)
		{
			_state = 1;
		}
		else
		{
			_state = 0;
			VolumeDb = Mathf.LinearToDb(volume);
		}

	}
	public override void _Process(double delta)
	{
		switch (_state)
		{
			case 1:
				_counter += (float)delta;

				if (_counter >= _duration)
				{
					_state = 0;
					VolumeDb=Mathf.LinearToDb(_volume);
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
				}
				else
				{
					VolumeDb = Mathf.LinearToDb(0);
				}
				break;
		}
	}
		private void _OnFinished()
	{
		if (Loop)
		{
			PlayStream(Stream, true, 0, _volume);
		}
		else
		{
			Stop();
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Finished+=_OnFinished;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
}
