using Godot;
using System;
using System.Threading;
public class Wa2Animator
{
	public bool Wait = true;
	public Wa2Timer Timer;
	public bool IsActive()
	{
		return !Timer.IsDone();
	}
	public virtual void Update()
	{
		if (Timer == null || !Timer.IsStart())
		{
			return;
		}
	}
	public virtual void Finish()
	{
		Timer.Done();
		Update();
	}
}
public class Wa2AdvAnimator : Wa2Animator
{
	public Wa2AdvMain Adv;
	public AnimType Type;
	public enum AnimType
	{
		FEAD_IN,
		FEAD_OUT
	}
	public Wa2AdvAnimator(Wa2AdvMain adv)
	{
		Adv = adv;
		Wa2EngineMain.Engine.Animators.Add(this);

	}
	public void InitFade(float time, bool fadeIn)
	{
		Timer = new Wa2Timer();
		Timer.Start(time);
		if (fadeIn)
		{
			Type = AnimType.FEAD_IN;
		}
		else
		{
			Type = AnimType.FEAD_OUT;
		}
	}
	public override void Update()
	{
		base.Update();

		if (Type == AnimType.FEAD_IN)
		{
			Adv.Modulate = new Color(1, 1, 1, MathF.Min(Timer.GetProgress(), 1f));
			Adv.Visible = true;

		}
		if (Type == AnimType.FEAD_OUT)
		{
			Adv.Modulate = new Color(1, 1, 1, MathF.Max(0, 1.0f - Timer.GetProgress()));
			if (Timer.IsDone())
			{
				Adv.Visible = false;
			}
		}
	}
}
public class Wa2ImageAnimator : Wa2Animator
{
	public enum AnimType
	{
		FEAD,
		MOVE,
		HIDE,
		SHOW
	}
	public AnimType Type;
	public Wa2Image Image;
	public Texture2D CurTexture;
	public Vector2 MoveDistance;
	public Vector2 StartOffset;
	public Wa2ImageAnimator(Wa2Image image)
	{
		Image = image;
		Wa2EngineMain.Engine.Animators.Add(this);
	}
	public void InitMove(float time, int x = 0, int y = 0)
	{
		Type = AnimType.MOVE;
		MoveDistance = new Vector2(x, y) - Image.GetCurOffset();
		StartOffset = Image.GetCurOffset();
		Timer = new Wa2Timer();
		Timer.Start(time);
	}
	public void InitHide(float time)
	{
		Type = AnimType.HIDE;
		Timer = new Wa2Timer();
		Timer.Start(time);
	}
	public void InitFade(float time, int ox = 0)
	{
		Image.Show();
		Timer = new Wa2Timer();
		Timer.Start(time);
		Type = AnimType.FEAD;
		Image.SetBlend(0f);
		if (Image.GetMaskTexture() == null)
		{
			(Image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 0.5);
		}
		else
		{
			(Image.Material as ShaderMaterial).SetShaderParameter("fead_weight", 1.0);
		}

	}
	public override void Update()
	{
		base.Update();
		// GD.Print("类型",Timer);
		if (Type == AnimType.FEAD)
		{
			Image.SetBlend(Timer.GetProgress());
			if (Timer.GetProgress() >= 1f)
			{
				Image.SetCurTexture(Image.GetNextTexture());
				Image.SetMaskTexture(null);
				Image.SetNextTexture(null);
				Image.SetBlend(0.0f);
			}
		}
		if (Type == AnimType.MOVE)
		{
			Image.SetCurOffset(Timer.GetProgress() * MoveDistance + StartOffset);
			if (Timer.GetProgress() >= 1f && Wait)
			{
				Image.SetCurOffset(MoveDistance + StartOffset);
			}
		}
		if (Type == AnimType.HIDE)
		{
			if (Timer.GetProgress() >= 1f)
			{
				Image.SetCurTexture(null);
				Image.SetMaskTexture(null);
				Image.SetNextTexture(null);
			}
		}

	}
	public override void Finish()
	{
		Timer.Done();
		if (Wait)
		{
			Update();
		}
	}
}
