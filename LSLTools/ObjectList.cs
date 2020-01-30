// Decompiled with JetBrains decompiler
// Type: ObjectList
// Assembly: Tools, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7664DE95-CB1F-45A9-9E49-805BE209CFAA

using System.Collections;

public class ObjectList
{
  private ObjectList.Link head;
  private ObjectList.Link last;
  private int count;

  private void Add0(ObjectList.Link a)
  {
    if (this.head == null)
      this.head = this.last = a;
    else
      this.last = this.last.next = a;
  }

  private object Get0(ObjectList.Link a, int x)
  {
    if (a == null || x < 0)
      return (object) null;
    if (x == 0)
      return a.it;
    return this.Get0(a.next, x - 1);
  }

  public void Add(object o)
  {
    this.Add0(new ObjectList.Link(o, (ObjectList.Link) null));
    ++this.count;
  }

  public void Push(object o)
  {
    this.head = new ObjectList.Link(o, this.head);
    ++this.count;
  }

  public object Pop()
  {
    object it = this.head.it;
    this.head = this.head.next;
    --this.count;
    return it;
  }

  public object Top
  {
    get
    {
      return this.head.it;
    }
  }

  public int Count
  {
    get
    {
      return this.count;
    }
  }

  public object this[int ix]
  {
    get
    {
      return this.Get0(this.head, ix);
    }
  }

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new ObjectList.OListEnumerator(this);
  }

  private class Link
  {
    internal object it;
    internal ObjectList.Link next;

    internal Link(object o, ObjectList.Link x)
    {
      this.it = o;
      this.next = x;
    }
  }

  public class OListEnumerator : IEnumerator
  {
    private ObjectList list;
    private ObjectList.Link cur;

    public OListEnumerator(ObjectList o)
    {
      this.list = o;
    }

    public object Current
    {
      get
      {
        return this.cur.it;
      }
    }

    public bool MoveNext()
    {
      this.cur = this.cur != null ? this.cur.next : this.list.head;
      return this.cur != null;
    }

    public void Reset()
    {
      this.cur = (ObjectList.Link) null;
    }
  }
}
