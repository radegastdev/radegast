// Decompiled with JetBrains decompiler
// Type: Tools.EOF
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class EOF : CSymbol
  {
    public EOF(SymbolsGen yyp)
      : base(yyp)
    {
      this.yytext = nameof (EOF);
      this.m_yynum = 2;
      this.m_symtype = CSymbol.SymType.eofsymbol;
    }

    public EOF(Lexer yyl)
      : base(yyl)
    {
      this.yytext = nameof (EOF);
      this.pos = yyl.m_LineManager.end;
      this.m_symtype = CSymbol.SymType.eofsymbol;
    }

    private EOF()
    {
    }

    public override string yyname
    {
      get
      {
        return nameof (EOF);
      }
    }

    public override int yynum
    {
      get
      {
        return 2;
      }
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new EOF();
      return CSymbol.Serialise(o, s);
    }
  }
}
