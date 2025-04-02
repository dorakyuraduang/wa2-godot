using Godot;
public partial class Wa2Font : FontFile
{
	private string _chars;
	[Export]
	Texture2D FontTexture;
	[Export]
	public Vector2I FontSize = new();
	[Export]
	public string Chars
	{
		get { return _chars; }
		set
		{
			_chars = value;
			BuildFont();
		}
	}
	[Export]
	public Vector2I MapSize = new();
	public void BuildFont()
	{
		ClearTextures(0, FontSize);
		ClearGlyphs(0, FontSize);
		ClearKerningMap(0, FontSize.X);
		ClearSizeCache(0);
		FixedSizeScaleMode = TextServer.FixedSizeScaleMode.Disable;
		Oversampling = 1.0f;
		for (int i = 0; i < _chars.Length; i++)
		{
			Vector2I pos = new Vector2I(i % MapSize.X, i / MapSize.X) * FontSize;
			SetGlyphTextureIdx(0, FontSize, _chars[i], i);
			SetGlyphUVRect(0, FontSize, _chars[i], new Rect2(pos, FontSize));
			SetGlyphOffset(0, FontSize, _chars[i], Vector2.Zero);
			SetGlyphAdvance(0, FontSize.X, _chars[i], Vector2.Zero);
			SetGlyphSize(0, FontSize, _chars[i], FontSize);
		}
	}
}