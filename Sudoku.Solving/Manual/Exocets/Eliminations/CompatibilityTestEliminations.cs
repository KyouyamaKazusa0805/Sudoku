#pragma warning disable CA1815

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
	/// Indicates the compatibility test eliminations.
	/// </summary>
	public struct CompatibilityTestEliminations : IEnumerable<Conclusion>
	{
		/// <inheritdoc cref="ExocetElimination(IList{ExocetElimination.Conclusion})"/>
		public CompatibilityTestEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


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
		public readonly CompatibilityTestEliminations Merge(params CompatibilityTestEliminations?[] eliminations)
		{
			var result = new CompatibilityTestEliminations();
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
		public override readonly string? ToString() =>
			Conclusions is null ? null : $"  * Compatibility test: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <inheritdoc cref="ExocetElimination.MergeAll(IEnumerable{ExocetElimination})"/>
		public static CompatibilityTestEliminations MergeAll(IEnumerable<CompatibilityTestEliminations> list)
		{
			var result = new CompatibilityTestEliminations();
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
		public static CompatibilityTestEliminations MergeAll(params CompatibilityTestEliminations[] list) =>
			MergeAll(list.AsEnumerable());
	}
}
