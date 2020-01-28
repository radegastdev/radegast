// Decompiled with JetBrains decompiler
// Type: Tools.CatTest
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Globalization;

namespace Tools
{
  public class CatTest
  {
    private UnicodeCategory cat;

    public CatTest(UnicodeCategory c)
    {
      this.cat = c;
    }

    public bool Test(char ch)
    {
      return char.GetUnicodeCategory(ch) == this.cat;
    }
  }
}
