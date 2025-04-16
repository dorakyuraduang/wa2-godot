using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
// [Tool]
[GlobalClass]
public partial class Wa2Label : Node2D
{
	
	[Export]
	public Color Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[Export]
	public float VisibleRatio;

	// 存储原始文本（可能包含Ruby标记）
	private string _originalText = "";
	
	[Export]
	public string Text 
	{ 
		get => _originalText; 
		set 
		{
			if (_originalText != value)
			{
				_originalText = value;
				ParseRubyText();
			}
		}
	}
	
	// 处理后的纯文本（不含Ruby标记）
	private string _processedText = "";
	
	// 解析带有Ruby标记的文本
	private void ParseRubyText()
	{
		// 清除之前的Ruby注释
		ClearRuby();
		
		if (string.IsNullOrEmpty(_originalText))
		{
			_processedText = "";
			return;
		}
		
		_processedText = "";
		int processedIndex = 0;
		int currentIndex = 0;
		
		while (currentIndex < _originalText.Length)
		{
			// 查找Ruby标记的开始
			int rubyStart = _originalText.IndexOf("<R", currentIndex);
			
			if (rubyStart == -1)
			{
				// 没有更多Ruby标记，添加剩余文本
				_processedText += _originalText.Substring(currentIndex);
				break;
			}
			
			// 添加Ruby标记之前的文本到处理后的文本
			_processedText += _originalText.Substring(currentIndex, rubyStart - currentIndex);
			processedIndex += rubyStart - currentIndex;
			
			// 查找Ruby标记的分隔符和结束位置
			int separatorPos = _originalText.IndexOf("|", rubyStart);
			int rubyEnd = _originalText.IndexOf(">", separatorPos > 0 ? separatorPos : rubyStart);
			
			if (separatorPos != -1 && rubyEnd != -1 && separatorPos < rubyEnd)
			{
				// 提取Ruby文本和主文本
				string mainText = _originalText.Substring(rubyStart + 2, separatorPos - (rubyStart + 2));
				string rubyText = _originalText.Substring(separatorPos + 1, rubyEnd - (separatorPos + 1));
				
				// 添加主文本到处理后的文本
				_processedText += mainText;
				
				// 注册Ruby
				AddRuby(processedIndex, mainText.Length, rubyText);
				
				// 更新索引
				processedIndex += mainText.Length;
				currentIndex = rubyEnd + 1;
			}
			else
			{
				// Ruby标记不完整，将"<R"作为普通文本处理
				_processedText += "<R";
				processedIndex += 2;
				currentIndex = rubyStart + 2;
			}
		}
	}
	
	[Export]
	public Texture2D FontTexture;
	[Export]
	public Texture2D ShadowTexture;
	[Export]
	public bool Shadow=true;
	[Export]
	public int FontSize = 32;
	[Export]
	public int Rect1Size = 40;
	[Export]
	public int Rect2Size = 32;
	[Export]
	public int LineSpacin = 8;
	[Export]
	public string EllipsisChar = "…";
	[Export]
	public int MaxChar = 999;
	[Export]
	public float RubyFontScale = 0.5f; // Ruby文字的大小比例
	[Export]
	public float RubyVerticalOffset = 0.9f; // Ruby文字垂直偏移系数
	[Export]
	public float RubyWidthRatio = 0.75f; // Ruby文字整体宽度相对于原文本的比例

	[Export]
	public bool UseTTF = false; // 是否使用TTF字体
	[Export]
	public FontFile CustomFont; // 在Inspector中设置TTF字体
	[Export]
	public int TTFFontSize = 26; // TTF字体大小
	[Export]
	public int FontWeight = 400; // 字重：400普通，700粗体
	[Export]
	public float CharacterSpacing = 0.22f; // 字符间距
	[Export]
	public int TTFLineSpacin = 14; // TTF行间距
	[Export]
	public Color ShadowColor = new Color(0, 0, 0, 0.9f); // 阴影颜色
	[Export]
	public int ShadowSize = 8; // 阴影大小
	
	// Ruby注释信息类
	public class RubyInfo
	{
		public int StartIndex; // 主文本中的起始索引
		public int Length; // 主文本的长度
		public string RubyText; // Ruby文本内容
	}
	
	// 存储所有Ruby注释
	public List<RubyInfo> RubyList = new List<RubyInfo>();
	
	// 添加Ruby注释的方法
	public void AddRuby(int startIndex, int length, string rubyText)
	{
		RubyList.Add(new RubyInfo
		{
			StartIndex = startIndex,
			Length = length,
			RubyText = rubyText
		});
		// GD.Print("rubyText: " + rubyText); 
	}
	
	// 清除所有Ruby注释
	public void ClearRuby()
	{
		RubyList.Clear();
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// 确保初始文本被正确解析
		ParseRubyText();
		// 在_Ready或其他方法中加载字体
		if (UseTTF && CustomFont == null) {
			CustomFont = GD.Load<FontFile>("res://assets/fonts/fzzyjt.ttf");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		QueueRedraw();
	}
	public Vector2 GetEndPosition()
	{
		Vector2 drawPos = new(0, 0);

		int lastNewLineIndex = _processedText.LastIndexOf('\n');
		if (lastNewLineIndex != -1)
		{
			// 存在换行符，计算最后一行长度和总行数
			drawPos.X = _processedText.Length - lastNewLineIndex - 1;
			drawPos.Y = _processedText.Count(c => c == '\n');
		}
		else
		{
			// 不存在换行符，整个文本长度就是X
			drawPos.X = _processedText.Length;
			// Y保持为0
		}

		if (CustomFont != null) {
			// TODO: Magic Number
			float x = drawPos.X * (TTFFontSize + CharacterSpacing) * 1.2f;
			float y = drawPos.Y * (TTFFontSize + TTFLineSpacin);
			Vector2 pos = new Vector2(x, y) + Position;
			return pos;
		}
		return drawPos * new Vector2(FontSize, FontSize + LineSpacin) + Position;

	}
	
	// 使用TTF字体绘制单个字符，返回字符宽度
	private float DrawCharWithTTF(Vector2 position, char ch, float fontSize, float shadowSize)
	{
		// 字符宽度
		float charWidth = CustomFont.GetCharSize(ch, (int)fontSize).X;

		// 坐标整数化避免渲染模糊
		position.X = Mathf.Round(position.X);
		position.Y = Mathf.Round(position.Y);
		
		// 绘制阴影
		if (Shadow && shadowSize > 0)
		{
			DrawCharOutline(CustomFont, position, ch.ToString(),
				size: (int)shadowSize,
				fontSize: (int)fontSize,
				modulate: ShadowColor);
		}
		
		// 绘制字符
		DrawChar(CustomFont, position, ch.ToString(),
			fontSize: (int)fontSize,
			modulate: Color);
			
		return charWidth;
	}
	
	// 使用TTF字体绘制文本，返回最终绘制位置
	private Vector2 DrawTextWithTTF(Vector2 startPos, string text, float fontSize, 
		float spacing, float shadowSize, Dictionary<int, Vector2> charPositions = null, bool isRuby = false)
	{
		Vector2 drawPos = startPos;
		
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			
			// 记录字符位置（如果需要）
			if (charPositions != null && !isRuby)
			{
				charPositions[i] = drawPos * new Vector2(TTFFontSize, TTFFontSize + TTFLineSpacin);
			}
			
			if (ch == '\n')
			{
				drawPos = new Vector2(0, drawPos.Y + 1);
				continue;
			}

			// 计算字体基线偏移
			float fontAscent = CustomFont.GetAscent((int)fontSize);
			Vector2 fontOffset = new Vector2(0, fontAscent);

			Vector2 adjustedPos = charPositions[i] + fontOffset;
			
			// 绘制字符并获取宽度
			float charWidth = DrawCharWithTTF(adjustedPos, ch, fontSize, shadowSize);
			
			// 更新绘制位置
			drawPos.X += charWidth / fontSize + spacing;
		}
		
		return drawPos;
	}
	
	// 绘制Ruby注音文本
	private void DrawRubyWithTTF(RubyInfo ruby, Dictionary<int, Vector2> charPositions, string visibleText, 
		float fontSize, float shadowSize, Color textColor)
	{
		// 如果Ruby开始位置超出可见文本范围，跳过
		if (ruby.StartIndex >= visibleText.Length) return;
		
		// 确定实际长度（考虑可见文本范围）
		int actualLength = Math.Min(ruby.Length, visibleText.Length - ruby.StartIndex);
		if (actualLength <= 0) return;

		// 计算主文本区域的宽度
		float baseWidth = actualLength * TTFFontSize;
		
		// 获取Ruby文本的起始位置（主文本的起始位置）
		Vector2 basePos = charPositions[ruby.StartIndex];

		// 计算主文本的中心点X坐标
		float baseTextCenterX = basePos.X + baseWidth / 2;

		// 计算Ruby文本所需的总宽度
		float desiredRubyWidth = baseWidth * RubyWidthRatio;

		int rubyFontSizeInt = (int)fontSize;
		int rubyShadowSizeInt = (int)shadowSize;

		// 为Ruby文本添加适当的垂直偏移
		float rubyAscent = CustomFont.GetAscent(rubyFontSizeInt);
		
		// 直接使用中心点计算，确保Ruby文本居中于主文本上方
		Vector2 rubyPos = new Vector2(baseTextCenterX - desiredRubyWidth / 2,
									basePos.Y - rubyFontSizeInt * RubyVerticalOffset + rubyAscent);
		
		// 计算字符间距
		float totalRubyWidth = 0;

		// 计算每个字符的宽度总和
		for (int j = 0; j < ruby.RubyText.Length; j++)
		{
			totalRubyWidth += CustomFont.GetStringSize(ruby.RubyText[j].ToString(), fontSize: rubyFontSizeInt).X;
		}
		
		// 计算需要的字符间距
		float rubySpacing = 0;
		if (ruby.RubyText.Length > 1)
		{
			rubySpacing = (desiredRubyWidth - totalRubyWidth) / (ruby.RubyText.Length - 1);
		}

		// 由于DrawString不支持spacing参数，手动绘制每个字符
		float currentX = rubyPos.X;
		for (int j = 0; j < ruby.RubyText.Length; j++)
		{
			char charStr = ruby.RubyText[j];
			float charWidth = CustomFont.GetStringSize(charStr.ToString(), fontSize: rubyFontSizeInt).X;

			DrawCharWithTTF(new Vector2(currentX, rubyPos.Y), charStr, rubyFontSizeInt, rubyShadowSizeInt);

			
			// 更新下一个字符的位置（添加字符宽度和间距）
			currentX += charWidth + rubySpacing;
		}
	}

	public override void _Draw()
	{
		Vector2 drawPos = new(0, 0);
		if (_processedText == "")
		{
			return;
		}
		string text;
		if (_processedText.Length >=MaxChar)
		{
			text = _processedText[..MaxChar] + EllipsisChar;
		}
		else
		{
			text = _processedText;
		}
		int end = (int)(VisibleRatio * text.Length);
		string str = text.Substr(0, end);
		
		// 存储每个字符的位置，用于后续绘制Ruby
		Dictionary<int, Vector2> charPositions = new Dictionary<int, Vector2>();

		// 如果设置了TTF字体，则使用TTF渲染
		if (CustomFont != null)
		{
			// 使用封装的方法绘制主文本
			Vector2 finalPos = DrawTextWithTTF(drawPos, str, TTFFontSize, CharacterSpacing, ShadowSize, charPositions);
			
			// 更新尾部坐标
			// GD.Print("EndPosition: " + finalPos);
			// _endPosition = finalPos;
			
			// 绘制Ruby文本
			foreach (var ruby in RubyList)
			{
				// 计算Ruby文本的大小
				float rubyFontSize = TTFFontSize * RubyFontScale;
				float rubyShadowSize = ShadowSize * RubyFontScale;

				DrawRubyWithTTF(ruby, charPositions, str, rubyFontSize, rubyShadowSize, Color);
			}
			
			return;
		}
		
		// 渲染主文本
		for (int i = 0; i < str.Length; i++)
		{
			char ch = str[i];
			// byte[] shiftJISBytes = Encoding.GetEncoding("shift_jis").GetBytes(new char[] { ch });
			// if (!Wa2Def.FontMap.ContainsKey(ch))
			// {
			// 	GD.Print($"未知字符 '{ch}' 的 Shift-JIS 编码是: {BitConverter.ToString(shiftJISBytes)}");
			// }

			// 记录当前字符位置
			charPositions[i] = drawPos * new Vector2(FontSize, FontSize + LineSpacin);
			
			if (ch == '\n')
			{
				drawPos = new Vector2(0, drawPos.Y + 1);
			}
			else
			{
				if (!Wa2Def.FontMap.ContainsKey(ch))
				{
					GD.Print($"未知字符 '{ch}'");
					continue;
				}
				int pos = Wa2Def.FontMap[ch];

				if (pos >= 0)
				{
					int x = pos % 80;
					int y = pos / 80;
					Rect2 rect = new(drawPos * new Vector2(FontSize, FontSize + LineSpacin), new Vector2(FontSize, FontSize));
					Rect2 srcRect = new(new Vector2(x, y) * Rect1Size + new Vector2(Rect1Size - Rect2Size, Rect1Size - Rect2Size) / 2, new Vector2(Rect2Size, Rect2Size));
					if (Shadow)
					{
						DrawTextureRectRegion(ShadowTexture, rect, srcRect, new Color(0.15f, 0.15f, 0.15f, 1));
					}
					DrawTextureRectRegion(FontTexture, rect, srcRect, Color);
				}
				drawPos.X++;
			}
		}
		
		// 渲染Ruby文本 - 放在主循环外部
		foreach (var ruby in RubyList)
		{
			// 如果Ruby开始位置超出可见文本范围，跳过
			if (ruby.StartIndex >= str.Length) continue;
			
			// 确定实际长度（考虑可见文本范围）
			int actualLength = Math.Min(ruby.Length, str.Length - ruby.StartIndex);
			if (actualLength <= 0) continue;
			
			// 计算主文本区域的宽度
			float baseWidth = actualLength * FontSize;
			
			// 获取Ruby文本的起始位置（主文本的起始位置）
			Vector2 basePos = charPositions[ruby.StartIndex];
			
			// 渲染Ruby文本（居中于主文本上方）
			float rubyFontSize = FontSize * RubyFontScale;
			
			// 计算Ruby字符之间的间距使整体宽度为原文本宽度的RubyWidthRatio比例
			float desiredRubyWidth = baseWidth * RubyWidthRatio;
			float charSpacing = 0;
			
			if (ruby.RubyText.Length > 1)
			{
				float totalRubyCharWidth = ruby.RubyText.Length * rubyFontSize;
				float availableSpace = desiredRubyWidth - totalRubyCharWidth;
				charSpacing = availableSpace / (ruby.RubyText.Length - 1);
			}
			
			// 计算Ruby文本起始位置（居中）
			float rubyStartX = (baseWidth - desiredRubyWidth) / 2;
			Vector2 rubyPos = basePos + new Vector2(rubyStartX, -rubyFontSize * RubyVerticalOffset);
			
			// 绘制Ruby文本的每个字符
			for (int j = 0; j < ruby.RubyText.Length; j++)
			{
				char chu = ruby.RubyText[j];
				if (!Wa2Def.FontMap.ContainsKey(chu)) continue;
				
				int pos = Wa2Def.FontMap[chu];
				if (pos >= 0)
				{
					int x = pos % 80;
					int y = pos / 80;
					// 应用计算的字符间距
					Vector2 charPos = rubyPos + new Vector2(j * (rubyFontSize + charSpacing), 0);
					Rect2 rect = new(charPos, new Vector2(rubyFontSize, rubyFontSize));
					Rect2 srcRect = new(new Vector2(x, y) * Rect1Size + new Vector2(Rect1Size - Rect2Size, Rect1Size - Rect2Size) / 2, new Vector2(Rect2Size, Rect2Size));
					if (Shadow)
					{
						DrawTextureRectRegion(ShadowTexture, rect, srcRect, new Color(0.15f, 0.15f, 0.15f, 1));
					}
					DrawTextureRectRegion(FontTexture, rect, srcRect, Color);
				}
			}
		}
	}

}
