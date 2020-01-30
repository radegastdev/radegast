// Decompiled with JetBrains decompiler
// Type: Tools.ParserShift
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class ParserShift : ParserEntry
  {
    public ParseState m_next;

    public ParserShift()
    {
    }

    public ParserShift(ParserAction action, ParseState next)
      : base(action)
    {
      this.m_next = next;
    }

    public override void Pass(ref ParseStackEntry top)
    {
      Parser yyps = top.yyps;
      if (this.m_action == null)
      {
        yyps.Push(top);
        top = new ParseStackEntry(yyps, this.m_next.m_state, yyps.NextSym());
      }
      else
      {
        yyps.Push(new ParseStackEntry(yyps, top.m_state, this.m_action.Action(yyps)));
        top.m_state = this.m_next.m_state;
      }
    }

    public override string str
    {
      get
      {
        if (this.m_next == null)
          return "?? null shift";
        return string.Format("shift {0}", (object) this.m_next.m_state);
      }
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParserShift();
      ParserShift parserShift = (ParserShift) o;
      if (s.Encode)
      {
        ParserEntry.Serialise((object) parserShift, s);
        s.Serialise((object) parserShift.m_next);
        return (object) null;
      }
      ParserEntry.Serialise((object) parserShift, s);
      parserShift.m_next = (ParseState) s.Deserialise();
      return (object) parserShift;
    }
  }
}
