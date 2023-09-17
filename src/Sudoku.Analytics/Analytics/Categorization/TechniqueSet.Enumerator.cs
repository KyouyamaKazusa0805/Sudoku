using System.Collections;

namespace Sudoku.Analytics.Categorization;

partial struct TechniqueSet
{
	/// <summary>
	/// The enumerator that can iterate with <see cref="Technique"/> fields for a <see cref="TechniqueSet"/> instance.
	/// </summary>
	/// <param name="bits">The internal bits.</param>
	public ref struct Enumerator(BitArray bits)
	{
		/// <summary>
		/// The internal fields.
		/// </summary>
		private readonly BitArray _bits = bits;

		/// <summary>
		/// The total length.
		/// </summary>
		private readonly int _length = bits.Length;

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly Technique Current => (Technique)_currentIndex;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			for (_currentIndex++; _currentIndex < _length; _currentIndex++)
			{
				if (_bits[_currentIndex])
				{
					return true;
				}
			}

			return false;
		}
	}
}
