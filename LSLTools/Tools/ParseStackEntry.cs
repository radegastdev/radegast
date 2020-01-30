// Decompiled with JetBrains decompiler
// Type: Tools.ParseStackEntry
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class ParseStackEntry
  {
    public Parser yyps;
    public int m_state;
    public SYMBOL m_value;

    public ParseStackEntry(Parser yyp)
    {
      this.yyps = yyp;
    }

    public ParseStackEntry(Parser yyp, int state, SYMBOL value)
    {
      this.yyps = yyp;
      this.m_state = state;
      this.m_value = value;
    }
  }
}
