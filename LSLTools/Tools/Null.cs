// Decompiled with JetBrains decompiler
// Type: Tools.Null
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class Null : TOKEN
  {
    public Null(Lexer yyl, string proxy)
      : base(yyl)
    {
      this.yytext = proxy;
    }

    public Null(Parser yyp, string proxy)
      : base(yyp)
    {
      this.yytext = proxy;
    }

    public override string yyname
    {
      get
      {
        return this.yytext;
      }
    }
  }
}
