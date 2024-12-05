namespace Sudoku.Analytics.Construction;

/// <summary>
/// Represents an element that is constructible.
/// </summary>
/// <typeparam name="TEnum">The type of enumeration.</typeparam>
public interface IConstructible<TEnum> : IDataStructure where TEnum : Enum
{
	/// <summary>
	/// Indicates the type of the constructible element.
	/// </summary>
	public new abstract TEnum Type { get; }
}
