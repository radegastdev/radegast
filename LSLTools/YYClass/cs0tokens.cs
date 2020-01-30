// Decompiled with JetBrains decompiler
// Type: YYClass.cs0tokens
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class cs0tokens : Lexer
  {
    public string Out;

    public cs0tokens()
      : base((YyLexer) new yycs0tokens(new ErrorHandler(false)))
    {
    }

    public cs0tokens(ErrorHandler eh)
      : base((YyLexer) new yycs0tokens(eh))
    {
    }

    public cs0tokens(YyLexer tks)
      : base(tks)
    {
    }
  }
}
