using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Indicates the mirror cell eliminations.
	/// </summary>
	public struct MirrorEliminations : IEnumerable<Conclusion>
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="IEliminations"]'/>
		public MirrorEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <include file='SolvingDocComments.xml' path='comments/property[@name="Count" and @type="IEliminations"]'/>
		public readonly int Count => Conclusions?.Count ?? 0;

		/// <include file='SolvingDocComments.xml' path='comments/property[@name="Conclusions" and @type="IEliminations"]'/>
		public IList<Conclusion>? Conclusions { readonly get; private set; }


		/// <include file='SolvingDocComments.xml' path='comments/method[@name="Add" and @type="IEliminations"]'/>
		public void Add(Conclusion conclusion) =>
			(Conclusions ??= new List<Conclusion>()).AddIfDoesNotContain(conclusion);

		/// <include file='SolvingDocComments.xml' path='comments/method[@name="AddRange" and @type="IEliminations"]'/>
		public void AddRange(IEnumerable<Conclusion> conclusions) =>
			(Conclusions ??= new List<Conclusion>()).AddRange(conclusions, true);

		/// <include file='SolvingDocComments.xml' path='comments/method[@name="Merge" and @type="IEliminations"]'/>
		public readonly MirrorEliminations Merge(params MirrorEliminations?[] eliminations)
		{
			var result = new MirrorEliminations();
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
			Conclusions is null ? null : $"  * Mirror eliminations: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='SolvingDocComments.xml' path='comments/method[@name="MergeAll" and @type="IEliminations"]'/>
		public static MirrorEliminations MergeAll(IEnumerable<MirrorEliminations> list)
		{
			var result = new MirrorEliminations();
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

		/// <include file='SolvingDocComments.xml' path='comments/method[@name="MergeAll" and @type="IEliminations"]'/>
		public static MirrorEliminations MergeAll(params MirrorEliminations[] list) =>
			MergeAll((IEnumerable<MirrorEliminations>)list);
	}
}
