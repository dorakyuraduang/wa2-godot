using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class AudioStreamFFmpeg : AudioStream
{
    private Variant _backer;

	public bool UseIcy
	{
		get => _backer.AsGodotObject().Get("use_icy").AsBool();
		set => _backer.AsGodotObject().Set("use_icy", value);
	}

    public AudioStreamFFmpeg()
    {
        _backer = ClassDB.Instantiate("AudioStreamFFmpeg");
    }

    public AudioStreamFFmpeg(Variant hndl)
    {
        _backer = hndl;
    }

    public Error Open(string path, int streamIndex = -1) => _backer.AsGodotObject().Call("open", path, streamIndex).As<Error>();

	public Dictionary GetIcyHeaders() => _backer.AsGodotObject().Call("get_icy_headers").As<Dictionary>();

	public string GetStreamTitle() => _backer.AsGodotObject().Call("get_stream_title").AsString();

	public Dictionary GetTags() => _backer.AsGodotObject().Call("get_tags").As<Dictionary>();

    public override AudioStreamPlayback _InstantiatePlayback() => _backer.AsGodotObject().Call("__instantiate_playback").As<AudioStreamPlayback>();

    public override double _GetLength() => _backer.AsGodotObject().Call("get_length").AsDouble();

}
