using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Classes
{
    /// <summary>
    /// A list that enforces a limit size. Adding an item once the limit is met will remove the last item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedList<T>
    {
        /// <summary>
        /// The capacity limit for the list. Must be at least 1. 
        /// </summary>
        public int Limit { get; }
        private List<T> _items;

        public List<T> Items
        {
            get
            {
                return _items;
            }
        }

        public LimitedList() : this(5)
        {
        }

        /// <summary>
        /// Initialize a limited list
        /// </summary>
        /// <param name="limit">Capacity of the list. Anything below 1 will be set to 1.</param>
        public LimitedList(int limit)
        {

            Limit = limit < 1 ? 1 : limit;
            _items = new List<T>(limit);
        }

        public void Add(T item)
        {
            if (_items.Count == Limit)
            {
                _items.RemoveAt(_items.Count - 1);
            }

            _items.Add(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count
        {
            get { return _items.Count; }
        }
    }
}
