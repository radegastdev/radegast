// Decompiled with JetBrains decompiler
// Type: Tools.Arc
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class Arc
  {
    public char m_ch;
    public NfaNode m_next;

    public Arc()
    {
    }

    public Arc(char ch, NfaNode next)
    {
      this.m_ch = ch;
      this.m_next = next;
    }

    public virtual bool Match(char ch)
    {
      return (int) ch == (int) this.m_ch;
    }

    public virtual void Print(TextWriter s)
    {
      s.WriteLine(string.Format("  {0} {1}", (object) this.m_ch, (object) this.m_next.m_state));
    }
  }
}
