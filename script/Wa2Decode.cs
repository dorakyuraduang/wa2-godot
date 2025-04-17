using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Decode CN Strings
public partial class Wa2Decode : Resource
{
	// 静态缓存字典，只需加载一次
	private static Dictionary<char, char> _mappingCache = null;
	// 简单的结果缓存，避免重复计算
	private static readonly Dictionary<string, string> _resultCache = new(256);
	// 缓存大小限制，避免内存泄漏
	private const int MAX_CACHE_SIZE = 1000;

	/// <summary>
	/// 根据assets/map.json中的映射关系替换字符串
	/// </summary>
	/// <param name="input">需要替换的字符串</param>
	/// <returns>替换后的字符串</returns>
	public static string ReplaceWithJsonMap(string input)
	{
		// 如果输入为空，直接返回
		if (string.IsNullOrEmpty(input))
			return input;

		// 首先检查结果缓存
		if (_resultCache.TryGetValue(input, out string cachedResult))
		{
			return cachedResult;
		}

		// 处理逗号
		if (input.Contains(','))
		{
			return SplitAndProcess(input, ',');
		}

		// 处理换行符
		if (input.Contains('\n'))
		{
			return SplitAndProcess(input, '\n');
		}

		// 确保映射字典已加载
		if (_mappingCache == null)
		{
			LoadMappingDictionary();
			if (_mappingCache == null) // 加载失败
			{
				return input;
			}
		}

		// 单字符映射可以更高效地处理
		var sb = new StringBuilder(input.Length);
		foreach (char c in input)
		{
			if (_mappingCache.TryGetValue(c, out char replacement))
			{
				sb.Append(replacement);
			}
			else
			{
				sb.Append(c);
			}
		}
		string result = sb.ToString();

		// 缓存结果（限制缓存大小）
		if (_resultCache.Count >= MAX_CACHE_SIZE)
		{
			// 简单策略：清空一半缓存项
			var keysToRemove = _resultCache.Keys.Take(_resultCache.Count / 2).ToList();
			foreach (var key in keysToRemove)
			{
				_resultCache.Remove(key);
			}
		}
		_resultCache[input] = result;

		// GD.Print($"当前缓存大小: {_resultCache.Count}");
		// GD.Print($"替换结果: {result}");
		return result;
	}

	/// <summary>
	/// 按指定分隔符拆分字符串并处理每个部分
	/// </summary>
	/// <param name="input">输入字符串</param>
	/// <param name="separator">分隔符</param>
	/// <returns>处理后的字符串</returns>
	private static string SplitAndProcess(string input, char separator)
	{
		// 在分隔符处分割文本
		string[] parts = input.Split(separator);
		var resultParts = new StringBuilder(input.Length);

		// 处理每一部分
		for (int i = 0; i < parts.Length; i++)
		{
			// 递归处理每一部分
			resultParts.Append(ReplaceWithJsonMap(parts[i]));

			// 除了最后一部分，每部分后面添加分隔符
			if (i < parts.Length - 1)
				resultParts.Append(separator);
		}

		return resultParts.ToString();
	}

	/// <summary>
	/// 加载映射字典（懒加载模式）
	/// </summary>
	private static void LoadMappingDictionary()
	{
		string filePath = "res://map.json";
		FileAccess file = null;

		// 检查文件是否存在
		if (!FileAccess.FileExists(filePath))
		{
			GD.PrintErr($"映射文件不存在: {filePath}");
			return;
		}

		string jsonContent;
		try
		{
			// 打开并读取文件内容
			file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			jsonContent = file.GetAsText();
			file.Close();
		}
		catch (System.Exception e)
		{
			GD.PrintErr($"读取JSON文件出错: {e.Message}");
			if (file != null && file.IsOpen())
				file.Close();
			return;
		}

		// 解析JSON内容
		Json json = new();
		var error = json.Parse(jsonContent);
		if (error != Error.Ok)
		{
			GD.PrintErr($"解析JSON出错: {json.GetErrorMessage()}");
			return;
		}

		// 获取JSON数据并转换为字符映射字典
		var jsonData = json.Data.AsGodotDictionary();
		_mappingCache = new Dictionary<char, char>(jsonData.Count);

		foreach (var key in jsonData.Keys)
		{
			// 确保键和值都是单字符
			string keyStr = key.ToString();
			string valueStr = jsonData[key].ToString();

			if (keyStr.Length == 1 && valueStr.Length == 1)
			{
				_mappingCache[keyStr[0]] = valueStr[0];
			}
			else
			{
				GD.PushWarning($"忽略非单字符映射: {keyStr} -> {valueStr}");
			}
		}
	}

	/// <summary>
	/// 主动预加载映射字典，可在游戏启动时调用
	/// </summary>
	public static void PreloadMappingDictionary()
	{
		if (_mappingCache == null)
		{
			LoadMappingDictionary();
		}
	}

	/// <summary>
	/// 清除缓存（在内存紧张时可调用）
	/// </summary>
	public static void ClearCache()
	{
		_resultCache.Clear();
	}

}
