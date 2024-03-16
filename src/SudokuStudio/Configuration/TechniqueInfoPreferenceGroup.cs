namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a preference group that defines the custom difficulty level and rating values for the techniques.
/// </summary>
[DependencyProperty<decimal>("RatingScale", DocSummary = "Indicates the rating scale value. The value will be used by scaling the value stored in property <see cref=\"CustomizedTechniqueData\"/>.")]
[DependencyProperty<Dictionary<Technique, TechniqueData>>("CustomizedTechniqueData", DocSummary = "Indicates the customized technique data.")]
public sealed partial class TechniqueInfoPreferenceGroup : PreferenceGroup
{
	[Default]
	internal static readonly decimal RatingScaleDefaultValue = 10M;

	[Default]
	private static readonly Dictionary<Technique, TechniqueData> CustomizedTechniqueDataDefaultValue = [];


	/// <summary>
	/// Try to append or update the technique data in the property <see cref="CustomizedTechniqueData"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the data to be modified. The supported types are <see cref="int"/> or <see cref="DifficultyLevel"/>.
	/// </typeparam>
	/// <param name="technique">The technique.</param>
	/// <param name="ratingOrLevel">The value to be updated or appended.</param>
	public void AppendOrUpdateValue<T>(Technique technique, T ratingOrLevel)
	{
		Debug.Assert(ratingOrLevel is int or DifficultyLevel);

		scoped ref var data = ref CollectionsMarshal.GetValueRefOrNullRef(CustomizedTechniqueData, technique);
		var isNullRef = Ref.IsNullReference(in data);
		var newData = ratingOrLevel switch
		{
			int value => isNullRef ? new(value, technique.GetDifficultyLevel()) : data with { Rating = value },
			DifficultyLevel value => isNullRef
				? new((int)(technique.GetBaseDifficulty(out _) * RatingScaleDefaultValue), value)
				: data with { Level = value }
		};
		if (isNullRef)
		{
			CustomizedTechniqueData.Add(technique, newData);
		}
		else
		{
			data = newData;
		}
	}

	/// <inheritdoc cref="GetRatingOrDefault(Technique)"/>
	public int? GetRating(Technique technique) => CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Rating : null;

	/// <summary>
	/// Try to get the rating value for the specified technique.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <returns>The rating value.</returns>
	public int GetRatingOrDefault(Technique technique)
		=> CustomizedTechniqueData.TryGetValue(technique, out var pair)
			? pair.Rating
			: (int)(technique.GetBaseDifficulty(out _) * RatingScaleDefaultValue);

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
