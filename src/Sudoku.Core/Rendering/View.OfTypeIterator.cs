using System.Collections;
using System.Runtime.CompilerServices;

namespace Sudoku.Rendering;

partial class View
{
	/// <summary>
	/// Represents an enumerator that iterates for <typeparamref name="T"/>-typed instances.
	/// </summary>
	/// <typeparam name="T">The type of the element node.</typeparam>
	/// <param name="view">The internal nodes.</param>
	public ref struct OfTypeEnumerator<T>(View view) where T : ViewNode
	{
		/// <summary>
		/// The total number of elements.
		/// </summary>
		private readonly Count _count = view.Count;

		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private Enumerator _enumerator = view.GetEnumerator();


#nullable disable
		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public T Current { get; private set; }
#nullable restore


		/// <summary>
		/// Determines whether the collection of elements of type <typeparamref name="T"/> contains at least one element
		/// satisfying the specified condition.
		/// </summary>
		/// <param name="predicate">The condition.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public readonly bool Any(Predicate<T> predicate)
		{
			foreach (var element in this)
			{
				if (predicate(element))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Creates an <see cref="OfTypeEnumerator{T}"/> instance.
		/// </summary>
		/// <returns>An <see cref="OfTypeEnumerator{T}"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<T> GetEnumerator() => this;

		/// <summary>
		/// Casts the iterator, enumerating all elements and converting into an array.
		/// </summary>
		/// <returns>An array of elements.</returns>
		public readonly T[] ToArray()
		{
			var result = new List<T>(_count);
			foreach (var element in this)
			{
				result.Add(element);
			}

			return [.. result];
		}

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_enumerator.Current is T current)
				{
					Current = current;
					return true;
				}
			}

			return false;
		}
	}
}
