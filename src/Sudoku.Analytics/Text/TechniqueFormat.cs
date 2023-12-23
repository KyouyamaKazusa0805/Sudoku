namespace Sudoku.Analytics;

/// <summary>
/// Represents a resource format. This type is used by <see cref="Step"/> instances to describe the technique format
/// stored in resource dictionary.
/// </summary>
/// <seealso cref="Step"/>
[InterpolatedStringHandler]
[StructLayout(LayoutKind.Auto)]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[EqualityOperators]
[method: EditorBrowsable(EditorBrowsableState.Never)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: DebuggerStepThrough]
public partial struct TechniqueFormat([Data(DataMemberKinds.Field)] int literalLength, [Data(DataMemberKinds.Field)] int holeCount)
{
	/// <summary>
	/// The format prefix.
	/// </summary>
	internal const string FormatPrefix = "TechniqueFormat";


	/// <summary>
	/// The suffix of the format.
	/// </summary>
	private string? _formatSuffix;


	/// <inheritdoc cref="DefaultInterpolatedStringHandler.AppendFormatted(string?)"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerStepThrough]
	public void AppendFormatted(string formatSuffix) => _formatSuffix = formatSuffix;

	/// <summary>
	/// Indicates the format key. The value can be <see langword="null"/> if the step does not contain an equivalent resource key.
	/// </summary>
	/// <param name="cultureInfo">The culture information.</param>
	public readonly string? GetTargetFormat(CultureInfo? cultureInfo)
		=> _formatSuffix is null ? null : GetString($"{FormatPrefix}_{_formatSuffix}", cultureInfo ?? CultureInfo.CurrentUICulture);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => GetTargetFormat(null) ?? "<Unspecified>";

	/// <summary>
	/// Get the format string for the current instance.
	/// </summary>
	/// <param name="cultureInfo">The culture information.</param>
	/// <param name="formatArguments">The format arguments.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CultureInfo? cultureInfo, params string[] formatArguments)
		=> GetTargetFormat(cultureInfo) is { } p
			? string.Format(p, formatArguments)
			: throw new ResourceNotFoundException($"{FormatPrefix}_{_formatSuffix}", typeof(TechniqueFormat).Assembly);
}
