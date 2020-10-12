using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the true base eliminations.
	/// </summary>
	public struct TrueBaseEliminations : IEnumerable<Conclusion>
	{
		/// <inheritdoc cref="ExocetElimination(IList{ExocetElimination.Conclusion})"/>
		public TrueBaseEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <inheritdoc cref="ExocetElimination.Count"/>
		public readonly int Count => Conclusions?.Count ?? 0;

		/// <inheritdoc cref="ExocetElimination.Conclusions"/>
		public IList<Conclusion>? Conclusions { readonly get; private set; }


		/// <inheritdoc cref="ExocetElimination.Add(ExocetElimination.Conclusion)"/>
		public void Add(Conclusion conclusion) =>
			(Conclusions ??= new List<Conclusion>()).AddIfDoesNotContain(conclusion);

		/// <inheritdoc cref="ExocetElimination.AddRange(IEnumerable{ExocetElimination.Conclusion})"/>
		public void AddRange(IEnumerable<Conclusion> conclusions) =>
			(Conclusions ??= new List<Conclusion>()).AddRange(conclusions, true);

		/// <inheritdoc cref="ExocetElimination.Merge(ExocetElimination?[])"/>
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
		public readonly IEnumerator<Conclusion> GetEnumerator() => Conclusions.NullableCollection().GetEnumerator();


		/// <inheritdoc cref="object.ToString"/>
		public override readonly string? ToString()
		{
			switch (Conclusions)
			{
				case null:
				{
					return null;
				}
				default:
				{
					using var elims = new ConclusionCollection(Conclusions);
					return $"  * True base eliminations: { elims.ToString()}";
				}
			}
		}

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <inheritdoc cref="ExocetElimination.MergeAll(IEnumerable{ExocetElimination})"/>
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

		/// <inheritdoc cref="ExocetElimination.MergeAll(ExocetElimination[])"/>
		public static TrueBaseEliminations MergeAll(params TrueBaseEliminations[] list) =>
			MergeAll(list.AsEnumerable());
	}
}
