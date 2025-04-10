using Godot;
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
	public static VideoStream GetMovie(string name){
		VideoStream video=new();
		video.File=ResPath+"movie/mv100.mp4";
		return video;
	}
	public static AudioStream LoadWavSound(string path)
	{
		GD.Print(path);
		byte[] buffer = LoadFileBuffer(path);
		if (buffer == null)
		{
			return null;
		}
		AudioStream wavStream = AudioStreamWav.LoadFromBuffer(buffer);
		return wavStream;

	}
	public static Texture2D GetMaskImage(int id)
	{
		return GetBmpImage(string.Format("f0{0:D3}.bmp", id));
	}
	public static AudioStream GetVoiceStream(int label, int id, int chr)
	{
		return GetOggStream(string.Format("{0:D4}_{1:D4}_{2:D2}.ogg", label, id, chr));
	}
	public static AudioStream GetSeStream(uint id)
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
		GD.Print(path);


		if (buffer == null)
		{
			return null;
		}
		Image image = new();
		image.LoadTgaFromBuffer(buffer);
		ImageTexture tgaImage = ImageTexture.CreateFromImage(image);
		return tgaImage;
		
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
		FileAccess file = FileAccess.Open(ResPath + entry.PkgPath, FileAccess.ModeFlags.Read);
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
			byte flag, byte1 , byte2 ;
			
			byte[] arr = new byte[0x1000];
			for (int i = 0; i < 0xfee; i++)
			{
				arr[i] = 0x20;
			}
			uint arr_r, arr_w = 0xfee;
			uint counter;
			uint insize = 0, outsize = 0;
			uint inlim = file.Get32();
			uint outlim = file.Get32();
			byte[] readBuffer=file.GetBuffer(inlim);
			file.Close();
			buffer = new byte[outlim];
			while (true )
			{
				if (insize>=inlim){
					return buffer;
				}
				flag = readBuffer[insize++];
				for (int j = 0; j < 8; j++)
				{
					if (insize >= inlim || outsize >= outlim)
					{
						return buffer;
					}
					byte1 = readBuffer[insize++];
					if ((flag & 1)==0)
					{
						byte2 = readBuffer[insize++];
						arr_r = (uint)(byte1 | (byte2 & 0xF0) << 4);
						counter = (uint)(byte2 & 0xF) + 3;
						while (counter > 0)
						{
							byte1 = arr[arr_r++ & 0xfff];
							arr[arr_w++ & 0xfff] = byte1;
							buffer[outsize++] = byte1;
							counter--;
						}
					}
					else
					{
						arr[arr_w++ & 0xfff] = byte1;
						buffer[outsize++] = byte1;
					}
					flag >>= 1;
				}
			}
			// byte[] arr = new byte[0x1000];

			// uint insize = 0;
			// uint outsize = 0;
			// uint arrw = 0xfee;
			// buffer = new byte[outceil];
			// while (insize < inceil && outsize < outceil)
			// {
			// 	byte flag = file.Get8();
			// 	insize++;
			// 	for (int j = 0; j < 8; j++)
			// 	{
			// 		if (insize >= inceil || outsize >= outceil)
			// 		{
			// 			break;
			// 		}
			// 		byte b1 = file.Get8();
			// 		insize++;
			// 		if ((flag & 1) == 0)
			// 		{
			// 			byte b2 = file.Get8();
			// 			insize++;
			// 			uint arrr = b1 | (uint)(b2 & 0xf0) << 4;
			// 			uint counter = (uint)(b2 & 0xf) + 3;
			// 			while (counter > 0)
			// 			{
			// 				b1 = arr[arrr & 0xfff];
			// 				arr[arrw & 0xFFF] = b1;
			// 				buffer[outsize] = b1;
			// 				arrr++;
			// 				arrw++;
			// 				outsize++;
			// 				counter--;
			// 			}
			// 		}
			// 		else
			// 		{
			// 			arr[arrw & 0xfff] = b1;
			// 			buffer[outsize] = b1;
			// 			arrw++;
			// 			outsize++;
			// 		}
			// 		flag >>= 1;
			// 	}
			// }
			// return buffer;
		}
	}
	public static void LoadPak(string path)
	{
		FileAccess file = FileAccess.Open(ResPath + path, FileAccess.ModeFlags.Read);

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
				FileDic[fileName] = entry;
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
						tempBuffer[j] = (byte)(~tempBuffer[j] & 0xff);
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

				FileDic[fileName] = entry;
				// GD.Print(fileName);
			}
		}
		// PakDir[path] = file;
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
