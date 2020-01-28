// Decompiled with JetBrains decompiler
// Type: Tools.ProdItem
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;

namespace Tools
{
  public class ProdItem
  {
    public Production m_prod;
    public int m_pos;
    public bool m_done;
    private SymbolSet follow;

    public ProdItem(Production prod, int pos)
    {
      this.m_prod = prod;
      this.m_pos = pos;
      this.m_done = false;
    }

    public ProdItem()
    {
      this.m_prod = (Production) null;
      this.m_pos = 0;
      this.m_done = false;
    }

    public CSymbol Next()
    {
      if (this.m_pos < this.m_prod.m_rhs.Count)
        return (CSymbol) this.m_prod.m_rhs[this.m_pos];
      return (CSymbol) null;
    }

    public bool IsReducingAction()
    {
      if (this.m_pos == this.m_prod.m_rhs.Count - 1)
        return this.Next().IsAction();
      return false;
    }

    public SymbolSet FirstOfRest(SymbolsGen syms)
    {
      if (this.follow != null)
        return this.follow;
      this.follow = new SymbolSet(syms);
      bool flag = false;
      int count = this.m_prod.m_rhs.Count;
      for (int index = this.m_pos + 1; index < count; ++index)
      {
        CSymbol rh = (CSymbol) this.m_prod.m_rhs[index];
        foreach (CSymbol key in (IEnumerable) rh.m_first.Keys)
          this.follow.CheckIn(key);
        if (!rh.IsNullable())
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.follow.Add(this.m_prod.m_lhs.m_follow);
      this.follow = this.follow.Resolve();
      return this.follow;
    }

    public void Print()
    {
      Console.Write("   {0}    {1} : ", (object) this.m_prod.m_pno, this.m_prod.m_lhs == null ? (object) "$start" : (object) this.m_prod.m_lhs.yytext);
      int index;
      for (index = 0; index < this.m_prod.m_rhs.Count; ++index)
      {
        if (index == this.m_pos)
          Console.Write("_");
        else
          Console.Write(" ");
        string str = ((TOKEN) this.m_prod.m_rhs[index]).yytext;
        if (str.Equals("\n"))
          str = "\\n";
        Console.Write(str);
      }
      if (index == this.m_pos)
        Console.Write("_");
      Console.Write("  ");
    }
  }
}
