namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a data structure that provides the basic usage in <see cref="IDrawable"/> members.
/// </summary>
/// <param name="Conclusions"><inheritdoc cref="IDrawable.Conclusions" path="/summary"/></param>
/// <param name="Views"><inheritdoc cref="IDrawable.Views" path="/summary"/></param>
/// <seealso cref="IDrawable"/>
public record struct UserDefinedDrawable(Conclusion[] Conclusions, View[]? Views) : IDrawable;
