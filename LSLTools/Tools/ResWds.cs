// Decompiled with JetBrains decompiler
// Type: Tools.ResWds
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System.Collections;

namespace Tools
{
  public class ResWds
  {
    public Hashtable m_wds = new Hashtable();
    public bool m_upper;

    public static ResWds New(TokensGen tks, string str)
    {
      ResWds resWds = new ResWds();
      str = str.Trim();
      if (str[0] == 'U')
      {
        resWds.m_upper = true;
        str = str.Substring(1).Trim();
      }
      if (str[0] == '{' && str[str.Length - 1] == '}')
      {
        str = str.Substring(1, str.Length - 2).Trim();
        string str1 = str;
        char[] chArray = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray))
        {
          string str3 = str2.Trim();
          string name = str3;
          int num = str3.IndexOf(' ');
          if (num > 0)
          {
            name = str3.Substring(num).Trim();
            str3 = str3.Substring(0, num);
          }
          resWds.m_wds[(object) str3] = (object) name;
          if (tks.m_tokens.tokens[(object) name] == null)
          {
            TokClassDef tokClassDef = new TokClassDef((GenBase) tks, name, "TOKEN");
            tks.m_outFile.WriteLine("//%{0}+{1}", (object) name, (object) tokClassDef.m_yynum);
            tks.m_outFile.Write("public class {0} : TOKEN", (object) name);
            tks.m_outFile.WriteLine("{ public override string yyname { get { return \"" + name + "\";}}");
            tks.m_outFile.WriteLine("public override int yynum { get { return " + (object) tokClassDef.m_yynum + "; }}");
            tks.m_outFile.WriteLine(" public " + name + "(Lexer yyl):base(yyl) {}}");
          }
        }
        return resWds;
      }
      tks.m_tokens.erh.Error(new CSToolsException(47, "bad ResWds element"));
      return (ResWds) null;
    }

    public void Check(Lexer yyl, ref TOKEN tok)
    {
      string str = tok.yytext;
      if (this.m_upper)
        str = str.ToUpper();
      object wd = this.m_wds[(object) str];
      if (wd == null)
        return;
      tok = (TOKEN) Tfactory.create((string) wd, yyl);
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new ResWds();
      ResWds resWds = (ResWds) o;
      if (s.Encode)
      {
        s.Serialise((object) resWds.m_upper);
        s.Serialise((object) resWds.m_wds);
        return (object) null;
      }
      resWds.m_upper = (bool) s.Deserialise();
      resWds.m_wds = (Hashtable) s.Deserialise();
      return (object) resWds;
    }
  }
}
