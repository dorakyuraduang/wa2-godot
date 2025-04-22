using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Wa2SoundMgr : Node
{
	const int MAX_SE_CHANNELS = 10;
	const int MAX_VOICE_CHANNELS = 10;
	private Wa2BgmAudio _bgmAudio = new();
	private AudioStreamPlayer _sysSeAudio = new();
	private Wa2Audio[] _voiceAudios;
	private Wa2Audio[] _seAudios;
	private Wa2EngineMain _engine;
	public int BgmId { private set; get; }
	public void Init(Wa2EngineMain e)
	{
		_engine = e;
	}
	public void SetVoiceVolume(int idx,int volume,int frame)
	{
		_voiceAudios[idx].SetVolume(volume/256, frame*_engine.FrameTime);

	}
	public void StopVoice(int idx, float time = 0.0f)
	{
		_voiceAudios[idx].StopStream(time);
	}
	public void StopAll()
	{
		for (int i = 0; i < MAX_SE_CHANNELS; i++)
		{
			_voiceAudios[i].Stream = null;
		}
		StopBgm();
		for (int i = 0; i < MAX_SE_CHANNELS; i++)
		{
			_seAudios[i].Stream = null;
		}
	}
	public float GetVoiceRemainingTime(int idx)
	{
		Wa2Audio audio = _voiceAudios[idx];
		if (audio.Stream == null)
		{
			return 0;
		}
		return (float)audio.Stream.GetLength() - audio.GetPlaybackPosition();
	}
	public void SetSeVolume(int channel, int volume, float time)
	{
		if (channel >= MAX_SE_CHANNELS)
		{
			return;
		}
		_seAudios[channel].SetVolume(volume / 255.0f, time);
	}
	public void PlayVoice(int label, int id, int chr, int volume = 256, bool loop = false, int channel = 0)
	{
		Wa2Audio audio = _voiceAudios[channel];
		if (label == -1)
		{
			label = _engine.GameSav.Label;
		}
		if (_engine.Prefs.CanPlayCharVoice(chr))
		{
			audio.PlayStream(Wa2Resource.GetVoiceStream(label, id, chr), false, 0, 1);
			audio.SetVolume(volume / 256.0f, 0);
			(audio.Stream as AudioStreamOggVorbis).Loop = loop;
		}
	}
	public void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	{
		if (id < 0)
		{
			return;
		}
		BgmId = id;
		Wa2EngineMain.Engine.WirtSysFlag(100 + id, 1);
		_bgmAudio.PlayStream(Wa2Resource.GetBgmStream(id, false), loopFlag, 0, volume / 255.0f);
		_bgmAudio.SetLoopStream(Wa2Resource.GetBgmStream(id, true));
	}
	public void StopBgm(float time = 0.0f)
	{
		_bgmAudio.StopStream(time);
		Wa2EngineMain.Engine.GameSav.BgmInfo.Id = -1;
		BgmId = -1;
	}
	public float GetVoiceTime()
	{
		Wa2Audio audio = _voiceAudios[0];
		if (audio.Stream == null)
		{
			return 0;
		}
		return (float)audio.Stream.GetLength() - audio.GetPlaybackPosition();
	}
	public float GetSeTime(int channel)
	{
		return (float)_seAudios[channel].Stream.GetLength() - _seAudios[channel].GetPlaybackPosition();
	}
	public static Wa2SoundMgr Instance
	{
		private set; get;

	}
	public void OnVoiceFinished(int idx)
	{
		_voiceAudios[idx].Stream = null;
	}
	public override void _Ready()
	{
		Instance = this;
		_bgmAudio.Bus = "BGM";
		_sysSeAudio.Stream = new AudioStreamPolyphonic();
		_bgmAudio.Name = "BgmAudio";
		_sysSeAudio.Name = "SysSeAudio";
		AddChild(_bgmAudio);
		AddChild(_sysSeAudio);
		_seAudios = new Wa2Audio[MAX_SE_CHANNELS];
		_voiceAudios = new Wa2Audio[MAX_VOICE_CHANNELS];
		for (int i = 0; i < MAX_SE_CHANNELS; i++)
		{
			Wa2Audio audio = new();
			audio.Name = "SeAudio" + i;
			_seAudios[i] = audio;
			AddChild(audio);
		}
		for (int i = 0; i < MAX_VOICE_CHANNELS; i++)
		{
			Wa2Audio audio = new();
			audio.Name = "VoiceAudio" + i;
			_voiceAudios[i] = audio;
			int idx = i;
			_voiceAudios[i].Finished += () => OnVoiceFinished(idx);
			AddChild(audio);
		}
		_sysSeAudio.Play();
	}
	public void PlaySysSe(AudioStream stream)
	{
		AudioStreamPlaybackPolyphonic playBack = (AudioStreamPlaybackPolyphonic)_sysSeAudio.GetStreamPlayback();
		if (stream != null)
		{
			playBack.PlayStream(stream);
		}
	}
	public void PlaySe(int channel, int id, bool loopFlag = false, float time = 0.0f, int volume = 255)
	{
		// GD.Print("播放音效2");
		GD.Print("音效id", id);
		_seAudios[channel].PlayStream(Wa2Resource.GetSeStream((uint)id), loopFlag, time, volume / 255.0f);
	}
	public void StopSe(int channel, float time = 0.0f)
	{
		if(channel>=MAX_SE_CHANNELS){
			return;
		}
		_seAudios[channel].StopStream(time);
	}
}
