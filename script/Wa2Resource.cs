using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public class FileEntry
{
	public uint Crypted { get; set; }
	public string FileName { get; set; }
	public uint Offset { get; set; }
	public uint Size { get; set; }
	public string PkgPath { get; set; }
}

public class Wa2Resource
{
	public static void Clear()
	{
		SoundDic.Clear();
	}
	public static Dictionary<string, AudioStream> SoundDic { get; private set; } = new();

	public static Dictionary<string, FileEntry> PkgDic { get; private set; } = new();
	public static void LoadOggSound(string path)
	{
		byte[] buffer = LoadFileBuffer(path);
		
		if (buffer == null)
		{
			return;
		}
		AudioStream oggStream = AudioStreamOggVorbis.LoadFromBuffer(buffer);
		SoundDic[path] = oggStream;
	}
	public static AudioStream GetOggSound(string path)
	{
		path = path.ToLower();
		if (!SoundDic.ContainsKey(path))
		{
			LoadOggSound(path);
		}
		GD.Print(SoundDic.GetValueOrDefault(path));
		return SoundDic.GetValueOrDefault(path);
	}
	public static AudioStream GetBgmStream(int id, bool loop = false)
	{
		if (GetOggSound(string.Format("BGM_{0:D3}.OGG", id)) != null)
		{
			return GetOggSound(string.Format("BGM_{0:D3}.OGG", id));
		}
		else
		{
			if (!loop)
			{
				return GetOggSound(string.Format("BGM_{0:D3}_A.OGG", id));
			}
			else
			{
				return GetOggSound(string.Format("BGM_{0:D3}_B.OGG", id));
			}
		}
	}
	public static byte[] LoadFileBuffer(string path)
	{
		path = path.ToLower();
		FileEntry entry = PkgDic.GetValueOrDefault(path);

		if (entry == null)
		{
			return null;
		}
		FileAccess file = FileAccess.Open(entry.PkgPath, FileAccess.ModeFlags.Read);

		file.Seek((ulong)entry.Offset);
		byte[] buffer;
		if (entry.Crypted == 0)
		{
			buffer = file.GetBuffer((int)entry.Size);
			file.Close();
			return buffer;
		}
		else
		{
			uint inceil = file.Get32();
			uint outceil = file.Get32();
			byte[] arr = new byte[0x1000];
			for (int i = 0; i < 0xfee; i++)
			{
				arr[i] = 0x20;
			}
			uint insize = 0;
			uint outsize = 0;
			uint arrw = 0xfee;
			buffer = new byte[outceil];
			while (insize < inceil && outsize < outceil)
			{
				byte flag = file.Get8();
				insize++;
				for (int j = 0; j < 8; j++)
				{
					if (insize >= inceil || outsize >= outceil)
					{
						break;
					}
					byte b1 = file.Get8();
					insize++;
					if ((flag & 1) == 0)
					{
						byte b2 = file.Get8();
						insize++;
						uint arrr = b1 | (uint)(b2 & 0xf0) << 4;
						uint counter = (uint)(b2 & 0xf) + 3;
						while (counter > 0)
						{
							b1 = arr[arrr & 0xfff];
							arr[arrw & 0xFFF] = b1;
							buffer[outsize] = b1;
							arrr++;
							arrw++;
							outsize++;
							counter--;
						}
					}
					else
					{
						arr[arrw & 0xfff] = b1;
						buffer[outsize] = b1;
						arrw++;
						outsize++;
					}
					flag >>= 1;
				}
			}
			file.Close();
			return buffer;
		}
	}
	public static void LoadPak(string path)
	{
		path = path.ToLower();
		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		uint magic = file.Get32();
		if (magic == 0x5041434B)
		{
			file.Get64();
			uint nentry = file.Get32();
			
			for (int i = 0; i < nentry; i++)
			{
				file.Seek((ulong)(16 + i * 44));
				uint crypted = file.Get32();
				string fileName = Encoding.GetEncoding("shift_jis").GetString(file.GetBuffer(24)).ToLower().Replace("\0", "");
				file.Get64();
				uint offset = file.Get32();
				uint size = file.Get32();
				FileEntry entry = new()
				{
					PkgPath = path,
					Offset = offset,
					Size = size,
					Crypted = crypted,
					FileName = fileName
				};
				PkgDic[fileName] = entry;
					// GD.Print(fileName);
			}
		}
		else if (magic == 0x0043414c)
		{
			uint nentry = file.Get32();
			
			for (int i = 0; i < nentry; i++)
			{
				file.Seek((ulong)(8 + i * 40));
				byte[] tempBuffer = file.GetBuffer(32);
				for (int j = 0; j < tempBuffer.Length; j++)
				{
					if (tempBuffer[j] != 0)
					{
						tempBuffer[j] =(byte)(~tempBuffer[j]&0xff);
					}
				}
				string fileName = Encoding.GetEncoding("shift_jis").GetString(tempBuffer).ToLower().Replace("\0", "");
				uint size = file.Get32();
				uint offset = file.Get32();
				FileEntry entry = new()
				{
					PkgPath = path,
					Offset = offset,
					Size = size,
					Crypted = 0,
					FileName = fileName
				};
				PkgDic[fileName] = entry;
				// GD.Print(fileName);
			}
		}
		file.Close();
	}

	// 	var nentry=file.get_32()
	// 	for i in nentry:
	// 		file.seek(16+i*44*4)
	// 		var crypted=file.get_32()
	// 		var file_name=file.get_buffer(24*4).get_string_from_utf8()
	// 		file.get_64()
	// 		var offset=file.get_32()
	// 		var size=file.get_32()
	// 		print(file_name)
	// 		print(offset)
	// 		print(size)	
	// 	file.Close();
	// }
}
