using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableList<T> : IEnumerable<T> // : ISerializationCallbackReceiver
{
    private List<T> _list = new List<T>();

    // Delegate most List methods to the internal list
    public void Add(T item)                         => _list.Add(item);
    public void Remove(T item)                      => _list.Remove(item);
    public void Clear()                             => _list.Clear();
    public bool Contains(T item)                    => _list.Contains(item);
    public int IndexOf(T item)                      => _list.IndexOf(item);
    public int Count                                => _list.Count;
    public void AddRange(IEnumerable<T> items)      => _list.AddRange(items);
    public List<T>  GetList()                       => _list;

    public void SetList(List<T> newList){
        _list = newList;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Allows indexing 'normally' (will go into inner list)
    public T this[int key]{
        get
        {
            return _list[key];
        }
        set
        {
            _list[key] = value;
        }
    }
}