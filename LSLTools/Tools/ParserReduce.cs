// Decompiled with JetBrains decompiler
// Type: Tools.ParserReduce
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;

namespace Tools
{
  public class ParserReduce : ParserEntry
  {
    public int m_depth;
    public Production m_prod;
    public SymbolSet m_lookAhead;

    public ParserReduce(ParserAction action, int depth, Production prod)
      : base(action)
    {
      this.m_depth = depth;
      this.m_prod = prod;
    }

    private ParserReduce()
    {
    }

    public void BuildLookback(Transition a)
    {
      SymbolsGen sgen = a.m_ps.m_sgen;
      if (this.m_lookAhead != null)
        return;
      this.m_lookAhead = new SymbolSet(sgen);
      foreach (ParseState q in (IEnumerable) sgen.m_symbols.m_states.Values)
      {
        Transition transition = (Transition) q.m_transitions[(object) this.m_prod.m_lhs.yytext];
        if (transition != null)
        {
          Path path = new Path(q, this.m_prod.Prefix(this.m_prod.m_rhs.Count));
          if (path.valid && path.Top == a.m_ps)
            transition.m_lookbackOf[(object) this] = (object) true;
        }
      }
    }

    public override void Pass(ref ParseStackEntry top)
    {
      Parser yyps = top.yyps;
      SYMBOL ns = this.m_action.Action(yyps);
      yyps.m_ungot = top.m_value;
      if (yyps.m_debug)
        Console.WriteLine("about to pop {0} count is {1}", (object) this.m_depth, (object) yyps.m_stack.Count);
      yyps.Pop(ref top, this.m_depth, ns);
      if (ns.pos == 0)
        ns.pos = top.m_value.pos;
      top.m_value = ns;
    }

    public override bool IsReduce()
    {
      return true;
    }

    public override string str
    {
      get
      {
        if (this.m_prod == null)
          return "?? null reduce";
        return string.Format("reduce {0}", (object) this.m_prod.m_pno);
      }
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParserReduce();
      ParserReduce parserReduce = (ParserReduce) o;
      if (s.Encode)
      {
        ParserEntry.Serialise((object) parserReduce, s);
        s.Serialise((object) parserReduce.m_depth);
        s.Serialise((object) parserReduce.m_prod);
        return (object) null;
      }
      ParserEntry.Serialise((object) parserReduce, s);
      parserReduce.m_depth = (int) s.Deserialise();
      parserReduce.m_prod = (Production) s.Deserialise();
      return (object) parserReduce;
    }
  }
}
