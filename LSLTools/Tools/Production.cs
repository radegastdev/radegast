// Decompiled with JetBrains decompiler
// Type: Tools.Production
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Collections;

namespace Tools
{
  public class Production
  {
    public ObjectList m_rhs = new ObjectList();
    public Hashtable m_alias = new Hashtable();
    public int m_pno;
    public CSymbol m_lhs;
    public bool m_actionsOnly;
    public int m_prec;

    public Production(SymbolsGen syms)
    {
      this.m_lhs = (CSymbol) null;
      this.m_prec = 0;
      this.m_pno = syms.pno++;
      this.m_actionsOnly = true;
      syms.prods.Add((object) this);
    }

    public Production(SymbolsGen syms, CSymbol lhs)
    {
      this.m_lhs = lhs;
      this.m_prec = 0;
      this.m_pno = syms.pno++;
      this.m_actionsOnly = true;
      syms.prods.Add((object) this);
      lhs.m_prods.Add((object) this);
    }

    private Production()
    {
    }

    public void AddToRhs(CSymbol s)
    {
      this.m_rhs.Add((object) s);
      this.m_actionsOnly = this.m_actionsOnly && s.IsAction();
    }

    public void AddFirst(CSymbol s, int j)
    {
      for (; j < this.m_rhs.Count; ++j)
      {
        CSymbol rh = (CSymbol) this.m_rhs[j];
        s.AddFollow(rh.m_first);
        if (!rh.IsNullable())
          break;
      }
    }

    public bool CouldBeEmpty(int j)
    {
      for (; j < this.m_rhs.Count; ++j)
      {
        if (!((CSymbol) this.m_rhs[j]).IsNullable())
          return false;
      }
      return true;
    }

    public CSymbol[] Prefix(int i)
    {
      CSymbol[] csymbolArray = new CSymbol[i];
      for (int index = 0; index < i; ++index)
        csymbolArray[index] = (CSymbol) this.m_rhs[index];
      return csymbolArray;
    }

    public void StackRef(ref string str, int ch, int ix)
    {
      int num = this.m_rhs.Count + 1;
      CSymbol rh = (CSymbol) this.m_rhs[ix - 1];
      str += string.Format("\n\t(({0})(yyq.StackAt({1}).m_value))\n\t", (object) rh.TypeStr(), (object) (num - ix - 1));
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new Production();
      Production production = (Production) o;
      if (s.Encode)
      {
        s.Serialise((object) production.m_pno);
        return (object) null;
      }
      production.m_pno = (int) s.Deserialise();
      return (object) production;
    }
  }
}
