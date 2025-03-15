using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Wa2SoundMgr : Node
{
	const int MAX_SE_CHANNELS=255;
	private Wa2BgmAudio _bgmAudio=new ();
	private AudioStreamPlayer _sysSeAudio=new();
	private Wa2Audio _voiceAudio=new();
	private Wa2Audio[] _seAudios;
	public void SetSeVolume(int channel,int volume,float time){
		if(channel>=MAX_SE_CHANNELS){
			return ;
		}
		_seAudios[channel].SetVolume(volume/255.0f,time);
	}
	public void PlayViceo(int label,int id,int chr){
		_voiceAudio.PlayStream(Wa2Resource.GetVoiceStream(label,id,chr), true, 0, 1);
	}
	public void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	{
		_bgmAudio.PlayStream(Wa2Resource.GetBgmStream(id, false), loopFlag, 0, volume / 255.0f);
		_bgmAudio.SetLoopStream(Wa2Resource.GetBgmStream(id, true));
	}
	public void StopBgm(float time=0.0f)
	{
		_bgmAudio.StopStream(time);
	}
	public float GetViceoTime(){
		if (_voiceAudio.Stream==null){
			return 0;
		}
		return (float)_voiceAudio.Stream.GetLength()-_voiceAudio.GetPlaybackPosition();
	}
	public float GetSeTime(int channel){
		return (float)_seAudios[channel].Stream.GetLength()-_seAudios[channel].GetPlaybackPosition();
	}
	public static Wa2SoundMgr Instance
	{
		private set; get;

	}
	public override void _Ready()
	{
		Instance = this;
		_sysSeAudio.Stream=new AudioStreamPolyphonic();
		_bgmAudio.Name="BgmAudio";
		_sysSeAudio.Name="SysSeAudio";
		AddChild(_bgmAudio);
		AddChild(_sysSeAudio);
		AddChild(_voiceAudio);
		_seAudios=new Wa2Audio[MAX_SE_CHANNELS];
		for (int i=0;i<MAX_SE_CHANNELS;i++){
			Wa2Audio audio=new();
			audio.Name="SeAudio"+i;
			_seAudios[i]=audio;
			AddChild(audio);
		}
		_sysSeAudio.Play();
	}
	public void PlaySysSe(AudioStream stream)
	{
		AudioStreamPlaybackPolyphonic playBack=(AudioStreamPlaybackPolyphonic)_sysSeAudio.GetStreamPlayback();
		if (stream != null)
		{
			playBack.PlayStream(stream);
		}
	}
	public void PlaySe(int channel, int id, bool loopFlag = false, float time=0.0f, int volume = 255){
		// GD.Print("播放音效2");
		_seAudios[channel].PlayStream(Wa2Resource.GetSeStream((uint)id), loopFlag, time, volume / 255.0f);
	}
	public void StopSe(int channel, float time=0.0f){
		_seAudios[channel].StopStream(time);
	}
}
