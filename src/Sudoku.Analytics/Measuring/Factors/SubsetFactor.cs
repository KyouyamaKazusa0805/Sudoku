namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes for a subset.
/// </summary>
/// <param name="size">Indicates the size of the subset.</param>
public abstract partial class SubsetFactor([PrimaryConstructorParameter] int size) : Factor;
