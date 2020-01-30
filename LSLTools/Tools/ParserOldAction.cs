// Decompiled with JetBrains decompiler
// Type: Tools.ParserOldAction
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class ParserOldAction : ParserAction
  {
    public int m_action;

    public ParserOldAction(SymbolsGen yyp)
      : base(yyp)
    {
      this.m_action = yyp.action_num++;
      yyp.actions.Add((object) this);
      this.m_sym = (CSymbol) null;
      this.m_symtype = CSymbol.SymType.oldaction;
      yyp.OldAction(this);
    }

    private ParserOldAction()
    {
    }

    public override SYMBOL Action(Parser yyp)
    {
      SYMBOL yysym = base.Action(yyp);
      object obj = yyp.m_symbols.Action(yyp, yysym, this.m_action);
      if (obj != null)
        yysym.m_dollar = obj;
      return yysym;
    }

    public override int ActNum()
    {
      return this.m_action;
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParserOldAction();
      ParserOldAction parserOldAction = (ParserOldAction) o;
      if (s.Encode)
      {
        ParserAction.Serialise((object) parserOldAction, s);
        s.Serialise((object) parserOldAction.m_action);
        return (object) null;
      }
      ParserAction.Serialise((object) parserOldAction, s);
      parserOldAction.m_action = (int) s.Deserialise();
      return (object) parserOldAction;
    }
  }
}
