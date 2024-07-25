namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that supports filtering on techniques.
/// </summary>
public interface ITechniqueGenerator
{
	/// <summary>
	/// Indicates the techniques supported in the current generator.
	/// </summary>
	public TechniqueSet SupportedTechniques { get; }
}
