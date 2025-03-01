using Godot;
using System;

public partial class Wa2Audio : AudioStreamPlayer
{
	protected int _duration;
	protected int _counter;
	protected bool _loop;
	protected int _state;
	protected float _volume;
	public void StopStream(int frame)
	{
		_duration = frame;
		_counter = 0;
		if (_duration > 0)
		{
			_state = 2;
		}
		else
		{
			Stop();
		}
	}
	public void SetVolume(float volume, int frame)
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
	public void PlayStream(AudioStream stream, bool loop, int frame, float volume)
	{
		_duration = frame;
		Seek(0);
		_counter = 0;
		_volume = volume;
		_loop = loop;
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
public void Update()
	{
		switch (_state)
		{
			case 1:
				_counter += 1;
				VolumeDb = Mathf.LinearToDb((float)_counter / _duration * _volume);
				if (_counter == _duration)
				{
					_state = 0;
				}
				break;
			case 2:
				_counter += 1;
				VolumeDb = Mathf.LinearToDb((1.0f - (float)_counter / _duration) * _volume);
				if (_counter == _duration)
				{
					_state = 0;
					Stop();
				}
				break;
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
