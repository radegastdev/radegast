// Decompiled with JetBrains decompiler
// Type: YYClass.cs0syntax
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class cs0syntax : Parser
  {
    public string Out;
    public string Cls;
    public string Par;
    public string Ctx;
    public bool defconseen;

    public cs0syntax()
      : base((YyParser) new yycs0syntax(), (Lexer) new cs0tokens())
    {
    }

    public cs0syntax(YyParser syms)
      : base(syms, (Lexer) new cs0tokens())
    {
    }

    public cs0syntax(YyParser syms, ErrorHandler erh)
      : base(syms, (Lexer) new cs0tokens(erh))
    {
    }
  }
}
