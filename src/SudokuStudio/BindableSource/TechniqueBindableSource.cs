namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates the techique bindable source.
/// </summary>
/// <param name="Technique">The technique enumeration field.</param>
/// <param name="DisplayName">The display name.</param>
public readonly record struct TechniqueBindableSource(Technique Technique, string DisplayName);
