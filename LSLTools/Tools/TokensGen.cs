// Decompiled with JetBrains decompiler
// Type: Tools.TokensGen
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Collections;

namespace Tools
{
  public abstract class TokensGen : GenBase
  {
    public Hashtable defines = new Hashtable();
    public ObjectList states = new ObjectList();
    protected bool m_showDfa;
    public YyLexer m_tokens;
    private int state;

    public TokensGen(ErrorHandler eh)
      : base(eh)
    {
    }

    public int NewState()
    {
      return ++this.state;
    }

    public string FixActions(string str)
    {
      return str.Replace("yybegin", "yym.yy_begin").Replace("yyl", "((" + this.m_outname + ")yym)");
    }
  }
}
