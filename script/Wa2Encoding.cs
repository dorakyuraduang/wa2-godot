using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using Godot;
public class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[] x, byte[] y)
    {
        if (x == null || y == null) return x == y;
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        if (obj == null) return 0;
        int hash = 17;
        foreach (byte b in obj)
        {
            hash = hash * 31 + b;
        }
        return hash;
    }
}
public class Wa2Encoding : Encoding
{
    public Wa2Encoding()
    {
        foreach (var kvp in customCharToBytes)
        {
            customBytesToChar[kvp.Value] = kvp.Key;
    
        }
    }
    private Encoding shiftJis = Encoding.GetEncoding(932);

    private readonly Dictionary<char, byte[]> customCharToBytes = new Dictionary<char, byte[]>()
    {
        // { '爸', new byte[] { 0x81, 0x7f } },
        // { '爹', new byte[] { 0x83, 0x7f } },
        // { '艺', new byte[] { 0x84, 0x7f } },
        // { '节', new byte[] { 0x87, 0x7f } },
        { '蹑', new byte[] { 0x89, 0x7f } },
        { '蹒', new byte[] { 0x8a, 0x7f } },
        { '荚', new byte[] { 0x8b, 0x7f } },
        { '荞', new byte[] { 0x8c, 0x7f } },
        { '蹦', new byte[] { 0x8d, 0x7f } },
        { '蹩', new byte[] { 0x8e, 0x7f } },
        { '蹭', new byte[] { 0x8f, 0x7f } },
        { '蹰', new byte[] { 0x90, 0x7f } },
        { '蹲', new byte[] { 0x91, 0x7f } },
        { '车', new byte[] { 0x92, 0x7f } },
        { '轨', new byte[] { 0x93, 0x7f } },
        { '转', new byte[] { 0x94, 0x7f } },
        { '轮', new byte[] { 0x95, 0x7f } },
        { '软', new byte[] { 0x96, 0x7f } },
        { '轰', new byte[] { 0x97, 0x7f } },
        { '轴', new byte[] { 0x99, 0x7f } },
        { '轶', new byte[] { 0x9a, 0x7f } },
        { '轻', new byte[] { 0x9b, 0x7f } },
        { '载', new byte[] { 0x9c, 0x7f } },
        { '较', new byte[] { 0x9d, 0x7f } },
        { '辄', new byte[] { 0x9e, 0x7f } },
        { '辅', new byte[] { 0x9f, 0x7f } },
        { '辆', new byte[] { 0xe0, 0x7f } },
        { '辈', new byte[] { 0xe1, 0x7f } },
        { '辉', new byte[] { 0xe2, 0x7f } },
        { '辍', new byte[] { 0xe3, 0x7f } },
        { '辐', new byte[] { 0xe4, 0x7f } },
        { '辑', new byte[] { 0xe5, 0x7f } },
        { '输', new byte[] { 0xe6, 0x7f } },
        { '辕', new byte[] { 0xe7, 0x7f } },
        { '辙', new byte[] { 0xe8, 0x7f } },
        { '邋', new byte[] { 0xe9, 0x7f } },
        { '锋', new byte[] { 0xea, 0x7f } },
    };

    private readonly Dictionary<byte[], char> customBytesToChar=new(new ByteArrayComparer());

    public override int GetByteCount(char[] chars, int index, int count)
    {
        return shiftJis.GetByteCount(chars, index, count) + (customCharToBytes.Count * 2);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        int byteCount = 0;
        for (int i = charIndex; i < charIndex + charCount; i++)
        {
            char c = chars[i];
            if (customCharToBytes.ContainsKey(c))
            {
                byte[] customBytes = customCharToBytes[c];
                Array.Copy(customBytes, 0, bytes, byteIndex + byteCount, customBytes.Length);
                byteCount += customBytes.Length;
            }
            else
            {
                byteCount += shiftJis.GetBytes(chars, i, 1, bytes, byteIndex + byteCount);
            }
        }
        return byteCount;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        
        return shiftJis.GetCharCount(bytes, index, count);
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        //  GD.Print(shiftJis.GetChars(bytes, byteIndex, byteCount, chars, charIndex));
        //  return shiftJis.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        int charCount = 0;
        int i = 0;
        if (byteCount == 0)
        {
            return 0;
        }

        while (i < byteIndex + byteCount)
        {

            byte b1 = bytes[i];
            byte[] pair;
            int byteSize;
            if (byteIndex + 1 < byteIndex + byteCount && b1 >= 0x81 && b1 <= 0x9F || b1 >= 0xE0 && b1 <= 0xFE)
            {
                byte b2 = bytes[i + 1];
                pair = [b1, b2];
                byteSize = 2;
            }
            else
            {
                pair = [b1];
                byteSize = 1;
            }
            if (customBytesToChar.ContainsKey(pair))
            {
                chars[charIndex + charCount] = customBytesToChar[pair];
                charCount++;
            }
            else
            {
                charCount += shiftJis.GetChars(bytes, i, byteSize, chars, charIndex + charCount);
            }
            i += byteSize;
        }
        return charCount;
    }

    public override int GetMaxByteCount(int charCount) => shiftJis.GetMaxByteCount(charCount);
    public override int GetMaxCharCount(int byteCount) => shiftJis.GetMaxCharCount(byteCount);
}

