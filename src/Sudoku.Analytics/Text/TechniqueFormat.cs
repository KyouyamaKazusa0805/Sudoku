namespace Sudoku.Analytics;

/// <summary>
/// Represents a resource format. This type is used by <see cref="Step"/> instances to describe the technique format
/// stored in resource dictionary.
/// </summary>
/// <seealso cref="Step"/>
[StructLayout(LayoutKind.Auto)]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[EqualityOperators]
[method: DebuggerStepThrough]
[method: EditorBrowsable(EditorBrowsableState.Never)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public partial struct TechniqueFormat([RecordParameter(DataMemberKinds.Field)] string formatName)
{
	/// <summary>
	/// The format prefix.
	/// </summary>
	private const string FormatPrefix = "TechniqueFormat";


	/// <summary>
	/// Full name of the format.
	/// </summary>
	private readonly string _formatFullName = $"{FormatPrefix}_{formatName}";


	/// <summary>
	/// Indicates the format key. The value can be <see langword="null"/> if the step does not contain an equivalent resource key.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	public readonly string? GetTargetFormat(CultureInfo? culture)
		=> _formatName is null ? null : ResourceDictionary.GetOrNull(_formatFullName, culture ?? CultureInfo.CurrentUICulture);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => GetTargetFormat(null) ?? "<Unspecified>";

	/// <summary>
	/// Get the format string for the current instance.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <param name="formatArguments">The format arguments.</param>
	/// <returns>The final result.</returns>
	/// <exception cref="TargetResourceNotFoundException">Throws when the specified culture doesn't contain the specified resource.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CultureInfo? culture, params string[] formatArguments)
		=> GetTargetFormat(culture) is { } p
			? string.Format(p, formatArguments)
			: throw new TargetResourceNotFoundException(typeof(TechniqueFormat).Assembly, _formatFullName, culture);


	/// <summary>
	/// Creates a <see cref="TechniqueFormat"/> instance using the specified format name stored in resource file.
	/// </summary>
	/// <param name="formatName">TThe format name.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TechniqueFormat(string formatName) => new(formatName);
}
