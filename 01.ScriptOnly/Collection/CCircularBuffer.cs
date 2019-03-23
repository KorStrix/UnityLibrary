#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-04 오후 4:49:48
 *	기능 : 
 *	
 *	원본 코드
 *	http://geekswithblogs.net/blackrob/archive/2014/09/01/circular-buffer-in-c.aspx
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using NUnit.Framework;
using UnityEngine.TestTools;

public interface ICircularBuffer<T>
{
    int Count { get; }
    int Capacity { get; set; }
    T Enqueue(T item);
    T Dequeue();
    void Clear();
    T this[int index] { get; set; }
    int IndexOf(T item);
    void Insert(int index, T item);
    void RemoveAt(int index);
}

public class CCircularBuffer<T> : ICircularBuffer<T>, IEnumerable<T>
{
    private T[] _buffer;
    private int _head;
    private int _tail;
    private int _iCapacity;

    public CCircularBuffer(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException("capacity", "must be positive");
        _buffer = new T[capacity];
        _iCapacity = capacity;
        Reset();
    }

    public void Reset()
    {
        _head = _iCapacity - 1;
        _tail = 0;
    }

    public int Count { get; private set; }

    public int Capacity
    {
        get { return _buffer.Length; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "must be positive");

            if (value == _buffer.Length)
                return;

            var buffer = new T[value];
            var count = 0;
            while (Count > 0 && count < value)
                buffer[count++] = Dequeue();

            _buffer = buffer;
            Count = count;
            _head = count - 1;
            _tail = 0;
        }
    }

    public T Enqueue(T item)
    {
        _head = (_head + 1) % Capacity;
        var overwritten = _buffer[_head];
        _buffer[_head] = item;
        if (Count == Capacity)
            _tail = (_tail + 1) % Capacity;
        else
            ++Count;
        return overwritten;
    }

    public bool Enqueue(IEnumerable<T> pIterable)
    {
        IEnumerator<T> pIter = pIterable.GetEnumerator();
        while (pIter.MoveNext())
        {
            Enqueue(pIter.Current);
        }

        return Count > Capacity;
    }

    public bool Dequeue(out T dequeued)
    {
        if (Count == 0)
        {
            dequeued = default(T);
            return false;
        }

        dequeued = Dequeue();
        return true;
    }

    public T Dequeue()
    {
        if (Count == 0)
            return default(T);

        var dequeued = _buffer[_tail];
        _buffer[_tail] = default(T);
        _tail = (_tail + 1) % Capacity;
        --Count;

        return dequeued;
    }

    public T Peek(int iPeekDepth = 0)
    {
        if (Count == 0 || Count < iPeekDepth)
            return default(T);

        int iPeekTail = _tail;
        if (_tail + iPeekDepth >= Capacity)
            iPeekTail = 0;

        return _buffer[iPeekTail + iPeekDepth];
    }

    public bool Peek(out T peeked, int iPeekDepth = 0)
    {
        if (Count == 0 || Count < iPeekDepth + 1)
        {
            peeked = default(T);
            return false;
        }
        
        peeked = Peek(iPeekDepth);
        return true;
    }

    private LinkedList<T> _listTemp = new LinkedList<T>();
    public T[] Dequeue_OrNull(int iDequeueCount)
    {
        if (Count == 0)
            return null;

        _listTemp.Clear();
        for (int i = 0; i < iDequeueCount; i++)
        {
            T pDequeued;
            if (Dequeue(out pDequeued))
                _listTemp.AddLast(pDequeued);
            else
                break;
        }

        return _listTemp.ToArray(); ;
    }

    public T[] Peek_OrNull(int iPeekCount)
    {
        if (Count == 0)
            return null;

        _listTemp.Clear();
        for(int i = 0; i < iPeekCount; i++)
        {
            T pDequeued;
            if (Peek(out pDequeued, i))
                _listTemp.AddLast(pDequeued);
            else
                break;
        }

        return _listTemp.ToArray();
    }


    public void Clear()
    {
        _head = Capacity - 1;
        _tail = 0;
        Count = 0;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            return _buffer[(_tail + index) % Capacity];
        }
        set
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            _buffer[(_tail + index) % Capacity] = value;
        }
    }

    public int IndexOf(T item)
    {
        for (var i = 0; i < Count; ++i)
            if (Equals(item, this[i]))
                return i;
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException("index");

        if (Count == index)
            Enqueue(item);
        else
        {
            var last = this[Count - 1];
            for (var i = index; i < Count - 2; ++i)
                this[i + 1] = this[i];
            this[index] = item;
            Enqueue(last);
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException("index");

        for (var i = index; i > 0; --i)
            this[i] = this[i - 1];
        Dequeue();
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (Count == 0 || Capacity == 0)
            yield break;

        for (var i = 0; i < Count; ++i)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

#region Test

public class CCircularBuffer_Test
{
    [Test]
    public void TestOverwrite()
    {
        var buffer = new CCircularBuffer<long>(3);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(1, buffer.Enqueue(4));
        Assert.AreEqual(3, buffer.Count);
        Assert.AreEqual(2, buffer.Dequeue());
        Assert.AreEqual(3, buffer.Dequeue());
        Assert.AreEqual(4, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);
    }

    [Test]
    public void TestUnderwrite()
    {
        var buffer = new CCircularBuffer<long>(5);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(3, buffer.Count);
        Assert.AreEqual(1, buffer.Dequeue());
        Assert.AreEqual(2, buffer.Dequeue());
        Assert.AreEqual(3, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);
    }

    [Test]
    public void TestIncreaseCapacityWhenFull()
    {
        var buffer = new CCircularBuffer<long>(3);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(3, buffer.Count);
        buffer.Capacity = 4;
        Assert.AreEqual(3, buffer.Count);
        Assert.AreEqual(1, buffer.Dequeue());
        Assert.AreEqual(2, buffer.Dequeue());
        Assert.AreEqual(3, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);
    }

    [Test]
    public void TestDecreaseCapacityWhenFull()
    {
        var buffer = new CCircularBuffer<long>(3);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(3, buffer.Count);
        buffer.Capacity = 2;
        Assert.AreEqual(2, buffer.Count);
        Assert.AreEqual(1, buffer.Dequeue());
        Assert.AreEqual(2, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);
    }

    [Test]
    public void TestEnumerationWhenFull()
    {
        var buffer = new CCircularBuffer<long>(3);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        var i = 0;
        foreach (var value in buffer)
            Assert.AreEqual(++i, value);
        Assert.AreEqual(i, 3);
    }

    [Test]
    public void TestEnumerationWhenPartiallyFull()
    {
        var buffer = new CCircularBuffer<long>(3);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        var i = 0;
        foreach (var value in buffer)
            Assert.AreEqual(++i, value);
        Assert.AreEqual(i, 2);
    }

    [Test]
    public void TestEnumerationWhenEmpty()
    {
        var buffer = new CCircularBuffer<long>(3);
        foreach (var value in buffer)
            Assert.Fail("Unexpected Value: " + value);
    }

    [Test]
    public void TestRemoveAt()
    {
        var buffer = new CCircularBuffer<long>(5);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(default(long), buffer.Enqueue(4));
        Assert.AreEqual(default(long), buffer.Enqueue(5));
        buffer.RemoveAt(buffer.IndexOf(2));
        buffer.RemoveAt(buffer.IndexOf(4));
        Assert.AreEqual(3, buffer.Count);
        Assert.AreEqual(1, buffer.Dequeue());
        Assert.AreEqual(3, buffer.Dequeue());
        Assert.AreEqual(5, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));
        Assert.AreEqual(default(long), buffer.Enqueue(4));
        Assert.AreEqual(default(long), buffer.Enqueue(5));
        buffer.RemoveAt(buffer.IndexOf(1));
        buffer.RemoveAt(buffer.IndexOf(3));
        buffer.RemoveAt(buffer.IndexOf(5));
        Assert.AreEqual(2, buffer.Count);
        Assert.AreEqual(2, buffer.Dequeue());
        Assert.AreEqual(4, buffer.Dequeue());
        Assert.AreEqual(0, buffer.Count);

    }

    [Test]
    public void TestPeek()
    {
        var buffer = new CCircularBuffer<long>(5);
        Assert.AreEqual(default(long), buffer.Enqueue(1));
        Assert.AreEqual(default(long), buffer.Enqueue(2));
        Assert.AreEqual(default(long), buffer.Enqueue(3));

        Assert.AreEqual(1, buffer.Peek());

        long[] arrPeek = buffer.Peek_OrNull(4);
        Assert.AreEqual(1, arrPeek[0]);
        Assert.AreEqual(2, arrPeek[1]);
        Assert.AreEqual(3, arrPeek[2]);
        Assert.AreEqual(3, arrPeek.Length);
    }
}

#endregion