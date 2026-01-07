using System;
using System.Reflection;
using Godot;

public partial class Wa2Sprite : Sprite2D
{
    public int Mode;
    public string Path;
    public Wa2Sprite()
    {
        ShaderMaterial material = new();
		Material = material;
		Centered = false;
    }
    public void SetMode(int mode)
    {
        Mode=mode;
        switch (mode)
        {
            case 1:
                break;
            case 3:
                (Material as ShaderMaterial).Shader = ResourceLoader.Load<Shader>("res://shader/bmp_add.gdshader");
                break;
            case 4:
                (Material as ShaderMaterial).Shader = ResourceLoader.Load<Shader>("res://shader/add.gdshader");
                break;
            case 6:
                (Material as ShaderMaterial).Shader = ResourceLoader.Load<Shader>("res://shader/bmp_mask.gdshader");
                break;
        }
    }
}