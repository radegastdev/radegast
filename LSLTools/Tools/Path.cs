// Decompiled with JetBrains decompiler
// Type: Tools.Path
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class Path
  {
    public bool valid = true;
    private ParseState[] m_states;

    public Path(ParseState[] s)
    {
      this.m_states = s;
    }

    public Path(ParseState q, CSymbol[] x)
    {
      this.m_states = new ParseState[x.Length + 1];
      ParseState parseState = this.m_states[0] = q;
      for (int index1 = 0; index1 < x.Length; ++index1)
      {
        int index2 = index1;
        while (index2 < x.Length && x[index2].IsAction())
          ++index2;
        if (index2 >= x.Length)
        {
          this.m_states[index1 + 1] = parseState;
        }
        else
        {
          Transition transition = (Transition) parseState.m_transitions[(object) x[index2].yytext];
          if (transition == null || transition.m_next == null)
          {
            this.valid = false;
            break;
          }
          parseState = this.m_states[index1 + 1] = transition.m_next.m_next;
        }
      }
    }

    public Path(CSymbol[] x)
      : this((ParseState) x[0].m_parser.m_symbols.m_states[(object) 0], x)
    {
    }

    public CSymbol[] Spelling
    {
      get
      {
        CSymbol[] csymbolArray = new CSymbol[this.m_states.Length - 1];
        for (int index = 0; index < csymbolArray.Length; ++index)
          csymbolArray[index] = this.m_states[index].m_accessingSymbol;
        return csymbolArray;
      }
    }

    public ParseState Top
    {
      get
      {
        return this.m_states[this.m_states.Length - 1];
      }
    }
  }
}
