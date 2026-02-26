using Godot;
using System;
using Godot.Collections;

public partial class GoZenVideo : Resource
{
    private Variant _gdHndl;
    public GoZenVideo(Resource hndl)
    {
        _gdHndl = hndl;
    }

    public GoZenVideo()
    {
        _gdHndl = ClassDB.Instantiate("GoZenVideo");
    }

    public static Dictionary GetFileMeta(string file_path) =>
        ClassDB.ClassCallStatic("GoZenVideo", "get_file_meta", Variant.CreateFrom(file_path)).AsGodotDictionary();

    public int Open(string video_path) => _gdHndl.AsGodotObject().Call("open", Variant.CreateFrom(video_path)).AsInt32();
    public void Close() => _gdHndl.AsGodotObject().Call("close");
    public bool IsOpen() => _gdHndl.AsGodotObject().Call("is_open").AsBool();
    
    public int SeekFrame(int frame_nr) => _gdHndl.AsGodotObject().Call("seek_frame", Variant.CreateFrom(frame_nr)).AsInt32();
    public bool NextFrame(bool skip = false) => _gdHndl.AsGodotObject().Call("next_frame", Variant.CreateFrom(skip)).AsBool();
    
    // Stream info
    public int[] GetStreams(int streamType) => _gdHndl.AsGodotObject().Call("get_streams", Variant.CreateFrom(streamType)).AsInt32Array();
    public Dictionary GetStreamMetadata(int streamIndex) => _gdHndl.AsGodotObject().Call("get_stream_metadata", Variant.CreateFrom(streamIndex)).AsGodotDictionary();

    // Chapter info
    public int GetChapterCount() => _gdHndl.AsGodotObject().Call("get_chapter_count").AsInt32();
    public float GetChapterStart(int chapterIndex) => _gdHndl.AsGodotObject().Call("get_chapter_start", Variant.CreateFrom(chapterIndex)).AsSingle();
    public float GetChapterEnd(int chapterIndex) => _gdHndl.AsGodotObject().Call("get_chapter_end", Variant.CreateFrom(chapterIndex)).AsSingle();
    public Dictionary GetChapterMetadata(int chapterIndex) => _gdHndl.AsGodotObject().Call("get_chapter_metadata", Variant.CreateFrom(chapterIndex)).AsGodotDictionary();

    public string GetPath() => _gdHndl.AsGodotObject().Call("get_path").AsString();
    public float GetFramerate() => _gdHndl.AsGodotObject().Call("get_framerate").AsSingle();
    public int GetFrameCount() => _gdHndl.AsGodotObject().Call("get_frame_count").AsInt32();
    public Vector2I GetResolution() => _gdHndl.AsGodotObject().Call("get_resolution").AsVector2I();
    public Vector2I GetActualResolution() => _gdHndl.AsGodotObject().Call("get_actual_resolution").AsVector2I();
    public int GetWidth() => _gdHndl.AsGodotObject().Call("get_width").AsInt32();
    public int GetHeight() => _gdHndl.AsGodotObject().Call("get_height").AsInt32();
    public int GetPadding() => _gdHndl.AsGodotObject().Call("get_padding").AsInt32();
    public int GetRotation() => _gdHndl.AsGodotObject().Call("get_rotation").AsInt32();
    public int GetInterlaced() => _gdHndl.AsGodotObject().Call("get_interlaced").AsInt32();
    public float GetSar() => _gdHndl.AsGodotObject().Call("get_sar").AsSingle();
    
    public void EnableDebug() => _gdHndl.AsGodotObject().Call("enable_debug");
    public void DisableDebug() => _gdHndl.AsGodotObject().Call("disable_debug");
    public bool GetDebugEnabled() => _gdHndl.AsGodotObject().Call("get_debug_enabled").AsBool();
    
    public string GetPixelFormat() => _gdHndl.AsGodotObject().Call("get_pixel_format").AsString();
    public string GetColorProfile() => _gdHndl.AsGodotObject().Call("get_color_profile").AsString();

    public bool GetHasAlpha() => _gdHndl.AsGodotObject().Call("get_has_alpha").AsBool();
    
    public bool IsFullColorRange() => _gdHndl.AsGodotObject().Call("is_full_color_range").AsBool();
    public bool IsUsingSws() => _gdHndl.AsGodotObject().Call("is_using_sws").AsBool();

    public Image GetYData() => _gdHndl.AsGodotObject().Call("get_y_data").As<Image>();
    public Image GetUData() => _gdHndl.AsGodotObject().Call("get_u_data").As<Image>();
    public Image GetVData() => _gdHndl.AsGodotObject().Call("get_v_data").As<Image>();
    public Image GetAData() => _gdHndl.AsGodotObject().Call("get_a_data").As<Image>();
}
