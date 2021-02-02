// Decompiled with JetBrains decompiler
// Type: Tools.Charset
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Tools
{
  public class Charset
  {
    internal Hashtable m_chars = new Hashtable();
    internal UnicodeCategory m_cat;
    internal char m_generic;

    private Charset()
    {
    }

    internal Charset(UnicodeCategory cat)
    {
      this.m_cat = cat;
      this.m_generic = char.MinValue;
      while (char.GetUnicodeCategory(this.m_generic) != cat)
        ++this.m_generic;
      this.m_chars[(object) this.m_generic] = (object) true;
    }

    public static Encoding GetEncoding(string enc, ref bool toupper, ErrorHandler erh)
    {
      string str1 = enc;
      if (str1 != null)
      {
        string str2 = string.IsInterned(str1);
        if ((object) str2 == (object) "")
          return Encoding.Default;
        if ((object) str2 == (object) "ASCII")
          return Encoding.ASCII;
        if ((object) str2 == (object) "ASCIICAPS")
        {
          toupper = true;
          return Encoding.ASCII;
        }
        if ((object) str2 == (object) "UTF7")
          return Encoding.UTF7;
        if ((object) str2 == (object) "UTF8")
          return Encoding.UTF8;
        if ((object) str2 == (object) "Unicode")
          return Encoding.Unicode;
      }
      try
      {
        if (char.IsDigit(enc[0]))
          return Encoding.GetEncoding(int.Parse(enc));
        return Encoding.GetEncoding(enc);
      }
      catch (Exception)
      {
        erh.Error(new CSToolsException(43, "Warning: Encoding " + enc + " unknown: ignored"));
      }
      return Encoding.ASCII;
    }

    public static object Serialise(object o, Serialiser s)
    {
      if (s == null)
        return (object) new Charset();
      Charset charset = (Charset) o;
      if (s.Encode)
      {
        s.Serialise((object) (int) charset.m_cat);
        s.Serialise((object) charset.m_generic);
        s.Serialise((object) charset.m_chars);
        return (object) null;
      }
      charset.m_cat = (UnicodeCategory) s.Deserialise();
      charset.m_generic = (char) s.Deserialise();
      charset.m_chars = (Hashtable) s.Deserialise();
      return (object) charset;
    }
  }
}
