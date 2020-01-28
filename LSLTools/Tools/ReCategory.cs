// Decompiled with JetBrains decompiler
// Type: Tools.ReCategory
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReCategory : Regex
  {
    private string m_str;
    private ChTest m_test;

    public ReCategory(TokensGen tks, string str)
    {
      this.m_str = str;
      this.m_test = tks.m_tokens.GetTest(str);
    }

    public override bool Match(char ch)
    {
      return this.m_test(ch);
    }

    public override void Print(TextWriter s)
    {
      s.WriteLine("{" + this.m_str + "}");
    }

    public override void Build(Nfa nfa)
    {
      nfa.AddArcEx((Regex) this, nfa.m_end);
    }
  }
}
