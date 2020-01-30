// Decompiled with JetBrains decompiler
// Type: Tools.Tfactory
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class Tfactory
  {
    public Tfactory(YyLexer tks, string cls_name, TCreator cr)
    {
      tks.types[(object) cls_name] = (object) cr;
    }

    public static object create(string cls_name, Lexer yyl)
    {
      TCreator type1 = (TCreator) yyl.tokens.types[(object) cls_name];
      if (type1 == null)
        yyl.tokens.erh.Error(new CSToolsException(6, yyl, cls_name, string.Format("no factory for {0}", (object) cls_name)));
      try
      {
        return type1(yyl);
      }
      catch (CSToolsException ex)
      {
        yyl.tokens.erh.Error(ex);
      }
      catch (Exception ex)
      {
        yyl.tokens.erh.Error(new CSToolsException(7, yyl, cls_name, string.Format("Line {0}: Create of {1} failed ({2})", (object) yyl.Saypos(yyl.m_pch), (object) cls_name, (object) ex.Message)));
      }
      int length = cls_name.LastIndexOf('_');
      if (length > 0)
      {
        TCreator type2 = (TCreator) yyl.tokens.types[(object) cls_name.Substring(0, length)];
        if (type2 != null)
          return type2(yyl);
      }
      return (object) null;
    }
  }
}
