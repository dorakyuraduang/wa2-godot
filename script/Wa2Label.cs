using Godot;
using System;
using System.Collections;
using System.Text;
using System.Text.Unicode;

public partial class Wa2Label : Node2D
{
	[Export]
	public float VisibleRatio;
	[Export]
	public string Text = "";
	[Export]
	public Texture2D FontTexture;
	[Export]
	public Texture2D ShadowTexture;
	[Export]
	public bool Shadow;
	[Export]
	public int FontSize = 32;
	[Export]
	public int Rect1Size = 40;
	[Export]
	public int Rect2Size = 32;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Update()
	{
		QueueRedraw();
	}
	public override void _Draw()
	{
		Vector2 drawPos = new(0, 0);
		if (Text == "")
		{
			return;
		}
		
		int end = (int)(VisibleRatio * Text.Length);
		string str = Text.Substr(0, end);
		for (int i = 0; i < str.Length; i++)
		{
			char ch = str[i];
			// byte[] shiftJISBytes = Encoding.GetEncoding("shift_jis").GetBytes(new char[] { ch });
			// if (!Wa2Def.FontMap.ContainsKey(ch))
			// {
			// 	GD.Print($"未知字符 '{ch}' 的 Shift-JIS 编码是: {BitConverter.ToString(shiftJISBytes)}");
			// }

			
			if (ch == '\n')
			{
				drawPos = new Vector2(0, drawPos.Y + 1);
			}
			else
			{
				int pos = Wa2Def.FontMap[ch];
				if (pos >= 0)
				{
					int x = pos % 80;
					int y = pos / 80;
					Rect2 rect = new(drawPos * new Vector2(FontSize, FontSize), new Vector2(FontSize, FontSize));
					Rect2 srcRect = new(new Vector2(x, y) * Rect1Size + new Vector2(Rect1Size - Rect2Size, Rect1Size - Rect2Size) / 2, new Vector2(Rect2Size, Rect2Size));
					if (Shadow)
					{
						DrawTextureRectRegion(ShadowTexture, rect, srcRect, new Color(0.15f, 0.15f, 0.15f, 1));
					}
					DrawTextureRectRegion(FontTexture, rect, srcRect);
				}
				drawPos.X++;
			}
		}
	}

}
