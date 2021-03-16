using System;
using System.Collections;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.DocComments;
using Sudoku.Techniques;

namespace Sudoku.Generating
{
	/// <summary>
	/// Encapsulates a technique code filter that contains some of technique codes.
	/// </summary>
	public sealed class TechniqueCodeFilter : ICloneable<TechniqueCodeFilter>, IEnumerable<Technique>
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly BitArray _internalList = new(Enum.GetValues<Technique>().Length);


		/// <inheritdoc cref="DefaultConstructor"/>
		public TechniqueCodeFilter()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified technique codes.
		/// </summary>
		/// <param name="techniqueCodes">The technique codes.</param>
		public TechniqueCodeFilter(params Technique[] techniqueCodes)
		{
			foreach (var techniqueCode in techniqueCodes)
			{
				_internalList[(int)techniqueCode] = true;
				Count++;
			}
		}

		/// <summary>
		/// Initializes an instance with the specified bit array.
		/// </summary>
		/// <param name="bitArray">The bit array.</param>
		private TechniqueCodeFilter(BitArray bitArray)
		{
			_internalList = bitArray;
			Count = bitArray.GetCardinality();
		}


		/// <summary>
		/// The total number of techniques.
		/// </summary>
		public int Count { get; private set; }


		/// <summary>
		/// To add a technique code.
		/// </summary>
		/// <param name="techniqueCode">The technique code.</param>
		/// <returns>The current instance.</returns>
		public TechniqueCodeFilter Add(Technique techniqueCode)
		{
			if (!Contains(techniqueCode))
			{
				_internalList[(int)techniqueCode] = true;
				Count++;
			}

			return this;
		}

		/// <summary>
		/// Add a serial of technique codes to this list.
		/// </summary>
		/// <param name="techniqueCodes">The codes.</param>
		/// <returns>The current instance.</returns>
		public TechniqueCodeFilter AddRange(IEnumerable<Technique> techniqueCodes)
		{
			foreach (var techniqueCode in techniqueCodes)
			{
				Add(techniqueCode);
			}

			return this;
		}

		/// <summary>
		/// To remove a technique code.
		/// </summary>
		/// <param name="techniqueCode">The technique code.</param>
		/// <returns>The current instance.</returns>
		public TechniqueCodeFilter Remove(Technique techniqueCode)
		{
			if (Contains(techniqueCode))
			{
				_internalList[(int)techniqueCode] = false;
				Count--;
			}

			return this;
		}

		/// <summary>
		/// To determine whether the specified filter contains the technique.
		/// </summary>
		/// <param name="techniqueCode">The technique code to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool Contains(Technique techniqueCode) => _internalList[(int)techniqueCode];

		/// <inheritdoc/>
		public IEnumerator<Technique> GetEnumerator()
		{
			for (int i = 0, count = _internalList.Count; i < count; i++)
			{
				if (_internalList[i])
				{
					yield return (Technique)i;
				}
			}
		}

		/// <inheritdoc/>
		public TechniqueCodeFilter Clone() => new(_internalList.CloneAs<BitArray>());

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
