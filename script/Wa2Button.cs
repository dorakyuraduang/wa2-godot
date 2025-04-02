using Godot;
using System;
[GlobalClass]
public partial class Wa2Button : TextureButton
{
	[Export]
	public AudioStream HoverStream;
	[Export]
	public AudioStream ClickStream;
	private Wa2EngineMain _engine;
	public override void _Ready()
	{
		_engine=Wa2EngineMain.Engine;
		ButtonDown+=OnClick;
		
	}
	public void OnClick(){
		_engine.SoundMgr.PlaySysSe(ClickStream);
	}
	private void OnHover(){
	}
}
