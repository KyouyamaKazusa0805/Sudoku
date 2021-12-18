namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute that applies to a syntax checker, to tell the source generator that the type:
/// <list type="number">
/// <item>Has explicitly implemented the interface <see cref="ISyntaxContextReceiver"/>.</item>
/// <item>
/// Contains an extra <see langword="get"/>-only property named <c>Diagnostics</c> of type
/// <see cref="List{T}"/> of <see cref="Diagnostic"/>, i.e. <c><![CDATA[public List<Diagnostic> Diagnostics { get; } = new();]]></c>.
/// </item>
/// <item>
/// Contains an extra <see langword="private readonly"/> field named <c>_cancellationToken</c>
/// of type <see cref="CancellationToken"/>, i.e. <c><![CDATA[private readonly CancellationToken _cancellationToken]]></c>.
/// </item>
/// <item>
/// Contains a constructor that initializes the field <c>_cancellationToken</c>,
/// i.e. <c><![CDATA[public SyntaxChecker(CancellationToken cancellationToken) => _cancellationToken = cancellationToken]]></c>.
/// </item>
/// </list>
/// </summary>
/// <seealso cref="ISyntaxContextReceiver"/>
/// <seealso cref="List{T}"/>
/// <seealso cref="Diagnostic"/>
/// <seealso cref="CancellationToken"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SyntaxCheckerAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SyntaxCheckerAttribute"/> via a <see cref="string"/>[]
	/// indicating the possible diagnostic IDs that the current type supports.
	/// </summary>
	/// <param name="names">The possible diagnostic IDs that the type support.</param>
	public SyntaxCheckerAttribute(params string[] diagnosticIds) => SupportedDiagnosticIds = diagnosticIds;


	/// <summary>
	/// Indicates whether the source generator will generates the debugger code snippets
	/// on method <see cref="ISourceGenerator.Initialize(GeneratorInitializationContext)"/>:
	/// <code>
	/// if (!Debugger.IsAttach)
	/// {
	///     Debugger.Launch();
	/// }
	/// </code>
	/// </summary>
	/// <seealso cref="ISourceGenerator.Initialize(GeneratorInitializationContext)"/>
	public bool Debugging { get; init; }

	/// <summary>
	/// Indicates the supported diagnostic IDs that the current type.
	/// </summary>
	public string[] SupportedDiagnosticIds { get; }
}
