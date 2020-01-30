// Decompiled with JetBrains decompiler
// Type: Tools.error
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class error : SYMBOL
  {
    public int state;
    public SYMBOL sym;

    public error(Parser yyp, ParseStackEntry s)
      : base(yyp)
    {
      this.state = s.m_state;
      this.sym = s.m_value;
    }

    public error(Parser yyp)
      : base(yyp)
    {
    }

    public override string yyname
    {
      get
      {
        return nameof (error);
      }
    }

    public override string ToString()
    {
      string str = "syntax error occurred in state " + (object) this.state;
      if (this.sym == null)
        return str;
      if (!(this.sym is TOKEN))
        return str + " on symbol " + this.sym.yyname;
      TOKEN sym = (TOKEN) this.sym;
      return str + " on input token " + sym.yytext;
    }
  }
}
