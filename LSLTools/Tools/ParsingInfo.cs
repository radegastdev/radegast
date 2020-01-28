// Decompiled with JetBrains decompiler
// Type: Tools.ParsingInfo
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Collections;

namespace Tools
{
  public class ParsingInfo
  {
    public Hashtable m_parsetable = new Hashtable();
    public string m_name;
    public int m_yynum;

    public ParsingInfo(string name, int num)
    {
      this.m_name = name;
      this.m_yynum = num;
    }

    private ParsingInfo()
    {
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParsingInfo();
      ParsingInfo parsingInfo = (ParsingInfo) o;
      if (s.Encode)
      {
        s.Serialise((object) parsingInfo.m_name);
        s.Serialise((object) parsingInfo.m_yynum);
        s.Serialise((object) parsingInfo.m_parsetable);
        return (object) null;
      }
      parsingInfo.m_name = (string) s.Deserialise();
      parsingInfo.m_yynum = (int) s.Deserialise();
      parsingInfo.m_parsetable = (Hashtable) s.Deserialise();
      return (object) parsingInfo;
    }
  }
}
