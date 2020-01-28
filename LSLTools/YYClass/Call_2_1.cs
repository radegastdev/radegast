// Decompiled with JetBrains decompiler
// Type: YYClass.Call_2_1
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class Call_2_1 : Call_2
  {
    public Call_2_1(Parser yyq)
      : base(yyq)
    {
      if (((TOKEN) yyq.StackAt(3).m_value).yytext.Trim() != ((cs0syntax) yyq).Cls)
        this.yytext = ((TOKEN) yyq.StackAt(3).m_value).yytext + "(" + ((TOKEN) yyq.StackAt(1).m_value).yytext + ")";
      else if (((TOKEN) yyq.StackAt(1).m_value).yytext.Length == 0)
        this.yytext = ((TOKEN) yyq.StackAt(3).m_value).yytext + "(" + ((cs0syntax) yyq).Par + ")";
      else
        this.yytext = ((TOKEN) yyq.StackAt(3).m_value).yytext + "(" + ((cs0syntax) yyq).Par + "," + ((TOKEN) yyq.StackAt(1).m_value).yytext + ")";
    }
  }
}
