using System.Extensions;
using System.Linq;
using Sudoku.CodeGen.DelegatedEquality.Annotations;
using Sudoku.DocComments;

namespace System.Collections.Generic
{
	/// <summary>
	/// Indicates a set which contains the different elements.
	/// </summary>
	/// <typeparam name="TEquatable">The type of the element.</typeparam>
	public sealed partial class Set<TEquatable> : IEnumerable<TEquatable>, IEquatable<Set<TEquatable>>, ISet<TEquatable>
		where TEquatable : IEquatable<TEquatable>
	{
		/// <summary>
		/// The inner list.
		/// </summary>
		private readonly IList<TEquatable> _list = new List<TEquatable>();


		/// <inheritdoc cref="DefaultConstructor"/>
		public Set()
		{
		}

		/// <summary>
		/// Add a series of elements.
		/// </summary>
		/// <param name="elements">The elements.</param>
		public Set(IEnumerable<TEquatable> elements)
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
		bool ICollection<TEquatable>.IsReadOnly => false;


		/// <summary>
		/// Get the first element that is equal to the specified parameter.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The first element to satisfy the condition.</returns>
		public TEquatable? this[TEquatable element] => _list.FirstOrDefault(e => e.Equals(element));


		/// <inheritdoc/>
		public void Clear() => _list.Clear();

		/// <inheritdoc/>
		public void CopyTo(TEquatable[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		/// <summary>
		/// Sort the list.
		/// </summary>
		public void Sort() => ((List<TEquatable>)_list).Sort();

		/// <summary>
		/// Sort the list with the specified comparison.
		/// </summary>
		/// <param name="comparison">The comparison.</param>
		public void Sort(Comparison<TEquatable> comparison) => ((List<TEquatable>)_list).Sort(comparison);

		/// <summary>
		/// Add an element into the set.
		/// </summary>
		/// <param name="item">The item.</param>
		public bool Add(TEquatable item)
		{
			if (_list.Contains(item))
			{
				return false;
			}

			_list.Add(item);
			return true;
		}

		/// <summary>
		/// Get the instance in the collection that is same (or similar) as another one
		/// specified as the parameter.
		/// </summary>
		/// <param name="other">The value to compare.</param>
		/// <param name="result">The result.</param>
		/// <returns>Indicates whether the searching is successful.</returns>
		/// <remarks>
		/// Note that <paramref name="other"/> and <paramref name="result"/> aren't totally same.
		/// The comparison is decided by the implementation of its <c>Equals</c> method.
		/// </remarks>
		public bool TryGetValue(TEquatable other, out TEquatable? result)
		{
			foreach (var value in _list)
			{
				if (value.Equals(other))
				{
					result = value;
					return true;
				}
			}

			result = default;
			return false;
		}

		/// <summary>
		/// Remove the last element out of the list.
		/// </summary>
		/// <returns>The element removed.</returns>
		public TEquatable Remove() => RemoveAt(^1);

		/// <summary>
		/// Remove the element at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The element removed.</returns>
		public TEquatable RemoveAt(int index)
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
		public TEquatable RemoveAt(in Index index)
		{
			var result = _list[index];
			_list.RemoveAt(index.GetOffset(Count));
			return result;
		}

		/// <inheritdoc/>
		public bool Contains(TEquatable item) => _list.Contains(item);

		/// <inheritdoc/>
		public bool Remove(TEquatable item) => _list.Remove(item);

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
		public void ExceptWith(IEnumerable<TEquatable> other)
		{
			foreach (var element in new List<TEquatable>(_list)) // Should not use lonely '_list'.
			{
				if (other.Contains(element))
				{
					_list.Remove(element);
				}
			}
		}

		/// <inheritdoc/>
		public void IntersectWith(IEnumerable<TEquatable> other)
		{
			foreach (var element in new List<TEquatable>(_list)) // Should not use lonely '_list'.
			{
				if (!other.Contains(element))
				{
					_list.Remove(element);
				}
			}
		}

		/// <inheritdoc/>
		public bool IsProperSubsetOf(IEnumerable<TEquatable> other) =>
			IsSubsetOf(other) && other.Take(Count + 1).Count() != Count;

		/// <inheritdoc/>
		public bool IsProperSupersetOf(IEnumerable<TEquatable> other) =>
			IsSupersetOf(other) && other.Take(Count + 1).Count() != Count;

		/// <inheritdoc/>
		public bool IsSubsetOf(IEnumerable<TEquatable> other)
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
		public bool IsSupersetOf(IEnumerable<TEquatable> other)
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
		public bool Overlaps(IEnumerable<TEquatable> other) => other.Any(element => _list.Contains(element));

		/// <inheritdoc/>
		public bool SetEquals(IEnumerable<TEquatable> other) => other.All(element => _list.Contains(element));

		/// <inheritdoc/>
		public void SymmetricExceptWith(IEnumerable<TEquatable> other)
		{
			var elements = _list.Except(other).Union(other.Except(_list));
			_list.Clear();
			_list.AddRange(elements);
		}

		/// <inheritdoc/>
		public void UnionWith(IEnumerable<TEquatable> other)
		{
			foreach (var element in other)
			{
				if (!_list.Contains(element))
				{
					_list.Add(element);
				}
			}
		}

		/// <inheritdoc/>
		public override string ToString() => $"Set (Count = {Count.ToString()})";

		/// <inheritdoc/>
		public IEnumerator<TEquatable> GetEnumerator() => _list.GetEnumerator();

		/// <inheritdoc/>
		void ICollection<TEquatable>.Add(TEquatable item) => Add(item);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// The internal equality determination.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[DelegatedEqualityMethod]
		private static bool InternalEquals(Set<TEquatable>? left, Set<TEquatable>? right) => (left, right) switch
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
		private static bool SetEquals(Set<TEquatable>? left, Set<TEquatable>? right)
		{
			switch ((left, right))
			{
				case (null, null):
				{
					return true;
				}
				case (not null, not null):
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


		/// <summary>
		/// Calls the method <see cref="IntersectWith(IEnumerable{TEquatable})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The intersection result.</returns>
		/// <seealso cref="IntersectWith(IEnumerable{TEquatable})"/>
		public static Set<TEquatable> operator &(Set<TEquatable> left, IEnumerable<TEquatable> right)
		{
			left.IntersectWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="UnionWith(IEnumerable{TEquatable})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The union result.</returns>
		/// <seealso cref="UnionWith(IEnumerable{TEquatable})"/>
		public static Set<TEquatable> operator |(Set<TEquatable> left, IEnumerable<TEquatable> right)
		{
			left.UnionWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="SymmetricExceptWith(IEnumerable{TEquatable})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The symmetric exception result.</returns>
		/// <seealso cref="SymmetricExceptWith(IEnumerable{TEquatable})"/>
		public static Set<TEquatable> operator ^(Set<TEquatable> left, IEnumerable<TEquatable> right)
		{
			left.SymmetricExceptWith(right);
			return left;
		}

		/// <summary>
		/// Calls the method <see cref="ExceptWith(IEnumerable{TEquatable})"/>, and returns the
		/// reference of the <paramref name="left"/> parameter.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The exception result.</returns>
		/// <seealso cref="ExceptWith(IEnumerable{TEquatable})"/>
		public static Set<TEquatable> operator -(Set<TEquatable> left, IEnumerable<TEquatable> right)
		{
			left.ExceptWith(right);
			return left;
		}
	}
}
