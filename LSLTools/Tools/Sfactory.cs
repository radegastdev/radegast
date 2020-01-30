// Decompiled with JetBrains decompiler
// Type: Tools.Sfactory
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class Sfactory
  {
    public Sfactory(YyParser syms, string cls_name, SCreator cr)
    {
      syms.types[(object) cls_name] = (object) cr;
    }

    public static object create(string cls_name, Parser yyp)
    {
      SCreator type1 = (SCreator) yyp.m_symbols.types[(object) cls_name];
      if (type1 == null)
        yyp.m_symbols.erh.Error(new CSToolsException(16, yyp.m_lexer, "no factory for {" + cls_name + ")"));
      try
      {
        return type1(yyp);
      }
      catch (CSToolsException ex)
      {
        yyp.m_symbols.erh.Error(ex);
      }
      catch (Exception ex)
      {
        yyp.m_symbols.erh.Error(new CSToolsException(17, yyp.m_lexer, string.Format("Create of {0} failed ({1})", (object) cls_name, (object) ex.Message)));
      }
      int length = cls_name.LastIndexOf('_');
      if (length > 0)
      {
        SCreator type2 = (SCreator) yyp.m_symbols.types[(object) cls_name.Substring(0, length)];
        if (type2 != null)
        {
          SYMBOL symbol = (SYMBOL) type2(yyp);
          symbol.m_dollar = (object) 0;
          return (object) symbol;
        }
      }
      return (object) null;
    }
  }
}
