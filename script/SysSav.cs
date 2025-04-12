using System.Collections.Generic;

public class SysSav{
  public int[] Flags=new int[0x1000];
  public List<ScriptReadPos> ScriptReadPosList=new();
}
public struct ScriptReadPos{
  public string name;
  public int pos;
}