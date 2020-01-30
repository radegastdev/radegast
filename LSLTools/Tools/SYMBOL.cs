// Decompiled with JetBrains decompiler
// Type: Tools.SYMBOL
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class SYMBOL
  {
    public ObjectList kids = new ObjectList();
    public object m_dollar;
    public int pos;
    public Lexer yylx;
    public Parser yyps;

    protected SYMBOL()
    {
    }

    public SYMBOL(Lexer yyl)
    {
      this.yylx = yyl;
    }

    public SYMBOL(Parser yyp)
    {
      this.yyps = yyp;
      this.yylx = yyp.m_lexer;
    }

    public int Line
    {
      get
      {
        return this.yylx.sourceLineInfo(this.pos).lineNumber;
      }
    }

    public int Position
    {
      get
      {
        return this.yylx.sourceLineInfo(this.pos).rawCharPosition;
      }
    }

    public string Pos
    {
      get
      {
        return this.yylx.Saypos(this.pos);
      }
    }

    public object yylval
    {
      get
      {
        return this.m_dollar;
      }
      set
      {
        this.m_dollar = value;
      }
    }

    public virtual int yynum
    {
      get
      {
        return 0;
      }
    }

    public virtual bool IsTerminal()
    {
      return false;
    }

    public virtual bool IsAction()
    {
      return false;
    }

    public virtual bool IsCSymbol()
    {
      return false;
    }

    public YyParser yyact
    {
      get
      {
        if (this.yyps != null)
          return this.yyps.m_symbols;
        return (YyParser) null;
      }
    }

    public virtual bool Pass(YyParser syms, int snum, out ParserEntry entry)
    {
      ParsingInfo parsingInfo = (ParsingInfo) syms.symbolInfo[(object) this.yynum];
      if (parsingInfo == null)
      {
        string s = string.Format("No parsinginfo for symbol {0} {1}", (object) this.yyname, (object) this.yynum);
        syms.erh.Error((CSToolsException) new CSToolsFatalException(9, this.yylx, this.yyname, s));
      }
      bool flag = parsingInfo.m_parsetable.Contains((object) snum);
      entry = !flag ? (ParserEntry) null : (ParserEntry) parsingInfo.m_parsetable[(object) snum];
      return flag;
    }

    public virtual string yyname
    {
      get
      {
        return nameof (SYMBOL);
      }
    }

    public override string ToString()
    {
      return this.yyname;
    }

    public virtual bool Matches(string s)
    {
      return false;
    }

    public virtual void Print()
    {
      Console.WriteLine(this.ToString());
    }

    private void ConcreteSyntaxTree(string n)
    {
      if (this is error)
        Console.WriteLine(n + " " + this.ToString());
      else
        Console.WriteLine(n + "-" + this.ToString());
      int num = 0;
      foreach (SYMBOL kid in this.kids)
        kid.ConcreteSyntaxTree(n + (num++ != this.kids.Count - 1 ? " |" : "  "));
    }

    public virtual void ConcreteSyntaxTree()
    {
      this.ConcreteSyntaxTree("");
    }

    public static implicit operator int(SYMBOL s)
    {
      object dollar;
      for (; (dollar = s.m_dollar) is SYMBOL; s = (SYMBOL) dollar)
      {
        if (dollar == null)
          break;
      }
      try
      {
        return (int) dollar;
      }
      catch (Exception ex)
      {
        Console.WriteLine("attempt to convert from " + (object) s.m_dollar.GetType());
        throw ex;
      }
    }
  }
}
