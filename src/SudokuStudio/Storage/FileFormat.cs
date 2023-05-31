namespace SudokuStudio.Storage;

/// <summary>
/// Represents a file format.
/// </summary>
/// <param name="Description">The description to the format.</param>
/// <param name="Formats">The formats.</param>
/// <completionlist cref="FileFormats"/>
public sealed record FileFormat(string Description, params string[] Formats);
