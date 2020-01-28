// Decompiled with JetBrains decompiler
// Type: Tools.UArc
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class UArc : Arc
  {
    public UArc()
    {
    }

    public UArc(char ch, NfaNode next)
      : base(ch, next)
    {
    }

    public override bool Match(char ch)
    {
      return (int) char.ToUpper(ch) == (int) char.ToUpper(this.m_ch);
    }

    public override void Print(TextWriter s)
    {
      s.WriteLine(string.Format("  U'{0}' {1}", (object) this.m_ch, (object) this.m_next.m_state));
    }
  }
}
