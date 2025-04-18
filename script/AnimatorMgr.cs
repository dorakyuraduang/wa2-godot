using System.Collections.Generic;
using Godot;
public class Animator
{
  private Tween _tween;
  private float _duration;
  public Animator(Tween tween, float duration)
  {
    _tween = tween;
    _duration = duration;
  }
  public void Finish()
  {
    _tween.Stop();
    _tween.CustomStep(_duration);
  }
  public bool IsActive()
  {
    return _tween != null && _tween.IsValid() && _tween.IsRunning();
  }
}
[GlobalClass]
public partial class AnimatorMgr : Node
{
  public Wa2EngineMain _engine;
  public List<Animator> _animators = new();
  public void AddAnimator(Animator animator)
  {
    _animators.Add(animator);

  }
  public void _Process()
  {
    for (int i = 0; i < _animators.Count; i++)
    {
      _animators[i].Finish();
      _animators.RemoveAt(i);
      i--;
    }
  }
  public void FinishAll()
  {
    for (int i = 0; i < _animators.Count; i++)
    {
      if (!_animators[i].IsActive())
      {
        _animators.RemoveAt(i);
        i--;
      }
    }
  }
  public void AddFeadAnimation(Sprite2D t, float duration, float target)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, duration);
    tween.TweenProperty(t, "modulate:a", target, duration);
    _animators.Add(animator);
  }
}