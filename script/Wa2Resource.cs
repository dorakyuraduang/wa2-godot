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
	public static string ResPath;
	// public static void Clear()
	// {
	// 	SoundDic.Clear();
	// 	ImageDic.Clear();

	// }
	// public static Dictionary<string, AudioStream> SoundDic { get; private set; } = new();
	// // public static Dictionary<string, FileAccess> PakDir { get; private set; } = new();
	public static Dictionary<string, FileEntry> FileDic { get; private set; } = new();
	// public static Dictionary<string, ImageTexture> ImageDic { get; private set; } = new();
	public static AudioStream LoadOggSound(string path)
	{
		byte[] buffer = LoadFileBuffer(path);

		if (buffer == null)
		{
			return null;
		}
		AudioStream oggStream = AudioStreamOggVorbis.LoadFromBuffer(buffer);
		return oggStream;
		// SoundDic[path] = oggStream;
	}
	// public static VideoStream GetMovie(string name){
	// 	VideoStream video=new();
	// 	video.File=ResPath+"movie/"+name+"0.mp4";
	// 	return video;
	// }
	public class BgImage
	{
		public Texture2D texture;
		public string Effect;
		public string Mask;
	}
	public static AudioStream LoadWavSound(string path)
	{
		// GD.Print(path);
		byte[] buffer = LoadFileBuffer(path);
		if (buffer == null)
		{
			return null;
		}
		AudioStream wavStream = AudioStreamWav.LoadFromBuffer(buffer);
		return wavStream;

	}
	public static Texture2D GetTvImage(int id)
	{
		return GetTgaImage(string.Format("tv{0:D6}.tga", id));
	}
	public static Texture2D GetMaskImage(int id)
	{
		return GetBmpImage(string.Format("f0{0:D3}.bmp", id));
	}
	public static AudioStream GetVoiceStream(int label, int id, int chr)
	{
		// GD.Print(string.Format("{0:D4}_{1:D4}_{2:D2}.ogg", label, id, chr));
		return GetOggStream(string.Format("{0:D4}_{1:D4}_{2:D2}.ogg", label, id, chr));
	}
	public static AudioStream GetSeStream(int id)
	{
		if (FileDic.ContainsKey(string.Format("se_{0:D4}.wav", id)))
		{
			return GetWavStream(string.Format("se_{0:D4}.wav", id));
		}
		else
		{
			return GetOggStream(string.Format("se_{0:D4}.ogg", id));
		}
	}
	public static AudioStream GetOggStream(string path)
	{
		path = path.ToLower();
		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadOggSound(path);
	}
	public static ImageTexture GetCgImage(int id, int no)
	{
		string path = string.Format("v{0:D5}{1:D1}.tga", id, no);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetCgImage(int id)
	{
		string path = string.Format("v{0:D6}.tga", id);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetBgImage(int id, int type, int no)
	{
		string path = string.Format("B{0:D4}{1:D1}{2:D1}.tga", id, no, type);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetChrImage(int id, int type)
	{
		string path = string.Format("{0:S}{1:D6}.tga", Wa2Def.CharDict[id], type);
		// GD.Print(path);
		return GetTgaImage(path);

	}
	public static ImageTexture GetTgaImage(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadTgaImage(path);
	}
	public static ImageTexture GetBmpImage(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadBmpImage(path);
	}
	public static ImageTexture LoadTgaImage(string path)
	{
		// ulong start = Time.GetTicksMsec();
		path = path.ToLower();
		byte[] buffer = LoadFileBuffer(path);
		// GD.Print(path);
		if (buffer == null)
		{
			return null;
		}
		Image image = new();
		image.LoadTgaFromBuffer(buffer);
		if (Wa2EngineMain.Engine.EffectMode != "")
		{
			SetImageEffect(image, buffer[17]);
		}
		ImageTexture tgaImage = ImageTexture.CreateFromImage(image);
		return tgaImage;
	}
	public static void SetImageEffect(Image image, int depth)
	{
		byte[] data = image.GetData();
		byte[] bytes = LoadFileBuffer(Wa2EngineMain.Engine.EffectMode);
		if (bytes.Length == 1280)
		{
			if (bytes != null)
			{
				for (int i = 0; i < data.Length; i += 4)
				{
					int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
					data[i] = bytes[256 + gray];
					data[i + 1] = bytes[512 + gray];
					data[i + 2] = bytes[768 + gray];
				}
				image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
			}
		}
		else if (bytes.Length == 768)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
				data[i] = bytes[0 + gray];
				data[i + 1] = bytes[256 + gray];
				data[i + 2] = bytes[512 + gray];
			}
			image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
		}
		else if (bytes.Length == 256)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				int gray = (77 * data[i] + 151 * data[i + 1] + 28 * data[i + 2]) >> 8;
				data[i] = bytes[gray];
				data[i + 1] = bytes[gray];
				data[i + 2] = bytes[gray];
			}
			image.SetData(image.GetWidth(), image.GetHeight(), false, image.GetFormat(), data);
		}

	}
	public static ImageTexture LoadBmpImage(string path)
	{
		path = path.ToLower();
		byte[] buffer = LoadFileBuffer(path);

		if (buffer == null)
		{
			return null;
		}
		Image image = new();
		image.LoadBmpFromBuffer(buffer);
		ImageTexture tgaImage = ImageTexture.CreateFromImage(image);
		return tgaImage;
	}
	public static AudioStream GetWavStream(string path)
	{
		path = path.ToLower();



		// GD.Print(SoundDic.GetValueOrDefault(path));
		return LoadWavSound(path);
	}

	public static AudioStream GetBgmStream(int id, bool loop = false)
	{
		if (GetOggStream(string.Format("BGM_{0:D3}.OGG", id)) != null)
		{
			return GetOggStream(string.Format("BGM_{0:D3}.OGG", id));
		}
		else
		{
			if (!loop)
			{
				return GetOggStream(string.Format("BGM_{0:D3}_A.OGG", id));
			}
			else
			{
				return GetOggStream(string.Format("BGM_{0:D3}_B.OGG", id));
			}
		}
	}
	public static byte[] LoadFileBuffer(string path)
	{
		path = path.ToLower();
		FileEntry entry = FileDic.GetValueOrDefault(path);
		if (entry == null)
		{
			return null;
		}

		string fullPath = System.IO.Path.Combine(ProjectSettings.GlobalizePath(ResPath), entry.PkgPath);

		using (System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
		using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
		{
			fs.Seek(entry.Offset, System.IO.SeekOrigin.Begin);

			if (entry.Crypted == 0)
			{
				return reader.ReadBytes((int)entry.Size);
			}
			else
			{
				// 解压缩流程
				byte[] arr = new byte[0x1000];
				for (int i = 0; i < 0xFEE; i++)
				{
					arr[i] = 0x20;
				}

				uint arr_w = 0xFEE;
				uint insize = 0, outsize = 0;

				uint inlim = reader.ReadUInt32();
				uint outlim = reader.ReadUInt32();

				byte[] readBuffer = reader.ReadBytes((int)inlim);
				byte[] buffer = new byte[outlim];

				while (true)
				{
					if (insize >= inlim)
					{
						return buffer;
					}

					byte flag = readBuffer[insize++];

					for (int j = 0; j < 8; j++)
					{
						if (insize >= inlim || outsize >= outlim)
						{
							return buffer;
						}

						byte byte1 = readBuffer[insize++];

						if ((flag & 1) == 0)
						{
							byte byte2 = readBuffer[insize++];
							uint arr_r = (uint)(byte1 | (byte2 & 0xF0) << 4);
							uint counter = (uint)(byte2 & 0xF) + 3;

							while (counter-- > 0)
							{
								byte b = arr[arr_r++ & 0xFFF];
								arr[arr_w++ & 0xFFF] = b;
								buffer[outsize++] = b;
							}
						}
						else
						{
							arr[arr_w++ & 0xFFF] = byte1;
							buffer[outsize++] = byte1;
						}

						flag >>= 1;
					}
				}
			}
		}
	}
	public static void LoadPak(string path)
	{
		string fullPath = System.IO.Path.Combine(ProjectSettings.GlobalizePath(ResPath), path);
		// GD.Print(ProjectSettings.GlobalizePath(ResPath));
		// GD.Print(path);
		using (System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
		using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
		{
			uint magic = reader.ReadUInt32();

			if (magic == 0x5041434B) // 'PACK'
			{
				reader.ReadUInt64(); // skip 8 bytes
				uint nentry = reader.ReadUInt32();

				for (int i = 0; i < nentry; i++)
				{
					fs.Seek(16 + i * 44, System.IO.SeekOrigin.Begin);

					uint crypted = reader.ReadUInt32();

					byte[] nameBuffer = reader.ReadBytes(24);
					string fileName = Encoding.GetEncoding("shift_jis")
							.GetString(nameBuffer).ToLower().Replace("\0", "");

					reader.ReadUInt64(); // skip 8 bytes

					uint offset = reader.ReadUInt32();
					uint size = reader.ReadUInt32();

					FileEntry entry = new()
					{
						PkgPath = path,
						Offset = offset,
						Size = size,
						Crypted = crypted,
						FileName = fileName
					};

					FileDic[fileName] = entry;
				}
			}
			else if (magic == 0x0043414C) // 'LAC\x00'
			{
				uint nentry = reader.ReadUInt32();

				for (int i = 0; i < nentry; i++)
				{
					fs.Seek(8 + i * 40, System.IO.SeekOrigin.Begin);

					byte[] nameBuffer = reader.ReadBytes(32);
					for (int j = 0; j < nameBuffer.Length; j++)
					{
						if (nameBuffer[j] != 0)
						{
							nameBuffer[j] = (byte)(~nameBuffer[j] & 0xFF);
						}
					}

					string fileName = Encoding.GetEncoding("shift_jis")
							.GetString(nameBuffer).ToLower().Replace("\0", "");

					uint size = reader.ReadUInt32();
					uint offset = reader.ReadUInt32();

					FileEntry entry = new()
					{
						PkgPath = path,
						Offset = offset,
						Size = size,
						Crypted = 0,
						FileName = fileName
					};

					FileDic[fileName] = entry;
				}
			}
		}
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
