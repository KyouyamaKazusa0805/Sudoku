using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Extensions;

namespace System.Collections.Generic
{
	/// <summary>
	/// Indicates a set which contains the different elements.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	[DebuggerStepThrough]
	public sealed class Set<T> : IEnumerable<T>, IEquatable<Set<T>?>, ISet<T> where T : notnull, IEquatable<T>
	{
		/// <summary>
		/// The inner list.
		/// </summary>
		private readonly IList<T> _list;


		/// <include file='.....\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public Set() => _list = new List<T>();

		/// <summary>
		/// Add a series of elements.
		/// </summary>
		/// <param name="elements">The elements.</param>
		public Set(IEnumerable<T> elements) : this()
		{
			foreach (var element in elements)
			{
				Add(element);
			}
		}


		/// <summary>
		/// The number of elements.
		/// </summary>
		public int Count => _list.Count;

		/// <inheritdoc/>
		bool ICollection<T>.IsReadOnly => false;


		/// <summary>
		/// Get the first element that is equal to the specified parameter.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The first element to satisfy the condition.</returns>
		[MaybeNull]
		public T this[T element] => _list.FirstOrDefault(e => e.Equals(element));


		/// <inheritdoc/>
		public void Clear() => _list.Clear();

		/// <inheritdoc/>
		public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		/// <summary>
		/// Sort the list.
		/// </summary>
		public void Sort() => ((List<T>)_list).Sort();

		/// <summary>
		/// Sort the list with the specified comparsion.
		/// </summary>
		/// <param name="comparison">The comparsion.</param>
		public void Sort(Comparison<T> comparison) => ((List<T>)_list).Sort(comparison);

		/// <summary>
		/// Add an element into the set.
		/// </summary>
		/// <param name="item">The item.</param>
		public bool Add(T item)
		{
			if (_list.Contains(item))
			{
				return false;
			}

			_list.Add(item);
			return true;
		}

		/// <summary>
		/// Remove the last element out of the list.
		/// </summary>
		/// <returns>The element removed.</returns>
		public T Remove() => RemoveAt(^1);

		/// <summary>
		/// Remove the element at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The element removed.</returns>
		public T RemoveAt(int index)
		{
			var result = _list[index];
			_list.RemoveAt(index);
			return result;
		}

		/// <summary>
		/// Remove the element at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The element removed.</returns>
		public T RemoveAt(Index index)
		{
			var result = _list[index];
			_list.RemoveAt(index);
			return result;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Set<T> comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Set<T>? other) => SetEquals(this, other);

		/// <inheritdoc/>
		public bool Contains(T item) => _list.Contains(item);

		/// <inheritdoc/>
		public bool Remove(T item) => _list.Remove(item);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var hashCode = new HashCode();
			foreach (var element in _list)
			{
				hashCode.Add(element);
			}

			return hashCode.ToHashCode();
		}

		/// <inheritdoc/>
		public void ExceptWith(IEnumerable<T> other)
		{
			foreach (var element in new List<T>(_list)) // Should not use lonely '_list'.
			{
				if (other.Contains(element))
				{
					_list.Remove(element);
				}
			}
		}

		/// <inheritdoc/>
		public void IntersectWith(IEnumerable<T> other)
		{
			foreach (var element in new List<T>(_list)) // Should not use lonely '_list'.
			{
				if (!other.Contains(element))
				{
					_list.Remove(element);
				}
			}
		}

		/// <inheritdoc/>
		public bool IsProperSubsetOf(IEnumerable<T> other) => IsSubsetOf(other) && other.Count() != Count;

		/// <inheritdoc/>
		public bool IsProperSupersetOf(IEnumerable<T> other) => IsSupersetOf(other) && other.Count() != Count;

		/// <inheritdoc/>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			foreach (var element in _list)
			{
				if (!other.Contains(element))
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			foreach (var element in other)
			{
				if (!Contains(element))
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public bool Overlaps(IEnumerable<T> other) => other.Any(element => _list.Contains(element));

		/// <inheritdoc/>
		public bool SetEquals(IEnumerable<T> other) => other.All(element => _list.Contains(element));

		/// <inheritdoc/>
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			var elements = _list.Except(other).Union(other.Except(_list));
			_list.Clear();
			_list.AddRange(elements);
		}

		/// <inheritdoc/>
		public void UnionWith(IEnumerable<T> other)
		{
			foreach (var element in other)
			{
				_list.AddIfDoesNotContain(element);
			}
		}

		/// <inheritdoc/>
		public override string ToString() => $"Set (Count = {Count})";

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

		/// <inheritdoc/>
		void ICollection<T>.Add(T item) => Add(item);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// The internal equality determination.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static bool InternalEquals(Set<T>? left, Set<T>? right) =>
			(left, right) switch
			{
				(null, null) => true,
				(not null, not null) => SetEquals(left, right),
				_ => false
			};

		/// <summary>
		/// Determine whether two <see cref="Set{T}"/>s contain the same elements.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static bool SetEquals(Set<T>? left, Set<T>? right)
		{
			switch ((left is null, right is null))
			{
				case (true, true):
				{
					return true;
				}
				case (false, false):
				{
					foreach (var element in left!._list)
					{
						if (!right!._list.Contains(element))
						{
							return false;
						}
					}

					return true;
				}
				default:
				{
					return false;
				}
			}
		}


		/// <include file='.....\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Set<T>? left, Set<T>? right) => InternalEquals(left, right);

		/// <include file='.....\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Set<T>? left, Set<T>? right) => !(left == right);

		/// <summary>
		/// Calls the method <see cref="IntersectWith(IEnumerable{T})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The intersection result.</returns>
		/// <seealso cref="IntersectWith(IEnumerable{T})"/>
		public static Set<T> operator &(Set<T> left, IEnumerable<T> right)
		{
			left.IntersectWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="UnionWith(IEnumerable{T})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The union result.</returns>
		/// <seealso cref="UnionWith(IEnumerable{T})"/>
		public static Set<T> operator |(Set<T> left, IEnumerable<T> right)
		{
			left.UnionWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="SymmetricExceptWith(IEnumerable{T})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The symmetric exception result.</returns>
		/// <seealso cref="SymmetricExceptWith(IEnumerable{T})"/>
		public static Set<T> operator ^(Set<T> left, IEnumerable<T> right)
		{
			left.SymmetricExceptWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="ExceptWith(IEnumerable{T})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The exception result.</returns>
		/// <seealso cref="ExceptWith(IEnumerable{T})"/>
		public static Set<T> operator -(Set<T> left, IEnumerable<T> right)
		{
			left.ExceptWith(right);
			return left;
		}
	}
}
