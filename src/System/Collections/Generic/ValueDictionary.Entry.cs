namespace System.Collections.Generic;

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
		/// 0-based index of next entry in chain. The possible values are:
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <description>The description</description>
		/// </listheader>
		/// <item>
		/// <term>-1</term>
		/// <description>
		/// End of chain also encodes whether this entry <i>itself</i> is part of the free list
		/// by changing sign and subtracting 3.
		/// </description>
		/// </item>
		/// <item>
		/// <term>-2</term>
		/// <description>End of free list.</description>
		/// </item>
		/// <item>
		/// <term>-3</term>
		/// <description>Index 0 but on free list.</description>
		/// </item>
		/// <item>
		/// <term>-4</term>
		/// <description>Index 1 but on free list.</description>
		/// </item>
		/// </list>
		/// etc.
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
