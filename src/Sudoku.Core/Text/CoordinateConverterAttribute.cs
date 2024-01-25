namespace Sudoku.Text;

/// <summary>
/// Represents an attribute type that specifies the bound concept for converter.
/// </summary>
/// <typeparam name="T">The type of the converter.</typeparam>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class CoordinateConverterAttribute<T> : Attribute where T : CoordinateConverter;
