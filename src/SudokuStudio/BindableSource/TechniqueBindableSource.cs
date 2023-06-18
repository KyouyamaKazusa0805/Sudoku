namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates the techique bindable source.
/// </summary>
/// <param name="Technique">The technique enumeration field.</param>
/// <param name="DisplayName">The display name.</param>
/// <param name="Feature">Indicates the feature for the current technique.</param>
public sealed record TechniqueBindableSource(Technique Technique, string DisplayName, TechniqueFeature Feature) : IBindableSource;
