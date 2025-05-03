using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseCode.Extensions
{
    [Serializable]
    public class IndexedList<T> : IEnumerable<T>
    {
        [SerializeField] private int currentIndex = 0;
        [SerializeField] private List<T> list = new List<T>();

        public int CurrentIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }

        public int Count => list.Count;

        public T GetCurrent()
        {
            if (IsValidIndex(currentIndex))
                return list[currentIndex];
    
            throw new IndexOutOfRangeException($"CurrentIndex {currentIndex} is out of bounds (Count: {Count}).");
        }

        public T GetNext()
        {
            int nextIndex = currentIndex + 1;
            if (IsValidIndex(nextIndex))
                return list[nextIndex];
    
            throw new IndexOutOfRangeException($"NextIndex {nextIndex} is out of bounds (Count: {Count}).");
        }

        public T GetIndex(int index)
        {
            if (IsValidIndex(index))
                return list[index];
    
            throw new IndexOutOfRangeException($"Requested index {index} is out of bounds (Count: {Count}).");
        }

        public bool MoveNext()
        {
            if (currentIndex < list.Count - 1)
            {
                currentIndex++;
                return true;
            }

            return false;
        }

        public bool IsLast()
        {
            return currentIndex >= list.Count - 1;
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < list.Count;
        }
        public void Add(T item) => list.Add(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public T this[int index] => list[index];
        public List<T> ToList() => list;

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}