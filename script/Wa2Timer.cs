using Godot;
using System;

public partial class Wa2Timer : RefCounted
{
	private float _delay = 0f;
	private float _progress = 0f;
	private bool _active = false;
	private float _waitTime;
	private float _startTime;
	public void Start(float waitTime, float delay = 0f)
	{
		_delay = delay;
		_progress = 0.0f;
		_startTime = 0f;
		_waitTime = waitTime;
		_active = true;
	}
	public bool IsStart()
	{
		return _delay <= 0;
	}
	public void Update(float delta)
	{
		if (!_active)
		{
			return;
		}
		if (_delay > 0f)
		{
			_delay -= delta;
			return;
		}
		if (_progress >= 1f)
		{
			return;
		}
		_progress = _startTime / _waitTime;
		_startTime += delta;
	}
	public void Done()
	{
		_progress = 1.0f;
		_delay = 0.0f;
	}
	public bool IsDone()
	{
		return _progress >= 1.0f;
	}
	public bool IsActive()
	{
		return _active;
	}
	public void DeActive()
	{
		_active = false;
	}
	public float GetProgress()
	{
		return _progress;
	}
}

