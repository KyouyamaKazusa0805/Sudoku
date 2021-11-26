namespace Sudoku.Solving.Manual.Exocets;

/// <summary>
/// Provides a usage of <b>exocet</b> technique.
/// </summary>
/// <param name="Views">All views.</param>
/// <param name="Exocet">The exocet.</param>
/// <param name="Digits">All digits.</param>
/// <param name="LockedMemberQ">The locked member Q.</param>
/// <param name="LockedMemberR">The locked member R.</param>
/// <param name="Eliminations">The eliminations.</param>
public abstract record ExocetStepInfo(
	IReadOnlyList<View> Views, in Pattern Exocet, IEnumerable<int> Digits,
	IEnumerable<int>? LockedMemberQ, IEnumerable<int>? LockedMemberR, IReadOnlyList<Elimination> Eliminations
) : StepInfo(GatherConclusions(Eliminations), Views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public abstract override decimal Difficulty { get; }

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Exocet;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <summary>
	/// Indiactes the digits string.
	/// </summary>
	[FormatItem]
	protected string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(Digits).ToString();
	}

	/// <summary>
	/// Indicates the base map string.
	/// </summary>
	[FormatItem]
	protected string BaseMapStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BaseMap.ToString();
	}

	/// <summary>
	/// Indicates the target map string.
	/// </summary>
	[FormatItem]
	protected string TargetMapStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TargetMap.ToString();
	}

	/// <summary>
	/// Indicates the locked member Q string.
	/// </summary>
	[FormatItem]
	[NotNullIfNotNull(nameof(LockedMemberQ))]
	protected string LockedMemberQStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string snippet = TextResources.Current.LockedMemberQSnippet;
			string? cells = LockedMemberQ is null ? null : new DigitCollection(LockedMemberQ).ToString();
			return $"{snippet}{cells}";
		}
	}

	/// <summary>
	/// Indicates the locked member R string.
	/// </summary>
	[FormatItem]
	[NotNullIfNotNull(nameof(LockedMemberR))]
	protected string LockedMemberRStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string snippet = TextResources.Current.LockedMemberRSnippet;
			string? cells = LockedMemberR is null ? null : new DigitCollection(LockedMemberR).ToString();
			return $"{snippet}{cells}";
		}
	}

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	private Cells BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var (baseMap, _, _) = Exocet;
			return baseMap;
		}
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	private Cells TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var (_, targetMap, _) = Exocet;
			return targetMap;
		}
	}


	/// <inheritdoc/>
	public sealed override string ToFullString()
	{
		var sb = new StringHandler(initialCapacity: 100);
		sb.Append(base.ToFullString());
		sb.AppendLine();
		sb.AppendRangeWithLines(Eliminations);

		return sb.ToStringAndClear();
	}


	/// <summary>
	/// Gather conclusions.
	/// </summary>
	/// <returns>The gathered result.</returns>
	private static IReadOnlyList<Conclusion> GatherConclusions(IReadOnlyList<Elimination> eliminations)
	{
		var result = new List<Conclusion>();
		foreach (var eliminationInstance in eliminations)
		{
			result.AddRange(eliminationInstance.AsSpan().ToArray());
		}

		return result;
	}
}
