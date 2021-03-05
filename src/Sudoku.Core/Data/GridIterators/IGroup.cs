using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Define a list with a key.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <remarks>
	/// This interface is implemented totally same as <see cref="IGrouping{TKey, TValue}"/>.
	/// </remarks>
	/// <seealso cref="IGrouping{TKey, TElement}"/>
	public interface IGroup<out TKey, out TValue> : IEnumerable<TValue>
	{
		/// <summary>
		/// Indicates the key of the list.
		/// </summary>
		public TKey Key { get; }


		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
