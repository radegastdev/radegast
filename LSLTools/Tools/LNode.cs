// Decompiled with JetBrains decompiler
// Type: Tools.LNode
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public abstract class LNode
  {
    public int m_state;
    public TokensGen m_tks;

    public LNode(TokensGen tks)
    {
      this.m_tks = tks;
      this.m_state = tks.NewState();
    }

    protected LNode()
    {
    }
  }
}
