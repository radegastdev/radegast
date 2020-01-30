// Decompiled with JetBrains decompiler
// Type: YYClass.BaseCall_6_1
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class BaseCall_6_1 : BaseCall_6
  {
    public BaseCall_6_1(Parser yyq)
      : base(yyq)
    {
      this.yytext = "this" + ((TOKEN) yyq.StackAt(1).m_value).yytext;
    }
  }
}
