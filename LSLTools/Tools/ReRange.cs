// Decompiled with JetBrains decompiler
// Type: Tools.ReRange
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Collections;
using System.IO;
using System.Text;

namespace Tools
{
  internal class ReRange : Regex
  {
    public Hashtable m_map = new Hashtable();
    public bool m_invert;

    public ReRange(TokensGen tks, string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = str.Length - 1;
      for (int index = 1; index < num1; ++index)
      {
        if (str[index] == '\\')
        {
          if (index + 1 < num1)
            ++index;
          if (str[index] >= '0' && str[index] <= '7')
          {
            int num2;
            for (num2 = (int) str[index++] - 48; index < num1 && str[index] >= '0' && str[index] <= '7'; ++index)
              num2 = num2 * 8 + (int) str[index] - 48;
            stringBuilder.Append((char) num2);
          }
          else
          {
            char ch = str[index];
            switch (ch)
            {
              case 'r':
                stringBuilder.Append('\r');
                continue;
              case 't':
                stringBuilder.Append('\t');
                continue;
              case 'v':
                stringBuilder.Append('\v');
                continue;
              default:
                if (ch == 'n')
                {
                  stringBuilder.Append('\n');
                  continue;
                }
                stringBuilder.Append(str[index]);
                continue;
            }
          }
        }
        else
          stringBuilder.Append(str[index]);
      }
      int length = stringBuilder.Length;
      if (length > 0 && stringBuilder[0] == '^')
      {
        this.m_invert = true;
        stringBuilder.Remove(0, 1).Append(char.MinValue).Append(char.MaxValue);
      }
      for (int index1 = 0; index1 < length; ++index1)
      {
        if (index1 + 1 < length && stringBuilder[index1 + 1] == '-')
        {
          for (int index2 = (int) stringBuilder[index1]; index2 <= (int) stringBuilder[index1 + 2]; ++index2)
            this.Set(tks, (char) index2);
          index1 += 2;
        }
        else
          this.Set(tks, stringBuilder[index1]);
      }
    }

    public override void Print(TextWriter s)
    {
      s.Write("[");
      if (this.m_invert)
        s.Write("^");
      foreach (char key in (IEnumerable) this.m_map.Keys)
        s.Write(key);
      s.Write("]");
    }

    private void Set(TokensGen tks, char ch)
    {
      this.m_map[(object) ch] = (object) true;
      tks.m_tokens.UsingChar(ch);
    }

    public override bool Match(char ch)
    {
      if (this.m_invert)
        return !this.m_map.Contains((object) ch);
      return this.m_map.Contains((object) ch);
    }

    public override int Match(string str, int pos, int max)
    {
      return max < pos || !this.Match(str[pos]) ? -1 : 1;
    }

    public override void Build(Nfa nfa)
    {
      nfa.AddArcEx((Regex) this, nfa.m_end);
    }
  }
}
