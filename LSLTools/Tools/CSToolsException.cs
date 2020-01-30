// Decompiled with JetBrains decompiler
// Type: Tools.CSToolsException
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class CSToolsException : Exception
  {
    public int nExceptionNumber;
    public SourceLineInfo slInfo;
    public string sInput;
    public SYMBOL sym;
    public bool handled;

    public CSToolsException(int n, string s)
      : this(n, new SourceLineInfo(0), "", s)
    {
    }

    public CSToolsException(int n, Lexer yl, string s)
      : this(n, yl, yl.yytext, s)
    {
    }

    public CSToolsException(int n, Lexer yl, string yy, string s)
      : this(n, yl, yl.m_pch, yy, s)
    {
    }

    public CSToolsException(int n, TOKEN t, string s)
      : this(n, t.yylx, t.pos, t.yytext, s)
    {
      this.sym = (SYMBOL) t;
    }

    public CSToolsException(int n, SYMBOL t, string s)
      : this(n, t.yylx, t.pos, t.yyname, s)
    {
      this.sym = t;
    }

    public CSToolsException(int en, Lexer yl, int p, string y, string s)
      : this(en, yl.sourceLineInfo(p), y, s)
    {
    }

    public CSToolsException(int en, SourceLineInfo s, string y, string m)
      : base(s.ToString() + ": " + m)
    {
      this.nExceptionNumber = en;
      this.slInfo = s;
      this.sInput = y;
    }

    public virtual void Handle(ErrorHandler erh)
    {
      if (erh.throwExceptions)
        throw this;
      if (this.handled)
        return;
      this.handled = true;
      erh.Report(this);
    }
  }
}
