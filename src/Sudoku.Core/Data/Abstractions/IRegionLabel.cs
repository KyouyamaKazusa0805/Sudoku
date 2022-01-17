namespace Sudoku.Data.Abstractions;

/// <summary>
/// Defines a basic constraint that applied onto a <see cref="RegionLabel"/>.
/// </summary>
/// <typeparam name="T">The type. The type is always <see cref="RegionLabel"/>.</typeparam>
internal interface IRegionLabel<[Self] T>
: IComparisonOperators<T, T>
, IEqualityOperators<T, T>
, IIncrementOperators<T>
where T : IRegionLabel<T>
{
	/// <summary>
	/// The block table.
	/// </summary>
	protected static readonly int[] BlockTable =
	{
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8
	};

	/// <summary>
	/// The row table.
	/// </summary>
	protected static readonly int[] RowTable =
	{
		 9,  9,  9,  9,  9,  9,  9,  9,  9,
		10, 10, 10, 10, 10, 10, 10, 10, 10,
		11, 11, 11, 11, 11, 11, 11, 11, 11,
		12, 12, 12, 12, 12, 12, 12, 12, 12,
		13, 13, 13, 13, 13, 13, 13, 13, 13,
		14, 14, 14, 14, 14, 14, 14, 14, 14,
		15, 15, 15, 15, 15, 15, 15, 15, 15,
		16, 16, 16, 16, 16, 16, 16, 16, 16,
		17, 17, 17, 17, 17, 17, 17, 17, 17
	};

	/// <summary>
	/// The column table.
	/// </summary>
	protected static readonly int[] ColumnTable =
	{
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26
	};


	/// <summary>
	/// The region kind.
	/// </summary>
	byte RegionKind { get; }


	/// <inheritdoc cref="object.ToString"/>
	string ToString();

	/// <inheritdoc cref="object.GetHashCode"/>
	int GetHashCode();


	/// <summary>
	/// Gets the row, column and block value and copies to the specified pointer.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="ptr">The pointer.</param>
	static abstract unsafe void ToRegion(int cell, int* ptr);

	/// <summary>
	/// Get the region index for the specified cell and the region type.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="label">The label to represent a region type.</param>
	/// <returns>The region index.</returns>
	static abstract int ToRegion(int cell, RegionLabel label);

	/// <summary>
	/// Get the label in the specified region.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The region label.</returns>
	static abstract RegionLabel ToLabel(int region);


	/// <summary>
	/// Explicit cast from <see cref="byte"/> to <typeparamref name="T"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	static abstract explicit operator T(byte value);

	/// <summary>
	/// Explicit cast from <typeparamref name="T"/> to <see cref="byte"/>.
	/// </summary>
	/// <param name="regionLabel">The region label.</param>
	static abstract explicit operator byte(T regionLabel);
}
