// Decompiled with JetBrains decompiler
// Type: Tools.ReCat
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReCat : Regex
  {
    private Regex m_next;

    public ReCat(TokensGen tks, Regex sub, int p, string str)
    {
      this.m_sub = sub;
      this.m_next = new Regex(tks, p, str);
    }

    public override void Print(TextWriter s)
    {
      s.Write("(");
      if (this.m_sub != null)
        this.m_sub.Print(s);
      s.Write(")(");
      if (this.m_next != null)
        this.m_next.Print(s);
      s.Write(")");
    }

    public override int Match(string str, int pos, int max)
    {
      int num1 = -1;
      if (this.m_next == null)
        return base.Match(str, pos, max);
      if (this.m_sub == null)
        return this.m_next.Match(str, pos, max);
      int num2;
      for (int max1 = max; max1 >= 0; max1 = num2 - 1)
      {
        num2 = this.m_sub.Match(str, pos, max1);
        if (num2 >= 0)
        {
          int num3 = this.m_next.Match(str, pos + num2, max);
          if (num3 >= 0 && num2 + num3 > num1)
            num1 = num2 + num3;
        }
        else
          break;
      }
      return num1;
    }

    public override void Build(Nfa nfa)
    {
      if (this.m_next != null)
      {
        if (this.m_sub != null)
        {
          Nfa nfa1 = new Nfa(nfa.m_tks, this.m_sub);
          Nfa nfa2 = new Nfa(nfa.m_tks, this.m_next);
          nfa.AddEps((NfaNode) nfa1);
          nfa1.m_end.AddEps((NfaNode) nfa2);
          nfa2.m_end.AddEps(nfa.m_end);
        }
        else
          this.m_next.Build(nfa);
      }
      else
        base.Build(nfa);
    }
  }
}
