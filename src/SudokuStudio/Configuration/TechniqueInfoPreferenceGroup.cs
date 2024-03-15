namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a preference group that defines the custom difficulty level and rating values for the techniques.
/// </summary>
[DependencyProperty<Dictionary<Technique, TechniqueData>>("CustomizedTechniqueData", DocSummary = "Indicates the customized technique data.")]
public sealed partial class TechniqueInfoPreferenceGroup : PreferenceGroup
{
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
	internal void AppendOrUpdateValue<T>(Technique technique, T ratingOrLevel)
	{
		Debug.Assert(ratingOrLevel is int or DifficultyLevel);

		scoped ref var data = ref CollectionsMarshal.GetValueRefOrNullRef(CustomizedTechniqueData, technique);
		var isNullRef = Ref.IsNullReference(in data);
		var newData = ratingOrLevel switch
		{
			int value => isNullRef ? new(value, technique.GetDifficultyLevel()) : data with { Rating = value },
			DifficultyLevel value => isNullRef ? new(technique.GetBaseDifficulty(out _), value) : data with { Level = value }
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

	/// <summary>
	/// Try to get the rating value for the specified technique.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <returns>The rating value.</returns>
	internal int GetRating(Technique technique)
	{
		var r = CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Rating : technique.GetBaseDifficulty(out _);
		return (int)(r * 10);
	}

	/// <summary>
	/// Try to get the difficulty level for the specified technique.
	/// </summary>
	/// <param name="technique">The technique.</param>
	/// <returns>The difficulty level.</returns>
	internal DifficultyLevel GetDifficultyLevel(Technique technique)
		=> CustomizedTechniqueData.TryGetValue(technique, out var pair) ? pair.Level : technique.GetDifficultyLevel();
}
