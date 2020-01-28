// Decompiled with JetBrains decompiler
// Type: Tools.Nfa
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class Nfa : NfaNode
  {
    public NfaNode m_end;

    public Nfa(TokensGen tks)
      : base(tks)
    {
      this.m_end = new NfaNode(this.m_tks);
    }

    public Nfa(TokensGen tks, Regex re)
      : base(tks)
    {
      this.m_end = new NfaNode(tks);
      re.Build(this);
    }
  }
}
