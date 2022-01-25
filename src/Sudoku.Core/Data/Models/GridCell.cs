namespace Sudoku.Data.Models;

/// <summary>
/// Defines a cell information in a sudoku grid.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct CellInfo :
	IEqualityOperators<CellInfo, CellInfo>,
	IEquatable<CellInfo>,
	IValueEquatable<CellInfo>
{
	/// <summary>
	/// Creates a <see cref="CellInfo"/> instance via the cell and the candidates list.
	/// </summary>
	/// <param name="cell">The current cell.</param>
	/// <param name="candidatesMask">The candidates.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidatesMask"/> is below 0 (or 0) or greater than 511.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellInfo(byte cell, short candidatesMask)
	{
		if (candidatesMask is <= 0 or > 511)
		{
			throw new ArgumentOutOfRangeException(nameof(candidatesMask));
		}

		Unsafe.SkipInit(out this);
		Cell = cell;
		CandidatesList = candidatesMask;
		Status = CellStatus.Empty;
	}

	/// <summary>
	/// Creates a <see cref="CellInfo"/> instance via the cell, digit and the current status.
	/// </summary>
	/// <param name="cell">Indicates the current cell.</param>
	/// <param name="digit">Indicates the current digit.</param>
	/// <param name="status">
	/// Indicates the current status.
	/// The value must be <see cref="CellStatus.Modifiable"/> or <see cref="CellStatus.Given"/>.
	/// </param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="status"/> is not <see cref="CellStatus.Modifiable"/>
	/// or <see cref="CellStatus.Given"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellInfo(byte cell, byte digit, CellStatus status)
	{
		if (status is not (CellStatus.Given or CellStatus.Modifiable))
		{
			throw new ArgumentOutOfRangeException(nameof(status));
		}

		Unsafe.SkipInit(out this);
		Cell = cell;
		Digit = digit;
		Status = status;
	}


	/// <summary>
	/// Indicates the cell used. The value is between 0 and 80.
	/// </summary>
	[field: FieldOffset(0)]
	public byte Cell { get; }

	/// <summary>
	/// Indicates the digit filled in the current cell. If the current cell is empty, the value will be -1;
	/// otherwise, this property will hold the real value.
	/// </summary>
	/// <remarks><i>
	/// Please check the property <see cref="Status"/> before using this property.
	/// </i></remarks>
	/// <seealso cref="Status"/>
	[field: FieldOffset(1)]
	public byte Digit { get; }

	/// <summary>
	/// Indicates the list of the candidates.
	/// </summary>
	/// <remarks><i>
	/// Please check the property <see cref="Status"/> before using this property.
	/// </i></remarks>
	/// <seealso cref="Status"/>
	[field: FieldOffset(1)]
	public short CandidatesList { get; }

	/// <summary>
	/// Indicates the current status of the cell.
	/// </summary>
	[field: FieldOffset(3)]
	public CellStatus Status { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is CellInfo comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CellInfo other) =>
		Status == other.Status && Cell == other.Cell && Status switch
		{
			CellStatus.Empty => CandidatesList == other.CandidatesList,
			CellStatus.Given or CellStatus.Modifiable => Digit == other.Digit,
			_ => false
		};

	/// <inheritdoc/>
	public override int GetHashCode() =>
		Status switch
		{
			CellStatus.Empty => Cell << 24 | CandidatesList << 8 | (byte)Status,
			CellStatus.Given or CellStatus.Modifiable => Cell << 24 | Digit << 8 | (byte)Status,
			_ => -1
		};

	/// <inheritdoc/>
	public override string ToString() =>
		Status switch
		{
			CellStatus.Given => $"{new Cells { Cell }}({Digit + 1})",
			CellStatus.Modifiable => $"{new Cells { Cell }}({Digit + 1})",
			CellStatus.Empty => $"{new Cells { Cell }}({new DigitCollection(CandidatesList).ToString()})",
			_ => "<Invalid status>"
		};

	/// <inheritdoc/>
	bool IValueEquatable<CellInfo>.Equals(in CellInfo other) => Equals(other);


	/// <summary>
	/// Gets the <see cref="CellInfo"/> instance via the specified grid and the desired cell.
	/// </summary>
	/// <param name="grid">The grid to get the cell information.</param>
	/// <param name="cell">The desired cell.</param>
	/// <returns>The <see cref="CellInfo"/> result.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>The current cell status is invalid.</item>
	/// <item>At the current status the candidate mask isn't the power of 2.</item>
	/// </list>
	/// </exception>
	public static CellInfo GetCellInfo(in Grid grid, int cell)
	{
		short mask = grid.GetMask(cell);
		byte statusMask = (byte)(mask >> 9 & 7);
		short candidatesMask = (short)(mask & 511);
		var status = (CellStatus)statusMask;
		return status switch
		{
			CellStatus.Empty => new((byte)cell, candidatesMask),
			CellStatus.Modifiable or CellStatus.Given =>
				new(
					(byte)cell,
					(byte)(
						IsPow2(candidatesMask)
							? TrailingZeroCount(candidatesMask)
							: throw new ArgumentException("At the curren status the candidate mask must be the power of 2.", nameof(candidatesMask))
					),
					status
				),
			_ => throw new ArgumentException("The current cell status is invalid.", nameof(grid))
		};
	}


	/// <inheritdoc/>
	public static bool operator ==(CellInfo left, CellInfo right) => left.Equals(right);

	/// <inheritdoc/>
	public static bool operator !=(CellInfo left, CellInfo right) => !(left == right);
}
