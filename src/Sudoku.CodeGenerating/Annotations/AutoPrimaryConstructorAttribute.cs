namespace Sudoku.CodeGenerating;

/// <summary>
/// Indicates the marked <see langword="class"/> should generate primary constructor automatically.
/// </summary>
[AttributeUsage(Class, AllowMultiple = false, Inherited = false)]
public sealed class AutoPrimaryConstructorAttribute : Attribute
{
	/// <summary>
	/// <para>Indicates the custom accessibility.</para>
	/// <para>The default value is <see cref="MemberAccessibility.Public"/>.</para>
	/// </summary>
	public MemberAccessibility Accessibility { get; init; } = MemberAccessibility.Public;

	/// <summary>
	/// Indicates the included member names.
	/// </summary>
	public string[]? IncludedMemberNames { get; init; }

	/// <summary>
	/// Indicates the excluded member names.
	/// </summary>
	public string[]? ExcludedMemberNames { get; init; }
}
