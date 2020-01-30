// Decompiled with JetBrains decompiler
// Type: Tools.TOKEN
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class TOKEN : SYMBOL
  {
    private int num = 1;
    private string m_str;

    public TOKEN(Parser yyp)
      : base(yyp)
    {
    }

    public TOKEN(Lexer yyl)
      : base(yyl)
    {
      if (yyl == null)
        return;
      this.m_str = yyl.yytext;
    }

    public TOKEN(Lexer yyl, string s)
      : base(yyl)
    {
      this.m_str = s;
    }

    protected TOKEN()
    {
    }

    public string yytext
    {
      get
      {
        return this.m_str;
      }
      set
      {
        this.m_str = value;
      }
    }

    public override bool IsTerminal()
    {
      return true;
    }

    public override bool Pass(YyParser syms, int snum, out ParserEntry entry)
    {
      if (this.yynum == 1)
      {
        Literal literal = (Literal) syms.literals[(object) this.yytext];
        if (literal != null)
          this.num = literal.m_yynum;
      }
      ParsingInfo parsingInfo = (ParsingInfo) syms.symbolInfo[(object) this.yynum];
      if (parsingInfo == null)
      {
        string s = string.Format("Parser does not recognise literal {0}", (object) this.yytext);
        syms.erh.Error((CSToolsException) new CSToolsFatalException(9, this.yylx, this.yyname, s));
      }
      bool flag = parsingInfo.m_parsetable.Contains((object) snum);
      entry = !flag ? (ParserEntry) null : (ParserEntry) parsingInfo.m_parsetable[(object) snum];
      return flag;
    }

    public override string yyname
    {
      get
      {
        return nameof (TOKEN);
      }
    }

    public override int yynum
    {
      get
      {
        return this.num;
      }
    }

    public override bool Matches(string s)
    {
      return s.Equals(this.m_str);
    }

    public override string ToString()
    {
      return this.yyname + "<" + this.yytext + ">";
    }

    public override void Print()
    {
      Console.WriteLine(this.ToString());
    }
  }
}
