namespace System.Collections.Generic
{
	partial struct ValueDictionary<TKey, TValue>
	{
		/// <summary>
		/// Indicates the value collection that only iterates value set.
		/// </summary>
		public ref partial struct ValueCollection
		{
			/// <summary>
			/// Indicates the dictionary instance.
			/// </summary>
			private readonly ValueDictionary<TKey, TValue> _dictionary;


			/// <summary>
			/// Initializes an instance with the specified dictionary.
			/// </summary>
			/// <param name="dictionary">The dictionary.</param>
			public ValueCollection(in ValueDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;


			/// <summary>
			/// Indicates the number of keys in this collection.
			/// </summary>
			public int Count => _dictionary.Count;


			/// <summary>
			/// Copies the current collection into an array of <typeparamref name="TValue"/>s.
			/// </summary>
			/// <param name="array">The array.</param>
			/// <param name="index">The index.</param>
			/// <exception cref="IndexOutOfRangeException">
			/// Throws when the argument <paramref name="index"/> is out of range.
			/// </exception>
			/// <exception cref="ArgumentException">
			/// Throws when the <paramref name="array"/> is too small.
			/// </exception>
			public void CopyTo(TValue[] array, int index)
			{
				if ((uint)index > array.Length)
				{
					throw new IndexOutOfRangeException("The specified index is out of range.");
				}

				if (array.Length - index < _dictionary.Count)
				{
					throw new ArgumentException("The specified array is too smal to store values.");
				}

				int count = _dictionary._count;
				var entries = _dictionary._entries;
				for (int i = 0; i < count; i++)
				{
					if (entries![i].NextValue >= -1)
					{
						array[index++] = entries[i].Value;
					}
				}
			}

			/// <summary>
			/// Gets the enumerator that can iterates the value collection.
			/// </summary>
			/// <returns>The enumerator that can iterates the value collection.</returns>
			public Enumerator GetEnumerator() => new(_dictionary);
		}
	}
}
