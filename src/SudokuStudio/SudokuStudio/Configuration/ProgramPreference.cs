namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a user-defined program preference.
/// </summary>
public sealed class ProgramPreference
{
	/// <inheritdoc cref="AnalysisPreferenceGroup"/>
	[DebuggerHidden]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, true, DiagnosticId = "SCA0103", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0103")]
	public AnalysisPreferenceGroup AnalysisPreferences { get; set; } = new();

	/// <inheritdoc cref="UIPreferenceGroup"/>
	[DebuggerHidden]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, true, DiagnosticId = "SCA0103", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0103")]
	public UIPreferenceGroup UIPreferences { get; set; } = new();


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
					_ => throw new NotSupportedException("One of two possible property values cannot be found in target property group type.")
				};
		}
	}
}
