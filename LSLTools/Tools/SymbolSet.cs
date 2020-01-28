// Decompiled with JetBrains decompiler
// Type: Tools.SymbolSet
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;

namespace Tools
{
  public class SymbolSet
  {
    private Hashtable m_set = new Hashtable();
    public SymbolsGen m_symbols;
    public SymbolSet m_next;

    public SymbolSet(SymbolsGen syms)
    {
      this.m_symbols = syms;
    }

    public SymbolSet(SymbolSet s)
      : this(s.m_symbols)
    {
      this.Add(s);
    }

    public bool Contains(CSymbol a)
    {
      return this.m_set.Contains((object) a);
    }

    public ICollection Keys
    {
      get
      {
        return this.m_set.Keys;
      }
    }

    public IDictionaryEnumerator GetEnumerator()
    {
      return this.m_set.GetEnumerator();
    }

    public int Count
    {
      get
      {
        return this.m_set.Count;
      }
    }

    public bool CheckIn(CSymbol a)
    {
      if (this.Contains(a))
        return false;
      this.AddIn(a);
      return true;
    }

    public SymbolSet Resolve()
    {
      return this.find(this.m_symbols.lahead);
    }

    private SymbolSet find(SymbolSet h)
    {
      if (h == null)
      {
        this.m_next = this.m_symbols.lahead;
        this.m_symbols.lahead = this;
        return this;
      }
      if (SymbolSet.Equals(h, this))
        return h;
      return this.find(h.m_next);
    }

    private static bool Equals(SymbolSet s, SymbolSet t)
    {
      if (s.m_set.Count != t.m_set.Count)
        return false;
      IDictionaryEnumerator enumerator1 = s.GetEnumerator();
      IDictionaryEnumerator enumerator2 = t.GetEnumerator();
      for (int index = 0; index < s.Count; ++index)
      {
        enumerator1.MoveNext();
        enumerator2.MoveNext();
        if (enumerator1.Key != enumerator2.Key)
          return false;
      }
      return true;
    }

    public void AddIn(CSymbol t)
    {
      this.m_set[(object) t] = (object) true;
    }

    public void Add(SymbolSet s)
    {
      if (s == this)
        return;
      foreach (CSymbol key in (IEnumerable) s.Keys)
        this.AddIn(key);
    }

    public void Print()
    {
      string str = "[";
      int num = 0;
      foreach (CSymbol key in (IEnumerable) this.Keys)
      {
        ++num;
        str = !key.yytext.Equals("\n") ? str + key.yytext : str + "\\n";
        if (num < this.Count)
          str += ",";
      }
      Console.WriteLine(str + "]");
    }

    public static SymbolSet operator +(SymbolSet s, SymbolSet t)
    {
      SymbolSet symbolSet = new SymbolSet(s);
      symbolSet.Add(t);
      return symbolSet.Resolve();
    }
  }
}
