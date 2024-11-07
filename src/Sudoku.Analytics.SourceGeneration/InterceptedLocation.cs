namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents location information for an intercepted method.
/// </summary>
/// <param name="FilePath">Indicates the file path of the target method.</param>
/// <param name="Line">Indicates the line number.</param>
/// <param name="Character">Indicates the character position.</param>
public readonly record struct InterceptedLocation(string FilePath, int Line, int Character);
