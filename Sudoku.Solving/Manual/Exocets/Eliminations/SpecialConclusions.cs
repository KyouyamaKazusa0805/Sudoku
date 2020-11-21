using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using System.Linq;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Encapsulates the elimination instance.
	/// </summary>
	public abstract class SpecialConclusions : IEnumerable<Conclusion>
	{
		/// <summary>
		/// Indicates the conclusion list.
		/// </summary>
		private readonly Lazy<ISet<Conclusion>> _innerCollection = new(static () => new Set<Conclusion>());


		/// <summary>
		/// Indicates the number of conclusions in this collection.
		/// </summary>
		public int Count => Conclusions?.Count ?? 0;

		/// <summary>
		/// Indicates the elimination type name.
		/// </summary>
		public abstract string EliminationTypeName { get; }

		/// <summary>
		/// Indicates the conclusions list. If the collection is empty,
		/// the value will be <see langword="null"/>.
		/// </summary>
		public ISet<Conclusion>? Conclusions => _innerCollection.IsValueCreated ? _innerCollection.Value : null;


		/// <summary>
		/// Add the specified conclusion into the list.
		/// </summary>
		/// <param name="conclusion">(<see langword="in"/> parameter) The element.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in Conclusion conclusion) => _innerCollection.Value.Add(conclusion);

		/// <summary>
		/// Add a serial of conclusions.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		public void AddRange(IEnumerable<Conclusion> conclusions)
		{
			foreach (var conclusion in conclusions)
			{
				Add(conclusion);
			}
		}

		/// <inheritdoc/>
		public sealed override string? ToString() =>
			Conclusions is null
			? null
			: $"  * {EliminationTypeName} conclusions: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		public IEnumerator<Conclusion> GetEnumerator() =>
			_innerCollection.IsValueCreated
			? _innerCollection.Value.GetEnumerator()
			: Array.Empty<Conclusion>().Cast<Conclusion>().GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Merge the specified eliminations into the <paramref name="list"/> list, and returns the reference
		/// of <paramref name="list"/>.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="elims">The eliminations to merge.</param>
		/// <returns>The reference same as <paramref name="list"/>.</returns>
		public static SpecialConclusions operator |(SpecialConclusions list, SpecialConclusions elims)
		{
			list.AddRange(elims);
			return list;
		}
	}
}
