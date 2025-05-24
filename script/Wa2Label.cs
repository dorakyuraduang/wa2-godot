using FFmpeg.AutoGen;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public class TextParseResult
{
	public Vector2 EndPosition = Vector2.Zero;
	public bool ParseEnd = false;
	public bool WaitKey = false;
}
public class TextTag
{
	public int Type;
	public int Value;
}
public class CharRenderData
{
	public int X;
	public int Y;
	public int Size;
	public float Alpha = 1.0f;
	public char Chr;
	public CharRenderData(char ch, int drawX, int drawY, int size, int v)
	{
		X = drawX;
		Y = drawY;
		Chr = ch;
		Size = size;
		if (v >= 0 && v < 16)
		{
			Alpha = Mathf.Pow(2, v - 16);
		}
		else if (v < 0)
		{
			Alpha = 0.0f;
		}
	}
}
[GlobalClass]
public partial class Wa2Label : Node2D
{
	[Export]
	public Color Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[Export]
	public string Text;
	[Export]
	public Texture2D FontTexture;
	[Export]
	public Texture2D ShadowTexture;
	[Export]
	public bool Shadow = true;
	[Export]
	public int FontSize = 28;
	[Export]
	public int Rect1Size = 40;
	// [Export]
	// public int Rect2Size = 31;
	[Export]
	public int ParagraphSpacing = 13;
	[Export]
	public int LineSpacing = 0;
	[Export]
	public int MaxLines = 4;
	[Export]
	public int MaxChars = 28;
	// [Export]
	// public string EllipsisChar = "…";
	[Export]
	public Color ShadowColor = new Color(0, 0, 0, 0.9f); // 阴影颜色
																											 // [Export]
																											 // public int ShadowSize = 8; // 阴影大小

	public int Segment = 0;
	public int ProgressStep = 0;
	private List<CharRenderData> _renderDatas = new();
	public override void _Draw()
	{
		foreach (CharRenderData r in _renderDatas)
		{
			DrawChar(r);
		}
	}
	// public override void _PhysicsProcess(double delta)
	// {

	// }
	public void Clear()
	{
		_renderDatas.Clear();
	}
	public TextParseResult Update(int progress)
	{
		//遍历所有字符
		_renderDatas.Clear();
		int Speed = 10;
		int curprogress = 0;
		int waitprogress = 0;
		int curSegment = 0;
		int modFontSize = FontSize;
		Stack<TextTag> tagList = new();
		int drawX = 0;
		int drawY = 0;
		int lastDrawX = 0;
		TextParseResult r = new();
		bool wrapHold = false;
		// int drawX = 0;
		// int drawY = 0;
		for (int i = 0; i < Text.Length; i++)
		{
			//当前进度超过目标进度时
			if (r.WaitKey)
			{
				break;
			}
			if (curprogress >= progress && progress != -1)
			{
				if (curSegment >= Segment)
				{
					// r.ParseEnd = true;

					break;
				}
			}
			if (waitprogress > 0)
			{
				curprogress += waitprogress;
				waitprogress = 0;
			}
			switch (Text[i])
			{
				case (char)0:
					break;
				case (char)0xa:
					break;
				case '<':
					if (tagList.Count < 15)
					{
						TextTag tag = new();
						switch (Text[++i])
						{
							case 'A':
							case 'a':
								tag.Type = 6;
								tagList.Push(tag);
								break;
							case 'B':
							case 'b':
								tag.Type = 2;
								tagList.Push(tag);
								break;
							case 'C':
							case 'c':
								tag.Type = 1;
								tagList.Push(tag);
								break;
							case 'D':
							case 'd':
								tag.Type = 0;
								tagList.Push(tag);
								break;
							case 'E':
							case 'G':
							case 'H':
							case 'I':
							case 'J':
							case 'L':
							case 'M':
							case 'N':
							case 'O':
							case 'P':
							case 'Q':
							case 'T':
							case 'U':
							case 'V':
							case 'X':
							case 'Y':
							case 'Z':
							case '[':
							case '\\':
							case ']':
							case '^':
							case '_':
							case '`':
							case 'e':
							case 'g':
							case 'h':
							case 'i':
							case 'j':
							case 'l':
							case 'm':
							case 'n':
							case 'o':
							case 'p':
							case 'q':
							case 't':
							case 'u':
							case 'v':
								break;
							case 'F':
							case 'f':
								tag.Type = 4;
								tag.Value = FontSize;
								i++;
								modFontSize = ParseDecimalDigits(Text, ref i);
								tagList.Push(tag);
								break;
							case 'K':
							case 'k':
								tag.Type = 3;
								tagList.Push(tag);
								break;
							case 'R':
							case 'r':
								//记录当前绘制的位置方便下次遇到|符号时绘制注音字符
								tag.Type = 5;
								tag.Value = drawX;
								tagList.Push(tag);
								break;
							case 'S':
							case 's':
								tag.Type = 7;
								tag.Value = Speed;
								i++;
								switch (ParseDecimalDigits(Text, ref i))
								{
									case 0:
										Speed = 100;
										break;
									case 1:
										Speed = 80;
										break;
									case 2:
										Speed = 60;
										break;
									case 3:
										Speed = 40;
										break;
									case 4:
										Speed = 20;
										break;
									case 5:
										Speed = 10;
										break;
									case 6:
										Speed = 6;
										break;
									case 7:
										Speed = 4;
										break;
									case 8:
										Speed = 2;
										break;
									case 9:
										Speed = 1;
										break;
									case 10:
										Speed = 0;
										break;
								}
								tagList.Push(tag);
								break;
							case 'W':
							case 'w':
								tag.Type = 8;
								i++;
								waitprogress = ParseDecimalDigits(Text, ref i);
								tagList.Push(tag);
								break;
						}
					}
					break;
				case '>':
					if (tagList.Count > 0)
					{
						TextTag tag = tagList.Pop();
						switch (tag.Type)
						{
							case 1:
							case 2:
								break;
							case 3:
							case 5:
								break;
							case 4:
								modFontSize = tag.Value;
								break;
							case 6:
								break;
							case 7:
								Speed = tag.Value;
								break;
						}
					}
					break;
				case '\\':
					switch (Text[++i])
					{
						case '<':
						case '>':
						case '\\':
						case '^':
						case '|':
						case '~':
							break;
						case '=':
						case '?':
						case '@':
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
						case 'G':
						case 'H':
						case 'I':
						case 'J':
						case 'K':
						case 'L':
						case 'M':
						case 'N':
						case 'O':
						case 'P':
						case 'Q':
						case 'R':
						case 'S':
						case 'T':
						case 'U':
						case 'V':
						case 'W':
						case 'X':
						case 'Y':
						case 'Z':
						case '[':
						case ']':
						case '_':
						case '`':
						case 'a':
						case 'b':
						case 'c':
						case 'd':
						case 'e':
						case 'f':
						case 'g':
						case 'h':
						case 'i':
						case 'j':
						case 'l':
						case 'm':
						case 'o':
						case 'p':
						case 'q':
						case 'r':
						case 's':
						case 't':
						case 'u':
						case 'v':
						case 'w':
						case 'x':
						case 'y':
						case 'z':
						case '{':
						case '}':
							break;
						case 'k':
							if (curSegment >= Segment && Segment != -1)
							{
								// if (progress != -1)
								// {
									r.WaitKey = true;
								// }

								// break;
								// r.ParseEnd = true;
							}
							curSegment++;
							break;
						case 'n':
							lastDrawX = drawX;
							drawY += FontSize + ParagraphSpacing;
							drawX = 0;
							break;
					}
					break;
				case '^':
					break;
				case '`':
					break;
				case '|':
					//标签类型为5时绘制注音字符
					if (tagList.Count > 0 && tagList.Peek().Type == 5)
					{
						int x = tagList.Peek().Value;
						int y = drawY - 13;
						i++;
						while (Text[i] != '>')
						{

							if (curSegment >= Segment)
							{
								_renderDatas.Add(new CharRenderData(Text[i], x + (FontSize - FontSize / 2) / 2, y, FontSize / 2, progress - curprogress));

							}
							else
							{
								_renderDatas.Add(new CharRenderData(Text[i], x + (FontSize - FontSize / 2) / 2, y, FontSize / 2, 16));
							}
							x += FontSize + LineSpacing;
							i++;
						}
					}
					break;
				case '~':
					break;
				default:
					if (curSegment >= Segment)
					{
						curprogress += Speed / 10;
						int v = 16;
						if (progress >= 0)
						{
							v = progress - curprogress;
						}
						_renderDatas.Add(new CharRenderData(Text[i], drawX, drawY, modFontSize, v));
					}
					else
					{
						_renderDatas.Add(new CharRenderData(Text[i], drawX, drawY, modFontSize, 16));
					}
					lastDrawX = drawX;
					if (drawX >= (MaxChars * (FontSize + LineSpacing)))
					{
						drawY += FontSize + ParagraphSpacing;
						drawX = 0;
					}
					else
					{
						drawX += modFontSize + LineSpacing;
					}


					break;
			}
		}
		if (r.WaitKey)
		{
			if (drawX != 0 || drawY == 0 || drawX > (MaxChars + 1) * (FontSize + LineSpacing))
			{
				r.EndPosition = new Vector2(drawX, drawY);

			}
			else
			{
				r.EndPosition = new Vector2(lastDrawX, drawY - FontSize - ParagraphSpacing);

			}
		}
		else
		{
			r.EndPosition = new Vector2(drawX, drawY);

			// GD.Print(r.EndPosition);
		}
		r.ParseEnd = curprogress <= (progress - 16);
		QueueRedraw();
		return r;
	}
	public int ParseDecimalDigits(string input, ref int index)
	{
		int result = 0;

		while (index < input.Length && char.IsDigit(input[index]))
		{
			result = result * 10 + (input[index] - '0');
			index++;
		}
		if (index >= input.Length || input[index] != ':')
			index = Math.Max(0, index - 1);

		return result;
	}
	public void DrawChar(CharRenderData r)
	{

		if (!Wa2Def.FontMap.ContainsKey(r.Chr))
		{
			GD.Print($"未知字符 '{r.Chr}'");
			return;
		}
		int pos = Wa2Def.FontMap[r.Chr];
		if (pos >= 0)
		{
			int x = pos % 80;
			int y = pos / 80;
			Rect2 rect = new(new Vector2(r.X, r.Y), new Vector2(r.Size, r.Size));
			Rect2 rect2 = new(new Vector2(r.X - r.Size / 28f * 2, r.Y - r.Size / 28f * 2), new Vector2(r.Size / 28f * 32, r.Size / 28f * 32));
			Rect2 srcRect = new(new Vector2(x, y) * Rect1Size + new Vector2(4, 4), new Vector2(28, 28));
			Rect2 shadowRect = new(new Vector2(x, y) * Rect1Size + new Vector2(2, 2), new Vector2(32, 32));
			if (Shadow)
			{
				DrawTextureRectRegion(ShadowTexture, rect2, shadowRect, new Color(0.15f, 0.15f, 0.15f, r.Alpha));
				DrawTextureRectRegion(FontTexture, rect, srcRect, new Color(Color.R, Color.G, Color.B, r.Alpha));
			}
			else
			{
				DrawTextureRectRegion(FontTexture, rect, srcRect, new Color(Color.R, Color.G, Color.B, r.Alpha));
			}



		}
	}
	public TextParseResult SetText(string text, int progress = -1)
	{
		Text = text;
		return Update(progress);
	}
}