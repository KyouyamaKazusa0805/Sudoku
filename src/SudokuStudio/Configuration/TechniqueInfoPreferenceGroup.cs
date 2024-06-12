namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a preference group that defines the custom difficulty level and rating values for the techniques.
/// </summary>
[DependencyProperty<decimal>("RatingScale", DocSummary = "Indicates the rating scale value. The value will be used by scaling the value stored in property <see cref=\"CustomizedTechniqueData\"/>.")]
[DependencyProperty<Dictionary<Technique, TechniqueData>>("CustomizedTechniqueData", DocSummary = "Indicates the customized technique Data.")]
public sealed partial class TechniqueInfoPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly decimal RatingScaleDefaultValue = 10M;

	[Default]
	private static readonly Dictionary<Technique, TechniqueData> CustomizedTechniqueDataDefaultValue = [];


	/// <summary>
	/// Add or update rating value.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <param name="value">The rating value to be set.</param>
	public unsafe void AppendOrUpdateRating(Technique technique, int value)
	{
		AppendOrUpdateValue(technique, value, &dataCreator, &dataModifier);


		static TechniqueData dataCreator(Technique technique, int value)
		{
			technique.GetDefaultRating(out var directRating);
			return new(value, directRating, technique.GetDifficultyLevel());
		}

		static TechniqueData dataModifier(ref readonly TechniqueData data, int value) => data with { Rating = value };
	}

	/// <summary>
	/// Add or update direct rating value.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <param name="value">The rating value to be set.</param>
	public unsafe void AppendOrUpdateDirectRating(Technique technique, int value)
	{
		AppendOrUpdateValue(technique, value, &dataCreator, &dataModifier);


		static TechniqueData dataCreator(Technique technique, int value)
			=> new(technique.GetDefaultRating(out _), value, technique.GetDifficultyLevel());

		static TechniqueData dataModifier(ref readonly TechniqueData data, int value) => data with { DirectRating = value };
	}

	/// <summary>
	/// Add or update difficulty level.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <param name="value">The difficulty level to be set.</param>
	public unsafe void AppendOrUpdateDifficultyLevel(Technique technique, DifficultyLevel value)
	{
		AppendOrUpdateValue(technique, value, &dataCreator, &dataModifier);


		static TechniqueData dataCreator(Technique technique, DifficultyLevel value)
		{
			var rating = technique.GetDefaultRating(out var directRating);
			return new(rating, directRating, value);
		}

		static TechniqueData dataModifier(ref readonly TechniqueData data, DifficultyLevel value) => data with { Level = value };
	}


	/// <summary>
	/// Append or update value.
	/// </summary>
	/// <typeparam name="T">The type of the value to be set.</typeparam>
	/// <param name="technique">The technique.</param>
	/// <param name="value">The value to be set.</param>
	/// <param name="dataCreator">The data creator method that will be called when the collection doesn't contain the technique.</param>
	/// <param name="dataModifier">The data modifier method that will be called when the collection contains the technique.</param>
	private unsafe void AppendOrUpdateValue<T>(
		Technique technique,
		T value,
		delegate*<Technique, T, TechniqueData> dataCreator,
		delegate*<ref readonly TechniqueData, T, TechniqueData> dataModifier
	)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
		ref var data = ref CollectionsMarshal.GetValueRefOrNullRef(CustomizedTechniqueData, technique);
		var isNullRef = @ref.IsNullRef(in data);
		var a = valueUpdaterWhenNullRef;
		var b = valueUpdaterWhenNotNullRef;
		(isNullRef ? a : b)(ref data, isNullRef ? dataCreator(technique, value) : dataModifier(in data, value));


		void valueUpdaterWhenNullRef(ref TechniqueData data, TechniqueData newData) => CustomizedTechniqueData.Add(technique, newData);

		void valueUpdaterWhenNotNullRef(ref TechniqueData data, TechniqueData newData) => data = newData;
	}

	/// <inheritdoc cref="GetRatingOrDefault(Technique)"/>
	public int? GetRating(Technique technique) => CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Rating : null;

	/// <summary>
	/// Try to get the rating value for the specified technique.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <returns>The rating value.</returns>
	public int GetRatingOrDefault(Technique technique)
		=> CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Rating : technique.GetDefaultRating(out _);

	/// <inheritdoc cref="GetDifficultyLevelOrDefault(Technique)"/>
	public DifficultyLevel? GetDifficultyLevel(Technique technique)
		=> CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Level : null;

	/// <summary>
	/// Try to get the difficulty level for the specified technique.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <returns>The difficulty level.</returns>
	public DifficultyLevel GetDifficultyLevelOrDefault(Technique technique)
		=> CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Level : technique.GetDifficultyLevel();
}
