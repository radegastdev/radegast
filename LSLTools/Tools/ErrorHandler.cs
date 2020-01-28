// Decompiled with JetBrains decompiler
// Type: Tools.ErrorHandler
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA
// Assembly location: F:\Developer\radegast\Radegast\assemblies\Tools.dll

using System;

namespace Tools
{
  public class ErrorHandler
  {
    public int counter;
    public bool throwExceptions;

    public ErrorHandler()
    {
    }

    public ErrorHandler(bool ee)
    {
      this.throwExceptions = ee;
    }

    public virtual void Error(CSToolsException e)
    {
      ++this.counter;
      e.Handle(this);
    }

    public virtual void Report(CSToolsException e)
    {
      Console.WriteLine(e.Message);
    }
  }
}
