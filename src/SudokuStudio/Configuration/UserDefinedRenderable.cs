namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a data structure that provides the basic usage in <see cref="IRenderable"/> members.
/// </summary>
/// <param name="Conclusions"><inheritdoc cref="IRenderable.Conclusions" path="/summary"/></param>
/// <param name="Views"><inheritdoc cref="IRenderable.Views" path="/summary"/></param>
/// <seealso cref="IRenderable"/>
public record struct UserDefinedRenderable(Conclusion[] Conclusions, View[]? Views) : IRenderable;
