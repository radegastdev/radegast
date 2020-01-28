// Decompiled with JetBrains decompiler
// Type: Tools.ParserEntry
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public abstract class ParserEntry
  {
    public ParserAction m_action;
    public int m_priority;

    public ParserEntry()
    {
      this.m_action = (ParserAction) null;
    }

    public ParserEntry(ParserAction action)
    {
      this.m_action = action;
    }

    public virtual void Pass(ref ParseStackEntry top)
    {
    }

    public virtual bool IsReduce()
    {
      return false;
    }

    public virtual string str
    {
      get
      {
        return "";
      }
    }

    public static object Serialise(object o, Serialiser s)
    {
      ParserEntry parserEntry = (ParserEntry) o;
      if (s.Encode)
      {
        s.Serialise((object) parserEntry.m_action);
        return (object) null;
      }
      parserEntry.m_action = (ParserAction) s.Deserialise();
      return (object) parserEntry;
    }
  }
}
