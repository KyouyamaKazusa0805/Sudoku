namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a user-defined program preference.
/// </summary>
public sealed class ProgramPreference
{
	/// <inheritdoc cref="AnalysisPreferenceGroup"/>
	public AnalysisPreferenceGroup AnalysisPreferences { get; set; } = new();

	/// <inheritdoc cref="ConstraintPreferenceGroup"/>
	public ConstraintPreferenceGroup ConstraintPreferences { get; set; } = new();

	/// <inheritdoc cref="LibraryPreferenceGroup"/>
	public LibraryPreferenceGroup LibraryPreferences { get; set; } = new();

	/// <inheritdoc cref="UIPreferenceGroup"/>
	public UIPreferenceGroup UIPreferences { get; set; } = new();

	/// <inheritdoc cref="StepSearcherOrderingPreferenceGroup"/>
	public StepSearcherOrderingPreferenceGroup StepSearcherOrdering { get; set; } = new();

	/// <inheritdoc cref="TechniqueInfoPreferenceGroup"/>
	public TechniqueInfoPreferenceGroup TechniqueInfoPreferences { get; set; } = new();


	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="new"/>.
	/// </summary>
	/// <param name="new">The newer instance that is used for covering the current instance.</param>
	/// <exception cref="NotSupportedException">Throws when the property type is not supported to be serialized, or not found.</exception>
	public void CoverBy(ProgramPreference @new)
	{
		foreach (var propertyInfo in typeof(ProgramPreference).GetProperties())
		{
			f(this, propertyInfo).CoverBy(f(@new, propertyInfo));


			static PreferenceGroup f(ProgramPreference p, PropertyInfo propertyInfo)
				=> propertyInfo.GetValue(p) switch
				{
					PreferenceGroup t => t,
					_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("PropertyValueCannotBeFoundInPropertyGroupType"))
				};
		}
	}
}
