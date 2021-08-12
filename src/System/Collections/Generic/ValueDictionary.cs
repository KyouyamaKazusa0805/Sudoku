using static System.Collections.Generic.InsertionBehavior;

namespace System.Collections.Generic
{
	/// <summary>
	/// Represents a collection of keys and values, which is similar with the type
	/// <see cref="Dictionary{TKey, TValue}"/>. Different with that type, this type is represented as a
	/// <see langword="ref struct"/>, which means you can't use the instance of this type outside any methods
	/// as the data member of the types unless it's also a <see langword="ref struct"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	/// <seealso cref="Dictionary{TKey, TValue}"/>
	public unsafe ref partial struct ValueDictionary<TKey, TValue> where TKey : unmanaged where TValue : unmanaged
	{
		/// <summary>
		/// Indicates the free list start value.
		/// </summary>
		private const int StartOfFreeList = -3;


		/// <summary>
		/// The inner counting variable.
		/// </summary>
		private int _count;

		/// <summary>
		/// Indicates the free list.
		/// </summary>
		private int _freeList;

		/// <summary>
		/// Indicates the free count.
		/// </summary>
		private int _freeCount;

		/// <summary>
		/// Indicates how many times the instance being modified.
		/// </summary>
		private int _version;

		/// <summary>
		/// The buckets that stores the inner dictionary key informations.
		/// </summary>
#if false
		[NotNullIfNotNull(nameof(_entries))]
#endif
		private int[]? _buckets;

		/// <summary>
		/// Indicates the fast modulo multiplier that is used for optimization the performance.
		/// </summary>
		private ulong _fastModMultiplier;

		/// <summary>
		/// All entries of the collection that stores the pair of key and value information.
		/// </summary>
		private Span<Entry> _entries;


		/// <summary>
		/// Initializes a <see cref="ValueDictionary{TKey, TValue}"/> instance with the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the <paramref name="capacity"/> is a negative value.
		/// </exception>
#if false
		[MemberNotNull(new[] { nameof(_buckets), nameof(_entries) })]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueDictionary(int capacity) : this()
		{
			switch (capacity)
			{
				case < 0:
				{
					throw new ArgumentOutOfRangeException(nameof(capacity));
				}
				case > 0:
				{
					Initialize(capacity);
					break;
				}
			}
		}

		/// <summary>
		/// Initializes a <see cref="ValueDictionary{TKey, TValue}"/> instance with the specified collection
		/// of <see cref="ValueTuple{T1, T2}"/> of <typeparamref name="TKey"/> and <typeparamref name="TValue"/>
		/// pair.
		/// </summary>
		/// <param name="collection">The collection.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueDictionary(IEnumerable<(TKey, TValue)> collection) : this(0) => AddRange(collection);


		/// <summary>
		/// Indicates the number of elements stored in this collection.
		/// </summary>
		public readonly int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _count - _freeCount;
		}

		/// <summary>
		/// Indicates the key collection.
		/// </summary>
		public readonly KeyCollection Keys
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new(this);
		}

		/// <summary>
		/// Indicates the value collection.
		/// </summary>
		public readonly ValueCollection Values
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new(this);
		}


		/// <summary>
		/// Gets or sets the specified value from/into the collection.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <value>The value you want to assign into.</value>
		/// <returns>The value of this key corresponded to.</returns>
		/// <exception cref="KeyNotFoundException">Throws when the specified key doesn't found.</exception>
		public TValue this[TKey key]
		{
			readonly get
			{
				fixed (TValue* valuePtr = &FindValue(key))
				{
					if (valuePtr != null)
					{
						return *valuePtr;
					}
				}

				throw new KeyNotFoundException($"The specified key '{nameof(key)}' isn't found.");
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => TryInsert(key, value, OverwriteExisting);
		}


		/// <summary>
		/// Checks the collection, whether the collection contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public readonly bool ContainsKey(TKey key)
		{
			fixed (TValue* valuePtr = &FindValue(key))
			{
				return valuePtr != null;
			}
		}

		/// <summary>
		/// Checks the collection, whether the collection contains the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public readonly bool ContainsValue(TValue value)
		{
			var entries = _entries;

			for (int i = 0; i < _count; i++)
			{
				if (entries[i].NextValue >= -1 && UnsafeConvert(entries[i].Value) == UnsafeConvert(value))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Copies the current collection into a new array of <see cref="ValueTuple{T1, T2}"/>s.
		/// </summary>
		/// <param name="array">The array of <see cref="ValueTuple{T1, T2}"/>s.</param>
		/// <param name="index">The index you want to copy as the start one.</param>
		/// <seealso cref="ValueTuple{T1, T2}"/>
		/// <exception cref="IndexOutOfRangeException">
		/// Throws when the specified argument <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="ArgumentException">Throws when the array is too small to store values.</exception>
		public readonly void CopyTo((TKey, TValue)[] array, int index)
		{
			if ((uint)index > (uint)array.Length)
			{
				throw new IndexOutOfRangeException("The specified index is out of range.");
			}

			if (array.Length - index < Count)
			{
				throw new ArgumentException("The array is too small to store values.");
			}

			int count = _count;
			var entries = _entries;
			for (int i = 0; i < count; i++)
			{
				if (entries[i].NextValue >= -1)
				{
					array[index++] = (entries[i].Key, entries[i].Value);
				}
			}
		}

		/// <summary>
		/// Try to get the value from the collection using the specified key as the entry point to visit that value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">
		/// The value found. If the return value is <see langword="false"/>, the value will be
		/// <see langword="default"/>(<typeparamref name="TValue"/>).
		/// </param>
		/// <returns>The <see cref="bool"/> result indicating whether the finding operation is successful.</returns>
		public readonly bool TryGetValue(TKey key, out TValue value)
		{
			fixed (TValue* valuePtr = &FindValue(key))
			{
				if (valuePtr != null)
				{
					value = *valuePtr;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the enumertor to iterate the collection.
		/// </summary>
		/// <returns>The enumerator.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Enumerator GetEnumerator() => new(this);

		/// <summary>
		/// Gets the bucket for using specified hash code.
		/// </summary>
		/// <param name="hashCode">The hash code.</param>
		/// <returns>
		/// The bucket result located. The return value is a <see langword="ref"/> <see cref="int"/> in order to
		/// help you use the value and re-assign the value.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly ref int GetBucket(uint hashCode)
		{
			int[] buckets = _buckets!;
			return ref buckets[HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)];
		}

		/// <summary>
		/// Finds the specified value, and returns the reference of the value in this collection, in order to
		/// allow you using <c><![CDATA[&FindValue(key)]]></c> syntax to get the pointer.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// The reference of the value. If don't found any satisfied result, the result will be the reference
		/// of <see langword="null"/>.
		/// </returns>
		private readonly ref TValue FindValue(TKey key)
		{
			ref var entry = ref *(Entry*)null;
			if (_buckets is not null)
			{
				uint hashCode = (uint)key.GetHashCode();
				int i = GetBucket(hashCode);
				var entries = _entries!;
				uint collisionCount = 0;

				// Value in _buckets is 1-based; subtract 1 from i.
				// We do it here so it fuses with the following conditional.
				i--;
				do
				{
					// Should be a while loop https://github.com/dotnet/runtime/issues/9422
					// Test in if to drop range check for following array access
					if ((uint)i >= (uint)entries.Length)
					{
						goto ReturnNotFound;
					}

					entry = ref entries[i];
					if (entry.HashCode == hashCode && UnsafeConvert(entry.Key) == UnsafeConvert(key))
					{
						goto ReturnFound;
					}

					i = entry.NextValue;

					collisionCount++;
				} while (collisionCount <= (uint)entries.Length);

				// The chain of entries forms a loop; which means a concurrent update has happened.
				// Break out of the loop and throw, rather than looping forever.
				goto ConcurrentOperation;
			}

			goto ReturnNotFound;


		ConcurrentOperation:
			throw new InvalidOperationException("The concurrent operation is not supported.");

		ReturnFound:
			ref var value = ref entry.Value;

		Return:
			return ref value;

		ReturnNotFound:
			value = ref *(TValue*)null;
			goto Return;
		}

		/// <summary>
		/// Add the specified key and value into the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(TKey key, TValue value) => TryInsert(key, value, ThrowOnExisting);

		/// <summary>
		/// Add a serial of pair of <typeparamref name="TKey"/> and <typeparamref name="TValue"/> instance
		/// into the collection.
		/// </summary>
		/// <param name="collection">
		/// The collection of pairs of type <typeparamref name="TKey"/> and <typeparamref name="TValue"/>.
		/// </param>
		public void AddRange(IEnumerable<(TKey, TValue)> collection)
		{
			foreach (var (key, value) in collection)
			{
				Add(key, value);
			}
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			int count = _count;
			if (count > 0)
			{
				fixed (int* pBucket = _buckets)
				{
					Unsafe.InitBlock(pBucket, 0, (uint)(sizeof(int) * _buckets!.Length));
				}

				(_count, _freeCount, _freeList) = (0, 0, -1);
				fixed (Entry* pEntry = _entries!)
				{
					Unsafe.InitBlock(pEntry, 0, (uint)(sizeof(Entry) * count));
				}
			}
		}

		/// <summary>
		/// Removes the value from the current collection using the specified key as the entry point
		/// to visit the corresponding value.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="bool"/> result indicating whether the operation is successful to remove.</returns>
		public bool Remove(TKey key)
		{
			// The overload 'Remove(TKey key, out TValue value)' is a copy of this method with one additional
			// statement to copy the value for entry being removed into the output parameter.
			// Code has been intentionally duplicated for performance reasons.
			if (_buckets is not null)
			{
				uint collisionCount = 0;
				uint hashCode = (uint)key.GetHashCode();
				ref int bucket = ref GetBucket(hashCode);
				var entries = _entries;
				int last = -1;
				int i = bucket - 1; // Value in buckets is 1-based.
				while (i >= 0)
				{
					ref var entry = ref entries[i];

					if (entry.HashCode == hashCode && UnsafeConvert(entry.Key) == UnsafeConvert(key))
					{
						if (last < 0)
						{
							bucket = entry.NextValue + 1; // Value in buckets is 1-based
						}
						else
						{
							entries[last].NextValue = entry.NextValue;
						}

						entry.NextValue = StartOfFreeList - _freeList;

						if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
						{
							entry.Key = default;
						}

						if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
						{
							entry.Value = default;
						}

						_freeList = i;
						_freeCount++;
						return true;
					}

					last = i;
					i = entry.NextValue;

					if (++collisionCount > (uint)entries.Length)
					{
						// The chain of entries forms a loop; which means a concurrent update has happened.
						// Break out of the loop and throw, rather than looping forever.
						throw new InvalidOperationException("The concurrent operation doesn't supported.");
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Removes the element from the current collection using the specified key as the entry point
		/// to visit the corresponding value, and returns the value from the <see langword="out"/>
		/// parameter <paramref name="value"/>.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <param name="value">
		/// The value. If failed to remove, the value will be
		/// <see langword="default"/>(<typeparamref name="TValue"/>).
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating whether the operation is successful to remove.</returns>
		public bool Remove(TKey key, out TValue value)
		{
			// This overload is a copy of the overload 'Remove(TKey key)' with one additional
			// statement to copy the value for entry being removed into the output parameter.
			// Code has been intentionally duplicated for performance reasons.
			if (_buckets is not null)
			{
				uint collisionCount = 0, hashCode = (uint)key.GetHashCode();
				ref int bucket = ref GetBucket(hashCode);
				var entries = _entries;
				int last = -1, i = bucket - 1; // Value in buckets is 1-based.
				while (i >= 0)
				{
					ref var entry = ref entries[i];

					if (entry.HashCode == hashCode && UnsafeConvert(entry.Key) == UnsafeConvert(key))
					{
						if (last < 0)
						{
							bucket = entry.NextValue + 1; // Value in buckets is 1-based.
						}
						else
						{
							entries[last].NextValue = entry.NextValue;
						}

						value = entry.Value;

						entry.NextValue = StartOfFreeList - _freeList;

						if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
						{
							entry.Key = default;
						}

						if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
						{
							entry.Value = default;
						}

						_freeList = i;
						_freeCount++;
						return true;
					}

					last = i;
					i = entry.NextValue;

					if (++collisionCount > (uint)entries.Length)
					{
						// The chain of entries forms a loop; which means a concurrent update has happened.
						// Break out of the loop and throw, rather than looping forever.
						throw new InvalidOperationException("The concurrent operation doesn't supported.");
					}
				}
			}

			value = default;
			return false;
		}

		/// <summary>
		/// Try to add the specified key and the value into the collection. If the collection contains the same
		/// key, the operation will be failed but don't throw exceptions.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>A <see cref="bool"/> result indicating whether the adding operation is successful.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryAdd(TKey key, TValue value) => TryInsert(key, value, None);

		/// <summary>
		/// Ensures that the dictionary can hold up to <paramref name="capacity"/> entries
		/// without any further expansion of its backing storage.
		/// </summary>
		/// <returns>The current capacity created.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the argument <paramref name="capacity"/> is a negative value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			int currentCapacity = _entries.Length;
			if (currentCapacity >= capacity)
			{
				return currentCapacity;
			}

			_version++;

			if (_buckets is null)
			{
				return Initialize(capacity);
			}

			int newSize = HashHelpers.GetPrime(capacity);
			Resize(newSize);
			return newSize;
		}

		/// <summary>
		/// Sets the capacity of this dictionary to what it would be if it had been originally
		/// initialized with all its entries.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method can be used to minimize the memory overhead
		/// once it is known that no new elements will be added.
		/// </para>
		/// <para>
		/// To allocate minimum size storage array, execute the following statements:
		/// <list type="bullet">
		/// <item><c>dictionary.Clear();</c></item>
		/// <item><c>dictionary.TrimExcess();</c></item>
		/// </list>
		/// </para>
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrimExcess() => TrimExcess(Count);

		/// <summary>
		/// Sets the capacity of this dictionary to hold up <paramref name="capacity"/> entries
		/// without any further expansion of its backing storage.
		/// </summary>
		/// <remarks>
		/// This method can be used to minimize the memory overhead
		/// once it is known that no new elements will be added.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when <paramref name="capacity"/> is a negative value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrimExcess(int capacity)
		{
			if (capacity < Count)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			int newSize = HashHelpers.GetPrime(capacity);
			var oldEntries = _entries;
			if (newSize >= oldEntries.Length)
			{
				return;
			}

			int oldCount = _count;
			_version++;
			Initialize(newSize);

			fixed (Entry* pEntry = oldEntries)
			{
				CopyEntries(pEntry, oldCount);
			}
		}

		/// <summary>
		/// Resizes the collection.
		/// </summary>
		[MemberNotNull(nameof(_entries))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Resize() => Resize(HashHelpers.ExpandPrime(_count));

		/// <summary>
		/// Resizes the collection with the specified size value and a <see cref="bool"/> value indicating
		/// whether the hash code will be re-calculated.
		/// </summary>
		/// <param name="newSize">The new size.</param>
		[MemberNotNull(nameof(_entries))]
		private void Resize(int newSize)
		{
			var entries = new Entry[newSize];

			int count = _count;
			fixed (Entry* pEntry = _entries, pNewEntry = entries)
			{
				Unsafe.CopyBlock(pNewEntry, pEntry, (uint)(sizeof(Entry) * count));
			}

			// Assign member variables after both arrays allocated to guard
			// against corruption from OOM if second fails.
			_buckets = new int[newSize];
			_fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
			for (int i = 0; i < count; i++)
			{
				if (entries[i].NextValue >= -1)
				{
					ref int bucket = ref GetBucket(entries[i].HashCode);
					entries[i].NextValue = bucket - 1; // Value in _buckets is 1-based.
					bucket = i + 1;
				}
			}

			_entries = entries;
		}

		/// <summary>
		/// Copies all entries of this collection into the new array specified as the parameter.
		/// </summary>
		/// <param name="entries">The entries stores the copied entries.</param>
		/// <param name="count">The count you want to copy.</param>
		private void CopyEntries(Entry* entries, int count)
		{
			var newEntries = _entries;
			int newCount = 0;
			for (int i = 0; i < count; i++)
			{
				uint hashCode = entries[i].HashCode;
				if (entries[i].NextValue >= -1)
				{
					ref var entry = ref newEntries![newCount];
					entry = entries[i];
					ref int bucket = ref GetBucket(hashCode);
					entry.NextValue = bucket - 1; // Value in _buckets is 1-based.
					bucket = newCount + 1;
					newCount++;
				}
			}

			_count = newCount;
			_freeCount = 0;
		}

		/// <summary>
		/// Try to insert the specified key and the value into the collection using the specified behavior.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		/// <param name="behavior">The behavior.</param>
		/// <returns>A <see cref="bool"/> result indicating whether the operaion is successful.</returns>
		/// <exception cref="ArgumentException">Throws when the duplicate keys encountered.</exception>
		/// <exception cref="InvalidOperationException">
		/// Throws when teh current operation is concurrent operation.
		/// </exception>
		private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
		{
			// NOTE: this method is mirrored in 'CollectionsMarshal.GetValueRefOrAddDefault' below.
			// If you make any changes here, make sure to keep that version in sync as well.
			if (_buckets is null)
			{
				Initialize(0);
			}

			var entries = _entries;

			uint hashCode = (uint)key.GetHashCode(), collisionCount = 0;
			ref int bucket = ref GetBucket(hashCode);
			int i = bucket - 1; // Value in _buckets is 1-based.

			while (true)
			{
				// Should be a while loop https://github.com/dotnet/runtime/issues/9422
				// Test uint in if rather than loop condition to drop range check for following array access.
				if ((uint)i >= (uint)entries.Length)
				{
					break;
				}

				if (entries[i].HashCode == hashCode && UnsafeConvert(entries[i].Key) == UnsafeConvert(key))
				{
					return behavior switch
					{
						OverwriteExisting => assign(ref entries[i], value),
						ThrowOnExisting => throw new ArgumentException("The key can't be added because of the duplicate.", nameof(key)),
						_ => false
					};


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static bool assign(ref Entry entry, TValue value)
					{
						entry.Value = value;
						return true;
					}
				}

				i = entries[i].NextValue;

				if (++collisionCount > (uint)entries.Length)
				{
					// The chain of entries forms a loop; which means a concurrent update has happened.
					// Break out of the loop and throw, rather than looping forever.
					throw new InvalidOperationException("The concurrent operation doesn't supported.");
				}
			}

			int index;
			if (_freeCount > 0)
			{
				index = _freeList;
				_freeList = StartOfFreeList - entries[_freeList].NextValue;
				_freeCount--;
			}
			else
			{
				int count = _count;
				if (count == entries.Length)
				{
					Resize();
					bucket = ref GetBucket(hashCode);
				}
				index = count;
				_count = count + 1;
				entries = _entries;
			}

			ref var entry = ref entries[index];
			entry.HashCode = hashCode;
			entry.NextValue = bucket - 1; // Value in '_buckets' is 1-based.
			entry.Key = key;
			entry.Value = value;
			bucket = index + 1; // Value in '_buckets' is 1-based.
			_version++;

			// Value types never rehash. Just returns true.
			return true;
		}

		/// <summary>
		/// Initializes the current collection as the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		/// <returns>The size of the collection.</returns>
		[MemberNotNull(new[] { nameof(_buckets), nameof(_entries) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int Initialize(int capacity)
		{
			int size = HashHelpers.GetPrime(capacity);
			int[] buckets = new int[size];
			var entries = new Entry[size];

			// Assign member variables after both arrays allocated to guard against corruption
			// from OOM if second fails.
			_freeList = -1;
			_fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
			_buckets = buckets;
			_entries = entries;

			return size;
		}


		/// <summary>
		/// Converts the current value into an <see cref="int"/> value.
		/// </summary>
		/// <returns>The result value.</returns>
		/// <typeparam name="TUnmanaged">The type of the value.</typeparam>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long UnsafeConvert<TUnmanaged>(TUnmanaged value) where TUnmanaged : unmanaged =>
			Unsafe.As<TUnmanaged, long>(ref value);
	}
}
