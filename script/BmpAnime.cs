using System;
using System.Collections.Generic;
using System.Text;
using Godot;
public struct FrameInfo
{
    public int TextureIdx;
    public int DurationFrame;
}
public partial class BmpAnime : Sprite2D
{
    public List<AtlasTexture> Textures = new();
    public List<FrameInfo> FrameInfos = new();
    public int CurFrameIdx;
    public int NextFrame;
    public BmpAnime(string aniPath)
    {
        byte[] bytes = Wa2Resource.LoadFileBuffer(aniPath);
        int textureCount = BitConverter.ToInt32(bytes, 20);
        int frameCount = BitConverter.ToInt32(bytes, 24);
        GD.Print("textureCount:", textureCount, "frameCount:", frameCount);
        int strStartPos = 56 + textureCount * 64 + frameCount * 8;
        byte[] slice = new byte[bytes.Length - strStartPos];
        Buffer.BlockCopy(bytes, strStartPos, slice, 0, bytes.Length - strStartPos);
        Encoding sjis = Encoding.GetEncoding("shift_jis");
        string fileName = sjis.GetString(slice).Replace("\0", "").Replace(".tga",".png");
        for (int i = 0; i < textureCount; i++)
        {
            AtlasTexture atlas = new();

            atlas.Atlas = GD.Load<Texture2D>("res://assets/grp/"+fileName);
            int x = BitConverter.ToInt16(bytes, 36 + i * 64 + 48);
            int y = BitConverter.ToInt16(bytes, 36 + i * 64 + 50);
            int w = BitConverter.ToInt16(bytes, 36 + i * 64 + 52);
            int h = BitConverter.ToInt16(bytes, 36 + i * 64 + 54);
            GD.Print("idx:", i, "x:", x, "y:", y, "w:", w, "h:", h);
            atlas.Region = new Rect2(x, y, w, h);
            Textures.Add(atlas);
        }
        for (int i = 0; i < frameCount; i++)
        {
            FrameInfo info = new();
            info.DurationFrame = BitConverter.ToInt32(bytes, 36 + textureCount * 64 + 12 + i * 8);
            info.TextureIdx = BitConverter.ToInt16(bytes, 36 + textureCount * 64 + 12 + i * 8 + 6);
            FrameInfos.Add(info);
        }

        GD.Print(fileName);
    }
    public override void _PhysicsProcess(double delta)
    {
        NextFrame--;
        if (NextFrame == 0)
        {
            CurFrameIdx++;
            if (CurFrameIdx >= FrameInfos.Count)
            {
                CurFrameIdx = 0;
            }
            SetFrameInfo(CurFrameIdx);
        }
    }
    public override void _Ready()
    {
        if (Textures?.Count > 0 && FrameInfos?.Count > 0)
        {
            Texture = Textures[0];
            SetFrameInfo(0);
        }
    }
    public void SetFrameInfo(int idx)
    {
        FrameInfo frameInfo = FrameInfos[idx];
        CurFrameIdx = idx;
        NextFrame = frameInfo.DurationFrame;
        Texture = Textures[frameInfo.TextureIdx];
    }

}