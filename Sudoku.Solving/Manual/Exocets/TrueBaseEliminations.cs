using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Indicates the true base eliminations.
	/// </summary>
	public struct TrueBaseEliminations : IEnumerable<Conclusion>
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		public TrueBaseEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <summary>
		/// Indicates the number of all conclusions.
		/// </summary>
		public readonly int Count => Conclusions?.Count ?? 0;

		/// <summary>
		/// Indicates the conclusions.
		/// </summary>
		public IList<Conclusion>? Conclusions { readonly get; private set; }


		/// <summary>
		/// Add the conclusion into the collection.
		/// </summary>
		/// <param name="conclusion">The conclusion.</param>
		public void Add(Conclusion conclusion) =>
			(Conclusions ??= new List<Conclusion>()).AddIfDoesNotContain(conclusion);

		/// <summary>
		/// Add a serial of conclusions into this collection.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		public void AddRange(IEnumerable<Conclusion> conclusions) =>
			(Conclusions ??= new List<Conclusion>()).AddRange(conclusions, true);

		/// <summary>
		/// Merge all eliminations.
		/// </summary>
		/// <param name="eliminations">All instances to merge.</param>
		/// <returns>The merged result.</returns>
		public readonly TrueBaseEliminations Merge(params TrueBaseEliminations?[] eliminations)
		{
			var result = new TrueBaseEliminations();
			foreach (var instance in eliminations)
			{
				if (instance is null)
				{
					continue;
				}

				result.AddRange(instance);
			}

			return result;
		}

		/// <inheritdoc/>
		public readonly IEnumerator<Conclusion> GetEnumerator() =>
			(Conclusions ?? Array.Empty<Conclusion>()).GetEnumerator();


		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string? ToString() =>
			Conclusions is null ? null : $"  * True base eliminations: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Merge all conclusions.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <returns>The merged result.</returns>
		public static TrueBaseEliminations MergeAll(IEnumerable<TrueBaseEliminations> list)
		{
			var result = new TrueBaseEliminations();
			foreach (var z in list)
			{
				if (z.Conclusions is null)
				{
					continue;
				}

				result.AddRange(z);
			}

			return result;
		}

		/// <summary>
		/// Merge all conclusions.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <returns>The merged result.</returns>
		public static TrueBaseEliminations MergeAll(params TrueBaseEliminations[] list) =>
			MergeAll((IEnumerable<TrueBaseEliminations>)list);
	}
}
