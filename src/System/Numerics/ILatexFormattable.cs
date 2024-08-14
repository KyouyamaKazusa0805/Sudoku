namespace System.Numerics;

/// <summary>
/// Represents a type that supports formatting on LaTeX format.
/// </summary>
public interface ILatexFormattable
{
	/// <summary>
	/// Returns a LaTeX string to represent the current object.
	/// </summary>
	/// <returns>A <see cref="string"/> representation.</returns>
	public abstract string ToLatexString();
}
