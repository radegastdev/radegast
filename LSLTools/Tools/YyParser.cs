// Decompiled with JetBrains decompiler
// Type: Tools.YyParser
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;
using System.IO;

namespace Tools
{
  public class YyParser
  {
    public ErrorHandler erh = new ErrorHandler(true);
    public Hashtable symbols = new Hashtable();
    public Hashtable literals = new Hashtable();
    public Hashtable symbolInfo = new Hashtable();
    public Hashtable m_states = new Hashtable();
    public Hashtable types = new Hashtable();
    public bool m_concrete;
    public CSymbol EOFSymbol;
    public CSymbol Special;
    public CSymbol m_startSymbol;
    public ParseState m_accept;
    public int[] arr;

    public string StartSymbol
    {
      get
      {
        if (this.m_startSymbol != null)
          return this.m_startSymbol.yytext;
        return "<null>";
      }
      set
      {
        CSymbol symbol = (CSymbol) this.symbols[(object) value];
        if (symbol == null)
          this.erh.Error(new CSToolsException(25, "No such symbol <" + value + ">"));
        this.m_startSymbol = symbol;
      }
    }

    public ParsingInfo GetSymbolInfo(string name, int num)
    {
      ParsingInfo parsingInfo = (ParsingInfo) this.symbolInfo[(object) num];
      if (parsingInfo == null)
        this.symbolInfo[(object) num] = (object) (parsingInfo = new ParsingInfo(name, num));
      return parsingInfo;
    }

    public void ClassInit(SymbolsGen yyp)
    {
      this.Special = new CSymbol(yyp);
      this.Special.yytext = "S'";
      this.EOFSymbol = new EOF(yyp).Resolve();
    }

    public void Transitions(Builder b)
    {
      foreach (ParseState parseState in (IEnumerable) this.m_states.Values)
      {
        foreach (Transition t in (IEnumerable) parseState.m_transitions.Values)
          b(t);
      }
    }

    public void PrintTransitions(Func f, string s)
    {
      foreach (ParseState parseState in (IEnumerable) this.m_states.Values)
      {
        foreach (Transition a in (IEnumerable) parseState.m_transitions.Values)
          a.Print(f(a), s);
      }
    }

    public virtual object Action(Parser yyp, SYMBOL yysym, int yyact)
    {
      return (object) null;
    }

    public void GetEOF(Lexer yyl)
    {
      this.EOFSymbol = (CSymbol) this.symbols[(object) "EOF"];
      if (this.EOFSymbol != null)
        return;
      this.EOFSymbol = (CSymbol) new EOF(yyl);
    }

    public void Emit(TextWriter m_outFile)
    {
      Serialiser serialiser = new Serialiser(m_outFile);
      serialiser.VersionCheck();
      Console.WriteLine("Serialising the parser");
      serialiser.Serialise((object) this.m_startSymbol);
      serialiser.Serialise((object) this.m_accept);
      serialiser.Serialise((object) this.m_states);
      serialiser.Serialise((object) this.literals);
      serialiser.Serialise((object) this.symbolInfo);
      serialiser.Serialise((object) this.m_concrete);
      m_outFile.WriteLine("0};");
    }

    public void GetParser(Lexer m_lexer)
    {
      Serialiser serialiser = new Serialiser(this.arr);
      serialiser.VersionCheck();
      this.m_startSymbol = (CSymbol) serialiser.Deserialise();
      this.m_startSymbol.kids = new ObjectList();
      this.m_accept = (ParseState) serialiser.Deserialise();
      this.m_states = (Hashtable) serialiser.Deserialise();
      this.literals = (Hashtable) serialiser.Deserialise();
      this.symbolInfo = (Hashtable) serialiser.Deserialise();
      this.m_concrete = (bool) serialiser.Deserialise();
      this.GetEOF(m_lexer);
    }
  }
}
