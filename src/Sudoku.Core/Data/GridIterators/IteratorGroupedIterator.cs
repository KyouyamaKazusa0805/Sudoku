using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Provides a grouped iterator that is used in
	/// <see langword="let"/>-<see langword="group"/>-<see langword="by"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	internal sealed class IteratorGroupedIterator<TKey, TValue> : IIterator<IGroup<TKey, TValue>>
		where TKey : notnull
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<TValue>>.Enumerator _enumerator;

		/// <summary>
		/// Indicates the inner dictionary to group them up.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<TValue>> _innerDictionary;


		/// <summary>
		/// Initializes an instance with the specified enumerator and the key selector.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="keySelector">The key selecting method.</param>
		public IteratorGroupedIterator(
			IIterator<TValue> enumerator, Func<TValue, TKey> keySelector)
		{
			_innerDictionary = new Dictionary<TKey, IEnumerable<TValue>>();

			while (enumerator.MoveNext())
			{
				var value = enumerator.Current;
				var key = keySelector(value);
				if (!_innerDictionary.ContainsKey(key))
				{
					_innerDictionary.Add(key, new List<TValue> { value });
				}
				else
				{
					((List<TValue>)_innerDictionary[key]).Add(value);
				}
			}

			_enumerator = _innerDictionary.GetEnumerator();
		}


		/// <inheritdoc/>
		[NotNull]
		public IGroup<TKey, TValue>? Current
		{
			get
			{
				var (key, value) = _enumerator.Current;
				return new KeyValuePairUsedInHere<TKey, IEnumerable<TValue>, TValue>(key, value);
			}
		}


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}


	/// <summary>
	/// Provides a grouped iterator that is used in
	/// <see langword="let"/>-<see langword="group"/>-<see langword="by"/> clause in LINQ.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TConverted">The type that the value is converted.</typeparam>
	public sealed class IteratorGroupedIterator<TKey, TValue, TConverted> : IIterator<IGroup<TKey, TConverted>>
		where TKey : notnull
	{
		/// <summary>
		/// Indicates the enumerator.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<TValue>>.Enumerator _enumerator;

		/// <summary>
		/// Indicates the inner dictionary to group them up.
		/// </summary>
		private readonly Dictionary<TKey, IEnumerable<TValue>> _innerDictionary;

		/// <summary>
		/// Indicates the convert method.
		/// </summary>
		private readonly Func<TValue, TConverted> _converter;


		/// <summary>
		/// Initializes an instance with the specified enumerator and the key selector.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="keySelector">The key selecting method.</param>
		/// <param name="converter">The convert method.</param>
		public IteratorGroupedIterator(
			IIterator<TValue> enumerator, Func<TValue, TKey> keySelector, Func<TValue, TConverted> converter)
		{
			_converter = converter;
			_innerDictionary = new Dictionary<TKey, IEnumerable<TValue>>();

			while (enumerator.MoveNext())
			{
				var value = enumerator.Current;
				var key = keySelector(value);
				if (!_innerDictionary.ContainsKey(key))
				{
					_innerDictionary.Add(key, new List<TValue> { value });
				}
				else
				{
					((List<TValue>)_innerDictionary[key]).Add(value);
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
				return new KeyValuePairUsedInHere<TKey, IEnumerable<TValue>, TValue, TConverted>(
					key, value, _converter
				);
			}
		}


		/// <inheritdoc/>
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
