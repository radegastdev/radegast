// Decompiled with JetBrains decompiler
// Type: Tools.ParseState
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;

namespace Tools
{
  public class ParseState
  {
    private bool m_changed = true;
    public Hashtable m_transitions = new Hashtable();
    public int m_state;
    public CSymbol m_accessingSymbol;
    public SymbolsGen m_sgen;
    internal ProdItemList m_items;

    public ParseState(SymbolsGen syms, CSymbol acc)
    {
      this.m_sgen = syms;
      this.m_state = syms.state++;
      this.m_accessingSymbol = acc;
      this.m_items = new ProdItemList();
    }

    private ParseState()
    {
    }

    public Transition GetTransition(CSymbol s)
    {
      return (Transition) this.m_transitions[(object) s.yytext] ?? new Transition(this, s);
    }

    public bool Accessor(CSymbol[] x)
    {
      return new Path(x).Top == this;
    }

    public bool Lookback(Production pr, ParseState p)
    {
      return new Path(this, pr.Prefix(pr.m_rhs.Count)).Top == this;
    }

    public void MaybeAdd(ProdItem item)
    {
      if (!this.m_items.Add(item))
        return;
      this.m_changed = true;
    }

    public void Closure()
    {
      while (this.m_changed)
      {
        this.m_changed = false;
        for (ProdItemList prodItemList = this.m_items; prodItemList.m_pi != null; prodItemList = prodItemList.m_next)
          this.CheckClosure(prodItemList.m_pi);
      }
    }

    public void CheckClosure(ProdItem item)
    {
      CSymbol csymbol = item.Next();
      if (csymbol == null)
        return;
      csymbol.AddStartItems(this, item.FirstOfRest(csymbol.m_parser));
      if (!item.IsReducingAction())
        return;
      this.MaybeAdd(new ProdItem(item.m_prod, item.m_pos + 1));
    }

    public void AddEntries()
    {
      for (ProdItemList prodItemList = this.m_items; prodItemList.m_pi != null; prodItemList = prodItemList.m_next)
      {
        ProdItem pi1 = prodItemList.m_pi;
        if (!pi1.m_done)
        {
          CSymbol csymbol = pi1.Next();
          if (csymbol != null && !pi1.IsReducingAction())
          {
            ParseState parseState = new ParseState(this.m_sgen, csymbol);
            parseState.MaybeAdd(new ProdItem(pi1.m_prod, pi1.m_pos + 1));
            for (ProdItemList next = prodItemList.m_next; next != null && next.m_pi != null; next = next.m_next)
            {
              ProdItem pi2 = next.m_pi;
              if (csymbol == pi2.Next())
              {
                parseState.MaybeAdd(new ProdItem(pi2.m_prod, pi2.m_pos + 1));
                pi2.m_done = true;
              }
            }
            if (!this.m_items.AtEnd)
            {
              if (csymbol.IsAction())
              {
                ParseState next = parseState.CheckExists();
                foreach (CSymbol key in (IEnumerable) csymbol.m_follow.Keys)
                {
                  if (key != this.m_sgen.m_symbols.EOFSymbol)
                    this.GetTransition(key).m_next = new ParserShift((ParserAction) csymbol, next);
                }
              }
              else
                this.GetTransition(csymbol).m_next = new ParserShift((ParserAction) null, parseState.CheckExists());
            }
          }
        }
      }
    }

    public void ReduceStates()
    {
      for (ProdItemList prodItemList = this.m_items; prodItemList.m_pi != null; prodItemList = prodItemList.m_next)
      {
        ProdItem pi = prodItemList.m_pi;
        if (pi.Next() == null)
        {
          Production prod = pi.m_prod;
          if (prod.m_pno != 0)
          {
            int count = prod.m_rhs.Count;
            CSymbol rh;
            ParserReduce parserReduce;
            if (count > 0 && (rh = (CSymbol) prod.m_rhs[count - 1]) != null && rh.IsAction())
            {
              ParserAction action = (ParserAction) rh;
              action.m_len = count;
              parserReduce = new ParserReduce(action, count - 1, prod);
            }
            else
            {
              this.m_sgen.m_lexer.yytext = "%" + prod.m_lhs.yytext;
              this.m_sgen.m_prod = prod;
              ParserSimpleAction parserSimpleAction = new ParserSimpleAction(this.m_sgen);
              parserSimpleAction.m_sym = prod.m_lhs;
              parserSimpleAction.m_len = count;
              parserReduce = new ParserReduce((ParserAction) parserSimpleAction, count, prod);
            }
            foreach (CSymbol key in (IEnumerable) pi.m_prod.m_lhs.m_follow.Keys)
              this.GetTransition(key).m_reduce[(object) prod] = (object) parserReduce;
          }
        }
      }
    }

    public bool SameAs(ParseState p)
    {
      if (this.m_accessingSymbol != p.m_accessingSymbol)
        return false;
      ProdItemList prodItemList1 = this.m_items;
      ProdItemList prodItemList2;
      for (prodItemList2 = p.m_items; !prodItemList1.AtEnd && !prodItemList2.AtEnd && (prodItemList1.m_pi.m_prod == prodItemList2.m_pi.m_prod && prodItemList1.m_pi.m_pos == prodItemList2.m_pi.m_pos); prodItemList2 = prodItemList2.m_next)
        prodItemList1 = prodItemList1.m_next;
      if (prodItemList1.AtEnd)
        return prodItemList2.AtEnd;
      return false;
    }

    public ParseState CheckExists()
    {
      this.Closure();
      foreach (ParseState p in (IEnumerable) this.m_sgen.m_symbols.m_states.Values)
      {
        if (this.SameAs(p))
          return p;
      }
      this.m_sgen.m_symbols.m_states[(object) this.m_state] = (object) this;
      this.AddEntries();
      return this;
    }

    ~ParseState()
    {
      if (this.m_sgen == null || this.m_state != this.m_sgen.state - 1)
        return;
      --this.m_sgen.state;
    }

    public void Print()
    {
      Console.WriteLine();
      if (this.m_state == 0)
        Console.WriteLine("state 0");
      else
        Console.WriteLine("state {0} accessed by {1}", (object) this.m_state, (object) this.m_accessingSymbol.yytext);
      if (this.m_items != null)
      {
        for (ProdItemList prodItemList = this.m_items; prodItemList.m_pi != null; prodItemList = prodItemList.m_next)
        {
          prodItemList.m_pi.Print();
          prodItemList.m_pi.m_prod.m_lhs.m_follow.Print();
        }
      }
      foreach (Transition transition in (IEnumerable) this.m_transitions.Values)
        transition.Print0();
    }

    public void Print0()
    {
      Console.WriteLine();
      if (this.m_state == 0)
        Console.WriteLine("state 0");
      else
        Console.WriteLine("state {0} accessed by {1}", (object) this.m_state, (object) this.m_accessingSymbol.yytext);
      if (this.m_items != null)
      {
        for (ProdItemList prodItemList = this.m_items; prodItemList.m_pi != null; prodItemList = prodItemList.m_next)
        {
          prodItemList.m_pi.Print();
          Console.WriteLine();
        }
      }
      Console.WriteLine();
      foreach (ParsingInfo pi in (IEnumerable) this.m_sgen.m_symbols.symbolInfo.Values)
        this.PrintTransition(pi);
    }

    private void PrintTransition(ParsingInfo pi)
    {
      ParserEntry parserEntry = (ParserEntry) pi.m_parsetable[(object) this.m_state];
      if (parserEntry == null)
        return;
      Console.Write("        {0}  {1}  ", (object) pi.m_name, (object) parserEntry.str);
      if (parserEntry.m_action != null)
        parserEntry.m_action.Print();
      Console.WriteLine();
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ParseState();
      ParseState parseState = (ParseState) o;
      if (s.Encode)
      {
        s.Serialise((object) parseState.m_state);
        s.Serialise((object) parseState.m_accessingSymbol);
        s.Serialise((object) parseState.m_changed);
        return (object) true;
      }
      parseState.m_state = (int) s.Deserialise();
      parseState.m_accessingSymbol = (CSymbol) s.Deserialise();
      parseState.m_changed = (bool) s.Deserialise();
      return (object) parseState;
    }
  }
}
