namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating a compatible prefix value defined by Hodoku.
/// </summary>
/// <param name="prefix">The prefix value.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class HodokuTechniquePrefixAttribute(string prefix) : Attribute
{
	/// <summary>
	/// The prefix value.
	/// </summary>
	public string Prefix { get; } = prefix;
}
