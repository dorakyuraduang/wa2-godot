using Godot;
using System;

public partial class GoZenAudio : Resource
{
	public static byte[] GetAudioData(string path, int streamIndex = -1, bool stereo = true) =>
        ClassDB.ClassCallStatic("GoZenAudio", "get_audio_data", Variant.CreateFrom(path), Variant.CreateFrom(streamIndex), Variant.CreateFrom(stereo)).AsByteArray();
    public static byte[] CombineData(byte[] audio1, byte[] audio2) => ClassDB.ClassCallStatic("GoZenAudio",
        "combine_data", Variant.CreateFrom(audio1), Variant.CreateFrom(audio2)).AsByteArray();
    
    public static byte[] ChangeDb(byte[] audio, float db) => ClassDB.ClassCallStatic("GoZenAudio", "change_db", Variant.CreateFrom(audio), Variant.CreateFrom(db)).AsByteArray();
    
    public static byte[] ChangeToMono(byte[] audio) => ClassDB.ClassCallStatic("GoZenAudio", "change_to_mono", Variant.CreateFrom(audio)).AsByteArray();
}
