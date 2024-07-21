namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a data structure that provides the basic usage in <see cref="IDrawable"/> members.
/// </summary>
/// <param name="Conclusions"><inheritdoc cref="IDrawable.Conclusions" path="/summary"/></param>
/// <param name="Views"><inheritdoc cref="IDrawable.Views" path="/summary"/></param>
/// <seealso cref="IDrawable"/>
public record struct UserDefinedDrawable(
	[property: JsonConverter(typeof(ReadOnlyMemoryConverter<Conclusion>))] ReadOnlyMemory<Conclusion> Conclusions,
	[property: JsonConverter(typeof(ReadOnlyMemoryConverter<View>))] ReadOnlyMemory<View> Views
) : IDrawable;
