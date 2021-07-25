namespace System.Collections.Generic
{
	partial struct ValueDictionary<TKey, TValue>
	{
		/// <summary>
		/// Defines an entry of the dictionary.
		/// </summary>
		private struct Entry
		{
			/// <summary>
			/// The hash code calculated.
			/// </summary>
			public uint HashCode;

			/// <summary>
			/// 0-based index of next entry in chain: -1 means end of chain
			/// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
			/// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
			/// </summary>
			public int NextValue;

			/// <summary>
			/// The key of the entry.
			/// </summary>
			public TKey Key;

			/// <summary>
			/// The value of the entry.
			/// </summary>
			public TValue Value;
		}
	}
}
