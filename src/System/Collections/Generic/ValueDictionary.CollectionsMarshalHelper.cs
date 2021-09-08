namespace System.Collections.Generic;

partial struct ValueDictionary<TKey, TValue>
{
	/// <summary>
	/// <para>
	/// A helper class containing APIs exposed through <see cref="CollectionsMarshal"/>.
	/// </para>
	/// <para>
	/// These methods are relatively niche and only used in specific scenarios,
	/// so adding them in a separate type avoids the additional overhead on each
	/// <see cref="ValueDictionary{TKey, TValue}"/> instantiation, especially in AOT scenarios.
	/// </para>
	/// </summary>
	/// <seealso cref="CollectionsMarshal"/>
	private static class CollectionsMarshalHelper
	{
		/// <summary>
		/// Gets a reference to a <typeparamref name="TValue"/> in the
		/// <see cref="ValueDictionary{TKey, TValue}"/>, adding a new entry with a default value
		/// if it does not exist in the <paramref name="dictionary"/>.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary to get the ref to <typeparamref name="TValue"/> from.
		/// </param>
		/// <param name="key">The key used for lookup.</param>
		/// <param name="exists">
		/// Whether or not a new entry for the given key was added to the dictionary.
		/// </param>
		/// <remarks>
		/// Items should not be added to or removed from the <see cref="ValueDictionary{TKey, TValue}"/>
		/// while the ref <typeparamref name="TValue"/> is in use.
		/// </remarks>
		public static ref TValue GetValueRefOrAddDefault(
			ref ValueDictionary<TKey, TValue> dictionary,
			TKey key,
			out bool exists
		)
		{
			// NOTE: this method is mirrored by 'Dictionary<TKey, TValue>.TryInsert' above.
			// If you make any changes here, make sure to keep that version in sync as well.

			if (dictionary._buckets is null)
			{
				dictionary.Initialize(0);
			}

			var entries = dictionary._entries;
			uint hashCode = (uint)key.GetHashCode();

			uint collisionCount = 0;
			ref int bucket = ref dictionary.GetBucket(hashCode);
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
					exists = true;

					return ref entries[i].Value!;
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
			if (dictionary._freeCount > 0)
			{
				index = dictionary._freeList;
				dictionary._freeList = StartOfFreeList - entries[dictionary._freeList].NextValue;
				dictionary._freeCount--;
			}
			else
			{
				int count = dictionary._count;
				if (count == entries.Length)
				{
					dictionary.Resize();
					bucket = ref dictionary.GetBucket(hashCode);
				}
				index = count;
				dictionary._count = count + 1;
				entries = dictionary._entries;
			}

			ref var entry = ref entries[index];
			entry.HashCode = hashCode;
			entry.NextValue = bucket - 1; // Value in _buckets is 1-based.
			entry.Key = key;
			entry.Value = default;
			bucket = index + 1; // Value in _buckets is 1-based.
			dictionary._version++;

			// Value types never rehash. Just assigns the value false into 'exists'.
			exists = false;
			return ref entry.Value;
		}
	}
}
