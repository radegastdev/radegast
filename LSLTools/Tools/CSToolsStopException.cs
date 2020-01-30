// Decompiled with JetBrains decompiler
// Type: Tools.CSToolsStopException
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class CSToolsStopException : CSToolsException
  {
    public CSToolsStopException(int n, string s)
      : base(n, s)
    {
    }

    public CSToolsStopException(int n, Lexer yl, string s)
      : base(n, yl, yl.yytext, s)
    {
    }

    public CSToolsStopException(int n, Lexer yl, string yy, string s)
      : base(n, yl, yl.m_pch, yy, s)
    {
    }

    public CSToolsStopException(int n, Lexer yl, int p, string y, string s)
      : base(n, yl, p, y, s)
    {
    }

    public CSToolsStopException(int n, TOKEN t, string s)
      : base(n, t, s)
    {
    }

    public CSToolsStopException(int n, SYMBOL t, string s)
      : base(n, t, s)
    {
    }

    public CSToolsStopException(int en, SourceLineInfo s, string y, string m)
      : base(en, s, y, m)
    {
    }

    public override void Handle(ErrorHandler erh)
    {
      throw this;
    }
  }
}
