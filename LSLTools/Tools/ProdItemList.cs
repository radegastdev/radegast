// Decompiled with JetBrains decompiler
// Type: Tools.ProdItemList
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  internal class ProdItemList
  {
    public ProdItem m_pi;
    public ProdItemList m_next;

    public ProdItemList(ProdItem pi, ProdItemList n)
    {
      this.m_pi = pi;
      this.m_next = n;
    }

    public ProdItemList()
    {
      this.m_pi = (ProdItem) null;
      this.m_next = (ProdItemList) null;
    }

    public bool Add(ProdItem pi)
    {
      if (this.m_pi == null)
      {
        this.m_next = new ProdItemList();
        this.m_pi = pi;
      }
      else if (this.m_pi.m_prod.m_pno < pi.m_prod.m_pno || this.m_pi.m_prod.m_pno == pi.m_prod.m_pno && this.m_pi.m_pos < pi.m_pos)
      {
        this.m_next = new ProdItemList(this.m_pi, this.m_next);
        this.m_pi = pi;
      }
      else
      {
        if (this.m_pi.m_prod.m_pno == pi.m_prod.m_pno && this.m_pi.m_pos == pi.m_pos)
          return false;
        return this.m_next.Add(pi);
      }
      return true;
    }

    public bool AtEnd
    {
      get
      {
        return this.m_pi == null;
      }
    }
  }
}
