using System;

namespace Gley.UrbanAssets.Internal
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}