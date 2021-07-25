namespace System.Collections.Generic
{
	partial struct ValueDictionary<TKey, TValue>
	{
		partial struct KeyCollection
		{
			/// <summary>
			/// Indicates the inner enumerator of this collection.
			/// </summary>
			public ref partial struct Enumerator
			{
				/// <summary>
				/// Indicates the version modified.
				/// </summary>
				private readonly int _version;

				/// <summary>
				/// Indicates the dictionary instance.
				/// </summary>
				private readonly ValueDictionary<TKey, TValue> _dictionary;


				/// <summary>
				/// Indicates the index.
				/// </summary>
				private int _index;


				/// <summary>
				/// Initializes an instance with the specified dictionary.
				/// </summary>
				/// <param name="dictionary">The dictionary.</param>
				public Enumerator(in ValueDictionary<TKey, TValue> dictionary)
				{
					_dictionary = dictionary;
					_version = dictionary._version;
					_index = 0;
					Current = default;
				}


				/// <summary>
				/// Indicates the current element that iterated.
				/// </summary>
				public TKey Current { get; private set; }


				/// <summary>
				/// Moves the iterator to the next element.
				/// </summary>
				/// <returns>
				/// A <see cref="bool"/> result indicating whether the moving operation is successful.
				/// </returns>
				/// <exception cref="InvalidOperationException">
				/// Throws when the version value is invalid.
				/// </exception>
				public bool MoveNext()
				{
					if (_version != _dictionary._version)
					{
						throw new InvalidOperationException("The version is invalid.");
					}

					while ((uint)_index < (uint)_dictionary._count)
					{
						ref var entry = ref _dictionary._entries![_index++];

						if (entry.NextValue >= -1)
						{
							Current = entry.Key;
							return true;
						}
					}

					_index = _dictionary._count + 1;
					Current = default;
					return false;
				}
			}
		}
	}
}
