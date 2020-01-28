// Decompiled with JetBrains decompiler
// Type: Tools.NfaNode
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class NfaNode : LNode
  {
    public string m_sTerminal = "";
    public ObjectList m_arcs = new ObjectList();
    public ObjectList m_eps = new ObjectList();

    public NfaNode(TokensGen tks)
      : base(tks)
    {
    }

    public void AddArc(char ch, NfaNode next)
    {
      this.m_arcs.Add((object) new Arc(ch, next));
    }

    public void AddUArc(char ch, NfaNode next)
    {
      this.m_arcs.Add((object) new UArc(ch, next));
    }

    public void AddArcEx(Regex re, NfaNode next)
    {
      this.m_arcs.Add((object) new ArcEx(re, next));
    }

    public void AddEps(NfaNode next)
    {
      this.m_eps.Add((object) next);
    }

    public void AddTarget(char ch, Dfa next)
    {
      for (int index = 0; index < this.m_arcs.Count; ++index)
      {
        Arc arc = (Arc) this.m_arcs[index];
        if (arc.Match(ch))
          next.AddNfaNode(arc.m_next);
      }
    }
  }
}
