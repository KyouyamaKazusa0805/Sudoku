namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>K9</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="CellNotation.Kind.K9" path="/remarks"/>
/// </remarks>
public sealed record K9Format : ICellMapFormatter, ICandidateMapFormatter
{
	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => CellNotation.ToCollectionString(cellMap, CellNotation.Kind.K9);

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
			return new K9Format();
		}

		if (formatType == GetType())
		{
			return this;
		}

		return null;
	}
}
