using Godot;
using System;
using System.Threading;

public class Wa2Animator
{
	public enum AnimType
	{
		FEAD,
		MOVE,
		HIDE,
		SHOW
	}
	public bool Wait = true;
	public Wa2Timer Timer;
	// public Texture2D CacheTexture;
	public AnimType Type;
	public Wa2Image Image;
	// public Wa2Image MaskTexure;
	public Texture2D CurTexture;
	public Vector2 MoveDistance;
	public Vector2 StartOffset;
	public Wa2Animator(Wa2Image image)
	{
		Image = image;
		// BodyTexure = bodyTexure;
		// MaskTexure = maskTexure;
		Wa2EngineMain.Engine.Animators.Add(this);
	}
	// public void InitScale(float x,float y,float time){
	// 	Timer = new Wa2Timer();
	// 	BodyTexure.SetImageScale(new Vector2(x,y));
	// 	Timer.Start(time);
	// }
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
	public void InitShow(float time)
	{
		Type = AnimType.SHOW;
		Image.Hide();
		Timer = new Wa2Timer();
		Timer.Start(time);
	}
	public void InitFade(float time, int ox = 0)
	{
		// BodyTexure.Hide();
		// MaskTexure.Show();
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

		// SetBg2Image(texture);
		// NextOffset = new Vector2(x, y);
		// NextScale = new Vector2(scaleX, scaleY);
		// SetBg2Offset(NextOffset);
		// SetBg2Scale(NextScale);
	}
	public void Update()
	{
		// if (!IsActive()){
		// 	return;
		// }
		if (Type == AnimType.SHOW)
		{
			if (Timer.GetProgress() >= 1f)
			{
			Image.Show();
			}
		}
		if (Type == AnimType.FEAD)
		{
			// GD.Print(Timer.GetProgress());
			Image.SetBlend(Timer.GetProgress());
			if (Timer.GetProgress() >= 1f)
			{
				Image.SetCurTexture(Image.GetNextTexture());
				Image.SetMaskTexture(null);
				Image.SetNextTexture(null);
				Image.SetBlend(0.0f);
				// Image.SetCurOffset(Image.GetNextOffset());
				// Image.SetCurScale(Image.GetNextScale());
				// MaskTexure.Hide();
				// BodyTexure.Show();
				// MaskTexure.SetCurTexture(null);
				// MaskTexure.SetMaskTexture(null);
				// MaskTexure.SetNextTexture(null);
				// MaskTexure.SetOffset(Vector2.Zero);
				// MaskTexure.SetImageScale(Vector2.Zero);

			}
		}
		if (Type == AnimType.MOVE)
		{
			Image.SetCurOffset(Timer.GetProgress() * MoveDistance + StartOffset);
			if (Timer.GetProgress() >= 1f)
			{
				// Image.SetCurOffset(MoveDistance + StartOffset);
				// MaskTexure.Hide();
				// BodyTexure.Show();
				// MaskTexure.SetCurTexture(null);
				// MaskTexure.SetMaskTexture(null);
				// MaskTexure.SetNextTexture(null);
			}
		}
		if (Type == AnimType.HIDE)
		{
			if (Timer.GetProgress() >= 1f)
			{
				// Image.Hide();
				Image.SetCurTexture(null);
				Image.SetMaskTexture(null);
				Image.SetNextTexture(null);
				// if (Image.Name == "MaskImage")
				// {
				// 	Wa2EngineMain.Engine.SubViewport.Show();
				// }
			}
		}

	}
	public void Finish()
	{
		Timer.Done();
		if (Type != AnimType.MOVE)
		{
			Update();
		}

	}
	public bool IsActive()
	{
		return Timer.IsActive();
	}
}
