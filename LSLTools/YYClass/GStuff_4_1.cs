// Decompiled with JetBrains decompiler
// Type: YYClass.GStuff_4_1
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class GStuff_4_1 : GStuff_4
  {
    public GStuff_4_1(Parser yyq)
      : base(yyq)
    {
      this.yytext = ((TOKEN) yyq.StackAt(1).m_value).yytext + ((TOKEN) yyq.StackAt(0).m_value).yytext;
    }
  }
}
