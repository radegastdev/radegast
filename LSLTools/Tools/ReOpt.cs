// Decompiled with JetBrains decompiler
// Type: Tools.ReOpt
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReOpt : Regex
  {
    public ReOpt(Regex sub)
    {
      this.m_sub = sub;
    }

    public override void Print(TextWriter s)
    {
      this.m_sub.Print(s);
      s.Write("?");
    }

    public override int Match(string str, int pos, int max)
    {
      int num = this.m_sub.Match(str, pos, max);
      if (num < 0)
        num = 0;
      return num;
    }

    public override void Build(Nfa nfa)
    {
      nfa.AddEps(nfa.m_end);
      base.Build(nfa);
    }
  }
}
