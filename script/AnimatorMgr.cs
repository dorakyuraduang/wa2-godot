using System.Collections.Generic;
using Godot;
public class Animator
{
  private Tween _tween;
  private float _duration;
  public bool Wait = true;
  public Animator(Tween tween, float duration)
  {
    _tween = tween;
    _duration = duration;
    // _tween.SetParallel(true);
  }
  public void Finish()
  {
    _tween.Stop();
    _tween.CustomStep(_duration+1.0);
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
  public override void _Ready()
  {
    _engine = Wa2EngineMain.Engine;
  }
  public void AddAnimator(Animator animator)
  {
    _animators.Add(animator);

  }
  public void AllWait()
  {
    for (int i = 0; i < _animators.Count; i++)
    {
      _animators[i].Wait = true;
    }
  }
  public bool WaitAnimation()
  {
    for (int i = 0; i < _animators.Count; i++)
    {
      if (_animators[i].Wait == true)
      {
        return true;
      }
    }
    return false;
  }
  public override void _Process(double delta)
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
  public void FinishAll(bool all = false)
  {

    for (int i = 0; i < _animators.Count; i++)
    {
      if (_animators[i].Wait || all)
      {
        if (_animators[i].IsActive())
        {
          _animators[i].Finish();
        }
        _animators.RemoveAt(i);
        i--;
      }

    }
  }
  public void AddBgMoveAnimation(Wa2Image image, float time, float x, float y)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    animator.Wait = false;
    Vector2 StartOffset = image.GetCurOffset();
    tween.TweenMethod(Callable.From<Vector2>(image.SetCurOffset), StartOffset, new Vector2(x, y), time);

  }
  public void AddMaskFeadAnimation(Wa2Image image, float time, bool mask = true)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    image.Show();
    image.SetBlend(0f);
    if (image.GetMaskTexture() == null)
    {
      (image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 0.5);
    }
    else
    {
      (image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 1.0);
    }
    tween.TweenMethod(Callable.From<float>(image.SetBlend), 0f, 1f, time);
    // tween.TweenCallback(Callable.From(() => image.SetCurTexture(image.GetNextTexture())));
    // tween.TweenCallback(Callable.From(() => image.SetNextTexture(null)));
    tween.TweenCallback(Callable.From(() =>
    {
      if (mask)
      {
        image.Hide();
        image.SetCurTexture(null);
      }else{
        image.SetCurTexture(image.GetNextTexture());
      }
      image.SetNextTexture(null);
      image.SetBlend(0);
    }));
    // tween.TweenCallback(Callable.From(() => image.SetMaskTexture(null))).SetDelay(time);
    // 				Image.SetCurTexture(Image.GetNextTexture());
    // 				Image.SetMaskTexture(null);
    // 				Image.SetNextTexture(null);
    // 				Image.SetBlend(0.0f);
    // 			}
  }
  public void AddFeadAnimation(CanvasItem t, float duration, float target)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, duration);
    AddAnimator(animator);
    tween.TweenProperty(t, "modulate:a", target, duration);
  }
  public void AddAdvFeadAnimation(CanvasItem t, float duration, bool fadein)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, duration);
    AddAnimator(animator);
    if (fadein)
    {
      t.Show();
    }
    tween.TweenProperty(t, "modulate:a", fadein ? 1 : 0, duration);
    tween.TweenCallback(Callable.From(() =>
      {
        if (!fadein)
        {
          t.Hide();
        }
      })).SetDelay(duration);
  }
  public void AddCharFeadAnimation(Wa2Image image, Texture2D texture, float time)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    image.SetNextTexture(texture);
    tween.TweenMethod(Callable.From<float>(image.SetBlend), 0f, 1f, time);
    tween.TweenCallback(Callable.From(() => image.SetCurTexture(texture)));
    tween.TweenCallback(Callable.From(() => image.SetNextTexture(null)));
    tween.TweenCallback(Callable.From(() => image.SetBlend(0)));
  }
  public void AddFBAnimation(Color color, float time)
  {
    GD.Print("时间", time);
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    tween.TweenMethod(Callable.From<Color>(_engine.SetFBColor), _engine.GetFBColor(), color, time);
  }
}