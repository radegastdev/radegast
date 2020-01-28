// Decompiled with JetBrains decompiler
// Type: Tools.ReAlt
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReAlt : Regex
  {
    public Regex m_alt;

    public ReAlt(TokensGen tks, Regex sub, int p, string str)
    {
      this.m_sub = sub;
      this.m_alt = new Regex(tks, p, str);
    }

    public override void Print(TextWriter s)
    {
      s.Write("(");
      if (this.m_sub != null)
        this.m_sub.Print(s);
      s.Write("|");
      if (this.m_alt != null)
        this.m_alt.Print(s);
      s.Write(")");
    }

    public override int Match(string str, int pos, int max)
    {
      int num1 = -1;
      int num2 = -1;
      if (this.m_sub != null)
        num1 = this.m_sub.Match(str, pos, max);
      if (this.m_alt != null)
        num2 = this.m_sub.Match(str, pos, max);
      if (num1 > num2)
        return num1;
      return num2;
    }

    public override void Build(Nfa nfa)
    {
      if (this.m_alt != null)
      {
        Nfa nfa1 = new Nfa(nfa.m_tks, this.m_alt);
        nfa.AddEps((NfaNode) nfa1);
        nfa1.m_end.AddEps(nfa.m_end);
      }
      base.Build(nfa);
    }
  }
}
