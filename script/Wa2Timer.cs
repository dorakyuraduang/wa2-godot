using Godot;
using System;

public partial class Wa2Timer : RefCounted
{
	private float _progress = 0f;
	private bool _active = false;
	private float _waitTime;
	private float _startTime;
	public void Start(float waitTime)
	{
		_progress = 0.0f;
		_startTime = 0f;
		_waitTime = waitTime;
		_active = true;
	}
	public void Update(float delta)
	{
		if (!_active)
		{
			return;
		}
		_progress = _startTime / _waitTime;
		_startTime += delta;
		if (_progress >= 1f)
		{
			Done();
			return;
		}
	}
	public void Done()
	{
		_progress = 1.0f;
		_active = false;
	}
	public bool IsActive()
	{
		return _active;
	}
	public float GetProgress()
	{
		return _progress;
	}
}

