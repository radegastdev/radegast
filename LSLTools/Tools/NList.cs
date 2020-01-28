// Decompiled with JetBrains decompiler
// Type: Tools.NList
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  internal class NList
  {
    public NfaNode m_node;
    public NList m_next;

    public NList()
    {
      this.m_node = (NfaNode) null;
      this.m_next = (NList) null;
    }

    private NList(NfaNode nd, NList nx)
    {
      this.m_node = nd;
      this.m_next = nx;
    }

    public bool Add(NfaNode n)
    {
      if (this.m_node == null)
      {
        this.m_next = new NList();
        this.m_node = n;
      }
      else if (this.m_node.m_state < n.m_state)
      {
        this.m_next = new NList(this.m_node, this.m_next);
        this.m_node = n;
      }
      else
      {
        if (this.m_node.m_state == n.m_state)
          return false;
        return this.m_next.Add(n);
      }
      return true;
    }

    public bool AtEnd
    {
      get
      {
        return this.m_node == null;
      }
    }
  }
}
