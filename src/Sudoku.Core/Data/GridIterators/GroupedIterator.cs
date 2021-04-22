#if SUDOKU_GRID_LINQ

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Provides a grouped iterator that is used in <see langword="group"/>-<see langword="by"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	internal sealed class GroupedIterator<TKey> : IIterator<IGroup<TKey, int>> where TKey : notnull
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<int>>.Enumerator _enumerator;

		/// <summary>
		/// Indicates the inner dictionary to group them up.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<int>> _innerDictionary;


		/// <summary>
		/// Initializes an instance with the specified enumerator and the key selector.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="keySelector">The key selecting method.</param>
		public GroupedIterator(in Enumerator enumerator, Func<int, TKey> keySelector)
		{
			_innerDictionary = new Dictionary<TKey, IEnumerable<int>>();

			while (enumerator.MoveNext())
			{
				int value = enumerator.Current;
				var key = keySelector(value);
				if (!_innerDictionary.ContainsKey(key))
				{
					_innerDictionary.Add(key, new List<int> { value });
				}
				else
				{
					((List<int>)_innerDictionary[key]).Add(value);
				}
			}

			_enumerator = _innerDictionary.GetEnumerator();
		}


		/// <inheritdoc/>
		[NotNull]
		public IGroup<TKey, int>? Current
		{
			get
			{
				var (key, value) = _enumerator.Current;
				return new KeyValuePairUsedInHere<TKey, IEnumerable<int>, int>(key, value);
			}
		}


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}


	/// <summary>
	/// Provides a grouped iterator that is used in <see langword="group"/>-<see langword="by"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TConverted">The type of the value converted.</typeparam>
	public sealed class GroupedIterator<TKey, TConverted> : IIterator<IGroup<TKey, TConverted>> where TKey : notnull
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<int>>.Enumerator _enumerator;

		/// <summary>
		/// Indicates the inner dictionary to group them up.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<int>> _innerDictionary;

		/// <summary>
		/// Indicates the convert method.
		/// </summary>
		private readonly Func<int, TConverted> _converter;


		/// <summary>
		/// Initializes an instance with the specified enumerator and the key selector.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="keySelector">The key selecting method.</param>
		/// <param name="converter">The convert method.</param>
		public GroupedIterator(
			in Enumerator enumerator, Func<int, TKey> keySelector, Func<int, TConverted> converter)
		{
			_innerDictionary = new Dictionary<TKey, IEnumerable<int>>();
			_converter = converter;

			while (enumerator.MoveNext())
			{
				int value = enumerator.Current;
				var key = keySelector(value);
				if (!_innerDictionary.ContainsKey(key))
				{
					_innerDictionary.Add(key, new List<int> { value });
				}
				else
				{
					((List<int>)_innerDictionary[key]).Add(value);
				}
			}

			_enumerator = _innerDictionary.GetEnumerator();
		}


		/// <inheritdoc/>
		[NotNull]
		public IGroup<TKey, TConverted>? Current
		{
			get
			{
				var (key, value) = _enumerator.Current;
				return new KeyValuePairUsedInHere<TKey, IEnumerable<int>, int, TConverted>(key, value, _converter);
			}
		}


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}
}

#endif