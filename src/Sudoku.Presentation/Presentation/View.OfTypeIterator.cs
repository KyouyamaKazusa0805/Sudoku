﻿#define ENHANCED_DRAWING_APIS
namespace Sudoku.Presentation;

partial class View
{
	/// <summary>
	/// Represents an enumerator that iterates for <typeparamref name="T"/>-typed instances.
	/// </summary>
	/// <typeparam name="T">The type of the element node.</typeparam>
	public ref struct OfTypeIterator<T> where T : ViewNode
	{
		/// <summary>
		/// The total number of elements.
		/// </summary>
		private readonly int _count;

		/// <summary>
		/// The internal enumerator.
		/// </summary>
		private List<ViewNode>.Enumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="OfTypeIterator{T}"/> instance via the specified list of nodes.
		/// </summary>
		/// <param name="view">The internal nodes.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal OfTypeIterator(View view)
		{
			_count = view._nodes.Count;
			_enumerator = view._nodes.GetEnumerator();
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public T Current { get; private set; } = default!;


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
		/// Creates an <see cref="OfTypeIterator{T}"/> instance.
		/// </summary>
		/// <returns>An <see cref="OfTypeIterator{T}"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeIterator<T> GetEnumerator() => this;

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

			return result.ToArray();
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
