using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

public partial class Wa2Viewport : SubViewportContainer
{


	// public AnimType Type { get; private set; }
	[Export]
	public Wa2Image BgTexure;
	[Export]
	public Control CharGroup;
	public SubViewport SubViewport;
	[Export]
	public Wa2Image RenderTexture;
	// public int BgDrawFrame;
	// public Texture BgTexture;
	// public Image BgImage = Image.CreateEmpty(1280, 720, false, Image.Format.Rgba8);
	// public static Image CurImage = Image.CreateEmpty(1280, 720, false, Image.Format.Rgba8);
	// public static Image NextImage = Image.CreateEmpty(1280, 720, false, Image.Format.Rgba8);
	// public ImageTexture NextTexture = ImageTexture.CreateFromImage(NextImage);
	// public ImageTexture CurTexture = ImageTexture.CreateFromImage(CurImage);
	// public Vector2 NextOffset;
	// public Vector2 NextScale;
	// public ImageTexture CacheTexture;

	public int Duration;
	public Wa2EngineMain Engine;
	// public void Finish()
	// {
	// 	Engine.WaitTimer.Done();
	// 	Update();
	// 	Type = AnimType.NONE;
	// }
	public void SetAmpMode(int mode)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("amp_mode", mode);
	}
	// Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// 	Engine = Wa2EngineMain.Engine;
	// 	(RenderTexture.Material as ShaderMaterial).SetShaderParameter("texture1", CurTexture);
	// 	(RenderTexture.Material as ShaderMaterial).SetShaderParameter("texture2", NextTexture);
	// 	// GD.Print((RenderTexture.Material as ShaderMaterial).GetShaderParameter("texture2"));
	// }
	// public void Clear(){
	// 	duration
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public void CharEx(){
	// 	Char();
	// }
	// public void Char()
	// {
	// 	Type |= AnimType.CHAR;
	// 	NextImage.Fill(new Color(0, 0, 0, 0));
	// 	List<Wa2CharImage> chars = new();
	// 	foreach (Wa2Char item in Engine.CharDic.Values)
	// 	{
	// 		if (item.show)
	// 		{
	// 			Wa2CharImage chr = new Wa2CharImage();
	// 			chr.x = Wa2Def.CharPos[item.pos];
	// 			chr.Image = Wa2Resource.GetChrImage(item.id, item.no).GetImage();
	// 			chars.Add(chr);
	// 		}
	// 	}
	// 	chars.Sort((a, b) =>
	// 	{
	// 		return b.x.CompareTo(a.x);
	// 	});

	// 	for (int i = 0; i < chars.Count; i++)
	// 	{
	// 		NextImage.BlendRect(chars[i].Image, new Rect2I(new Vector2I(0, 0), chars[i].Image.GetSize()), new Vector2I(chars[i].x, 0));
	// 		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("time", 0f);
	// 		CurTexture.Update(CurImage);
	// 		NextTexture.Update(NextImage);
	// 	}
	// }
	// public void Fade(ImageTexture texture, float x = 0, float y = 0, float scaleX = 1f, float scaleY = 1f)
	// {
	// 	Type |= AnimType.FEAD;
	// 	CacheTexture = texture;
	// 	SetBg2Image(texture);
	// 	NextOffset = new Vector2(x, y);
	// 	NextScale = new Vector2(scaleX, scaleY);
	// 	SetBg2Offset(NextOffset);
	// 	SetBg2Scale(NextScale);
	// }
	// public void Update()
	// {
	// 	if (Type==AnimType.NONE)
	// 	{
	// 		return;
	// 	}
	// 	if ((Type&AnimType.FEAD)>0)
	// 	{
	// 		SetBg2Alpha(Engine.WaitTimer.GetProgress());
	// 		// GD.Print("透明度:",Engine.WaitTimer.GetProgress());
	// 		if (Engine.WaitTimer.GetProgress() >= 1f)
	// 		{
	// 			SetBg2Alpha(0f);
	// 			SetBg1Image(CacheTexture);
	// 			SetBg1Scale(NextScale);
	// 			SetBg1Offset(NextOffset);
	// 			Clear();
	// 		}
	// 	}
	// 	if ((Type&AnimType.CHAR)>0)
	// 	{
	// 		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("time", Engine.WaitTimer.GetProgress());
	// 		if (Engine.WaitTimer.GetProgress() >= 1f)
	// 		{
	// 			CurImage = (Image)NextImage.Duplicate();
	// 			(RenderTexture.Material as ShaderMaterial).SetShaderParameter("time", 1f);
	// 		}
	// 	}
	// }

	public void SetBg1Image(ImageTexture texture)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg1_texture", texture);
	}
	public void SetBg1Offset(Vector2 offset)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg1_offset", offset);
	}
	public void SetBg1Scale(Vector2 scale)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg1_scale", scale);
	}
	public void SetBg2Image(ImageTexture texture)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg2_texture", texture);
	}
	public void SetBg2Offset(Vector2 offset)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg2_offset", offset);
	}
	public void SetBg2Scale(Vector2 scale)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg2_scale", scale);
	}
	public void SetBg2Alpha(float alpha)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg2_alpha", alpha);
	}
	public void SetBg1Alpha(float alpha)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("bg1_alpha", alpha);
	}
	public void SetAmpStrength(float strength)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("strength", strength);
	}
	public void SetCharsPriority(bool flag)
	{
		(RenderTexture.Material as ShaderMaterial).SetShaderParameter("chars_priority", flag);
	}
	// public void Clear(){
	// 	Engine.CharDic.Clear();
	// 	CurImage.Fill(new Color(0, 0, 0, 0));
	// 	NextImage.Fill(new Color(0, 0, 0, 0));
	// 	CurTexture.Update(CurImage);
	// 	NextTexture.Update(NextImage);
	// }
}