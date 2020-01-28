// Decompiled with JetBrains decompiler
// Type: Tools.TokClassDef
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class TokClassDef
  {
    public string m_refToken = "";
    public string m_initialisation = "";
    public string m_implement = "";
    public string m_name = "";
    public int m_yynum;

    public TokClassDef(GenBase gbs, string name, string bas)
    {
      if (gbs is TokensGen)
      {
        TokensGen tokensGen = (TokensGen) gbs;
        this.m_name = name;
        tokensGen.m_tokens.tokens[(object) name] = (object) this;
        this.m_refToken = bas;
      }
      this.m_yynum = ++gbs.LastSymbol;
    }

    private TokClassDef()
    {
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new TokClassDef();
      TokClassDef tokClassDef = (TokClassDef) o;
      if (s.Encode)
      {
        s.Serialise((object) tokClassDef.m_name);
        s.Serialise((object) tokClassDef.m_yynum);
        return (object) null;
      }
      tokClassDef.m_name = (string) s.Deserialise();
      tokClassDef.m_yynum = (int) s.Deserialise();
      return (object) tokClassDef;
    }
  }
}
