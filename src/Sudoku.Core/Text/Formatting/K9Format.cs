namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>K9</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="CellNotationKind.K9" path="/remarks"/>
/// </remarks>
public sealed record K9Format : ICellMapFormatter, ICandidateMapFormatter
{
	/// <inheritdoc cref="ICellMapFormatter.Instance"/>
	public static readonly K9Format Default = new();


	/// <inheritdoc/>
	static ICellMapFormatter ICellMapFormatter.Instance => Default;

	/// <inheritdoc/>
	static ICandidateMapFormatter ICandidateMapFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => CellConceptNotation.ToCollectionString(cellMap, CellNotationKind.K9);

	/// <inheritdoc/>
	public string ToString(scoped in CandidateMap candidateMap) => throw new NotImplementedException();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(formatType))]
	object? IFormatProvider.GetFormat(Type? formatType)
	{
		if ((formatType?.IsAssignableTo(typeof(ICellMapFormatter)) ?? false)
			|| (formatType?.IsAssignableTo(typeof(ICandidateMapFormatter)) ?? false))
		{
			return Default;
		}

		if (formatType == GetType())
		{
			return this;
		}

		return null;
	}
}
