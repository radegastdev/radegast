// Decompiled with JetBrains decompiler
// Type: Tools.ReUStr
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.IO;

namespace Tools
{
  internal class ReUStr : ReStr
  {
    public ReUStr(TokensGen tks, string str)
    {
      this.m_str = str;
      for (int index = 0; index < str.Length; ++index)
      {
        tks.m_tokens.UsingChar(char.ToLower(str[index]));
        tks.m_tokens.UsingChar(char.ToUpper(str[index]));
      }
    }

    public ReUStr(TokensGen tks, char ch)
    {
      this.m_str = new string(ch, 1);
      tks.m_tokens.UsingChar(char.ToLower(ch));
      tks.m_tokens.UsingChar(char.ToUpper(ch));
    }

    public override void Print(TextWriter s)
    {
      s.Write(string.Format("(U\"{0}\")", (object) this.m_str));
    }

    public override int Match(string str, int pos, int max)
    {
      int length = this.m_str.Length;
      if (length > max || length > max - pos)
        return -1;
      for (int index = 0; index < length; ++index)
      {
        if ((int) char.ToUpper(str[index]) != (int) char.ToUpper(this.m_str[index]))
          return -1;
      }
      return length;
    }

    public override void Build(Nfa nfa)
    {
      int length = this.m_str.Length;
      NfaNode nfaNode = (NfaNode) nfa;
      for (int index = 0; index < length; ++index)
      {
        NfaNode next = new NfaNode(nfa.m_tks);
        nfaNode.AddUArc(this.m_str[index], next);
        nfaNode = next;
      }
      nfaNode.AddEps(nfa.m_end);
    }
  }
}
