using System.Collections;
using System.Collections.Generic;
using System;

public class EventList<T> : IEnumerable<T> {

    public event Action onModified;

    private List<T> list;

    public EventList() {
        this.list = new List<T>();
    }

    public void Add(T item) {
        list.Add(item);
        if (onModified != null) {
            onModified();
        }
    }

    public void Remove(T item) {
        list.Remove(item);
        if (onModified != null) {
            onModified();
        }
    }

    public IEnumerator<T> GetEnumerator() {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return list.GetEnumerator();
    }
}
