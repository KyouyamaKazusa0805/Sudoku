namespace Sudoku.Text;

/// <summary>
/// Represents a resource format. This type is used by <see cref="Step"/> instances to describe the technique format
/// stored in resource dictionary.
/// </summary>
/// <param name="techniqueName">Indicates the technique identifier name.</param>
/// <seealso cref="Step"/>
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[EqualityOperators]
[method: EditorBrowsable(EditorBrowsableState.Never)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public partial struct TechniqueFormat([PrimaryConstructorParameter(MemberKinds.Field)] string techniqueName)
{
	/// <summary>
	/// The format prefix.
	/// </summary>
	private const string FormatPrefix = "TechniqueFormat";


	/// <summary>
	/// Full name of the format.
	/// </summary>
	private readonly string TechniqueResourceName => $"{FormatPrefix}_{_techniqueName}";


	/// <summary>
	/// Indicates the format key. The value can be <see langword="null"/> if the step does not contain an equivalent resource key.
	/// </summary>
	/// <param name="formatProvider">The culture information.</param>
	public readonly string? GetTargetFormat(IFormatProvider? formatProvider)
		=> _techniqueName is not null
		&& ResourceDictionary.TryGet(TechniqueResourceName, out var resource, formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture)
			? resource
			: null;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => GetTargetFormat(null) ?? "<Unspecified>";

	/// <summary>
	/// Get the format string for the current instance.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <param name="formatArguments">The format arguments.</param>
	/// <returns>The final result.</returns>
	/// <exception cref="ResourceNotFoundException">Throws when the specified culture doesn't contain the specified resource.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CultureInfo? culture, params string[] formatArguments)
		=> GetTargetFormat(culture) is { } p
			? string.Format(p, formatArguments)
			: throw new ResourceNotFoundException(typeof(TechniqueFormat).Assembly, TechniqueResourceName, culture);


	/// <summary>
	/// Creates a <see cref="TechniqueFormat"/> instance using the specified format name stored in resource file.
	/// </summary>
	/// <param name="techniqueName">TThe format name.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TechniqueFormat(string techniqueName) => new(techniqueName);
}
