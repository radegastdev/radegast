// Decompiled with JetBrains decompiler
// Type: Tools.LineList
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class LineList
  {
    public int head;
    public CommentList comments;
    public LineList tail;

    public LineList(int h, LineList t)
    {
      this.head = h;
      this.comments = (CommentList) null;
      this.tail = t;
    }

    public int getpos(int pos)
    {
      int num = pos - this.head;
      for (CommentList commentList = this.comments; commentList != null; commentList = commentList.tail)
      {
        if (pos > commentList.spos)
          num += commentList.len;
      }
      return num;
    }
  }
}
