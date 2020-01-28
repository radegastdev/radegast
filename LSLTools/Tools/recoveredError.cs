// Decompiled with JetBrains decompiler
// Type: Tools.recoveredError
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class recoveredError : error
  {
    public recoveredError(Parser yyp, ParseStackEntry s)
      : base(yyp, s)
    {
    }

    public override string ToString()
    {
      return "Parse contained " + (object) this.yyps.m_symbols.erh.counter + " errors";
    }

    public override void ConcreteSyntaxTree()
    {
      Console.WriteLine(this.ToString());
      if (this.sym == null)
        return;
      this.sym.ConcreteSyntaxTree();
    }

    public override void Print()
    {
      Console.WriteLine(this.ToString());
      if (this.sym == null)
        return;
      this.sym.Print();
    }
  }
}
