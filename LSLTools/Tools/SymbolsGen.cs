// Decompiled with JetBrains decompiler
// Type: Tools.SymbolsGen
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public abstract class SymbolsGen : GenBase
  {
    public bool m_lalrParser = true;
    public YyParser m_symbols = new YyParser();
    public ObjectList prods = new ObjectList();
    internal ObjectList actions = new ObjectList();
    public Lexer m_lexer;
    public int pno;
    public int m_trans;
    public int action;
    internal int action_num;
    public SymbolType stypes;
    internal int state;
    public SymbolSet lahead;

    protected SymbolsGen(ErrorHandler eh)
      : base(eh)
    {
    }

    public bool Find(CSymbol sym)
    {
      if (sym.yytext.Equals("Null") || sym.yytext[0] == '\'')
        return true;
      if (this.stypes == null)
        return false;
      return this.stypes._Find(sym.yytext) != null;
    }

    public abstract void ParserDirective();

    public abstract void Declare();

    public abstract void SetNamespace();

    public abstract void SetStartSymbol();

    public abstract void ClassDefinition(string s);

    public abstract void AssocType(Precedence.PrecType pt, int n);

    public abstract void CopySegment();

    public abstract void SimpleAction(ParserSimpleAction a);

    public abstract void OldAction(ParserOldAction a);
  }
}
