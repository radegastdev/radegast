// Decompiled with JetBrains decompiler
// Type: Tools.SourceLineInfo
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

namespace Tools
{
  public class SourceLineInfo
  {
    public int lineNumber;
    public int charPosition;
    public int startOfLine;
    public int endOfLine;
    public int rawCharPosition;
    public Lexer lxr;

    public SourceLineInfo(int pos)
    {
      this.lineNumber = 1;
      this.startOfLine = 0;
      this.endOfLine = this.rawCharPosition = this.charPosition = pos;
    }

    public SourceLineInfo(LineManager lm, int pos)
    {
      this.lineNumber = lm.lines;
      this.startOfLine = 0;
      this.endOfLine = lm.end;
      this.charPosition = pos;
      this.rawCharPosition = pos;
      LineList lineList = lm.list;
      while (lineList != null)
      {
        if (lineList.head > pos)
        {
          this.endOfLine = lineList.head;
          lineList = lineList.tail;
          --this.lineNumber;
        }
        else
        {
          this.startOfLine = lineList.head + 1;
          this.rawCharPosition = lineList.getpos(pos);
          this.charPosition = pos - this.startOfLine + 1;
          break;
        }
      }
    }

    public SourceLineInfo(Lexer lx, int pos)
      : this(lx.m_LineManager, pos)
    {
      this.lxr = lx;
    }

    public override string ToString()
    {
      return "Line " + (object) this.lineNumber + ", char " + (object) this.rawCharPosition;
    }

    public string sourceLine
    {
      get
      {
        if (this.lxr == null)
          return "";
        return this.lxr.sourceLine(this);
      }
    }
  }
}
