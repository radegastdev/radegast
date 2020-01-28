// Decompiled with JetBrains decompiler
// Type: YYClass.Cons_2_1
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class Cons_2_1 : Cons_2
  {
    public Cons_2_1(Parser yyq)
      : base(yyq)
    {
      cs0syntax cs0syntax = (cs0syntax) yyq;
      if (((TOKEN) yyq.StackAt(4).m_value).yytext.Trim() != cs0syntax.Cls)
      {
        this.yytext = ((TOKEN) yyq.StackAt(4).m_value).yytext + "(" + ((TOKEN) yyq.StackAt(2).m_value).yytext + ")";
      }
      else
      {
        if (((TOKEN) yyq.StackAt(2).m_value).yytext.Length == 0)
        {
          this.yytext = ((TOKEN) yyq.StackAt(4).m_value).yytext + "(" + cs0syntax.Ctx + ")";
          cs0syntax.defconseen = true;
        }
        else
          this.yytext = ((TOKEN) yyq.StackAt(4).m_value).yytext + "(" + cs0syntax.Ctx + "," + ((TOKEN) yyq.StackAt(2).m_value).yytext + ")";
        if (((TOKEN) yyq.StackAt(0).m_value).yytext.Length == 0)
        {
          Cons_2_1 cons21 = this;
          cons21.yytext = cons21.yytext + ":base(" + cs0syntax.Par + ")";
        }
        else
        {
          Cons_2_1 cons21 = this;
          cons21.yytext = cons21.yytext + ":" + ((TOKEN) yyq.StackAt(0).m_value).yytext.Substring(0, 4) + "(" + cs0syntax.Par + "," + ((TOKEN) yyq.StackAt(0).m_value).yytext.Substring(4) + ")";
        }
      }
    }
  }
}
