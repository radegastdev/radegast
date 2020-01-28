// Decompiled with JetBrains decompiler
// Type: YYClass.Item_12_1
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class Item_12_1 : Item_12
  {
    public Item_12_1(Parser yyq)
      : base(yyq)
    {
      this.yytext = " this[" + ((TOKEN) yyq.StackAt(1).m_value).yytext + "]";
    }
  }
}
