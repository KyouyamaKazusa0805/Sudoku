namespace Sudoku.Concepts.Specialized;

/// <summary>
/// Represents a graph that displays a list of <see cref="Cell"/> instances connected with rows and columns of adjacent positions.
/// </summary>
/// <seealso cref="Cell"/>
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.EqualityOperators | TypeImplFlags.Equatable)]
public readonly ref partial struct AdjacentCellGraph :
	IEquatable<AdjacentCellGraph>,
	//IEqualityOperators<AdjacentCellGraph, AdjacentCellGraph, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates the backing field of cells.
	/// </summary>
	[EquatableMember]
	public readonly ref readonly CellMap Cells;


	[HashCodeMember]
	private int HashCode => Cells.GetHashCode();


	/// <summary>
	/// Creates an <see cref="AdjacentCellGraph"/> instance via a list of cells connected by rows and columns.
	/// </summary>
	/// <param name="cells">A list of cells.</param>
	/// <returns>An <see cref="AdjacentCellGraph"/> instance created.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when at least one cell is missing connection with all rows and columns of the other cells.
	/// </exception>
	public AdjacentCellGraph(ref readonly CellMap cells)
	{
		if (!verify(in cells))
		{
			throw new ArgumentException(SR.ExceptionMessage("AllCellsMustBeConnectedWithAdjacentPositions"), nameof(cells));
		}
		Cells = ref cells;


		static bool verify(ref readonly CellMap cells)
		{
			// If the collection has no elements, return true to tell the runtime that it is expected and valid.
			if (!cells)
			{
				return true;
			}

			// Then we should recursively check for the last cells.
			var lastCells = cells;
			while (lastCells)
			{
				foreach (var cell in cells)
				{
					lastCells &= ~PeersMap[cell];
					if (!verify(in lastCells))
					{
						return false;
					}
				}
			}
			return true;
		}
	}


	/// <summary>
	/// Try to get a list of cells and its corresponding directions that inner border lines will be created.
	/// </summary>
	public ReadOnlySpan<(Cell Cell, AdjacentCellDirection Directions)> InnerBorderLines
	{
		get
		{
			// The basic algorithm is to find connected directions, and removed from the initial collection.
			// The initial collection should make all cells as marked with all 4 directions.
			var dictionary = new Dictionary<Cell, AdjacentCellDirection>(Cells.Count);
			foreach (var cell in Cells)
			{
				dictionary.Add(cell, AdjacentCellDirections.All);
			}

			foreach (var cell in Cells)
			{
				if (cell is not (>= 0 and < 9) && Cells.Contains(cell - 9)) { dictionary[cell] &= ~AdjacentCellDirection.Up; }
				if (cell is not (>= 72 and < 81) && Cells.Contains(cell + 9)) { dictionary[cell] &= ~AdjacentCellDirection.Down; }
				if (cell % 9 != 0 && Cells.Contains(cell - 1)) { dictionary[cell] &= ~AdjacentCellDirection.Left; }
				if ((cell + 1) % 9 != 0 && Cells.Contains(cell + 1)) { dictionary[cell] &= ~AdjacentCellDirection.Right; }
			}

			var result = new List<(Cell, AdjacentCellDirection)>(Cells.Count);
			foreach (var cell in Cells)
			{
				result.Add((cell, dictionary[cell]));
			}
			return result.AsSpan();
		}
	}


	/// <inheritdoc/>
	public override int GetHashCode() => Cells.GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => Cells.ToString(formatProvider);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
