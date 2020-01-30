// Decompiled with JetBrains decompiler
// Type: Tools.ParserAction
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class ParserAction : CSymbol
  {
    public CSymbol m_sym;
    public int m_len;

    public ParserAction(SymbolsGen yyp)
      : base(yyp)
    {
    }

    protected ParserAction()
    {
    }

    public virtual SYMBOL Action(Parser yyp)
    {
      SYMBOL symbol1 = (SYMBOL) Sfactory.create(this.m_sym.yytext, yyp);
      if (symbol1.yyname == this.m_sym.yytext)
      {
        SYMBOL symbol2 = yyp.StackAt(this.m_len - 1).m_value;
        symbol1.m_dollar = this.m_len == 0 || symbol2 == null ? (object) null : symbol2.m_dollar;
      }
      return symbol1;
    }

    public override void Print()
    {
      Console.Write(this.m_sym.yytext);
    }

    public override bool IsAction()
    {
      return true;
    }

    public virtual int ActNum()
    {
      return 0;
    }

    public new static object Serialise(object o, Serialiser s)
    {
      ParserAction parserAction = (ParserAction) o;
      if (s.Encode)
      {
        CSymbol.Serialise((object) parserAction, s);
        s.Serialise((object) parserAction.m_sym);
        s.Serialise((object) parserAction.m_len);
        return (object) null;
      }
      CSymbol.Serialise((object) parserAction, s);
      parserAction.m_sym = (CSymbol) s.Deserialise();
      parserAction.m_len = (int) s.Deserialise();
      return (object) parserAction;
    }
  }
}
