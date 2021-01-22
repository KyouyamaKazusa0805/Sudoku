using System;
using System.Collections.Generic;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Converts all elements to the target instances using the specified converter.
		/// </summary>
		/// <typeparam name="TConverted">The type of target element.</typeparam>
		/// <param name="converter">The converter.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public SelectIterator<TConverted> Select<TConverted>(Func<int, TConverted> converter) =>
			new(GetEnumerator(), converter);

		/// <summary>
		/// Checks all elements, and selects the values that satisfy the condition
		/// specified as a delegate method.
		/// </summary>
		/// <param name="condition">The condition method.</param>
		/// <returns>The iterator that iterates on each target element satisfying the condition.</returns>
		public WhereIterator Where(Predicate<int> condition) => new(GetEnumerator(), condition);

		/// <summary>
		/// Projects each element of a sequence to an <see cref="SelectIterator{T}"/>,
		/// flattens the resulting sequences into one sequence, and invokes a result selector function
		/// on each element therein.
		/// </summary>
		/// <typeparam name="T">The first type of the base elements.</typeparam>
		/// <typeparam name="TResult">The type of the projection result.</typeparam>
		/// <param name="collectionSelector">The collection selector.</param>
		/// <param name="resultSelector">The result selector.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IEnumerable<TResult> SelectMany<T, TResult>(
			Func<int, IEnumerable<T>> collectionSelector, Func<int, T, TResult> resultSelector)
		{
			foreach (int element in this)
			{
				foreach (var subElement in collectionSelector(element))
				{
					yield return resultSelector(element, subElement);
				}
			}
		}

		/// <summary>
		/// Projects each element of a sequence to an <see cref="SelectIterator{T}"/>,
		/// flattens the resulting sequences into one sequence, and invokes a result selector function
		/// on each element therein.
		/// </summary>
		/// <typeparam name="T">The first type of the base elements.</typeparam>
		/// <typeparam name="TResult">The type of the projection result.</typeparam>
		/// <param name="collectionSelector">The collection selector.</param>
		/// <param name="resultSelector">The result selector.</param>
		/// <returns>The iterator that iterates on each target element.</returns>
		public IEnumerable<TResult> SelectMany<T, TResult>(
			Func<int, SelectIterator<T>> collectionSelector, Func<int, T?, TResult> resultSelector)
		{
			foreach (int element in this)
			{
				foreach (var subElement in collectionSelector(element))
				{
					yield return resultSelector(element, subElement);
				}
			}
		}


		/// <summary>
		/// The iterator that used in the <see langword="select"/> clause in LINQ.
		/// </summary>
		/// <typeparam name="T">The type of the target elements.</typeparam>
		public struct SelectIterator<T>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly Enumerator _enumerator;

			/// <summary>
			/// Indicates the convert method.
			/// </summary>
			private readonly Func<int, T> _converter;


			/// <summary>
			/// Initializes an instance with the enumerator and the convert method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="converter">The convert method.</param>
			public SelectIterator(in Enumerator enumerator, Func<int, T> converter) : this()
			{
				_enumerator = enumerator;
				_converter = converter;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public T? Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					Current = _converter(_enumerator.Current);
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public SelectIterator<T> GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="where"/> clause in LINQ.
		/// </summary>
		public struct WhereIterator
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly Enumerator _enumerator;

			/// <summary>
			/// Indicates the condition.
			/// </summary>
			private readonly Predicate<int> _condition;


			/// <summary>
			/// Initializes an instance with the enumerator and the condition method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="condition">The condition method.</param>
			public WhereIterator(in Enumerator enumerator, Predicate<int> condition) : this()
			{
				_enumerator = enumerator;
				_condition = condition;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public int Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				while (_enumerator.MoveNext())
				{
					if (_condition(Current = _enumerator.Current))
					{
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public WhereIterator GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="let"/>-<see langword="select"/> clause in LINQ.
		/// </summary>
		/// <typeparam name="T">The type of the return value in the <see langword="let"/> clause.</typeparam>
		/// <typeparam name="TResult">The type of the target elements.</typeparam>
		public struct LetSelectIterator<T, TResult>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly SelectIterator<T> _enumerator;

			/// <summary>
			/// Indicates the convert method.
			/// </summary>
			private readonly Func<T?, TResult> _converter;


			/// <summary>
			/// Initializes an instance with the enumerator and the convert method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="converter">The convert method.</param>
			public LetSelectIterator(in SelectIterator<T> enumerator, Func<T?, TResult> converter) : this()
			{
				_enumerator = enumerator;
				_converter = converter;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public TResult? Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					Current = _converter(_enumerator.Current);
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public LetSelectIterator<T, TResult> GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="let"/>-<see langword="let"/>-<see langword="select"/>
		/// clause in LINQ.
		/// </summary>
		/// <typeparam name="T">The type of the return value in the <see langword="let"/> clause.</typeparam>
		/// <typeparam name="TAuxiliary">
		/// The type of the return value in the <see langword="let"/>-<see langword="let"/> clause.
		/// </typeparam>
		/// <typeparam name="TResult">The type of the target elements.</typeparam>
		public struct LetLetSelectIterator<T, TAuxiliary, TResult>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly LetSelectIterator<T, TAuxiliary> _enumerator;

			/// <summary>
			/// Indicates the convert method.
			/// </summary>
			private readonly Func<TAuxiliary?, TResult> _converter;


			/// <summary>
			/// Initializes an instance with the enumerator and the convert method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="converter">The convert method.</param>
			public LetLetSelectIterator(
				in LetSelectIterator<T, TAuxiliary> enumerator, Func<TAuxiliary?, TResult> converter)
				: this()
			{
				_enumerator = enumerator;
				_converter = converter;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public TResult? Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					Current = _converter(_enumerator.Current);
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public LetLetSelectIterator<T, TAuxiliary, TResult> GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="let"/>-<see langword="where"/> clause in LINQ.
		/// </summary>
		/// <typeparam name="T">The type of the return value in the <see langword="let"/> clause.</typeparam>
		public struct LetWhereIterator<T>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly SelectIterator<T> _enumerator;

			/// <summary>
			/// Indicates the condition.
			/// </summary>
			private readonly Predicate<T?> _condition;


			/// <summary>
			/// Initializes an instance with the enumerator and the condition method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="condition">The condition method.</param>
			public LetWhereIterator(in SelectIterator<T> enumerator, Predicate<T?> condition) : this()
			{
				_enumerator = enumerator;
				_condition = condition;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public T? Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				while (_enumerator.MoveNext())
				{
					if (_condition(Current = _enumerator.Current))
					{
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public LetWhereIterator<T> GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="let"/>-<see langword="where"/>-<see langword="select"/>
		/// clause in LINQ.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the return value in the <see langword="let"/> clause.
		/// </typeparam>
		/// <typeparam name="TResult">The type of the target elements.</typeparam>
		public struct LetWhereSelectIterator<T, TResult>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly LetWhereIterator<T?> _enumerator;

			private readonly Func<T?, TResult?> _converter;


			public LetWhereSelectIterator(
				in LetWhereIterator<T?> enumerator, Func<T?, TResult?> converter) : this()
			{
				_enumerator = enumerator;
				_converter = converter;
			}


			public TResult? Current { readonly get; private set; }


			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					Current = _converter(_enumerator.Current);
					return true;
				}
				else
				{
					return false;
				}
			}


			public LetWhereSelectIterator<T, TResult> GetEnumerator() => this;
		}

		/// <summary>
		/// The iterator that used in the <see langword="where"/>-<see langword="select"/> clause in LINQ.
		/// </summary>
		/// <typeparam name="T">The type of the target elements.</typeparam>
		public struct WhereSelectIterator<T>
		{
			/// <summary>
			/// The enumerator that iterates on all elements.
			/// </summary>
			private readonly WhereIterator _enumerator;

			/// <summary>
			/// Indicates the convert method.
			/// </summary>
			private readonly Func<int, T> _converter;


			/// <summary>
			/// Initializes an instance with the enumerator and the convert method.
			/// </summary>
			/// <param name="enumerator">(<see langword="in"/> parameter) The enumerator.</param>
			/// <param name="converter">The convert method.</param>
			public WhereSelectIterator(in WhereIterator enumerator, Func<int, T> converter) : this()
			{
				_enumerator = enumerator;
				_converter = converter;
			}


			/// <summary>
			/// Indicates the current element of this iteration.
			/// </summary>
			public T? Current { readonly get; private set; }


			/// <summary>
			/// Move to next element.
			/// </summary>
			/// <returns>A <see cref="bool"/> result indicating whether the iteration ends.</returns>
			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					Current = _converter(_enumerator.Current);
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Get the enumerator to iterate on each elements.
			/// </summary>
			/// <returns>The target enumerator.</returns>
			public WhereSelectIterator<T> GetEnumerator() => this;
		}
	}
}
