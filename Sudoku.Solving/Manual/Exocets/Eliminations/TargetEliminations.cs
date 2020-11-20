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
	/// Indicates the target eliminations.
	/// </summary>
	public struct TargetEliminations : IEnumerable<Conclusion>
	{
		/// <inheritdoc cref="ExocetElimination(IList{ExocetElimination.Conclusion})"/>
		public TargetEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <inheritdoc cref="ExocetElimination.Count"/>
		public readonly int Count => Conclusions?.Count ?? 0;

		/// <inheritdoc cref="ExocetElimination.Conclusions"/>
		public IList<Conclusion>? Conclusions { readonly get; private set; }


		/// <inheritdoc cref="ExocetElimination.Add(in ExocetElimination.Conclusion)"/>
		public void Add(in Conclusion conclusion) =>
			(Conclusions ??= new List<Conclusion>()).AddIfDoesNotContain(conclusion);

		/// <inheritdoc cref="ExocetElimination.AddRange(IEnumerable{ExocetElimination.Conclusion})"/>
		public void AddRange(IEnumerable<Conclusion> conclusions) =>
			(Conclusions ??= new List<Conclusion>()).AddRange(conclusions, true);

		/// <inheritdoc cref="ExocetElimination.Merge(ExocetElimination?[])"/>
		public readonly TargetEliminations Merge(params TargetEliminations?[] eliminations)
		{
			var result = new TargetEliminations();
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
			Conclusions is null
			? null
			: $"  * Target eliminations: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <inheritdoc cref="ExocetElimination.MergeAll(IEnumerable{ExocetElimination})"/>
		public static TargetEliminations MergeAll(IEnumerable<TargetEliminations> list)
		{
			var result = new TargetEliminations();
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
		public static TargetEliminations MergeAll(params TargetEliminations[] list) => MergeAll(list.AsEnumerable());
	}
}
