// Decompiled with JetBrains decompiler
// Type: Tools.ReStar
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReStar : Regex
  {
    public ReStar(Regex sub)
    {
      this.m_sub = sub;
    }

    public override void Print(TextWriter s)
    {
      this.m_sub.Print(s);
      s.Write("*");
    }

    public override int Match(string str, int pos, int max)
    {
      int num1 = this.m_sub.Match(str, pos, max);
      if (num1 < 0)
        return -1;
      int num2 = 0;
      while (num1 > 0)
      {
        num1 = this.m_sub.Match(str, pos + num2, max);
        if (num1 >= 0)
          num2 += num1;
        else
          break;
      }
      return num2;
    }

    public override void Build(Nfa nfa)
    {
      Nfa nfa1 = new Nfa(nfa.m_tks, this.m_sub);
      nfa.AddEps((NfaNode) nfa1);
      nfa.AddEps(nfa.m_end);
      nfa1.m_end.AddEps((NfaNode) nfa);
    }
  }
}
