// Decompiled with JetBrains decompiler
// Type: Tools.ParserSimpleAction
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class ParserSimpleAction : ParserAction
  {
    public ParserSimpleAction(SymbolsGen yyp)
      : base(yyp)
    {
      yyp.actions.Add((object) this);
      this.m_symtype = CSymbol.SymType.simpleaction;
      yyp.SimpleAction(this);
    }

    private ParserSimpleAction()
    {
    }

    public override string TypeStr()
    {
      return this.m_sym.yytext;
    }

    public override void Print()
    {
      Console.Write(" %{0}", (object) this.m_sym.yytext);
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParserSimpleAction();
      if (!s.Encode)
        return ParserAction.Serialise(o, s);
      ParserAction.Serialise(o, s);
      return (object) null;
    }
  }
}
