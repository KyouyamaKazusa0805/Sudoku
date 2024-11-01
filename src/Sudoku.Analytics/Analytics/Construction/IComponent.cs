namespace Sudoku.Analytics.Construction;

/// <summary>
/// Represents a component in analysis of patterns.
/// </summary>
public interface IComponent : IConstructible<ComponentType>
{
	/// <inheritdoc cref="IConstructible{TEnum}.Type"/>
	public new abstract ComponentType Type { get; }

	/// <inheritdoc/>
	ComponentType IConstructible<ComponentType>.Type => Type;
}
