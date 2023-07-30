namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>RxCy</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="CellNotationKind.RxCy" path="/remarks"/>
/// </remarks>
public sealed record RxCyFormat : ICellMapFormatter, ICandidateMapFormatter
{
	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => CellNotation.ToCollectionString(cellMap);

	/// <inheritdoc/>
	public string ToString(scoped in CandidateMap candidateMap) => CandidateNotation.ToCollectionString(candidateMap);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(formatType))]
	object? IFormatProvider.GetFormat(Type? formatType)
	{
		if ((formatType?.IsAssignableTo(typeof(ICellMapFormatter)) ?? false)
			|| (formatType?.IsAssignableTo(typeof(ICandidateMapFormatter)) ?? false))
		{
			return new RxCyFormat();
		}

		if (formatType == GetType())
		{
			return this;
		}

		return null;
	}
}
