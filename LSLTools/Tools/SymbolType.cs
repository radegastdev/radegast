// Decompiled with JetBrains decompiler
// Type: Tools.SymbolType
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class SymbolType
  {
    private string m_name;
    private SymbolType m_next;

    public SymbolType(SymbolsGen yyp, string name)
      : this(yyp, name, false)
    {
    }

    public SymbolType(SymbolsGen yyp, string name, bool defined)
    {
      Lexer lexer = yyp.m_lexer;
      int length = name.IndexOf("+");
      int num = 0;
      if (length > 0)
      {
        num = int.Parse(name.Substring(length + 1));
        if (num > yyp.LastSymbol)
          yyp.LastSymbol = num;
        name = name.Substring(0, length);
      }
      lexer.yytext = name;
      CSymbol csymbol1 = new CSymbol(yyp);
      if (num > 0)
        csymbol1.m_yynum = num;
      CSymbol csymbol2 = csymbol1.Resolve();
      if (defined)
        csymbol2.m_defined = true;
      this.m_name = name;
      this.m_next = yyp.stypes;
      yyp.stypes = this;
    }

    public SymbolType _Find(string name)
    {
      if (name.Equals(this.m_name))
        return this;
      if (this.m_next == null)
        return (SymbolType) null;
      return this.m_next._Find(name);
    }
  }
}
