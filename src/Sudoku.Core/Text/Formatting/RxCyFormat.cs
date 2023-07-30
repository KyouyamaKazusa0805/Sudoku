namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>RxCy</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="CellNotationKind.RxCy" path="/remarks"/>
/// </remarks>
public sealed record RxCyFormat : ICellMapFormatter, ICandidateMapFormatter
{
	/// <inheritdoc cref="ICellMapFormatter.Instance"/>
	public static readonly RxCyFormat Default = new();


	/// <inheritdoc/>
	static ICellMapFormatter ICellMapFormatter.Instance => Default;

	/// <inheritdoc/>
	static ICandidateMapFormatter ICandidateMapFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => CellConceptNotation.ToCollectionString(cellMap);

	/// <inheritdoc/>
	public string ToString(scoped in CandidateMap candidateMap) => CandidateConceptNotation.ToCollectionString(candidateMap);

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
