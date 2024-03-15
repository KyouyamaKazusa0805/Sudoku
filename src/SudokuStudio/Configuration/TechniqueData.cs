namespace SudokuStudio.Configuration;

/// <summary>
/// Represents the technique data to be used.
/// </summary>
/// <param name="Rating">Indicates the difficulty rating value.</param>
/// <param name="Level">
/// <para>Indicates the difficulty level value.</para>
/// <para>
/// Note: The value cannot be <see cref="DifficultyLevel.LastResort"/>, <see cref="DifficultyLevel.Unknown"/>
/// or other undefined values. Such fields are reserved values on purpose. In fact, all techniques that hold
/// one of the such difficulty levels cannot be modified its difficulty level and rating value.
/// </para>
/// </param>
public readonly record struct TechniqueData(decimal Rating, DifficultyLevel Level);
