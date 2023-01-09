namespace SudokuStudio.Models;

/// <summary>
/// Defines a data in a cell.
/// </summary>
internal sealed partial class GridCellData : Model, IEquatable<GridCellData>, IEqualityOperators<GridCellData, GridCellData, bool>
{
	private bool _isMouseHovered = false;

	private short _candidatesMask = Grid.MaxCandidatesMask;

	private CellStatus _cellStatus = CellStatus.Empty;


	/// <summary>
	/// Indicates whether the mouse pointer is hovered onto the current cell.
	/// </summary>
	public bool IsMouseHovered
	{
		get => _isMouseHovered;

		set => SetBackingField(ref _isMouseHovered, value, static (f, v) => f == v);
	}

	/// <summary>
	/// Indicates the cell index. The value can be between 0 and 80.
	/// </summary>
	public int CellIndex { get; set; }

	/// <summary>
	/// Indicates the status of the cell. The default value is <see cref="CellStatus.Empty"/>.
	/// </summary>
	/// <seealso cref="CellStatus.Empty"/>
	public CellStatus CellStatus
	{
		get => _cellStatus;

		set => SetBackingField(ref _cellStatus, value, static (f, v) => f == v, static v => Enum.IsDefined(v));
	}

	/// <summary>
	/// Indicates the candidate mask used. The value can be between 0 and 511. The default value is 511.
	/// </summary>
	public short CandidatesMask
	{
		get => _candidatesMask;

		set => SetBackingField(ref _candidatesMask, value, static (f, v) => f == v, static v => v is >= 0 and < 512);
	}

	[GeneratedDisplayName("Cell")]
	private string CellString => RxCyNotation.ToCellString(CellIndex);

	[GeneratedDisplayName("Candidates")]
	private string CandidatesString
	{
		get
		{
			var targetList = new List<int>(9);
			foreach (var digit in _candidatesMask)
			{
				targetList.Add(digit);
			}

			return string.Join(", ", targetList);
		}
	}


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] GridCellData? other)
		=> other is not null && CellIndex == other.CellIndex && CellStatus == other.CellStatus && CandidatesMask == other.CandidatesMask;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(_cellStatus), nameof(_cellStatus), nameof(_candidatesMask))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(CellString), nameof(CellStatus), nameof(CandidatesString))]
	public override partial string ToString();


	/// <inheritdoc/>
	public static bool operator ==(GridCellData? left, GridCellData? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	public static bool operator !=(GridCellData? left, GridCellData? right) => !(left == right);
}
