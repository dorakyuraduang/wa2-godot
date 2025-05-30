using Godot;
using System;
[GlobalClass]
public partial class Wa2Image : ColorRect
{
	// public Wa2Image(){
	// 	Material=new ShaderMaterial();
	// 	// Material.ResourceLocalToScene=true;
	// 	Shader shader=GD.Load<Shader>("res://shader/mask.gdshader").Duplicate(true) as Shader;
	// 	shader.ResourceLocalToScene=true;
	// 	(Material as ShaderMaterial).SetShader(shader);
	// }
	public void SetCenter(bool b)
	{
		(Material as ShaderMaterial).SetShaderParameter("center", b);
	}
	public void SetCurOffset(Vector2 val)
	{
		(Material as ShaderMaterial).SetShaderParameter("cur_offset", val);
	}
	public void SetCurScale(Vector2 val)
	{
		(Material as ShaderMaterial).SetShaderParameter("cur_scale", val);
	}
	public void SetNextOffset(Vector2 val)
	{
		(Material as ShaderMaterial).SetShaderParameter("next_offset", val);
	}
	public void SetNextScale(Vector2 val)
	{
		(Material as ShaderMaterial).SetShaderParameter("next_scale", val);
	}


	public Vector2 GetCurOffset()
	{
		return (Vector2)(Material as ShaderMaterial).GetShaderParameter("cur_offset");
	}
	public Vector2 GetCurScale()
	{
		return (Vector2)(Material as ShaderMaterial).GetShaderParameter("cur_scale");
	}

	public Vector2 GetNextOffset()
	{
		return (Vector2)(Material as ShaderMaterial).GetShaderParameter("next_offset");
	}
	public Vector2 GetNextScale()
	{
		return (Vector2)(Material as ShaderMaterial).GetShaderParameter("next_scale");
	}
	public void SetBlend(float val)
	{
		(Material as ShaderMaterial).SetShaderParameter("blend", val);
	}
	public void SetNextTexture(Texture2D texture)
	{
		(Material as ShaderMaterial).SetShaderParameter("next_texture", texture);

	}
	public void SetCurTexture(Texture2D texture)
	{
		(Material as ShaderMaterial).SetShaderParameter("cur_texture", texture);
		// GD.Print(texture.GetImage().SavePng("V.PNG"));
	}
	public void SetMaskTexture(Texture2D texture)
	{
		(Material as ShaderMaterial).SetShaderParameter("mask_texture", texture);
	}
	public Texture2D GetNextTexture()
	{
		return (Texture2D)(Material as ShaderMaterial).GetShaderParameter("next_texture");
	}
	public Texture2D GetCurTexture()
	{
		return (Texture2D)(Material as ShaderMaterial).GetShaderParameter("cur_texture");
	}
	public Texture2D GetMaskTexture()
	{
		return (Texture2D)(Material as ShaderMaterial).GetShaderParameter("mask_texture");
	}
	public void Reset()
	{
		SetCurOffset(Vector2.Zero);
		SetNextOffset(Vector2.Zero);
		SetCurScale(Vector2.One);
		SetNextScale(Vector2.One);
		SetCurTexture(null);
		SetNextTexture(null);
		SetMaskTexture(null);

	}
}
