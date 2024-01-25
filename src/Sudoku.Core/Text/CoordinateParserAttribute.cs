namespace Sudoku.Text;

/// <summary>
/// Represents an attribute type that specifies the bound concept for parser.
/// </summary>
/// <typeparam name="T">The type of the parser.</typeparam>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class CoordinateParserAttribute<T> : Attribute where T : CoordinateParser;
