using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
{
    #region Members

    [SerializeField]
    protected List<T> _array = new List<T>();

    #endregion Members

    #region Class Methods

    public ReorderableArray() : this(0) { }

    public ReorderableArray(int length)
    {
        _array = new List<T>(length);
    }

    public T this[int index]
    {
        get { return _array[index]; }
        set { _array[index] = value; }
    }

    public int Length
    {
        get { return _array.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public int Count
    {
        get { return _array.Count; }
    }

    public object Clone()
    {
        return new List<T>(_array);
    }

    public void CopyFrom(IEnumerable<T> value)
    {
        _array.Clear();
        _array.AddRange(value);
    }

    public bool Contains(T value)
    {
        return _array.Contains(value);
    }

    public int IndexOf(T value)
    {
        return _array.IndexOf(value);
    }

    public void Insert(int index, T item)
    {
        _array.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _array.RemoveAt(index);
    }

    public void Add(T item)
    {
        _array.Add(item);
    }

    public void Clear()
    {
        _array.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _array.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _array.Remove(item);
    }

    public T[] ToArray()
    {
        return _array.ToArray();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _array.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _array.GetEnumerator();
    }

    #endregion Class Methods
}