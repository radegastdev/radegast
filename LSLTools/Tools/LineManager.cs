// Decompiled with JetBrains decompiler
// Type: Tools.LineManager
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class LineManager
  {
    public int lines = 1;
    public int end;
    public LineList list;

    public void newline(int pos)
    {
      ++this.lines;
      this.backto(pos);
      this.list = new LineList(pos, this.list);
    }

    public void backto(int pos)
    {
      if (pos > this.end)
        this.end = pos;
      while (this.list != null && this.list.head >= pos)
      {
        this.list = this.list.tail;
        --this.lines;
      }
    }

    public void comment(int pos, int len)
    {
      if (pos > this.end)
        this.end = pos;
      if (this.list == null)
      {
        this.list = new LineList(0, this.list);
        this.lines = 1;
      }
      this.list.comments = new CommentList(pos, len, this.list.comments);
    }
  }
}
