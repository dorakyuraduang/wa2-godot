using System;
using System.Collections.Generic;
// using FFmpeg.AutoGen;
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
    if (Wait)
    {
      _tween.CustomStep(_duration + 1.0);
    }
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
  public void AddBgFeadAnimation(Wa2Image image, float time, Vector2 offset, Vector2 scale)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    image.SetBlend(0f);
    if (image.GetMaskTexture() == null)
    {
      (image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 0.5);
    }
    else
    {
      (image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 1.0);
    }
    image.SetNextOffset(offset);
    image.SetNextScale(scale);
    tween.TweenMethod(Callable.From<float>(image.SetBlend), 0f, 1f, time);
    // tween.TweenMethod(Callable.From<Vector2>(image.SetNextOffset), image.GetCurOffset(), offset, time);
    // tween.TweenMethod(Callable.From<Vector2>(image.SetNextScale), image.GetCurScale(), scale, time);
    // tween.SetParallel(false);
    tween.TweenCallback(Callable.From(() =>
    {
      image.SetCurOffset(offset);
      image.SetCurScale(scale);
      image.SetMaskTexture(null);
      image.SetCurTexture(image.GetNextTexture());
      image.SetNextTexture(null);
      image.SetBlend(0);
    }));
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
      }
      else
      {
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
  public void AddAdvFeadAnimation(Wa2AdvMain adv, float duration, bool fadein)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, duration);
    AddAnimator(animator);
    if (fadein)
    {
      adv.Show();

      adv.State = Wa2AdvMain.AdvState.FADE_IN;
    }
    else
    {
      adv.State = Wa2AdvMain.AdvState.FADE_OUT;
    }
    tween.TweenProperty(adv, "modulate:a", fadein ? 1 : 0, duration);
    tween.TweenCallback(Callable.From(() =>
      {
        if (!fadein)
        {
          adv.Hide();
        }
        else
        {
          adv.State = Wa2AdvMain.AdvState.PARSE_TEXT;
          adv.NameLabel.Update(-1);
        }
      }));
  }
  public void AddCharFeadAnimation(Wa2Image image, Texture2D texture, float time)
  {
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    image.SetNextTexture(texture);
    tween.TweenMethod(Callable.From<float>(image.SetBlend), 0f, 1f, time);
    tween.TweenCallback(Callable.From(() =>
    {
      image.SetCurTexture(texture);
      image.SetNextTexture(null);
      image.SetBlend(0);
      if (texture == null)
      {
        image.Hide();
      }
    }));
  }
  public void AddShakeAnimation(int type, int strength, int frame)
  {
    if (type != 9 && type != 1)
    {
      return;
    }
    Tween tween = CreateTween();
    Animator animator = new(tween, frame * _engine.FrameTime);
    AddAnimator(animator);
    Random rand = new Random();
    switch (type)
    {
      case 1:
        for (int i = 0; i < frame / 2; i++)
        {
          int dir = rand.Next(0, 2);
          Vector2 offset = new(0, 0);
          if (dir == 0)
          {
            offset.X = -strength;
          }
          else if (dir == 1)
          {
            offset.X = strength;
          }
          tween.TweenProperty(_engine.SubViewport, "position", offset, _engine.FrameTime);
          tween.TweenProperty(_engine.SubViewport, "position", new Vector2(0, 0), _engine.FrameTime);
        }
        break;
      case 9:
        for (int i = 0; i < frame / 2; i++)
        {
          int dir = rand.Next(0, 4);
          Vector2 offset = new(0, 0);
          if (dir == 0)
          {
            offset.X = -strength;
          }
          else if (dir == 1)
          {
            offset.X = strength;
          }
          else if (dir == 2)
          {
            offset.Y = -strength;
          }
          else if (dir == 3)
          {
            offset.Y = strength;
          }
          tween.TweenProperty(_engine.SubViewport, "position", offset, _engine.FrameTime);
          tween.TweenProperty(_engine.SubViewport, "position", new Vector2(0, 0), _engine.FrameTime);
        }
        break;
    }
    tween.TweenCallback(Callable.From(() => _engine.SubViewport.SetPosition(new Vector2(0, 0))));
  }
  public void AddFBAnimation(Color color, float time)
  {
    // GD.Print("时间", time);
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    tween.TweenMethod(Callable.From<Color>(_engine.SetFBColor), _engine.GetFBColor(), color, time);
  }
  public void AddFAnimation(Color color, float time)
  {
    // GD.Print("时间", time);
    Tween tween = CreateTween();
    Animator animator = new(tween, time);
    AddAnimator(animator);
    tween.TweenMethod(Callable.From<Color>(_engine.SetFBColor), color, new Color(0.5f, 0.5f, 0.5f, 1.0f), time);
  }
  // public void AddCalenderAnimation(){
  //   Tween tween = CreateTween();
  //   Animator animator = new(tween, 4.0f);
  // }
}