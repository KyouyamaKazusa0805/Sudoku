using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates a technique code filter that contains some of technique codes.
	/// </summary>
	public sealed class TechniqueCodeFilter : ICloneable<TechniqueCodeFilter>, IEnumerable<TechniqueCode>
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly BitArray _internalList = new(EnumEx.LengthOf<TechniqueCode>());


		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public TechniqueCodeFilter()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified technique codes.
		/// </summary>
		/// <param name="techniqueCodes">(<see langword="params"/> parameter) The technique codes.</param>
		public TechniqueCodeFilter(params TechniqueCode[]? techniqueCodes)
		{
			if (techniqueCodes is null)
			{
				return;
			}

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
		public void Add(TechniqueCode techniqueCode)
		{
			if (!_internalList[(int)techniqueCode])
			{
				_internalList[(int)techniqueCode] = true;
				Count++;
			}
		}

		/// <summary>
		/// Add a serial of technique codes to this list.
		/// </summary>
		/// <param name="techniqueCodes">The codes.</param>
		public void AddRange(IEnumerable<TechniqueCode> techniqueCodes)
		{
			foreach (var techniqueCode in techniqueCodes)
			{
				Add(techniqueCode);
			}
		}

		/// <summary>
		/// To remove a technique code.
		/// </summary>
		/// <param name="techniqueCode">The technique code.</param>
		public void Remove(TechniqueCode techniqueCode)
		{
			if (_internalList[(int)techniqueCode])
			{
				_internalList[(int)techniqueCode] = false;
				Count--;
			}
		}

		/// <summary>
		/// To determine whether the specified filter contains the technique.
		/// </summary>
		/// <param name="techniqueCode">The technique code to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool Contains(TechniqueCode techniqueCode) => _internalList[(int)techniqueCode];

		/// <inheritdoc/>
		public IEnumerator<TechniqueCode> GetEnumerator()
		{
			for (int i = 0; i < _internalList.Count; i++)
			{
				if (_internalList[i])
				{
					yield return (TechniqueCode)i;
				}
			}
		}

		/// <inheritdoc/>
		public TechniqueCodeFilter Clone() => new(_internalList.CloneAs<BitArray>());

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
