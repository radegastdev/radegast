// Decompiled with JetBrains decompiler
// Type: Tools.Literal
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class Literal : CSymbol
  {
    public Literal(SymbolsGen yyp)
      : base(yyp)
    {
      this.m_symtype = CSymbol.SymType.terminal;
    }

    private Literal()
    {
    }

    public override CSymbol Resolve()
    {
      int length = this.yytext.Length;
      string str = "";
      for (int index = 1; index + 1 < length; ++index)
      {
        if (this.yytext[index] == '\\')
        {
          if (index + 1 < length)
            ++index;
          if (this.yytext[index] >= '0' && this.yytext[index] <= '7')
          {
            int num;
            for (num = (int) this.yytext[index++] - 48; index < length && this.yytext[index] >= '0' && this.yytext[index] <= '7'; ++index)
              num = num * 8 + (int) this.yytext[index] - 48;
            str += (string) (object) (char) num;
          }
          else
          {
            char ch = this.yytext[index];
            switch (ch)
            {
              case 'r':
                str += (string) (object) '\r';
                continue;
              case 't':
                str += (string) (object) '\t';
                continue;
              default:
                str = ch == 'n' ? str + (object) '\n' : str + (object) this.yytext[index];
                continue;
            }
          }
        }
        else
          str += (string) (object) this.yytext[index];
      }
      this.yytext = str;
      CSymbol literal = (CSymbol) this.m_parser.m_symbols.literals[(object) this.yytext];
      if (literal != null)
        return literal;
      this.m_yynum = ++this.m_parser.LastSymbol;
      this.m_parser.m_symbols.literals[(object) this.yytext] = (object) this;
      this.m_parser.m_symbols.symbolInfo[(object) this.m_yynum] = (object) new ParsingInfo(this.yytext, this.m_yynum);
      return (CSymbol) this;
    }

    public bool CouldStart(CSymbol nonterm)
    {
      return false;
    }

    public override string TypeStr()
    {
      return "TOKEN";
    }

    public new static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new Literal();
      return CSymbol.Serialise(o, s);
    }
  }
}
