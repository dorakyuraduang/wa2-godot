using Godot;
using System;
using System.Text;
public partial class Main : Node
{
	[Export]
	public int Mode;
	[Export]
	public TextureRect Texture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		GD.Print(Time.GetTicksMsec());
		Wa2Resource.LoadPak("res://assets/bak.pak");
		Wa2Resource.LoadPak("res://assets/bgm.pak");
		Wa2Resource.LoadPak("res://assets/script.pak");
		Wa2Resource.LoadPak("res://assets/fnt.pak");
		Wa2Resource.LoadPak("res://assets/char.pak");
		Wa2Resource.LoadPak("res://assets/voice.pak");
		GD.Print(Time.GetTicksMsec());
		GetTree().ChangeSceneToFile("res://scene/as/title_menu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
