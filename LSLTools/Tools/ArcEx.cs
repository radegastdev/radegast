// Decompiled with JetBrains decompiler
// Type: Tools.ArcEx
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ArcEx : Arc
  {
    public Regex m_ref;

    public ArcEx(Regex re, NfaNode next)
    {
      this.m_ref = re;
      this.m_next = next;
    }

    public override bool Match(char ch)
    {
      return this.m_ref.Match(ch);
    }

    public override void Print(TextWriter s)
    {
      s.Write("  ");
      this.m_ref.Print(s);
      s.WriteLine(this.m_next.m_state);
    }
  }
}
