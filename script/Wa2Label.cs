using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
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
	public bool Shadow;
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
	public float RubyFontScale = 0.4f; // Ruby文字的大小比例
	[Export]
	public float RubyVerticalOffset = 0.8f; // Ruby文字垂直偏移系数
	[Export]
	public float RubyWidthRatio = 0.75f; // Ruby文字整体宽度相对于原文本的比例
	
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		QueueRedraw();
	}
	public Vector2 GetEndPosition()
	{
		Vector2 drawPos = new(0, 0);
		for (int i = 0; i < _processedText.Length; i++)
		{
			char ch = _processedText[i];
			if (ch == '\n')
			{
				drawPos = new Vector2(0, drawPos.Y + 1);
			}
			else
			{
				drawPos.X++;
			}
		}
		return drawPos * new Vector2(FontSize, FontSize + LineSpacin) + Position;

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
