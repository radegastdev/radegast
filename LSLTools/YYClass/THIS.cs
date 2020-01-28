// Decompiled with JetBrains decompiler
// Type: YYClass.THIS
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using Tools;

namespace YYClass
{
  public class THIS : TOKEN
  {
    public THIS(Lexer yyl)
      : base(yyl)
    {
    }

    public override string yyname
    {
      get
      {
        return nameof (THIS);
      }
    }

    public override int yynum
    {
      get
      {
        return 4;
      }
    }
  }
}
