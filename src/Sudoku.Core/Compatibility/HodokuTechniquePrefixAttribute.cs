namespace Sudoku.Compatibility;

/// <summary>
/// Defines an attribute that is applied to a field in type <see cref="Technique"/>,
/// indicating a compatible prefix value defined by Hodoku.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class HodokuTechniquePrefixAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="HodokuTechniquePrefixAttribute"/> via the specified prefix value.
	/// </summary>
	/// <param name="prefix">The prefix value.</param>
	public HodokuTechniquePrefixAttribute(string prefix) => Prefix = prefix;


	/// <summary>
	/// The prefix value.
	/// </summary>
	public string Prefix { get; }
}
