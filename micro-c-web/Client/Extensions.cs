using System.Collections.ObjectModel;
using System;

namespace micro_c_web.Client
{
    public static class Extensions
    {
        public static void RemoveRange<T>(this ObservableCollection<T> collection, int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index must be greater than zero");
            }

            if (count < 0)
            {
                throw new IndexOutOfRangeException("Count must be positive");
            }

            if (collection.Count - index < count)
            {
                throw new IndexOutOfRangeException("Index + Count must be less than collection count");
            }

            if (count > 0)
            {
                for(int i = index + count - 1; i >= index; i--)
                {
                    collection.RemoveAt(i);
                }
            }
        }
    }
}
