// Decompiled with JetBrains decompiler
// Type: Tools.CommentList
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class CommentList
  {
    public int spos;
    public int len;
    public CommentList tail;

    public CommentList(int st, int ln, CommentList t)
    {
      this.spos = st;
      this.len = ln;
      this.tail = t;
    }
  }
}
